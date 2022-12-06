using PaintTogetherLibrary;

namespace PaintTogether
{
    public partial class Form1 : Form
    {
        public PaintPointMessage Message { get; set; }

        private Bitmap bitmap { get; set; }

        public AutoResetEvent MessageIsReadyEvent;

        public Form1()
        {
            InitializeComponent();

            Message = new PaintPointMessage();
            MessageIsReadyEvent = new AutoResetEvent(false);

            bitmap = new Bitmap(150, 100);

            ControlOn(nameTextBox);
            ControlOn(startButton);

            ControlOff(paintingBox);
            ControlOff(playersTextBox);

            startButton.Click += (s, e) => Start();
            picture.MouseClick += (s, e) => Send(e);
        }

        private void Start()
        {
            if (nameTextBox.Text == null || nameTextBox.Text.Length == 0)
                return;

            var client = new ClientConnection();
            client.Form = this;
            Task.Run(() => client.ListenAsync(nameTextBox.Text));

            ControlOff(nameTextBox);
            ControlOff(startButton);

            ControlOn(paintingBox);
            ControlOn(playersTextBox);
        }

        private void ControlOff(Control control)
        {
            control.Enabled = false;
            control.Visible = false;
        }

        private void ControlOn(Control control)
        {
            control.Enabled = true;
            control.Visible = true;
        }

        private void Send(MouseEventArgs e)
        {
            var result = colorDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                lock (Message)
                {
                    Message.Color = colorDialog.Color;
                    Message.Location = e.Location;
                }

                Paint(Message);

                MessageIsReadyEvent.Set();
            }
        }

        public void Paint(PaintPointMessage message)
        {
            var location = new Point(message.Location.X / 4, message.Location.Y / 4);
            var color = message.Color;

            for (int i = -2; i < 3; i++)
                for (int j = -2; j < 3; j++)
                {
                    var x = location.X + i;
                    var y = location.Y + j;
                    var prevColor = bitmap.GetPixel(x, y);
                    var newColor = MixColors(prevColor, color, 0.75);
                    bitmap.SetPixel(x, y, newColor);
                }
            bitmap.SetPixel(location.X, location.Y, color);

            picture.Image = bitmap;
            picture.Invalidate();
        }

        private Color MixColors(Color first, Color second, double proportion)
        {
            var r = first.R * (1 - proportion) + second.R * proportion;
            var g = first.G * (1 - proportion) + second.G * proportion;
            var b = first.B * (1 - proportion) + second.B * proportion;
            var a = first.A * (1 - proportion) + second.A * proportion;

            var intR = Math.Min((int)r, 255);
            var intG = Math.Min((int)g, 255);
            var intB = Math.Min((int)b, 255);
            var intA = Math.Min((int)a, 255);

            return Color.FromArgb(intA, intR, intG, intB);
        }

        public void AddPlayer(string name)
        {
            playersTextBox.Lines = playersTextBox.Lines.Append(name).ToArray();
        }

        public void RemovePlayer(string name)
        {
            playersTextBox.Lines = playersTextBox.Lines.Except(new[] { name }).ToArray();
        }
    }
}