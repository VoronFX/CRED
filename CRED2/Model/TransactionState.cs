using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace CRED2.Model
{
    public class TransactionState
    {
	    public long Id { get; set; }

		public bool Complete { get; set; }
    }
}
