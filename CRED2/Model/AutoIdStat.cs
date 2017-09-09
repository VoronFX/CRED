using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace CRED2.Model
{
    public sealed class AutoIdStat
    {
		public string CollectionName { get; set; }

		public long LastAutoId { get; set; }
    }
}
