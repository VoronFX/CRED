using System;
using System.Collections.Generic;
using CsCodeGenerator.Interfaces.Common;
using Sealed = CsCodeGenerator.Interfaces.Sealed;
using Static = CsCodeGenerator.Interfaces.Static;
using Readonly = CsCodeGenerator.Interfaces.Readonly;

namespace CsCodeGenerator.Types
{
	internal partial class Member :
		IPublic,
		Sealed.IPublic,
		Sealed.Readonly.IPublic,
		Static.IPublic,
		Readonly.IPublic
	{
		Sealed.IPublic IPublic.Sealed() => Sealed();

		Sealed.Readonly.IPublic Sealed.IPublic.Readonly() => Readonly();

		Static.IPublic IPublic.Static() => Static();

		Readonly.IPublic Static.IPublic.Readonly() => Readonly();

		Readonly.IPublic IPublic.Readonly() => Readonly();

	}

	internal partial class Member :
		IInternal,
		Sealed.IInternal,
		Sealed.Readonly.IInternal,
		Static.IInternal,
		Readonly.IInternal
	{
		Sealed.IInternal IInternal.Sealed() => Sealed();

		Sealed.Readonly.IInternal Sealed.IInternal.Readonly() => Readonly();

		Static.IInternal IInternal.Static() => Static();

		Readonly.IInternal Static.IInternal.Readonly() => Readonly();

		Readonly.IInternal IInternal.Readonly() => Readonly();

	}

	internal partial class Member :
		IPrivate,
		Sealed.IPrivate,
		Sealed.Readonly.IPrivate,
		Static.IPrivate,
		Readonly.IPrivate
	{
		Sealed.IPrivate IPrivate.Sealed() => Sealed();

		Sealed.Readonly.IPrivate Sealed.IPrivate.Readonly() => Readonly();

		Static.IPrivate IPrivate.Static() => Static();

		Readonly.IPrivate Static.IPrivate.Readonly() => Readonly();

		Readonly.IPrivate IPrivate.Readonly() => Readonly();

	}

	internal partial class Member :
		IProtected,
		Sealed.IProtected,
		Sealed.Readonly.IProtected,
		Static.IProtected,
		Readonly.IProtected
	{
		Sealed.IProtected IProtected.Sealed() => Sealed();

		Sealed.Readonly.IProtected Sealed.IProtected.Readonly() => Readonly();

		Static.IProtected IProtected.Static() => Static();

		Readonly.IProtected Static.IProtected.Readonly() => Readonly();

		Readonly.IProtected IProtected.Readonly() => Readonly();

	}

	internal partial class Member :
		IProtectedInternal,
		Sealed.IProtectedInternal,
		Sealed.Readonly.IProtectedInternal,
		Static.IProtectedInternal,
		Readonly.IProtectedInternal
	{
		Sealed.IProtectedInternal IProtectedInternal.Sealed() => Sealed();

		Sealed.Readonly.IProtectedInternal Sealed.IProtectedInternal.Readonly() => Readonly();

		Static.IProtectedInternal IProtectedInternal.Static() => Static();

		Readonly.IProtectedInternal Static.IProtectedInternal.Readonly() => Readonly();

		Readonly.IProtectedInternal IProtectedInternal.Readonly() => Readonly();

	}
}