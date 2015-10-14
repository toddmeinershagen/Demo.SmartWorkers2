using System.Threading;

namespace Demo.SmartWorkers.WinForm
{
	public class SmartWorker
	{
		public void Execute(VisualCounter counter)
		{
			if (counter.Counter >= counter.MaxWorkers) return;

			counter.AddWorker();


			for (var i = 0; i < 3; i++)
			{
				var ms = counter.Id%2 == 0
					? 1000
					: 2000;

				Thread.Sleep(ms);
			}

			counter.RemoveWorker();
		}
	}
}
