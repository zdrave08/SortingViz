using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class ShellSort : SortAlgorithm
    {
        public override string Name => "Shell";
        public override string ComplexityBest => "O(n log n)";
        public override string ComplexityAverage => "O(n^(3/2))";
        public override string ComplexityWorst => "O(n^2)";

        public override void Sort(VisualSortArray a)
        {
            int n = a.Length;
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    int j = i;
                    while (j >= gap && a.Compare(j - gap, j) > 0)
                    {
                        a.Swap(j - gap, j);
                        j -= gap;
                    }
                }
            }
        }
    }
}
