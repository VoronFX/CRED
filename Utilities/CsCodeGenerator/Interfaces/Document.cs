using System;
using System.Collections.Generic;
using System.Text;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Namespace;

namespace CsCodeGenerator.Interfaces
{
    namespace Document
    {

		public interface IDocumentBodyElement : IElement
	    {
	    }

	    public interface IDocumentHeadElement : IElement
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
	}
}
