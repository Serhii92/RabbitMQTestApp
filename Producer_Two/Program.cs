using System;
using Common;
using ProducerCommon;


namespace Producer_Two
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcClient = new Producer(
				Helpers.ResponseQueueName,
				Helpers.ExchangeName,
				Helpers.RoutingKey);

			Console.WriteLine(" Producer Two");
			var startTimeSpan = TimeSpan.Zero;
			var periodTimeSpan = TimeSpan.FromSeconds(5);

			var timer = new System.Threading.Timer((e) =>
			{
				Console.WriteLine("Send Message");
				var response = rpcClient.Call("");

				Console.WriteLine(" [.] Got '{0}'", response);
			}, null, startTimeSpan, periodTimeSpan);

			Console.ReadLine();
			rpcClient.Close();
		}
	}
}
