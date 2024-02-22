using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using WpfApp.Classes;


namespace WpfApp
{
    /// <summary>
    /// ЭВОЛЮЦИЯ С ХИЩНИКОМ
    /// Based on: https://habr.com/ru/post/264433/
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public static Random rnd = new Random();
        public static double width, height;

        DrawingVisual visual;
        DrawingContext dc;

        System.Windows.Threading.DispatcherTimer Controltimer, Drawtimer;

        List<Bot> bots = new List<Bot>();
        List<Predator> predators = new List<Predator>();

        public MainWindow()
        {
            InitializeComponent();

            width = g.Width; height = g.Height;
            visual = new DrawingVisual();
            
            Controltimer = new System.Windows.Threading.DispatcherTimer();
            Controltimer.Tick += new EventHandler(ControltimerTick);
            Controltimer.Interval = new TimeSpan(0, 0, 0, 0, 50);


            for (int i = 0 ; i < 50; i++)
                bots.Add(new Bot());

            for (int i = 0; i < 5; i++)
                predators.Add(new Predator());

            Update();
        }

        private void ControltimerTick(object sender, EventArgs e) => Update();
        private void BtnStart_Click(object sender, RoutedEventArgs e) => Controltimer.Start();

        private void Update()
        {
            g.RemoveVisual(visual);

            using (dc = visual.RenderOpen())
            {
                foreach (var predator in predators)
                {
                    predator.Update(ref bots);

                    // Range
                    if (cbRange.IsChecked == true)
                        dc.DrawEllipse(null, new Pen(Brushes.White, 0.3), predator.pos.ToPoint(), (int)predator.range, (int)predator.range);
                    // Body
                    dc.DrawEllipse(Brushes.Red, null, predator.pos.ToPoint(), (int)predator.radius, (int)predator.radius);
                }


                foreach (var bot in bots.ToList())
                {
                    if (bot.isLive == false)
                    {
                        bots.RemoveAt(bots.IndexOf(bot)); // удаляем дохлого бота из списка
                        continue;
                    }
                    bot.MakeChild(bots);
                    bot.Update(predators);
                    dc.DrawEllipse(bot.color, null, bot.pos.ToPoint(), (int)bot.radius, (int)bot.radius);
                }

                dc.Close();
                g.AddVisual(visual);
            }
        }
    }
}
