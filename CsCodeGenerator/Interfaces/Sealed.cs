using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CsCodeGenerator.Interfaces;
using CsCodeGenerator.Types;

namespace CsCodeGenerator.Interfaces
{
	namespace Sealed
	{
		public interface ISealedMember :
			Member.IGenerateProperty,
			Member.IGenerateClass,
			Member.IGenerateMethod
		{ }

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IPublic : Namespace.IGenerateClass, ISealedMember
		{
			Readonly.IPublic Readonly();
		}

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IInternal : Namespace.IGenerateClass, ISealedMember
		{
			Readonly.IInternal Readonly();
		}

		public interface IProtected : ISealedMember
		{
			Readonly.IProtected Readonly();
		}

		public interface IPrivate : ISealedMember
		{
			Readonly.IPrivate Readonly();
		}

		public interface IProtectedInternal : ISealedMember
		{
			Readonly.IProtectedInternal Readonly();
		}

		namespace Readonly
		{
			public interface IPublic : Member.IGenerateProperty { }

			public interface IInternal : Member.IGenerateProperty { }

			public interface IProtected : Member.IGenerateProperty { }

			public interface IPrivate : Member.IGenerateProperty { }

			public interface IProtectedInternal : Member.IGenerateProperty { }
		}
	}
}
