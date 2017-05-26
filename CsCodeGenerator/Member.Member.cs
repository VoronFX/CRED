using System;
using System.Collections.Generic;
using CsCodeGenerator.Interfaces.Member;

namespace CsCodeGenerator.Types
{
	internal partial class Member :
		IClass,
		IStruct,
		IInterface,
		IEnum,
		IProperty, 
		IField,
		IMethod,
		IConstant
	{

		IClass IGenerateClass.Class(string name, IEnumerable<IMember> content) =>
			Class(name, content);

		IClass IGenerateClass.Class(string name, IMember content) =>
			Class(name, content);

		IStruct IGenerateStruct.Struct(string name, IEnumerable<IMember> content) =>
			Struct(name, content);

		IStruct IGenerateStruct.Struct(string name, IMember content) =>
			Struct(name, content);

		IEnum IGenerateEnum.Enum(string name, IEnumerable<IMember> content) =>
			Enum(name, content);
		IEnum IGenerateEnum.Enum(string name, IMember content) =>
			Enum(name, content);

		IInterface IGenerateInterface.Interface(string name, IEnumerable<IMember> content) =>
			Interface(name, content);

		IInterface IGenerateInterface.Interface(string name, IMember content) =>
			Interface(name, content);

		IMethod IGenerateMethod.Method(string returnType, string parameters, string name, bool expressionBody, IEnumerable<string> content) =>
			Method(returnType, parameters, name, expressionBody, content);

		IMethod IGenerateMethod.Method(string returnType, string parameters, string name, bool expressionBody, string content) =>
			Method(returnType, parameters, name, expressionBody, content);

		IProperty IGenerateProperty.Property(string type, string name, bool expressionBody, IEnumerable<string> content) =>
			Property(type, name, expressionBody, content);

		IProperty IGenerateProperty.Property(string type, string name, bool expressionBody, string content) =>
			Property(type, name, expressionBody, content);

		IField IGenerateField.Field(string type, string name, IEnumerable<string> content) =>
			Field(type, name, content);

		IField IGenerateField.Field(string type, string name, string content) =>
			Field(type, name, content);

		IConstant IGenerateConstant.Const(string type, string name, IEnumerable<string> content) =>
			Const(type, name, content);

		IConstant IGenerateConstant.Const(string type, string name, string content) =>
			Const(type, name, content);
	}
}