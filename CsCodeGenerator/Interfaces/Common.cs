using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Member;
using CsCodeGenerator.Interfaces.Namespace;

namespace CsCodeGenerator.Interfaces
{
	namespace Common
	{
		internal enum AccessModifiers
		{
			Public,
			Protected,
			Internal,
			Private,
			ProtectedInternal
		}

		internal enum MemberType
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

		public interface IGenerateNamespaceElement :
			Namespace.IGenerateClass,
			Namespace.IGenerateStruct,
			Namespace.IGenerateEnum,
			Namespace.IGenerateInterface { }


		public interface IGenerateMemberElement :
			Member.IGenerateClass,
			Member.IGenerateStruct,
			Member.IGenerateEnum,
			Member.IGenerateInterface,
			Member.IGenerateMethod,
			Member.IGenerateProperty,
			Member.IGenerateField,
			Member.IGenerateConstant
		{ }

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IPublic :
			IGenerateNamespaceElement,
			IGenerateMemberElement
		{
			Sealed.IPublic Sealed();
			Readonly.IPublic Readonly();
			Static.IPublic Static();
		}

		[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
		public interface IInternal :
			IGenerateNamespaceElement,
			IGenerateMemberElement
		{
			Sealed.IInternal Sealed();
			Readonly.IInternal Readonly();
			Static.IInternal Static();
		}

		public interface IProtected :
			IGenerateMemberElement
		{
			Sealed.IProtected Sealed();
			Readonly.IProtected Readonly();
			Static.IProtected Static();
		}

		public interface IPrivate :
			IGenerateMemberElement
		{
			Sealed.IPrivate Sealed();
			Readonly.IPrivate Readonly();
			Static.IPrivate Static();
		}

		public interface IProtectedInternal : 
			IGenerateMemberElement
		{
			Sealed.IProtectedInternal Sealed();
			Readonly.IProtectedInternal Readonly();
			Static.IProtectedInternal Static();
		}

		public interface IGenerateNamespace
		{
			INamespace Namespace();
		}

		public interface IGenerateComment
		{
			IComment Comment();
		}

		public interface IGenerateEmptyLine
		{
			IEmptyLine EmptyLine();
		}


		public interface IComment : IMember, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement
		{
		}

		public interface IEmptyLine : IMember, IDocumentBodyElement, IDocumentHeadElement, INamespaceElement
		{
		}

		public interface IDocument : IElement
		{
			void Save(string path);
		}

		public interface IElement
		{
			IEnumerable<string> Build();
		}
	}
}