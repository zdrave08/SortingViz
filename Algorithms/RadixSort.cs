using SortingViz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingViz.Algorithms
{
    public sealed class RadixSort : SortAlgorithm
    {
        public override string Name => "Radix (LSD)";
        public override string ComplexityBest => "O(nk)";
        public override string ComplexityAverage => "O(nk)";
        public override string ComplexityWorst => "O(nk)";

        // Pretpostavka: svi brojevi su >= 0 (bez negativnih).
        // Ako ima negativnih, vidi napomenu ispod.
        public override void Sort(VisualSortArray a)
        {
            int n = a.Length;
            if (n <= 1) return;

            // pronađi max za broj cifara
            int max = 0;
            var arr = a.Values; // direktan pristup si već izložio

            if (arr == null) return;

            for (int i = 0; i < n; i++) if (arr[i] > max) max = arr[i];

            int[] output = new int[n];
            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                // stabilno brojanje po cifri (0-9)
                int[] count = new int[10];

                // 1) prebroj
                for (int i = 0; i < n; i++)
                {
                    int digit = (arr[i] / exp) % 10;
                    count[digit]++;
                }

                // 2) prefiksi
                for (int d = 1; d < 10; d++)
                    count[d] += count[d - 1];

                // 3) stabilno popuni output (unazad)
                for (int i = n - 1; i >= 0; i--)
                {
                    int digit = (arr[i] / exp) % 10;
                    output[--count[digit]] = arr[i];
                }

                // 4) kopiraj nazad i vizuelizuj
                for (int i = 0; i < n; i++)
                {
                    arr[i] = output[i];

                    // vizuelni hint: oboji element kad „legne“
                    // leve polovine: crveno, desne: plavo — čisto da se vidi raspored
                    var color = (i < n / 2) ? Color.Red : Color.Blue;
                    a.Highlight(i, i + 1, color);
                    a.SmallDelay();
                }
            }

            // gotovo — ostatak sistema će već MarkAllDone() obaviti,
            // ili možeš ovde: for (int i = 0; i < n; i++) a.MarkDone(i);
        }
    }
}
