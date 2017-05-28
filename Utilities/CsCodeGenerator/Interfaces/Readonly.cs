using System;
using System.Collections.Generic;
using System.Text;
using CsCodeGenerator.Interfaces.Member.Nested;

namespace CsCodeGenerator.Interfaces
{
	namespace Readonly
	{

		public interface IReadonlyMember : IClassModifiers, IPropertyModifiers { }

		public interface IPublic : IReadonlyMember { }

		public interface IInternal : IReadonlyMember { }

		public interface IProtected : IReadonlyMember { }

		public interface IPrivate : IReadonlyMember { }

		public interface IProtectedInternal : IReadonlyMember { }
	}
}
