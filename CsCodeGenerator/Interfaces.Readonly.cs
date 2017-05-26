using System;
using System.Collections.Generic;
using System.Text;

namespace CsCodeGenerator.Types.Readonly
{
	public interface IReadonly : Nested.IGenerateProperty, Nested.IGenerateField { }

	public interface IPublic : IReadonly { }

	public interface IInternal : IReadonly { }

	public interface IProtected : IReadonly { }

	public interface IPrivate : IReadonly { }

	public interface IProtectedInternal : IReadonly { }
}
