﻿using System;
using System.Text;
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumerCommon
{
	public class Consumer
	{
		private readonly IConnection connection;
		private readonly IModel channel;
		private readonly string queueName;
		private readonly string exchangeName;
		private readonly string routingKey;


		public Consumer(string queue, string exchange, string routing)
		{
			queueName = queue;
			routingKey = routing;
			exchangeName = exchange;

			var factory = new ConnectionFactory() { HostName = "localhost" };
			connection = factory.CreateConnection();
			channel = connection.CreateModel();

			channel.ExchangeDeclare(exchangeName, "direct");
			channel.QueueDeclare(queue: queueName, durable: false,
				exclusive: false, autoDelete: false, arguments: null);
			channel.QueueBind(queue: queueName, exchange: exchangeName,
				routingKey: routingKey);
			channel.BasicQos(0, 1, false);
			var consumer = new EventingBasicConsumer(channel);
			channel.BasicConsume(queue: queueName,
				autoAck: false, consumer: consumer);

			Console.WriteLine(" [x] Awaiting RPC requests");

			consumer.Received += (model, ea) =>
			{
				Message response = new Message
				{
					ReponseStatus = "consumer"
				};

				var body = ea.Body;
				var props = ea.BasicProperties;
				var replyProps = channel.CreateBasicProperties();
				replyProps.CorrelationId = props.CorrelationId;

				try
				{
					var message = Encoding.UTF8.GetString(body);
					Message receivedMessage = JsonConvert.DeserializeObject<Message>(message);
					Console.WriteLine($" Received message({message.ToString()}) at: {DateTime.Now}");
					//response = receivedMessage;
					//response.ReponseStatus = CheckProducerTimeAndNumber();
				}
				catch (Exception e)
				{
					Console.WriteLine(" [.] " + e.Message);
					response = null;
				}
				finally
				{
					//var responseBytes = Encoding.UTF8.GetBytes(response);
					var jsonString = JsonConvert.SerializeObject(response);
					var responseBytes = Encoding.UTF8.GetBytes(jsonString);
					channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
						basicProperties: replyProps, body: responseBytes);
					channel.BasicAck(deliveryTag: ea.DeliveryTag,
						multiple: false);
				}
			};
		}

		public void Close()
		{
			connection.Close();
		}

		/// <summary>
		/// Check whether seconds(System Time) equals Consumer Number
		/// </summary>
		/// <returns></returns>
		private static string CheckProducerTimeAndNumber() =>
			DateTime.Now.Second.ToString() == Helpers.ConsumerNumber ? "FAIL" : "OK";
	}
}
