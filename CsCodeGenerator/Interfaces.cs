using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Interfaces
{
	private enum AccessModifiers
	{
		Public,
		Protected,
		Internal,
		Private,
		ProtectedInternal
	}

	private enum MemberType
	{
		Class,
		Struc,
		Interface,
		Enum,
		Property,
		Field,
		Method,
		Constant
	}

	public interface IPublic
	{

	}
	public interface IInternal { }
	public interface IProtected { }
	public interface IPrivate { }
	public interface IProtectedInternal { }

	public interface IMemberType
	{
		IClass Class(IEnumerable<IClassElement> content);

		//Struc,
		//Interface,
		//Enum,
		//Property,
		//Field,
		//Method,
		//Constant

		IComment SummaryComment(IEnumerable<string> content);

		IComment SummaryComment(string content);
		IComment Comment(IEnumerable<string> content);
		IComment Comment(string content);
		IEmptyLine EmptyLine();
	}

	public interface IGenerateClass
	{
		IClass Class();
	}

	public interface IGenerateStruct
	{
		IStruct Struct();
	}

	public interface IDocument : IClassElement, INamespaceElement
	{
	}

	public interface IClass : Nested.IClass, INamespaceElement
	{
	}

	public interface IStruct : Nested.IStruct, INamespaceElement
	{
	}

	public interface IInterface : Nested.IInterface, INamespaceElement
	{
	}

	public interface IEnum : Nested.IEnum, INamespaceElement
	{
	}



	public interface INamespace : IDocumentBodyElement, INamespaceElement
	{
		string Name { get; }
		INamespaceElement[] Elements { get; }
	}

	public interface IUsing : IDocumentHeadElement
	{
	}

	public interface IComment : IClassElement, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement
	{
	}

	public interface IEmptyLine : IClassElement, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement
	{
	}

	public interface IClassElement : IElement
	{
	}

	public interface IDocumentBodyElement : IElement
	{
	}

	public interface IDocumentHeadElement : IElement
	{
	}

	public interface INamespaceElement : IElement
	{
	}

	public interface IElement
	{
		IEnumerable<string> Build();
	}
}
