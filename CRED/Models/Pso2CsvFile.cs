using System;
using System.Collections.Generic;

namespace CRED.Models
{
	public class Pso2CsvFile
	{
		/// <summary>
		/// Primary key
		/// </summary>
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime Timestamp { get; set; }

		public List<Pso2CsvFileItem> Items { get; set; }
	}
}