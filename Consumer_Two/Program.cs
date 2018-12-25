using System;
using Common;
using ConsumerCommon;

namespace Consumer_Two
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcConsumerTwo =
				new Consumer(
					Helpers.QueueName,
					Helpers.ExchangeName,
					Helpers.RoutingKey);

			Console.WriteLine("Consumer Two");

			Console.WriteLine(" Press [enter] to exit.");
			Console.ReadLine();
			rpcConsumerTwo.Close();
		}
	}
}
