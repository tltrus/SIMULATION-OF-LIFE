using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant.myClasses
{
    class SimulationVars
    {
        public int lifespan;
        public int sight;
        public double foodPheremoneDecay;
        public double homePheremoneDecay;

        public SimulationVars()
        {
            lifespan = 5000;
            sight = 3;
            foodPheremoneDecay = 0.99;
            homePheremoneDecay = 0.99;
        }
    }
}
