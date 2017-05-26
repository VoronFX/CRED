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
