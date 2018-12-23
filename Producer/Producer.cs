using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProducerCommon
{
	public class Producer
	{
		private readonly IConnection connection;
		private readonly IModel channel;
		private readonly string replyQueueName;
		private readonly EventingBasicConsumer consumer;
		private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
		private readonly IBasicProperties props;
		private readonly string responseQueueName;
		private readonly string exchangeName;
		private readonly string routingKey;

		public Producer(string responeQueue, string exchange, string routing)
		{
			responseQueueName = responeQueue;
			exchangeName = exchange;
			routingKey = routing;

			var factory = new ConnectionFactory() { HostName = "localhost" };

			connection = factory.CreateConnection();
			channel = connection.CreateModel();
			channel.ExchangeDeclare(exchangeName, "direct");
			replyQueueName = channel.QueueDeclare(responseQueueName);
			channel.QueueBind(replyQueueName, exchangeName, routingKey);
			consumer = new EventingBasicConsumer(channel);

			props = channel.CreateBasicProperties();
			var correlationId = Guid.NewGuid().ToString();
			props.CorrelationId = correlationId;
			props.ReplyTo = replyQueueName;

			consumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var response = Encoding.UTF8.GetString(body);
				if (ea.BasicProperties.CorrelationId == correlationId)
				{
					respQueue.Add(response);
				}
			};
		}

		public string Call(string message)
		{
			var messageBytes = Encoding.UTF8.GetBytes(DateTime.Now.ToString(DateTimeFormatInfo.CurrentInfo));
			channel.BasicPublish(
				exchange: exchangeName,
				routingKey: routingKey,
				basicProperties: props,
				body: messageBytes);

			channel.BasicConsume(
				consumer: consumer,
				queue: replyQueueName,
				autoAck: true);

			return respQueue.Take();
		}

		public void Close()
		{
			connection.Close();
		}
	}
}
