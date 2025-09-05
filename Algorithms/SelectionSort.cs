using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class SelectionSort : SortAlgorithm
    {
        public override string Name => "Selection";
        public override string ComplexityBest => "O(n^2)";
        public override string ComplexityAverage => "O(n^2)";
        public override string ComplexityWorst => "O(n^2)";

        public override void Sort(VisualSortArray a)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                int min = i;
                for (int j = i + 1; j < a.Length; j++)
                    if (a.Compare(j, min) < 0) min = j;
                a.Swap(i, min);
            }
        }
    }
}
