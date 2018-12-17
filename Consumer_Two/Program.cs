using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer_Two
{
	class Program
	{
		private const int CONSUMER_NUMBER = 1;
		private const string QUEUE_NAME = "rpc_queue_TWO";
		private const string EXHANGE_NAME = "4Prod_4Consumer_Exchange";
		static void Main(string[] args)
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			using (var connection = factory.CreateConnection())
			using (var channel = connection.CreateModel())
			{
				channel.ExchangeDeclare(EXHANGE_NAME, "direct");
				channel.QueueDeclare(queue: QUEUE_NAME, durable: false,
					exclusive: false, autoDelete: false, arguments: null);
				channel.QueueBind(queue: QUEUE_NAME, exchange: EXHANGE_NAME,
					routingKey: "2");
				channel.BasicQos(0, 1, false);
				var consumer = new EventingBasicConsumer(channel);
				channel.BasicConsume(queue: QUEUE_NAME,
					autoAck: false, consumer: consumer);

				Console.WriteLine(" [x] Awaiting RPC requests");

				consumer.Received += (model, ea) =>
				{
					//Message response = new Message();
					string response = null;

					var body = ea.Body;
					var props = ea.BasicProperties;
					var replyProps = channel.CreateBasicProperties();
					replyProps.CorrelationId = props.CorrelationId;

					try
					{
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine($" Received message({message.ToString()}) at: {DateTime.Now}");
						//response.Text = $" Received message({message.ToString()}) at: {DateTime.Now}";
						//response.Status = CheckProducerTimeAndNumber();
						response = message.ToString() + CheckProducerTimeAndNumber();

					}
					catch (Exception e)
					{
						Console.WriteLine(" [.] " + e.Message);
						response = null;
					}
					finally
					{
						var responseBytes = Encoding.UTF8.GetBytes(response);
						channel.BasicPublish(exchange: EXHANGE_NAME, routingKey: props.ReplyTo,
							basicProperties: replyProps, body: responseBytes);
						channel.BasicAck(deliveryTag: ea.DeliveryTag,
							multiple: false);
					}
				};

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
		}


		/// <summary>
		/// Check whether seconds(System Time) equals Consumer NUmber
		/// </summary>
		/// <returns></returns>
		private static string CheckProducerTimeAndNumber() =>
			DateTime.Now.Second == CONSUMER_NUMBER ? "FAIL" : "OK";
	}
}