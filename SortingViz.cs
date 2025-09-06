using SortingViz.Core;
using System.Windows.Forms.DataVisualization.Charting;

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

        private readonly TrackBar _delay;
        private readonly Label _delayLbl;

        private readonly ComboBox _dataset;
        private readonly Chart _chart;

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

            _dataset = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            _dataset.Items.AddRange(new object[] { "Random", "Sorted", "Reversed", "NearlySorted", "FewUniques" });
            _dataset.SelectedIndex = 0;

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

            // Graf metrika
            _chart = new Chart { Dock = DockStyle.Fill, Height = 220 };
            var area = new ChartArea("main");
            area.AxisX.Title = "n (array size)";
            area.AxisY.Title = "Count / ms";
            _chart.ChartAreas.Add(area);
            _chart.Legends.Add(new Legend { Docking = Docking.Bottom });

            _chart.Series.Add(new Series("Comparisons") { ChartType = SeriesChartType.Line, BorderWidth = 2, MarkerStyle = MarkerStyle.Circle });
            _chart.Series.Add(new Series("Swaps") { ChartType = SeriesChartType.Line, BorderWidth = 2, MarkerStyle = MarkerStyle.Circle });
            _chart.Series.Add(new Series("Elapsed ms") { ChartType = SeriesChartType.Line, BorderWidth = 2, MarkerStyle = MarkerStyle.Circle });

            // Root layout: svako u svoj red
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3 };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // top
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));            // metrics
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));        // big-O / sadržaj dole
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            root.Controls.Add(top, 0, 0);
            root.Controls.Add(_metricsLbl, 0, 1);
            root.Controls.Add(_bigOLbl, 0, 2);
            root.Controls.Add(_chart, 0, 3);

            Controls.Add(root);

            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(720, 260);

            UpdateBigO();
            _algo.SelectedIndexChanged += (s, e) => UpdateBigO();
        }

        public enum DatasetType { Random, Sorted, Reversed, NearlySorted, FewUniques }

        public static class Datasets
        {
            public static int[] Generate(int n, DatasetType type, int seed = 0)
            {
                var rnd = seed == 0 ? new Random() : new Random(seed);
                var arr = Enumerable.Range(1, n).ToArray();

                switch (type)
                {
                    case DatasetType.Random:
                        for (int i = n - 1; i > 0; i--) 
                        { 
                            int j = rnd.Next(i + 1); (arr[i], arr[j]) = (arr[j], arr[i]); 
                        }
                        break;

                    case DatasetType.Sorted:
                        // već sortirano
                        break;

                    case DatasetType.Reversed:
                        Array.Reverse(arr);
                        break;

                    case DatasetType.NearlySorted:
                        // ~5% nasumičnih swapova
                        int k = Math.Max(1, n / 20);
                        for (int t = 0; t < k; t++)
                        {
                            int i = rnd.Next(n), j = rnd.Next(n);
                            (arr[i], arr[j]) = (arr[j], arr[i]);
                        }
                        break;

                    case DatasetType.FewUniques:
                        // mali broj različitih vrednosti
                        int kinds = Math.Max(2, Math.Min(7, n / 10));
                        for (int i = 0; i < n; i++) arr[i] = (rnd.Next(kinds) + 1) * (n / kinds);
                        break;
                }
                return arr;
            }
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

            var type = (DatasetType)_dataset.SelectedIndex; // redosled se poklapa sa Items
            int[] initial = Datasets.Generate(size, type);

            var array = new VisualSortArray(size, scale);
            var algo = _algos[_algo.SelectedIndex];
            array.LoadValues(initial);
            array.SetDelay(_delay.Value);
            var thread = new SortThread(array, algo);


            var frame = new SortFrame($"{algo.Name} sort", array.GetCanvas(), thread)
            { 
                Size = new Size(size * scale + 16, size * scale + 39) 
            };

            thread.Completed += () =>
            {
                // Thread → UI
                if (InvokeRequired) 
                    BeginInvoke(new Action(UpdateMetrics)); 
                else 
                    UpdateMetrics();

                void UpdateMetrics()
                {
                    var m = array.Metrics;
                    _metricsLbl.Text = $"Metrics: Comparisons = {m.Comparisons:N0}, Swaps = {m.Swaps:N0}, Steps = {m.Steps:N0}, Elapsed = {m.Elapsed.TotalMilliseconds:N0} ms";

                    _chart.Series["Comparisons"].Points.AddXY(size, m.Comparisons);
                    _chart.Series["Swaps"].Points.AddXY(size, m.Swaps);
                    _chart.Series["Elapsed ms"].Points.AddXY(size, m.Elapsed.TotalMilliseconds);
                }
            };


            frame.Show();
            thread.Start();
        }
    }
}
