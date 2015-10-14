using System.Drawing;
using System.Windows.Forms;

namespace Demo.SmartWorkers.WinForm
{
	public class VisualCounter
	{
		private readonly TextBox _textbox;

		public VisualCounter(TextBox textbox, int maxWorkers)
		{
			_textbox = textbox;
			MaxWorkers = maxWorkers;
		}

		public int Counter { get; private set; }

		public void AddWorker()
		{
			Counter++;
			BindCounter();
		}

		public void RemoveWorker()
		{
			Counter--;
			BindCounter();
		}

		private void BindCounter()
		{
			_textbox.Parent.Invoke(new MethodInvoker(delegate()
			{
				_textbox.Text = Counter.ToString();
				_textbox.BackColor = Counter > 0 
					? Counter == MaxWorkers
						? Color.Blue
						: Color.Green 
					: Color.Red;
				_textbox.Refresh();
			}));
		}

		public int Id
		{
			get { return int.Parse(_textbox.Name.Replace("textBox", "")); }
		}

		public int MaxWorkers { get; private set; }
	}
}
