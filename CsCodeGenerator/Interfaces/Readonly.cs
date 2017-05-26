using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Interfaces
{
	namespace Readonly
	{

		public interface IReadonlyMember : Member.IGenerateProperty, Member.IGenerateField { }

		public interface IPublic : IReadonlyMember { }

		public interface IInternal : IReadonlyMember { }

		public interface IProtected : IReadonlyMember { }

		public interface IPrivate : IReadonlyMember { }

		public interface IProtectedInternal : IReadonlyMember { }
	}
}
