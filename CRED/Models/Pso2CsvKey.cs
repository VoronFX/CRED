namespace A2SPA.Models
{
	public class Pso2CsvKey
	{
		/// <summary>
		/// Primary key
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// <para>Part of alternative key (Uniq) { Key1Id, Key2Id, Key3Id, Key4 }</para>
		/// </summary>
		public long Key1Id { get; set; }

		/// <summary>
		/// <para>Part of alternative key (Uniq) { Key1Id, Key2Id, Key3Id, Key4 }</para>
		/// </summary>
		public long Key2Id { get; set; }

		/// <summary>
		/// <para>Part of alternative key (Uniq) { Key1Id, Key2Id, Key3Id, Key4 }</para>
		/// </summary>
		public long Key3Id { get; set; }

		/// <summary>
		/// <para>Part of alternative key (Uniq) { Key1Id, Key2Id, Key3Id, Key4 }</para>
		/// </summary>
		public int Key4 { get; set; }

		public Pso2CsvKeyKey1 Key1 { get; set; }

		public Pso2CsvKeyKey2 Key2 { get; set; }

		public Pso2CsvKeyKey3 Key3 { get; set; }
	}
}