using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Producer_Two
{
	class Program
	{
		static void Main(string[] args)
		{
			var rpcClient = new ProducerTwo();

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
