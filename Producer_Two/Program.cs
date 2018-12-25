using System;
using Common;
using ProducerCommon;


namespace Producer_Two
{
	class Program
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		static void Main(string[] args)
		{
			var rpcClient = new Producer(
				Helpers.ResponseQueueName,
				Helpers.ExchangeName,
				Helpers.RoutingKey,
				"Producer TWO");

			Console.WriteLine(" Producer Two");
			var startTimeSpan = TimeSpan.Zero;
			var periodTimeSpan = TimeSpan.FromSeconds(5);

			var timer = new System.Threading.Timer((e) =>
			{
				Console.WriteLine("Send Message");
				var response = rpcClient.Call();
				if (response.ReponseStatus == "FAIL")
				{
					log.Info(response);
				}

				Console.WriteLine(" [.] Got '{0}'", response);
			}, null, startTimeSpan, periodTimeSpan);

			Console.ReadLine();
			rpcClient.Close();
		}
	}
}
