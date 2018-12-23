using System;
using Common;
using ConsumerCommon;

namespace ConsumerOne
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcConsumerOne =
				new Consumer(
					Helpers.QueueName,
					Helpers.ExchangeName,
					Helpers.RoutingKey);

			Console.WriteLine("Consumer One");

			Console.ReadLine();
			rpcConsumerOne.Close();
		}
	}
}

