using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Member;
using CsCodeGenerator.Interfaces.Namespace;
using CsCodeGenerator.Types;
using TopLevel = CsCodeGenerator.Interfaces.Member.TopLevel;
using Nested = CsCodeGenerator.Interfaces.Member.Nested;

namespace CsCodeGenerator.Interfaces
{
	namespace Common
	{
		public interface IClassModifiersBuilder
		{
			IPublic Public { get; }
			IInternal Internal { get; }
			IProtected Protected { get; }
			IPrivate Private { get; }
			IProtectedInternal ProtectedInternal { get; }
		}

		public interface IModifiersBuilder
		{
			IPublic Public { get; }
			IInternal Internal { get; }
			IProtected Protected { get; }
			IPrivate Private { get; }
			IProtectedInternal ProtectedInternal { get; }
		}

		public interface IModifiersBuilderResult
		{
			Modifiers Modifiers { get; }
		}

		public interface ITopLevelMemberModifiers :
			TopLevel.IClassModifiers,
			TopLevel.IStructModifiers,
			TopLevel.IEnumModifiers,
			TopLevel.IInterfaceModifiers
		{ }


		public interface INestedMemberModifiers :
			Nested.IClassModifiers,
			Nested.IStructModifiers,
			Nested.IEnumModifiers,
			Nested.IInterfaceModifiers,
			Nested.IMethodModifiers,
			Nested.IPropertyModifiers,
			Nested.IFieldModifiers,
			Nested.IConstantModifiers
		{ }
		
		public interface IPublic :
			ITopLevelMemberModifiers,
			INestedMemberModifiers
		{
			Sealed.IPublic Sealed { get; }
			Readonly.IPublic Readonly { get; }
			Static.IPublic Static { get; }
		}
		
		public interface IInternal :
			ITopLevelMemberModifiers,
			INestedMemberModifiers
		{
			Sealed.IInternal Sealed { get; }
			Readonly.IInternal Readonly { get; }
			Static.IInternal Static { get; }
		}

		public interface IProtected :
			INestedMemberModifiers
		{
			Sealed.IProtected Sealed { get; }
			Readonly.IProtected Readonly { get; }
			Static.IProtected Static { get; }
		}

		public interface IPrivate :
			INestedMemberModifiers
		{
			Sealed.IPrivate Sealed { get; }
			Readonly.IPrivate Readonly { get; }
			Static.IPrivate Static { get; }
		}

		public interface IProtectedInternal :
			INestedMemberModifiers
		{
			Sealed.IProtectedInternal Sealed { get; }
			Readonly.IProtectedInternal Readonly { get; }
			Static.IProtectedInternal Static { get; }
		}

		public interface IComment : IMember, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement
		{
		}

		public interface ILines : IMember, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement, IEnumerable<string>
		{
		}

		public interface IDocument : IElement
		{
			IDocumentWithHead Head(params IDocumentHeadElement[] content);
			IDocumentWithHead Body(params IDocumentBodyElement[] content);
		}

		public interface IDocumentWithHead : IElement
		{
			IDocumentWithHead Body(params IDocumentBodyElement[] content);
			void Save(string path);
		}

		public interface IElement
		{
			IEnumerable<string> Build();
		}
	}
}