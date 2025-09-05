namespace SortingViz.Core
{
    public sealed class VisualSortArray : SortArray
    {
        private readonly BufferedCanvas _canvas;
        private readonly int _scale;
        private readonly object _lock = new object();
        private volatile bool _stopRequested;
        public readonly SortMetrics Metrics = new SortMetrics();
        private int _delayMs = 20; //deafult delay
        public int[]? Values => values;       

        private static readonly Color[] COLORS = new Color[]
        {
            Color.FromArgb(0x29,0x40,0x99), // Active
            Color.FromArgb(0x95,0x9E,0xBF), // Inactive
            Color.FromArgb(0xD4,0xBA,0x0D), // Compare highlight
            Color.FromArgb(0x25,0x96,0x3D), // Done
        };
        private static readonly Color BACKGROUND = Color.White;

        public VisualSortArray(int size, int scale)
        {
            size = Math.Max(2, size);
            _scale = Math.Max(1, scale);
            values = Enumerable.Range(1, size).ToArray();
            Shuffle();
            _canvas = new BufferedCanvas(size * _scale);            
            Redraw(0, values.Length, full: true);
        }

        public Control GetCanvas() => _canvas;
        public void RequestStop() => _stopRequested = true;
        private void CheckStop() 
        { 
            if (_stopRequested) throw new StopException(); 
        }

        // --- Paint helpers ---
        public override void SetActive(int index) => Highlight(index, COLORS[0]);
        public override void SetInactive(int index) => Highlight(index, COLORS[1]);
        public override void SetDone(int index) => Highlight(index, COLORS[3]);
        public override void SetActive(int start, int end) => Highlight(start, end, COLORS[0]);
        public override void SetInactive(int start, int end) => Highlight(start, end, COLORS[1]);
        public override void SetDone(int start, int end) => Highlight(start, end, COLORS[3]);

        private void Highlight(int index, Color c) => Highlight(index, index + 1, c);
        public void Highlight(int start, int end, Color c)
        {
            lock (_lock)
            {
                if (values == null) return;

                int w = _canvas.ClientSize.Width, h = _canvas.ClientSize.Height;
                if (w <= 0 || h <= 0) return;

                int scale = Math.Max(1, _scale);
                int max = values.Max();
                if (max <= 0) return;

                start = Math.Max(0, start);
                end = Math.Min(values.Length, end);

                _canvas.WithGraphics(g =>
                {
                    for (int i = start; i < end; i++)
                    {
                        int val = values[i];
                        int barH = (int)Math.Round((val / (float)max) * (h - 2));
                        if (barH < 0) barH = 0;

                        var rect = new Rectangle(i * scale, h - barH, scale, barH);

                        // očisti kolonu
                        using (var bg = new SolidBrush(Color.White))
                            g.FillRectangle(bg, i * scale, 0, scale, h);

                        // nacrtaj stubac
                        using (var br = new SolidBrush(c))
                            g.FillRectangle(br, rect);
                    }
                });

                _canvas.Present();
            }
        }

        private void Redraw(int start, int end, bool full = false)
        {
            lock (_lock)
            {
                if (values == null) return;
                int w = _canvas.ClientSize.Width, h = _canvas.ClientSize.Height;
                if (w <= 0 || h <= 0) return;

                int scale = Math.Max(1, _scale);
                int max = values.Max();
                if (max <= 0) return;

                start = Math.Max(0, start);
                end = Math.Min(values.Length, end);

                _canvas.WithGraphics(g =>
                {
                    if (full) g.Clear(Color.White);

                    using (var br = new SolidBrush(Color.FromArgb(0x95, 0x9E, 0xBF))) // siva inactive
                    {
                        for (int i = start; i < end; i++)
                        {
                            int val = values[i];
                            int barH = (int)Math.Round((val / (float)max) * (h - 2));
                            if (barH < 0) barH = 0;

                            var rect = new Rectangle(i * scale, h - barH, scale, barH);
                            g.FillRectangle(br, rect);
                        }
                    }
                });

                _canvas.Present();
            }
        }

        public void SetDelay(int ms) => _delayMs = Math.Max(0, ms);
        public void SmallDelay() 
        { 
            if (_delayMs > 0) 
                Thread.Sleep(_delayMs); 
        }

        // --- Primitive koje algoritmi koriste (uz METRIKE) ---
        public int Compare(int i, int j)
        {
            if(values == null) throw new InvalidOperationException("Array not initialized.");
            CheckStop();
            Metrics.Comparisons++;
            //Thread.Sleep(_delayMs);
            SmallDelay();
            SetActive(i); SetActive(j);
            Metrics.Steps++;
            return Math.Sign(values[i] - values[j]);
        }

        public void Swap(int i, int j)
        {
            if (values == null) throw new InvalidOperationException("Array not initialized.");
            CheckStop(); if (i == j) return;
            (values[i], values[j]) = (values[j], values[i]);
            Metrics.Swaps++;
            //Thread.Sleep(_delayMs);
            Metrics.Steps++;
            Highlight(Math.Min(i, j), Math.Max(i, j) + 1, COLORS[0]);
            SmallDelay();
        }

        public void MarkDone(int i) 
        { 
            SetDone(i); 
            Metrics.Steps++; 
        }

        public void MarkAllDone() 
        { 
            for (int i = 0; i < Length; i++) MarkDone(i); 
        }

        public int CompareValues(int v1, int v2)
        {
            Metrics.Comparisons++;
            return v1.CompareTo(v2);
        }
    }
}
