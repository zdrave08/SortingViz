namespace SortingViz.Core
{
    public sealed class SortThread
    {
        private readonly SortAlgorithm _algorithm;
        private readonly VisualSortArray _array;
        private Thread? _thread; // Fix CS8618: make nullable
        public event Action? Completed; // Fix CS8618: make nullable

        public SortThread(VisualSortArray array, SortAlgorithm algorithm)
        {
            _array = array;
            _algorithm = algorithm;
        }

        public void Start()
        {
            _thread = new Thread(Run) { IsBackground = true };
            _thread.Start();
        }
        private void Run()
        {
            try 
            { 
                _array.Metrics.Reset(); 
                _array.Metrics.Start(); 
                _algorithm.Sort(_array); 
                _array.MarkAllDone(); 
            }
            catch (StopException) { }
            catch (ThreadInterruptedException) { }
            finally 
            { 
                _array.Metrics.Stop(); Completed?.Invoke(); 
            }
        }

        public void RequestStop() 
        { 
            _array.RequestStop(); 
            _thread?.Interrupt(); 
        }
    }
}
