using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class HeapSort : SortAlgorithm
    {
        public override string Name => "Heap";
        public override string ComplexityBest => "O(n log n)";
        public override string ComplexityAverage => "O(n log n)";
        public override string ComplexityWorst => "O(n log n)";

        public override void Sort(VisualSortArray a)
        {
            int n = a.Length;

            // build max-heap
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(a, n, i);

            // vadimo najveći i stavljamo na kraj
            for (int end = n - 1; end > 0; end--)
            {
                a.Swap(0, end);      // max na kraj
                Heapify(a, end, 0);  // re-heapify skraćenog heap-a
            }
        }

        private void Heapify(VisualSortArray a, int heapSize, int i)
        {
            while (true)
            {
                int largest = i;
                int l = 2 * i + 1;
                int r = 2 * i + 2;

                if (l < heapSize && a.Compare(l, largest) > 0) largest = l;
                if (r < heapSize && a.Compare(r, largest) > 0) largest = r;

                if (largest == i) return;

                a.Swap(i, largest);
                i = largest; // nastavljamo naniže
            }
        }
    }
}
