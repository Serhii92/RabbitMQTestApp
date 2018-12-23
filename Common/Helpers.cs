using System.Configuration;


namespace Common
{
	public static class Helpers
	{
		public static string QueueName =>
			ConfigurationManager.AppSettings.Get(ConfigurationConstants.QUEUE_NAME);

		public static string ResponseQueueName =>
			ConfigurationManager.AppSettings.Get(ConfigurationConstants.RESPONSE_QUEUE_NAME);

		public static string ExchangeName =>
			ConfigurationManager.AppSettings.Get(ConfigurationConstants.EXHANGE_NAME);

		public static string RoutingKey =>
			ConfigurationManager.AppSettings.Get(ConfigurationConstants.ROUTING_KEY);

		public static string ConsumerNumber =>
			ConfigurationManager.AppSettings.Get(ConfigurationConstants.CONSUMER_NUMBER);
	}
}
