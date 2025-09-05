using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class InsertionSort : SortAlgorithm
    {
        public override string Name => "Insertion";
        public override string ComplexityBest => "O(n)";
        public override string ComplexityAverage => "O(n^2)";
        public override string ComplexityWorst => "O(n^2)";

        public override void Sort(VisualSortArray a)
        {
            for (int i = 1; i < a.Length; i++)
            {
                int j = i;
                while (j > 0 && a.Compare(j - 1, j) > 0)
                {
                    a.Swap(j - 1, j);
                    j--;
                }
            }
        }
    }
}
