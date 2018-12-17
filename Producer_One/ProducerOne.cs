using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Producer_One
{
	public class ProducerOne
	{
		private readonly IConnection connection;
		private readonly IModel channel;
		private readonly string replyQueueName;
		private readonly EventingBasicConsumer consumer;
		private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
		private readonly IBasicProperties props;
		private const string RESPONSE_QUEUE_NAME = "rpc_response_queue_ONE";
		private const string EXHANGE_NAME = "4Prod_4Consumer_Exchange";


		public ProducerOne()
		{
			var factory = new ConnectionFactory() {HostName = "localhost"};

			connection = factory.CreateConnection();
			channel = connection.CreateModel();
			channel.ExchangeDeclare(EXHANGE_NAME, "direct");
			replyQueueName = channel.QueueDeclare(RESPONSE_QUEUE_NAME);
			channel.QueueBind(replyQueueName, EXHANGE_NAME, "1");
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
				exchange: EXHANGE_NAME,
				routingKey: "1",
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
