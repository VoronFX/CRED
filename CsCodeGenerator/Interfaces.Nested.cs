using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Types.Nested
{
	public interface IClass : IClassElement
	{
	}

	public interface IStruct : IClassElement
	{
	}

	public interface IInterface : IClassElement
	{
	}

	public interface IEnum : IClassElement
	{
	}

	public interface IGenerateClass
	{
		IClass Class();
	}

	public interface IProperty : IClassElement
	{
	}

	public interface IField : IClassElement
	{
	}

	public interface IMethod : IClassElement
	{
	}

	public interface IConstant : IClassElement
	{
	}

	public interface IGenerateField
	{
		IField Field();
	}

	public interface IGenerateProperty
	{
		IProperty Property();
	}

	public interface IGenerateMethod
	{
		IMethod Method();
	}

	public interface IGenerateStruct
	{
		IStruct Struct();
	}
}
