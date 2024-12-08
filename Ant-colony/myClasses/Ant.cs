using System;

namespace Ant.myClasses
{
    class Ant
    {
        Simulation simulation;
        public int x;
        public int y;
        public int steps;
        int angle; //Angles are in increments of 45 degrees, clockwise, with 0 = north
        public bool carryingFood;
        PointP[] directions;
        Random rnd;


        public Ant(Simulation sim, int x, int y, Random rnd)
        {
            simulation = sim;
            this.x = x;
            this.y = y;
            steps = 0;
            angle = 0; //Angles are in increments of 45 degrees, clockwise, with 0 = north
            carryingFood = false;
            directions = new PointP[] {
                                        new PointP (0, -1),   //N     {x, y}
			                            new PointP (1, -1),   //NE
			                            new PointP (1, 0),    //E
			                            new PointP (1, 1),    //SE
			                            new PointP (0, 1),    //S
			                            new PointP (-1, 1),   //SW
			                            new PointP (-1, 0),   //W,
			                            new PointP (-1, -1)   //NW
                                    };
            this.rnd = rnd;
        }

        private int Lifespan() => Math.Max(simulation.vars.lifespan, 1);

        public bool isDead() => steps > Lifespan();


        // Возрождение
        public void RespawnAtCell(Cell c)
        {
            x = c.x;
            y = c.y;
            steps = 0;
            RandomizeDirection();
        }

        // Вероятность возрождения
        public bool ShouldRespawn() => rnd.Next(0, 1000) < 5;

        public void TurnLeft()
        {
            angle -= 1;
            if (angle < 0)
                angle = directions.Length - 1;
        }

        public void TurnRight()
        {
            angle += 1;
            angle = angle % directions.Length;
        }

        public PointP Forward()
        {
            PointP fwd = directions[angle];
            return fwd;
        }

        // В зависимости от того, куда смотрит муравей выбирает направление левого и правого датчиков
        public PointP[] Sensed()
        {
            PointP fwd = Forward();
            int i;
            for (i = 0; i < directions.Length; i++)
            {
                if (directions[i].x == fwd.x && directions[i].y == fwd.y)
                {
                    break;
                }
            }
            PointP fwdLeft = directions[i > 0 ? i - 1 : directions.Length - 1];
            PointP fwdRight = directions[(i + 1) % directions.Length];

            return new PointP[] { fwdLeft, fwd, fwdRight }; // возвращает положение трех датчиков
        }

        public void WalkRandomly()
        {
            PointP fwd = Forward();
            int action = rnd.Next(6);
            //Slightly more likely to move forwards than to turn
            if (action < 4)
            {
                x += fwd.x;
                y += fwd.y;
            }
            else if (action == 4)
            {
                TurnLeft();
            }
            else if (action == 5)
            {
                TurnRight();
            }
        }

        public void RandomizeDirection() => angle = rnd.Next(directions.Length);

        //d is a direction {x, y}
        //isFood indicates if we're scoring for food or home
        double GetScoreForDirection(PointP d, bool isFood)
        {
            int num = 0;
            //I keep meaning to sketch this out on graph paper - 
            //I'm certain I made a few logical errors in here.
            //However, I actually found this particular behavior (right or not)
            //worked pretty well, and my attempts at fixing it always made it worse!

            int range = simulation.vars.sight;

            int x0 = x + d.x * range;
            int y0 = y + d.y * range;
            double score = 0;
            for (var x = x0 - range / 2; x <= x0 + (range / 2); x++)
            {
                for (var y = y0 - (range / 2); y <= y0 + (range / 2); y++)
                {
                    num = simulation.GetCellnum(x, y);
                    if (num == -1) continue;
                    Cell c = simulation.cells[num];
                    var wScore = ScoreForCell(c, isFood);

                    wScore /= (Dist(x0, y0, x, y) + 1); //This is the bit that's probably wrong
                    score += wScore;
                }
            }

            num = simulation.GetCellnum(x + d.x, y + d.y);
            if (num == -1) return score;
            Cell fwdCell = simulation.cells[num];
            score += ScoreForCell(fwdCell, isFood);
            return score;
        }

        double ScoreForCell(Cell c, bool isFood)
        {
            if (c == null)
                return 0;
            else
            {
                if (isFood)
                {
                    if (c.type == Type.FOOD)
                    {
                        return 100;
                    }
                    else
                    {
                        return c.foodPheremone;
                    }
                }
                else
                {
                    if (c.type == Type.HOME)
                    {
                        return 100;
                    }
                    else
                    {
                        return c.homePheremone;
                    }
                }
            }
        }

        public void ForageForFood() => Seek(true);
        public void LookForHome() => Seek(false);

        public void Seek(bool isFood)
        {
            PointP[] sensed = this.Sensed();

            PointP fwdLeft = sensed[0];
            PointP fwd = sensed[1];
            PointP fwdRight = sensed[2];

            double maxScore = 0;
            var bestDirection = fwd;

            double[] scores = new double[sensed.Length];

            for (var i = 0; i < sensed.Length; i++)
            {
                PointP direction = sensed[i];
                double score = GetScoreForDirection(direction, isFood);
                scores[i] = score;
                if (score > maxScore)
                {
                    maxScore = score;
                    bestDirection = direction;
                }
            }

            //If no direction is particularly good, move at random.
            //There's also a 20% chance the ant moves randomly even 
            //if there is an optimal direction,
            //just to give them a little more interesting behavior.
            if (maxScore < 0.01 || rnd.NextDouble() < 0.2)
            {
                WalkRandomly();
                return;
            }
            if (bestDirection.x == fwdRight.x && bestDirection.y == fwdRight.y)
            {
                TurnRight();
            }
            else if (bestDirection.x == fwdLeft.x && bestDirection.y == fwdLeft.y)
            {
                TurnLeft();
            }
            else
            {
                x += fwd.x;
                y += fwd.y;
            }
        }

        // Длина вектора
        double Dist(int x0, int y0, int x, int y) => Math.Sqrt((x0 - x) * (x0 - x) + (y0 - y) * (y0 - y));
    }
}
