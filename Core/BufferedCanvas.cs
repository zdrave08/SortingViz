namespace SortingViz.Core
{
    public sealed class BufferedCanvas : Control
    {
        private Bitmap? _buffer;
        private readonly object _sync = new object();

        public BufferedCanvas(int size)
        {
            Size = new Size(size, size);
            Init();
        }

        public BufferedCanvas() : this(400) { }

        private void Init()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            CreateBuffer(Width, Height);
        }

        private void CreateBuffer(int w, int h)
        {
            lock (_sync)
            {
                _buffer?.Dispose();
                _buffer = new Bitmap(Math.Max(1, w), Math.Max(1, h));
                using (var g = Graphics.FromImage(_buffer))
                    g.Clear(Color.White);
            }
        }

        /// <summary>
        /// Bezbedno crtanje po internom bufferu (thread-safe).
        /// </summary>
        public void WithGraphics(Action<Graphics> draw)
        {
            lock (_sync)
            {
                if (_buffer == null) return;
                using (var g = Graphics.FromImage(_buffer))
                {
                    draw(g);
                }
            }
        }

        public void Present()
        {
            if (IsDisposed) return;
            if (InvokeRequired)
                BeginInvoke(new Action(() => { if (!IsDisposed) Invalidate(); }));
            else
                Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (_sync)
            {
                if (_buffer != null)
                    e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                CreateBuffer(Width, Height);
                Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_sync)
                {
                    _buffer?.Dispose();
                    _buffer = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
