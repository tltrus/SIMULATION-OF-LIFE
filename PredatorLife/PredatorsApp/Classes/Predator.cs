using System.Collections.Generic;


namespace WpfApp.Classes
{
    class Predator : Particle
    {
        public double hungry;

        public Predator()
        {
            width = MainWindow.width;
            height = MainWindow.height;

            isLive = true;
            radius = 3;
            range = MainWindow.rnd.Next(50, 150);
            speed = 3;
            hungry = 0;

            var x = MainWindow.rnd.Next((int)radius, (int)(width - radius));
            var y = MainWindow.rnd.Next((int)radius, (int)(height - radius));
            pos = new Vector2D(x, y);


            var vx = MainWindow.rnd.NextDouble();
            var vy = MainWindow.rnd.NextDouble();
            velocity = new Vector2D(vx, vy);

            lifetime = 0;
        }

        void Target(double speed, double x2, double y2)
        {
            Vector2D bot = new Vector2D(x2, y2);
            Vector2D target = Vector2D.Sub(bot, pos);
            target.Normalize();
            target.Mult(speed);

            velocity = target.CopyToVector();
        }

        public void Update(ref List<Bot> bots)
        {
            double x2 = 0, y2 = 0;

            if (true)
            {
                // Find the food
                bool follow = false;
                
                if (hungry <= 0)
                {
                    double min = range;

                    foreach (var bot in bots)
                    {
                        double dist = Vector2D.Dist(pos, bot.pos);


                        if (dist < radius + bot.radius + velocity.Mag())
                        {
                            bot.isLive = false;
                            hungry += 0.5;
                            radius += 0.005;
                        }
                        else
                        if (dist < min)
                        {
                            // target catched. min - minimal distance
                            min = dist;
                            follow = true;
                            x2 = bot.pos.x;
                            y2 = bot.pos.y;
                        }
                    }
                }

                if (follow)
                    Target(speed, x2, y2);
                else
                {
                    if (MainWindow.rnd.NextDouble() < 0.3)
                        Turn(1, MainWindow.rnd.NextDouble());
                }
                    

                pos.Add(velocity);
                CollX();
                CollY();

                // Hunger
                hungry -= 0.001;
            }
        }
    }
}
