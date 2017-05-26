using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Types
{
	public interface INested :
		Nested.IGenerateProperty,
		Nested.IGenerateField,
		Nested.IGenerateClass,
		Nested.IGenerateStruct,
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
}
