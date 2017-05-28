using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CsCodeGenerator.Types;
using TopLevel = CsCodeGenerator.Interfaces.Member.TopLevel;
using Nested = CsCodeGenerator.Interfaces.Member.Nested;

namespace CsCodeGenerator.Interfaces
{
	namespace Sealed
	{
		public interface ISealedMember :
			Nested.IPropertyModifiers,
			Nested.IClassModifiers,
			Nested.IMethodModifiers
		{ }
		
		public interface IPublic : TopLevel.IClassModifiers, ISealedMember
		{
			Readonly.IPublic Readonly { get; }
		}
		
		public interface IInternal : TopLevel.IClassModifiers, ISealedMember
		{
			Readonly.IInternal Readonly { get; }
		}

		public interface IProtected : ISealedMember
		{
			Readonly.IProtected Readonly { get; }
		}

		public interface IPrivate : ISealedMember
		{
			Readonly.IPrivate Readonly { get; }
		}

		public interface IProtectedInternal : ISealedMember
		{
			Readonly.IProtectedInternal Readonly { get; }
		}

		namespace Readonly
		{
			public interface IPublic : Nested.IPropertyModifiers { }

			public interface IInternal : Nested.IPropertyModifiers { }

			public interface IProtected : Nested.IPropertyModifiers { }

			public interface IPrivate : Nested.IPropertyModifiers { }

			public interface IProtectedInternal : Nested.IPropertyModifiers { }
		}
	}
}
