using SortingViz.Algorithms;
using SortingViz.Algorithms.SortingViz;
using SortingViz.Core;

namespace SortingViz
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SortingViz(new SortAlgorithm[]
                {
                    new BubbleSort(),
                    new InsertionSort(),
                    new SelectionSort(),
                    new ShellSort(),
                    new QuickSort(),
                    new HeapSort(),
                    new MergeSort(),
                    new RadixSort()
                }));
        }
    }
}