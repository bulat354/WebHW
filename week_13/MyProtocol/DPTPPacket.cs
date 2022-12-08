using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyProtocol
{
    /*
MYPROTOCOL PACKET STRUCTURE
 
(offset: 0) HEADER (3 bytes) [ 0xAF, 0xAA, 0xAF ]
(offset: 3) PACKET ID
  (offset: 3) PACKET TYPE (1 byte)
  (offset: 4) PACKET SUBTYPE (1 byte)
(offset: 5) FIELDS (FIELD[])
(offset: END) PACKET ENDING (2 bytes) [ 0xFF, 0x00 ]
 
FIELD STRUCTURE
 
(offset: 0) FIELD ID (1 byte)
(offset: 1) FIELD SIZE (1 byte)
(offset: 2) FIELD CONTENTS
    */
    public class DPTPPacket
    {
        public bool Protected { get; set; }

        public byte PacketType { get; private set; }
        public byte PacketSubtype { get; private set; }
        public List<DPTPPacketField> Fields { get; set; } = new List<DPTPPacketField>();

        private bool ChangeHeaders { get; set; }

        private DPTPPacket() { }

        public static DPTPPacket Create(byte type, byte subtype)
        {
            return new DPTPPacket
            {
                PacketType = type,
                PacketSubtype = subtype
            };
        }

        public byte[] ToPacket()
        {
            var packet = new MemoryStream();

            packet.Write(ChangeHeaders
                ? new byte[] { 0x95, 0xAA, 0xFF, PacketType, PacketSubtype }
                : new byte[] { 0xAF, 0xAA, 0xAF, PacketType, PacketSubtype },
                0, 5);

            var fields = Fields.OrderBy(field => field.FieldID);

            foreach (var field in fields)
            {
                packet.Write(new[] { field.FieldID, field.FieldSize }, 0, 2);
                packet.Write(field.Contents, 0, field.Contents.Length);
            }

            packet.Write(new byte[] { 0xFF, 0x00 }, 0, 2);

            return packet.ToArray();
        }

        public static DPTPPacket Parse(byte[] packet, bool markAsEncrypted = false)
        {
            //Минимальный размер пакета — 7 байт.
            //HEADER(3) + TYPE(1) + SUBTYPE(1) + PACKET ENDING(2)
            if (packet.Length < 7)
            {
                return null;
            }

            var encrypted = false;

            if (packet[0] != 0xAF ||
                packet[1] != 0xAA ||
                packet[2] != 0xAF)
            {
                if (packet[0] == 0x95 ||
                    packet[1] == 0xAA ||
                    packet[2] == 0xFF)
                {
                    encrypted = true;
                }
                else
                {
                    return null;
                }
            }

            var mIndex = packet.Length - 1;

            if (packet[mIndex - 1] != 0xFF ||
                packet[mIndex] != 0x00)
            {
                return null;
            }

            var type = packet[3];
            var subtype = packet[4];

            var xpacket = Create(type, subtype);

            var fields = packet.Skip(5).ToArray();

            while (true)
            {
                if (fields.Length == 2)
                {
                    return encrypted ? DecryptPacket(xpacket) : xpacket;
                }

                var id = fields[0];
                var size = fields[1];

                var contents = size != 0 ?
                fields.Skip(2).Take(size).ToArray() : null;

                xpacket.Fields.Add(new DPTPPacketField
                {
                    FieldID = id,
                    FieldSize = size,
                    Contents = contents
                });

                fields = fields.Skip(2 + size).ToArray();
            }
        }

        public static DPTPPacket EncryptPacket(DPTPPacket packet)
        {
            if (packet == null)
            {
                return null;
            }

            var rawBytes = packet.ToPacket();
            var encrypted = DPTProtocolEncryptor.Encrypt(rawBytes);

            var p = Create(0, 0);
            p.SetValueRaw(0, encrypted);
            p.ChangeHeaders = true;

            return p;
        }

        private static DPTPPacket DecryptPacket(DPTPPacket packet)
        {
            if (!packet.HasField(0))
            {
                return null;
            }

            var rawData = packet.GetValueRaw(0);
            var decrypted = DPTProtocolEncryptor.Decrypt(rawData);

            return Parse(decrypted, true);
        }

        public DPTPPacket Encrypt()
        {
            return EncryptPacket(this);
        }

        public DPTPPacket Decrypt()
        {
            return DecryptPacket(this);
        }

        public void SetValueRaw(byte id, byte[] rawData)
        {
            var field = GetField(id);

            if (field == null)
            {
                field = new DPTPPacketField
                {
                    FieldID = id
                };

                Fields.Add(field);
            }

            if (rawData.Length > byte.MaxValue)
            {
                throw new Exception("Object is too big. Max length is 255 bytes.");
            }

            field.FieldSize = (byte)rawData.Length;
            field.Contents = rawData;
        }

        public byte[] GetValueRaw(byte id)
        {
            var field = GetField(id);

            if (field == null)
            {
                throw new Exception($"Field with ID {id} wasn't found.");
            }

            return field.Contents;
        }

        public byte[] FixedObjectToByteArray(object value)
        {
            var rawsize = Marshal.SizeOf(value);
            var rawdata = new byte[rawsize];

            var handle = GCHandle.Alloc(rawdata,
                GCHandleType.Pinned);

            Marshal.StructureToPtr(value,
                handle.AddrOfPinnedObject(),
                false);

            handle.Free();

            return rawdata;
        }

        private T ByteArrayToFixedObject<T>(byte[] bytes) where T : struct
        {
            T structure;

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

            try
            {
                structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }

            return structure;
        }

        public DPTPPacketField GetField(byte id)
        {
            foreach (var field in Fields)
            {
                if (field.FieldID == id)
                {
                    return field;
                }
            }

            return null;
        }

        public bool HasField(byte id)
        {
            return GetField(id) != null;
        }

        public T GetValue<T>(byte id) where T : struct
        {
            var field = GetField(id);

            if (field == null)
            {
                throw new Exception($"Field with ID {id} wasn't found.");
            }
            var neededSize = Marshal.SizeOf(typeof(T));

            if (field.FieldSize != neededSize)
            {
                throw new Exception($"Can't convert field to type {typeof(T).FullName}.\n" + $"We have {field.FieldSize} bytes but we need exactly {neededSize}.");
            }

            return ByteArrayToFixedObject<T>(field.Contents);
        }

        public void SetValue(byte id, object structure)
        {
            if (!structure.GetType().IsValueType)
            {
                throw new Exception("Only value types are available.");
            }

            var field = GetField(id);

            if (field == null)
            {
                field = new DPTPPacketField
                {
                    FieldID = id
                };

                Fields.Add(field);
            }

            var bytes = FixedObjectToByteArray(structure);

            if (bytes.Length > byte.MaxValue)
            {
                throw new Exception("Object is too big. Max length is 255 bytes.");
            }

            field.FieldSize = (byte)bytes.Length;
            field.Contents = bytes;
        }
    }
}
