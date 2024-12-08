using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant.myClasses
{
    class Cell
    {
        public int x, y;
        public double foodPheremone, homePheremone;
        public Type type;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
            type = Type.EMPTY;
            foodPheremone = 0;
            homePheremone = 0;
        }
    }
}
