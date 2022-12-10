using PaintTogetherLibrary;
using System.Windows.Forms;

namespace PaintTogether
{
    public partial class Form1 : Form
    {
        public LinkedList<Pixel> PointsToSend = new LinkedList<Pixel>();
        public EventWaitHandle MessageReady = new EventWaitHandle(false, EventResetMode.ManualReset);

        private Bitmap bitmap = new Bitmap(150, 100);
        private Color currentColor { get { return colorDialog.Color; } }
        private bool isPainting = false;

        private Point? lastLocation;

        public Form1()
        {
            InitializeComponent();

            ToStartScreen();

            startButton.Click += (s, e) => Start();
            nameTextBox.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    Start();
            };
            colorButton.Click += (s, e) => ShowColorDialog();

            picture.MouseDown += (s, e) => { isPainting = true; };
            picture.MouseMove += (s, e) => PaintPoints(e);
            picture.MouseUp += (s, e) => { isPainting = false; lastLocation = null; };
            picture.Image = bitmap;
            picture.Invalidate();
        }

        private void ToStartScreen(bool state = true)
        {
            SwitchState(nameTextBox, state);
            SwitchState(startButton, state);

            SwitchState(paintingBox, !state);
            SwitchState(playersTextBox, !state);
            SwitchState(colorButton, !state);
        }

        private void SwitchState(Control control, bool state = false)
        {
            control.Enabled = state;
            control.Visible = state;
        }

        private void ShowColorDialog()
        {
            colorDialog.ShowDialog();
        }

        private void Start()
        {
            if (nameTextBox.Text == null || nameTextBox.Text.Length == 0)
                return;

            ToStartScreen(false);

            var client = new ClientConnection();
            client.Form = this;
            Task.Run(() => client.ListenAsync(nameTextBox.Text));
        }

        private void PaintPoints(MouseEventArgs e)
        {
            if (isPainting && picture.IsInBounds(e.Location))
            {
                var coef = picture.GetScale();
                var end = new PointF(e.X / coef.X, e.Y / coef.Y).Round();
                var color = currentColor;

                var points = lastLocation == null
                    ? new[] { end }
                    : GetMiddlePoints(lastLocation.Value, end);
                lastLocation = end;

                var messages = points
                    .Where(x => bitmap.GetPixel(x) != color)
                    .Select(x => x.ToMessage(color));

                lock(PointsToSend) { PointsToSend.AddLastRange(messages); }
                PaintPoint(messages.ToArray());

                lock(PointsToSend)
                {
                    if (PointsToSend.Count > 0)
                        MessageReady.Set();
                }
            }
        }

        private IEnumerable<Point> GetMiddlePoints(Point start, Point end)
        {
            var count = Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y));
            var difX = (end.X - start.X) / (float)count;
            var difY = (end.Y - start.Y) / (float)count;

            yield return start;
            for (int i = 1; i < count; i++)
            {
                yield return new PointF(start.X + difX * i, start.Y + difY * i).Round();
            }
            yield return end;
        }

        public void PaintPoint(params Pixel[] messages)
        {
            foreach (var message in messages)
            {
                bitmap.SetPixel(message.Location, message.Color);
            }

            picture.Image = bitmap;
            picture.Invalidate();
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