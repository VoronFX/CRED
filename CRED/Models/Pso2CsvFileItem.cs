using System.Collections.Generic;

namespace CRED.Models
{
	public class Pso2CsvFileItem
	{
		public int Id { get; set; }

		public int FileId { get; set; }

		public FileItemState State { get; set; }

		public int ValueId { get; set; }

		public long KeyId { get; set; }

		public Pso2CsvFile File { get; set; }

		public List<Pso2CsvValue> Values { get; set; }

		public Pso2CsvKey Key { get; set; }
	}

	public enum FileItemState
	{
		Undone = 1, Review = 2, Completed = 3, Locked = 4
	}
}