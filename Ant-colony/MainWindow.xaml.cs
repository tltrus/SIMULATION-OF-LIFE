using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ant.myClasses;


namespace Ant
{
    // Based on: Ant Colony Optimization copy by chaski
    // https://editor.p5js.org/chaski/sketches/pV-NF4gkH

    public enum Type
    {
        EMPTY   = 0,
        ANT     = 1, // не используется
        FOOD    = 2,
        HOME    = 3,
        WALL    = 4
    }

    struct PointP
    {
        public int x, y;
        public PointP(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public partial class MainWindow : Window
    {
        Random rnd = new Random();
        WriteableBitmap wb;
        int imgWidth, imgHeight;
        System.Windows.Threading.DispatcherTimer timer;
        Simulation sim;

        public MainWindow()
        {
            InitializeComponent();

            imgWidth = (int)img.Width; imgHeight = (int)img.Height;

            // Для Битмапа
            wb = new WriteableBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Bgra32, null); img.Source = wb;

            // Таймер
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            sim = new Simulation(imgWidth, imgHeight, rnd);

            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            sim.Run();
            sim.Draw(wb);
        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => sim.AddFood((int)e.GetPosition(img).X, (int)e.GetPosition(img).Y);
    }
}
