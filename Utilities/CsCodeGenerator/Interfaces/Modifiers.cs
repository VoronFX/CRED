using System;
using System.Collections.Generic;
using System.Text;
using CsCodeGenerator.Interfaces.Common;

namespace CsCodeGenerator.Interfaces
{
	//Class
	//Struct,
	//Interface,
	//Enum,

	//Property,
	//Field,
	//Method,
	//Constant
	namespace Member
	{
		namespace TopLevel
		{

			public interface IClassModifiers : IModifiersBuilderResult { }

			public interface IStructModifiers : IModifiersBuilderResult { }

			public interface IInterfaceModifiers : IModifiersBuilderResult { }

			public interface IEnumModifiers : IModifiersBuilderResult { }
		}

		namespace Nested
		{
			public interface IClassModifiers : IModifiersBuilderResult { }

			public interface IStructModifiers : IModifiersBuilderResult { }

			public interface IInterfaceModifiers : IModifiersBuilderResult { }

			public interface IEnumModifiers : IModifiersBuilderResult { }

			public interface IPropertyModifiers : IModifiersBuilderResult { }

			public interface IFieldModifiers : IModifiersBuilderResult { }

			public interface IMethodModifiers : IModifiersBuilderResult { }

			public interface IConstantModifiers : IModifiersBuilderResult { }
		}


	}
}
