using System;

namespace Common
{
	public class Message
	{
		public string Id { get; set; }
		public string Body { get; set; }
		public DateTime? Date { get; set; }
		public string Direction { get; set; }
		public string ReponseStatus { get; set; }

		public override string ToString() => 
			$"ID: {Id}, Body: {Body}, Date: {Date}, Direction: {Direction}, Response Status: {ReponseStatus}";
	}
}