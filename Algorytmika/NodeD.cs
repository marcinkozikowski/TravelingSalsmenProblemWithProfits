using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorytmika
{
    class NodeD
    {
        private int _index;
        private double _cost;

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

        public double Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                _cost = value;
            }
        }

        public NodeD(int _index, double _cost)
        {
            Index = _index;
            Cost = _cost;
        }
    }
}
