using SortingViz.Core;

namespace SortingViz.Algorithms
{
    public sealed class BubbleSort : SortAlgorithm
    {
        public override string Name => "Bubble";
        public override string ComplexityBest => "O(n)";
        public override string ComplexityAverage => "O(n^2)";
        public override string ComplexityWorst => "O(n^2)";

        public override void Sort(VisualSortArray a)
        {
            for (int i = 0; i < a.Length - 1; i++)
                for (int j = 0; j < a.Length - i - 1; j++)
                    if (a.Compare(j, j + 1) > 0)
                        a.Swap(j, j + 1);
        }
    }
}
