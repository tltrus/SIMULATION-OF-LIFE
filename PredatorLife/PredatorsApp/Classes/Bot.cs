using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;


namespace WpfApp.Classes
{
    /*
     При столкновении двух особей, они будут умирать, и в случайных координатах будут появляться их дети (от 1 до 3). 
     чтобы число организмов не становилось слишком большим или слишком маленьким введено условие – 
     если численность популяции меньше 50, рождается больше детей (от 1 до 4), если численность больше 50, размножение будет медленнее.
     у каждой особи популяции должен быть свой генетический код. Для этого в программе создана отдельная структура кода. 

     Также необходима функция скрещивания, которая будет принимать генетические коды родителей и возвращать код ребенка. 
     У этих примитивных существ в коде заложены только минимальная и максимальная скорости, максимальная продолжительность жизни и максимальный угол поворота.
    */
    struct GEN
    {
        public int maxltime;                // максимальная продолжительность жизни
        public double minspeed, maxspeed;   // минимальная и максимальная скорости
        public int maxturn;                 // максимальный угол поворота
    };

    class Bot : Particle
    {
        public GEN code;


        public Bot()
        {
            width = MainWindow.width;
            height = MainWindow.height;

            isLive = true;
            radius = 1;
            range = 60;

            color = Brushes.White;

            var x = MainWindow.rnd.Next((int)radius, (int)(width - radius));
            var y = MainWindow.rnd.Next((int)radius, (int)(height - radius));
            pos = new Vector2D(x, y);

            var vx = MainWindow.rnd.NextDouble();
            var vy = MainWindow.rnd.NextDouble();
            velocity = new Vector2D(vx, vy);

            lifetime = 0;

            code.maxltime = 999999999;
            code.maxspeed = 2;
            code.minspeed = 1;
            code.maxturn = 2;
        }


        void Runaway(double speed, double x2, double y2)
        {
            Vector2D bot = new Vector2D(x2, y2);
            Vector2D target = Vector2D.Sub(pos, bot);
            target.Normalize();
            target.Mult(speed);

            velocity = target.CopyToVector();
        }

        public void Update(List<Predator> predators)
        {
            double x2 = 0, y2 = 0;

            if (isLive)
            {
                // Run away
                double min = range;
                bool see_enemy = false;

                foreach (var predator in predators)
                {
                    // засечен хищник. В min сохраняется минимальное расстояние
                    double dist = Vector2D.Dist(pos, predator.pos); // расстояние до хищника
                    if (dist < min)
                    {
                        min = dist;
                        see_enemy = true;
                        x2 = predator.pos.x;
                        y2 = predator.pos.y;
                    }
                }

                if (see_enemy)
                {
                    // если обнаружили хищника, то сваливаем на максимальной скорости
                    Runaway(code.maxspeed, x2, y2);
                }
                else
                {
                    if (MainWindow.rnd.NextDouble() < 0.3)
                        // повороты бота, задается случайный вектор и случайные градусы поворота
                        Turn(1, MainWindow.rnd.Next(-code.maxturn, code.maxturn));
                }

                pos.Add(velocity);
                CollX();
                CollY();

                // Life age
                lifetime++;
                if (lifetime > code.maxltime)
                    isLive = false;

                // Grow Up
                radius += 0.005;
            }
        }

        // Эта функция будет обнаруживать столкновения организмов, убивать их и создавать потомство.
        public void MakeChild(List<Bot> bots)
        {
            foreach (var neighbor in bots.ToList())
            {
                if (this.pos != neighbor.pos) // исключаем самого себя
                {
                    if ((lifetime > 200) && (neighbor.lifetime > 200))
                        if (Vector2D.Dist(this.pos, neighbor.pos) < radius + neighbor.radius)
                        {
                            isLive = false;
                            neighbor.isLive = false;

                            int c;
                            if (bots.Count < 50) // Если ботов на карте < ..., то рожаем от 1 до 4 ботов сразу
                                c = MainWindow.rnd.Next() % 4 + 1;
                            else
                                c = MainWindow.rnd.Next() % 3 + 1; // Если > ...  рожаем одного

                            // Цвет для новых детей
                            byte R = (byte)MainWindow.rnd.Next(255);
                            byte G = (byte)MainWindow.rnd.Next(255);
                            byte B = (byte)MainWindow.rnd.Next(255);
                            Brush newcolor = new SolidColorBrush(Color.FromRgb(R, G, B));

                            for (int j = 0; j < c; j++)
                            {
                                Bot b = new Bot();
                                b.SetColor(newcolor);
                                b.pos = this.pos.CopyToVector();
                                b.code = Childcode(code, neighbor.code); // Создаем детеныша
                                bots.Add(b);
                            }
                        }
                }
            }
        }

        // создание генома детеныша. Скрещивание + Мутация
        GEN Childcode(GEN c1, GEN c2)
        {
            int maxltime_mut = 2;
            double maxspeed_mut = 20;
            double minspeed_mut = 1;
            int maxturn_mut = 5;

            //СКРЕЩИВАНИЕ
            GEN c;
            c.maxltime = (c1.maxltime + c2.maxltime) / 2;
            c.minspeed = (c1.minspeed + c2.minspeed) / 2;
            c.maxspeed = (c1.maxspeed + c2.maxspeed) / 2;
            c.maxturn = (c1.maxturn + c2.maxturn) / 2;

            //МУТАЦИЯ
            c.maxltime += c.maxltime / 100 * ((MainWindow.rnd.Next() % (maxltime_mut * 2 + 1)) - maxltime_mut);
            c.maxspeed += c.maxspeed / 100 * ((MainWindow.rnd.Next() % (maxspeed_mut * 2 + 1)) - maxspeed_mut);
            c.minspeed += c.minspeed / 100 * ((MainWindow.rnd.Next() % (minspeed_mut * 2 + 1)) - minspeed_mut);
            c.maxturn += c.maxturn / 100 * (MainWindow.rnd.Next() % (maxturn_mut * 2 + 1) - maxturn_mut);
            return c;
        }
    }
}
