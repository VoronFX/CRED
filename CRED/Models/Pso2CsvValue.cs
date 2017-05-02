using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRED.Models
{
    public class Pso2CsvValue
    {
		/// <summary>
		/// Primary key
		/// </summary>
		public int Id { get; set; }

	    public int FileId { get; set; }

	    public int FileItemId { get; set; }

		public string Value { get; set; }

		public DateTime Timestamp { get; set; }

		public Pso2CsvFileItem FileItem { get; set; }

		public Pso2CsvFile File { get; set; }

	}
}
