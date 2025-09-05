using SortingViz.Core;

namespace SortingViz.Algorithms
{
    namespace SortingViz
    {
        public sealed class MergeSort : SortAlgorithm
        {
            public override string Name => "Merge";
            public override string ComplexityBest => "O(n log n)";
            public override string ComplexityAverage => "O(n log n)";
            public override string ComplexityWorst => "O(n log n)";

            public override void Sort(VisualSortArray a)
            {
                int[] aux = new int[a.Length];
                MergeSortRec(a, aux, 0, a.Length - 1);
            }

            private void MergeSortRec(VisualSortArray a, int[] aux, int left, int right)
            {
                if (left >= right) return;

                int mid = (left + right) / 2;

                MergeSortRec(a, aux, left, mid);
                MergeSortRec(a, aux, mid + 1, right);
                Merge(a, aux, left, mid, right);
            }

            private void Merge(VisualSortArray a, int[] aux, int left, int mid, int right)
            {
                // kopiraj originalne vrednosti u pomocni niz
                for (int k = left; k <= right; k++)
                    if(a.Values != null)
                        aux[k] = a.Values[k];

                int i = left, j = mid + 1;

                // vizuelno oboji podnizove (leva crveno, desna plavo)
                a.Highlight(left, mid + 1, Color.Red);
                a.Highlight(mid + 1, right + 1, Color.Blue);
                a.SmallDelay();

                for (int k = left; k <= right; k++)
                {
                    if (a.Values==null)
                    {
                        return;
                    }
                    if (i > mid) // leva polovina potrošena
                    {
                        a.Values[k] = aux[j++];
                    }
                    else if (j > right) // desna polovina potrošena
                    {
                        a.Values[k] = aux[i++];
                    }
                    else if (a.CompareValues(aux[i], aux[j]) <= 0)
                    {
                        a.Values[k] = aux[i++];
                    }
                    else
                    {
                        a.Values[k] = aux[j++];
                    }

                    // osveži stubac gde je element smešten
                    a.Highlight(k, k + 1, Color.FromArgb(0x29, 0x40, 0x99)); // tamno plava
                    a.SmallDelay();
                }
            }
        }
    }
}
