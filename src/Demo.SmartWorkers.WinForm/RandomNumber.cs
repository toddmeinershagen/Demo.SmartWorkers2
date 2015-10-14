using System;

namespace Demo.SmartWorkers.WinForm
{
	public class RandomNumber
	{
		private static readonly Random Instance = new Random();
		private static readonly Type LockObject = typeof (RandomNumber);

		static RandomNumber()
		{ }

		public static int Next()
		{
			lock (LockObject)
			{
				return Instance.Next(0, 25);
			}
		}
	}
}
