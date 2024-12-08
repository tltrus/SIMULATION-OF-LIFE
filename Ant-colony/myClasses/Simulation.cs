using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ant.myClasses
{
    // Based on: Ant Colony Optimization copy by chaski
    // https://editor.p5js.org/chaski/sketches/pV-NF4gkH

    class Simulation
    {
        List<Ant> ants;
        public List<Cell> cells;
        public SimulationVars vars = new SimulationVars();
        Cell home;
        List<Cell> foods;
        int width, height;
        Random rnd;

        Color homeColor = (Color)ColorConverter.ConvertFromString("#05445e");
        Color foodColor = (Color)ColorConverter.ConvertFromString("#189AB4");
        Color bgColor = (Color)ColorConverter.ConvertFromString("#d4f1f4");

        public Simulation(int w, int h, Random rnd)
        {
            width = w; height = h;
            this.rnd = rnd;
            ants = new List<Ant>();
            cells = new List<Cell>();
            vars = new SimulationVars();
            foods = new List<Cell>();

            int cellnum = 0;

            for  (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                    cells.Add(new Cell(x, y));

            // Формируем зону Home в центре поля
            int homeX = w / 2;
            int homeY = h / 2;
            for (int y = homeY; y <= homeY + 5; y++)
            {
                for (int x = homeX; x <= homeX + 5; x++)
                {
                    cellnum = GetCellnum(x, y);
                    cells[cellnum].type = Type.HOME;
                }
            }
            cellnum = GetCellnum(homeX, homeY);
            home = cells[cellnum];
            home.type = Type.HOME;

            // Формирование еды по умолчанию
            PointP auxfood = new PointP(home.x + 75, home.y + 25);
            for (int y = auxfood.y; y <= auxfood.y + 5; y++)  
            {
                for (int x = auxfood.x; x <= auxfood.x + 5; x++)
                {
                    cellnum = GetCellnum(x, y);
                    cells[cellnum].type = Type.FOOD;

                    foods.Add(cells[cellnum]);
                }
            }

            // Создание муравьев
            for (int i = 0; i < 200; i++)
                ants.Add(new Ant(this, homeX, homeY, rnd));
        }

        // получение номера индекса клетки одномерного массива клеток
        public int GetCellnum(int x, int y)
        {
            if (x < 0 || x >= width)
            {
                return -1;
            }
            if (y < 0 || y >= height)
            {
                return -1;
            }
            return x + y * width;
        }

        public void Run()
        {
            int num = 0;

            // Цикл по муравьям
            for (var i = 0; i < ants.Count; i++)
            {
                Ant a = ants[i];
                num = GetCellnum(a.x, a.y);
                Cell c = cells[num];

                // Если муравей мертв, то вновь возрождается дома
                if (a.isDead())
                {
                    if (a.ShouldRespawn())
                    {
                        a.RespawnAtCell(home);
                    }
                    continue;
                }

                PointP[] sensed = a.Sensed();
                PointP fwd = sensed[1];
                num = GetCellnum(a.x + fwd.x, a.y + fwd.y);
                
                // Если направление неудачное, например вышел за границу, то делаем случайное направление
                if (num == -1)
                {
                    a.RandomizeDirection();
                    continue;
                }

                Cell fwdCell = cells[num];

                if (a.carryingFood)
                {
                    //Look for home
                    if (fwdCell.type == Type.HOME)
                    {
                        //Drop food
                        a.carryingFood = false;

                        //Reset ttl
                        a.steps = 0;

                        //Turn around
                        a.TurnRight();
                        a.TurnRight();
                        a.TurnRight();
                        a.TurnRight();

                        a.ForageForFood();
                    }
                    else
                    {
                        a.LookForHome();
                    }
                }
                else
                {
                    //Look for food
                    if (fwdCell.type == Type.FOOD)
                    {
                        //Pick up food
                        a.carryingFood = true;
                        //Turn around
                        a.TurnRight();
                        a.TurnRight();
                        a.TurnRight();
                        a.TurnRight();

                        //Reset TTL
                        a.steps = 0;

                        ClearFood(fwdCell);

                        a.LookForHome();
                    }
                    else
                    {
                        a.ForageForFood();
                    }
                }

                if (!a.isDead() && c.type == Type.EMPTY && c.x != a.x && c.y != a.y)
                {
                    if (a.carryingFood)
                    {
                        c.foodPheremone += 1;
                    }
                    else
                    {
                        c.homePheremone += 1;
                    }
                }
                a.steps++;
            }

            // Цикл по клеткам
            for (var i = 0; i < cells.Count; i++)
            {
                Cell c = cells[i];
                if (c.foodPheremone > 0)
                {
                    c.foodPheremone *= vars.foodPheremoneDecay;
                }
                if (c.homePheremone > 0)
                {
                    c.homePheremone *= vars.homePheremoneDecay;
                }
            }
        }

        public void AddFood(int x, int y)
        {
            int num = GetCellnum(x, y);

            if (num == -1)  return;

            Cell c = cells[num];

            for (x = c.x - 2; x < c.x + 3; x++)
            {
                for (y = c.y - 2; y < c.y + 3; y++)
                {
                    num = GetCellnum(x, y);
                    Cell nc = cells[num];
                    if (nc != null && nc.type == Type.EMPTY)
                    {
                        nc.type = Type.FOOD;
                        foods.Add(nc);
                    }
                }
            }
        }

        void ClearFood(Cell c)
        {
            c.type = Type.EMPTY;
            int idx = foods.IndexOf(c);
            if (idx != -1)
            {
                foods.RemoveAt(idx);
            }
        }

        public void Draw(WriteableBitmap wb)
        {
            wb.Clear(bgColor);

            // Отображение дома
            wb.FillRectangle(home.x, home.y, home.x + 5, home.y + 5, homeColor);

            // Отображение еды
            for (int i = 0; i < foods.Count; i++)
            {
                Cell f = foods[i];
                wb.FillRectangle(f.x, f.y, f.x + 1, f.y + 1, foodColor);
            }

            // Отображение муравьев
            for (int i = 0; i < ants.Count; i++)
            {
                Ant a = ants[i];
                var antColor = (Color)ColorConverter.ConvertFromString("#000000");
                if (a.isDead())
                {
                    antColor = (Color)ColorConverter.ConvertFromString("#CCCCCC");
                }
                else if (a.carryingFood)
                {
                    antColor = foodColor;
                }
                else
                {
                    antColor = homeColor;
                }
                wb.FillRectangle(a.x, a.y, a.x + 2, a.y + 2, antColor);
            }
        }
    }
}
