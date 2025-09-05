using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class QuickSort : SortAlgorithm
    {
        public override string Name => "Quick";
        public override string ComplexityBest => "O(n log n)";
        public override string ComplexityAverage => "O(n log n)";
        public override string ComplexityWorst => "O(n^2)";

        public override void Sort(VisualSortArray a)
        {
            QuickSortRec(a, 0, a.Length - 1);
        }

        private void QuickSortRec(VisualSortArray a, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(a, low, high);
                QuickSortRec(a, low, pi - 1);
                QuickSortRec(a, pi + 1, high);
            }
        }

        private int Partition(VisualSortArray a, int low, int high)
        {
            int pivot = high;
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (a.Compare(j, pivot) <= 0)
                {
                    i++;
                    a.Swap(i, j);
                }
            }
            a.Swap(i + 1, high);
            return i + 1;
        }
    }
}
