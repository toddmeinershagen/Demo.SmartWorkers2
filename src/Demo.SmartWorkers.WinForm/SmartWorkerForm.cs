using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Demo.SmartWorkers.WinForm
{
	public partial class SmartWorkerForm : Form
	{
		private const string AlgorithmListText = "[Choose an Algorithm]";

		public SmartWorkerForm()
		{
			InitializeComponent();

			_algorithmList.Text = AlgorithmListText;
			Algorithm[] algorithms = 
			{
				new Algorithm() {Text = "Centralized - Priority Order", Function = RunSmartWorkersInPriorityOrder},
				new Algorithm() {Text = "Decentralized - Random Order", Function = RunSmartWorkersInRandom},
				new Algorithm() {Text = "Decentralized - Random Start, Sequence Order", Function = RunSmartWorkersInSequence}
			};

			_algorithmList.Items.AddRange(algorithms);
		}

		public class Algorithm
		{
			public string Text { get; set; }
			public Action<SmartWorker, VisualCounter[]> Function { get; set; }
		}

		public delegate void UpdateTextBoxMethod(string id);
		public UpdateTextBoxMethod MyMethod;

		public void UpdateTextBox(string id)
		{
			var textbox = Controls[id];
			if (textbox.InvokeRequired)
			{
				Invoke(MyMethod, id);
			}
			else
			{
				UpdateTextBox(id);
			}
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			if (_algorithmList.Text == AlgorithmListText)
			{
				return;
			}

			ThreadPool.QueueUserWorkItem(RunClock, clockLabel);

			var counters = new[]
			{
				new VisualCounter(textBox1, 3),
				new VisualCounter(textBox2, 10),
				new VisualCounter(textBox3, 3),
				new VisualCounter(textBox4, 3),
				new VisualCounter(textBox5, 4),
				new VisualCounter(textBox6, 3),
				new VisualCounter(textBox7, 3),
				new VisualCounter(textBox8, 3),
				new VisualCounter(textBox9, 6),
				new VisualCounter(textBox10, 3),
				new VisualCounter(textBox11, 1),
				new VisualCounter(textBox12, 2),
				new VisualCounter(textBox13, 3),
				new VisualCounter(textBox14, 3),
				new VisualCounter(textBox15, 1),
				new VisualCounter(textBox16, 3),
				new VisualCounter(textBox17, 8),
				new VisualCounter(textBox18, 3),
				new VisualCounter(textBox19, 3),
				new VisualCounter(textBox20, 9),
				new VisualCounter(textBox21, 3),
				new VisualCounter(textBox22, 3),
				new VisualCounter(textBox23, 7),
				new VisualCounter(textBox24, 2),
				new VisualCounter(textBox25, 1),
			};

			var state = new State {AlgorithmList = _algorithmList, Counters = counters};

			Enumerable.Range(1, 100)
				.ToList()
				.ForEach(index => ThreadPool.QueueUserWorkItem(RunSmartWorker, state));
		}

		public class State
		{
			public ComboBox AlgorithmList { get; set; }
			public VisualCounter[] Counters { get; set; }
			public ManualResetEvent ResetEvent { get; set; }
		}

		private void RunSmartWorker(object state)
		{
			var castState = state as State;
			var counters = castState.Counters;
			var worker = new SmartWorker();
			
			Algorithm algorithm = null;
			var algorithmList = castState.AlgorithmList;
			algorithmList.Parent.Invoke(new MethodInvoker(delegate()
			{
				algorithm = algorithmList.SelectedItem as Algorithm;
			}));

			algorithm.Function(worker, counters);
		}

		private void RunClock(object state)
		{
			var stopWatch = new Stopwatch();

			while (true)
			{
				stopWatch.Start();
				Thread.Sleep(1000);
				stopWatch.Stop();

				var elapsed = stopWatch.Elapsed;
				var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);

				var label = (Label)state;
				label.Parent.Invoke(new MethodInvoker(delegate()
				{
					label.Text = elapsedTime;
				}));
			}
		}

		private void RunSmartWorkersInSequence(SmartWorker worker, VisualCounter[] counters)
		{
			var index = RandomNumber.Next();

			while (true)
			{
				worker.Execute(counters[index]);

				if (++index > 24)
					index = 0;
			}
		}

		private void RunSmartWorkersInRandom(SmartWorker worker, VisualCounter[] counters)
		{
			while (true)
			{
				var index = RandomNumber.Next();
				worker.Execute(counters[index]);
			}
		}

		private void RunSmartWorkersInPriorityOrder(SmartWorker worker, VisualCounter[] counters)
		{
			while (true)
			{
				var item = counters
					.Select((counter, i) => new Tuple<int, VisualCounter>(i, counter))
					.Where(x => x.Item2.Counter != x.Item2.MaxWorkers)
					.OrderBy(x => x.Item2.Counter)
					.FirstOrDefault();

				if (item != null)
				{
					worker.Execute(counters[item.Item1]);
				}
			}
		}
	}
}
