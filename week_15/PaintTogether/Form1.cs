using PaintTogetherLibrary;
using System.Windows.Forms;

namespace PaintTogether
{
    public partial class Form1 : Form
    {
        public LinkedList<PaintPointMessage> PointsToSend = new LinkedList<PaintPointMessage>();
        public EventWaitHandle MessageReady = new EventWaitHandle(false, EventResetMode.ManualReset);

        private Bitmap bitmap;
        private Color currentColor
        {
            get { return colorDialog.Color; }
        }
        private bool isPainting = false;
        private PointF resizeCoef
        {
            get
            {
                var point = new PointF();
                point.X = picture.Width / (float)bitmap.Width;
                point.Y = picture.Height / (float)bitmap.Height;
                return point;
            }
        }

        public Form1()
        {
            InitializeComponent();

            bitmap = new Bitmap(150, 100);

            ControlOn(nameTextBox);
            ControlOn(startButton);

            ControlOff(paintingBox);
            ControlOff(playersTextBox);
            ControlOff(colorButton);

            startButton.Click += (s, e) => Start();
            nameTextBox.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    Start();
            };
            colorButton.Click += (s, e) => SelectColor();

            picture.MouseDown += (s, e) => { isPainting = true; };
            picture.MouseMove += (s, e) => PaintPoints(e);
            picture.MouseUp += (s, e) => { isPainting = false; };
        }

        private void SelectColor()
        {
            colorDialog.ShowDialog();
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
            ControlOn(colorButton);
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

        private void PaintPoints(MouseEventArgs e)
        {
            if (!isPainting)
                return;

            if (e.X < 0 || e.Y < 0 || 
                e.X >= picture.Width || 
                e.Y >= picture.Height) 
                return;

            var coef = resizeCoef;
            var location = new Point((int)Math.Round(e.X / coef.X), (int)Math.Round(e.Y / coef.Y));
            var color = currentColor;

            if (bitmap.GetPixel(location.X, location.Y) != color)
            {
                var message = new PaintPointMessage()
                {
                    Color = color,
                    Location = location
                };

                lock (PointsToSend)
                {
                    PointsToSend.AddLast(message);
                    if (PointsToSend.Count > 0)
                        MessageReady.Set();
                }

                PaintPoint(message);
            }
        }

        public void PaintPoint(PaintPointMessage message)
        {
            var location = new Point(message.Location.X, message.Location.Y);
            var color = message.Color;

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