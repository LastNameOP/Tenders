using System;
namespace Testovoe.Models
{
	public class TenderJson
	{
		public string Title { get; set; } = "";
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Url { get; set; } = "";
	}
}
