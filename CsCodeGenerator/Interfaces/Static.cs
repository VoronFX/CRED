using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CsCodeGenerator.Types;

namespace CsCodeGenerator.Interfaces
{
	namespace Static
	{
		public interface IStaticMember :
			Member.IGenerateProperty,
			Member.IGenerateField,
			Member.IGenerateClass,
			Member.IGenerateStruct,
			Member.IGenerateMethod
		{ }

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IPublic : Namespace.IGenerateClass, IStaticMember
		{
			Readonly.IPublic Readonly();
		}

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IInternal : Namespace.IGenerateClass, IStaticMember
		{
			Readonly.IInternal Readonly();
		}

		public interface IProtected : IStaticMember
		{
			Readonly.IProtected Readonly();
		}

		public interface IPrivate : IStaticMember
		{
			Readonly.IPrivate Readonly();
		}

		public interface IProtectedInternal : IStaticMember
		{
			Readonly.IProtectedInternal Readonly();
		}
	}
}

