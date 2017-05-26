using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Types.Sealed
{
	internal interface INested :
		Nested.IGenerateProperty,
		Nested.IGenerateClass,
		Nested.IGenerateMethod
	{ }

	public interface IPublic : IGenerateClass, INested
	{
		Readonly.IPublic Readonly();
	}

	public interface IInternal : IGenerateClass, INested
	{
		Readonly.IInternal Readonly();
	}

	public interface IProtected : INested
	{
		Readonly.IProtected Readonly();
	}

	public interface IPrivate : INested
	{
		Readonly.IPrivate Readonly();
	}

	public interface IProtectedInternal : INested
	{
		Readonly.IProtectedInternal Readonly();
	}

	public struct Readonly
	{
		public interface IPublic : Nested.IGenerateProperty { }

		public interface IInternal : Nested.IGenerateProperty { }

		public interface IProtected : Nested.IGenerateProperty { }

		public interface IPrivate : Nested.IGenerateProperty { }

		public interface IProtectedInternal : Nested.IGenerateProperty { }
	}
}
