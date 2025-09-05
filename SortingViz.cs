using SortingViz.Core;

namespace SortingViz
{
    public sealed partial class SortingViz : Form
    {
        private readonly ComboBox _algo;
        private readonly TextBox _size;
        private readonly TextBox _scale;
        private readonly Button _start;
        private readonly Label _metricsLbl;
        private readonly Label _bigOLbl;
        private readonly List<SortAlgorithm> _algos;
        private readonly Dictionary<string, (string Best, string Avg, string Worst, string Space)> _bigO;

        private TrackBar _delay;
        private Label _delayLbl;

        public SortingViz(IEnumerable<SortAlgorithm> algos)
        {
            Text = "MagicSort – Metrics & Big-O";
            _algos = new List<SortAlgorithm>(algos);

            _bigO = new Dictionary<string, (string, string, string, string)>(StringComparer.OrdinalIgnoreCase)
            {
                ["Bubble"] = ("Ω(n)", "Θ(n²)", "O(n²)", "O(1)"),
                ["Insertion"] = ("Ω(n)", "Θ(n²)", "O(n²)", "O(1)"),
                ["Selection"] = ("Ω(n²)", "Θ(n²)", "O(n²)", "O(1)"),
                ["Shell"] = ("Ω(n log n)", "Θ(? – zavisi od gap)", "O(n²)", "O(1)"),
                ["Quick"] = ("Ω(n log n)", "Θ(n log n)", "O(n²)", "O(log n)"),
                ["Heap"] = ("Ω(n log n)", "Θ(n log n)", "O(n log n)", "O(1)"),
                ["Merge"] = ("Ω(n log n)", "Θ(n log n)", "O(n log n)", "O(n)"),
                ["Radix (LSD)"] = ("Ω(nk)", "Θ(nk)", "O(nk)", "O(n + k)"),
            };

            // Gornji panel: 10 kolona (poslednja za Start)
            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 10,
                AutoSize = true,           // << ključ da se ne “seče”
                Padding = new Padding(8)
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 0  "Algo:"
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));  // 1  _algo
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 2  "Size:"
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));   // 3  _size
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 4  "Scale:"
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));   // 5  _scale
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 6  "Delay:"
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));   // 7  _delay (širi se)
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 8  _delayLbl
            top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));       // 9  _start

            _algo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            foreach (var a in _algos) _algo.Items.Add(a.Name);
            if (_algo.Items.Count > 0) _algo.SelectedIndex = 0;

            _size = new TextBox { Text = "60", Dock = DockStyle.Fill };
            _scale = new TextBox { Text = "8", Dock = DockStyle.Fill };

            _delay = new TrackBar { Minimum = 0, Maximum = 200, TickFrequency = 10, Value = 20, Dock = DockStyle.Fill };
            _delayLbl = new Label { Text = "20 ms", AutoSize = true, Padding = new Padding(8, 6, 0, 0) };
            _delay.ValueChanged += (s, e) => _delayLbl.Text = $"{_delay.Value} ms";

            _start = new Button { Text = "Start", AutoSize = true };
            _start.Click += (s, e) => StartSorting();

            // Raspored u jednoj vrsti
            top.Controls.Add(new Label { Text = "Algo:", AutoSize = true, Padding = new Padding(0, 6, 0, 0) }, 0, 0);
            top.Controls.Add(_algo, 1, 0);
            top.Controls.Add(new Label { Text = "Size:", AutoSize = true, Padding = new Padding(8, 6, 0, 0) }, 2, 0);
            top.Controls.Add(_size, 3, 0);
            top.Controls.Add(new Label { Text = "Scale:", AutoSize = true, Padding = new Padding(8, 6, 0, 0) }, 4, 0);
            top.Controls.Add(_scale, 5, 0);
            top.Controls.Add(new Label { Text = "Delay:", AutoSize = true, Padding = new Padding(8, 6, 0, 0) }, 6, 0);
            top.Controls.Add(_delay, 7, 0);
            top.Controls.Add(_delayLbl, 8, 0);
            top.Controls.Add(_start, 9, 0);

            _metricsLbl = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 6, 8, 6) };
            _bigOLbl = new Label { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8, 0, 8, 8) };

            // Root layout: svako u svoj red
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // top
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // metrics
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));        // big-O / sadržaj dole

            root.Controls.Add(top, 0, 0);
            root.Controls.Add(_metricsLbl, 0, 1);
            root.Controls.Add(_bigOLbl, 0, 2);

            Controls.Add(root);

            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(720, 260);

            UpdateBigO();
            _algo.SelectedIndexChanged += (s, e) => UpdateBigO();
        }

        private void UpdateBigO()
        {
            string key = _algo.SelectedItem?.ToString() ?? "Bubble";
            if (_bigO.TryGetValue(key, out var o))
                _bigOLbl.Text = $"Big‑O – {key}: Best {o.Best}, Average {o.Avg}, Worst {o.Worst}, Space {o.Space}";
        }

        private void StartSorting()
        {
            if (!int.TryParse(_size.Text, out int size) || size <= 1) { MessageBox.Show("Invalid size"); return; }
            if (!int.TryParse(_scale.Text, out int scale) || scale <= 0) { MessageBox.Show("Invalid scale"); return; }


            var array = new VisualSortArray(size, scale);
            var algo = _algos[_algo.SelectedIndex];
            array.SetDelay(_delay.Value);
            var thread = new SortThread(array, algo);


            var frame = new SortFrame($"{algo.Name} sort", array.GetCanvas(), thread)
            { Size = new Size(size * scale + 16, size * scale + 39) };


            thread.Completed += () =>
            {
                // Thread → UI
                if (InvokeRequired) BeginInvoke(new Action(UpdateMetrics)); else UpdateMetrics();
                void UpdateMetrics()
                {
                    var m = array.Metrics;
                    _metricsLbl.Text = $"Metrics: Comparisons = {m.Comparisons:N0}, Swaps = {m.Swaps:N0}, Steps = {m.Steps:N0}, Elapsed = {m.Elapsed.TotalMilliseconds:N0} ms";
                }
            };


            frame.Show();
            thread.Start();
        }
    }
}
