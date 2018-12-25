using System;
using Common;
using ConsumerCommon;

namespace Consumer_Four
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
					"Consumer FOUR");

			Console.WriteLine("Consumer Four");

			Console.ReadLine();
			rpcConsumerOne.Close();
		}
	}
}
