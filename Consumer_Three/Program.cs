using System;
using Common;
using ConsumerCommon;

namespace Consumer_Three
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcConsumerOne =
				new Consumer(
					Helpers.QueueName,
					Helpers.ExchangeName,
					Helpers.RoutingKey,
					"Consumer THREE");

			Console.WriteLine("Consumer Three");

			Console.ReadLine();
			rpcConsumerOne.Close();
		}
	}
}
