using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CsCodeGenerator.Types;
using TopLevel = CsCodeGenerator.Interfaces.Member.TopLevel;
using Nested = CsCodeGenerator.Interfaces.Member.Nested;

namespace CsCodeGenerator.Interfaces
{
	namespace Static
	{
		public interface IStaticMember :
			Nested.IPropertyModifiers,
			Nested.IFieldModifiers,
			Nested.IClassModifiers,
			Nested.IStructModifiers,
			Nested.IMethodModifiers
		{ }
		
		public interface IPublic : TopLevel.IClassModifiers, IStaticMember
		{
			Readonly.IPublic Readonly { get; }
		}
		
		public interface IInternal : TopLevel.IClassModifiers, IStaticMember
		{
			Readonly.IInternal Readonly { get; }
		}

		public interface IProtected : IStaticMember
		{
			Readonly.IProtected Readonly { get; }
		}

		public interface IPrivate : IStaticMember
		{
			Readonly.IPrivate Readonly { get; }
		}

		public interface IProtectedInternal : IStaticMember
		{
			Readonly.IProtectedInternal Readonly { get; }
		}
	}
}

