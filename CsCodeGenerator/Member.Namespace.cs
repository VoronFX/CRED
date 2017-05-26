using System;
using System.Collections.Generic;
using CsCodeGenerator.Interfaces.Namespace;

namespace CsCodeGenerator.Types
{
	internal partial class Member :
		IClass,
		IStruct,
		IInterface,
		IEnum
	{

		IClass IGenerateClass.Class(string name, IEnumerable<Interfaces.Member.IMember> content) =>
			Class(name, content);

		IClass IGenerateClass.Class(string name, Interfaces.Member.IMember content) => 
			Class(name, content);

		IStruct IGenerateStruct.Struct(string name, IEnumerable<Interfaces.Member.IMember> content) =>
			Struct(name, content);

		IStruct IGenerateStruct.Struct(string name, Interfaces.Member.IMember content) =>
			Struct(name, content);

		IEnum IGenerateEnum.Enum(string name, IEnumerable<Interfaces.Member.IMember> content) =>
			Enum(name, content);

		IEnum IGenerateEnum.Enum(string name, Interfaces.Member.IMember content) =>
			Enum(name, content);

		IInterface IGenerateInterface.Interface(string name, IEnumerable<Interfaces.Member.IMember> content) =>
			Interface(name, content);

		IInterface IGenerateInterface.Interface(string name, Interfaces.Member.IMember content) =>
			Interface(name, content);
	}
}