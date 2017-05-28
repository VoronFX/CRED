using System;
using System.Collections.Generic;
using System.Text;
using CsCodeGenerator.Interfaces.Common;
using CsCodeGenerator.Interfaces.Document;
using CsCodeGenerator.Interfaces.Member;

namespace CsCodeGenerator.Interfaces
{
    namespace Namespace
    {
	    //Class
	    //Struct,
	    //Interface,
	    //Enum,

	    //Property,
	    //Field,
	    //Method,
	    //Constant

	    public interface IClass : INamespaceElement
	    {
	    }

	    public interface IStruct : INamespaceElement
	    {
	    }

	    public interface IInterface : INamespaceElement
	    {
	    }

	    public interface IEnum : INamespaceElement
	    {
	    }

		public interface INamespaceElement : IElement
	    {
	    }
	}
}
