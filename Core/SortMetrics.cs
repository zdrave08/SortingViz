using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingViz.Core
{
    public sealed class SortMetrics
    {
        public long Comparisons { get; internal set; }
        public long Swaps { get; internal set; }
        public long Steps { get; internal set; } // svaki vidljiv korak
        public TimeSpan Elapsed => _sw.Elapsed;
        private readonly Stopwatch _sw = new Stopwatch();
        internal void Reset() { Comparisons = Swaps = Steps = 0; _sw.Reset(); }
        internal void Start() { _sw.Start(); }
        internal void Stop() { _sw.Stop(); }
    }
}
