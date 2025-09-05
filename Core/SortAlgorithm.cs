using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingViz.Core
{
    public abstract class SortAlgorithm
    {
        public abstract void Sort(VisualSortArray array);
        public abstract string Name { get; }
        public override string ToString() => Name;

        // Big-O informacije
        public abstract string ComplexityBest { get; }
        public abstract string ComplexityAverage { get; }
        public abstract string ComplexityWorst { get; }
    }
}
