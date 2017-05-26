using System.Collections.Generic;
using CsCodeGenerator.Interfaces.Common;

namespace CsCodeGenerator.Interfaces
{
	namespace Member
	{
		//Class
		//Struc,
		//Interface,
		//Enum,

		//Property,
		//Field,
		//Method,
		//Constant

		public interface IGenerateClass
		{
			IClass Class(string name, IEnumerable<IMember> content);
			IClass Class(string name, IMember content);
		}

		public interface IGenerateStruct
		{
			IStruct Struct(string name, IEnumerable<IMember> content);
			IStruct Struct(string name, IMember content);
		}

		public interface IGenerateInterface
		{
			IInterface Interface(string name, IEnumerable<IMember> content);
			IInterface Interface(string name, IMember content);
		}

		public interface IGenerateEnum
		{
			IEnum Enum(string name, IEnumerable<IMember> content);
			IEnum Enum(string name, IMember content);
		}

		public interface IGenerateField
		{
			IField Field(string type, string name, IEnumerable<string> content);
			IField Field(string type, string name, string content);
		}

		public interface IGenerateProperty
		{
			IProperty Property(string type, string name, bool expressionBody, IEnumerable<string> content);
			IProperty Property(string type, string name, bool expressionBody, string content);
		}

		public interface IGenerateMethod
		{
			IMethod Method(string returnType, string parameters, string name, bool expressionBody, IEnumerable<string> content);
			IMethod Method(string returnType, string parameters, string name, bool expressionBody, string content);
		}

		public interface IGenerateConstant
		{
			IConstant Const(string type, string name, IEnumerable<string> content);
			IConstant Const(string type, string name, string content);
		}

		public interface IMember : IElement
		{
		}

		public interface IClass : IMember
		{
		}

		public interface IStruct : IMember
		{
		}

		public interface IInterface : IMember
		{
		}

		public interface IEnum : IMember
		{
		}

		public interface IProperty : IMember
		{
		}

		public interface IField : IMember
		{
		}

		public interface IMethod : IMember
		{
		}

		public interface IConstant : IMember
		{
		}
	}
}
