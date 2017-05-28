using System;
using System.Collections.Generic;
using CsCodeGenerator.Interfaces.Common;
using Sealed = CsCodeGenerator.Interfaces.Sealed;
using Static = CsCodeGenerator.Interfaces.Static;
using Readonly = CsCodeGenerator.Interfaces.Readonly;

namespace CsCodeGenerator.Types
{
	//	{ public / protected / internal / private / protected internal } // access modifiers
	//	new
	//	{ abstract / virtual / override } // inheritance modifiers
	//	sealed
	//	static
	//	readonly
	//	extern
	//	unsafe
	//	volatile
	//	async

	internal enum AccessModifiers
	{
		Public,
		Protected,
		Internal,
		Private,
		ProtectedInternal
	}

	[Flags]
	public enum Modifiers
	{
		Public,
		Protected,
		Internal,
		Private,
		ProtectedInternal,
		New,
		Abstract,
		Virtual,
		Override,
		Sealed,
		Static,
		Reaonly,
		Extern,
		Unsafe,
		Volatile,
		Async,
		Partial
	}

	internal partial struct ModifiersBuilder : IModifiersBuilder
	{
		public ModifiersBuilder(Modifiers modifiers)
		{
			Modifiers = modifiers;
		}

		public Modifiers Modifiers { get; }

		public ModifiersBuilder Public() => new ModifiersBuilder(Modifiers | Modifiers.Public);
		public ModifiersBuilder Internal() => new ModifiersBuilder(Modifiers | Modifiers.Internal);
		public ModifiersBuilder Protected() => new ModifiersBuilder(Modifiers | Modifiers.Protected);
		public ModifiersBuilder Private() => new ModifiersBuilder(Modifiers | Modifiers.Private);
		public ModifiersBuilder ProtectedInternal() => new ModifiersBuilder(Modifiers | Modifiers.ProtectedInternal);

		public ModifiersBuilder Sealed() => new ModifiersBuilder(Modifiers | Modifiers.Sealed);
		public ModifiersBuilder Readonly() => new ModifiersBuilder(Modifiers | Modifiers.Reaonly);
		public ModifiersBuilder Static() => new ModifiersBuilder(Modifiers | Modifiers.Static);
		public ModifiersBuilder Partial() => new ModifiersBuilder(Modifiers | Modifiers.Partial);

		IPublic IModifiersBuilder.Public => Public();
		IInternal IModifiersBuilder.Internal => Internal();
		IProtected IModifiersBuilder.Protected => Protected();
		IPrivate IModifiersBuilder.Private => Private();
		IProtectedInternal IModifiersBuilder.ProtectedInternal => ProtectedInternal();
	}

	internal partial struct ModifiersBuilder :
		IPublic,
		Sealed.IPublic,
		Sealed.Readonly.IPublic,
		Static.IPublic,
		Readonly.IPublic
	{
		Sealed.IPublic IPublic.Sealed => Sealed();

		Sealed.Readonly.IPublic Sealed.IPublic.Readonly => Readonly();

		Static.IPublic IPublic.Static => Static();

		Readonly.IPublic Static.IPublic.Readonly => Readonly();

		Readonly.IPublic IPublic.Readonly => Readonly();

	}

	internal partial struct ModifiersBuilder :
		IInternal,
		Sealed.IInternal,
		Sealed.Readonly.IInternal,
		Static.IInternal,
		Readonly.IInternal
	{
		Sealed.IInternal IInternal.Sealed => Sealed();

		Sealed.Readonly.IInternal Sealed.IInternal.Readonly => Readonly();

		Static.IInternal IInternal.Static => Static();

		Readonly.IInternal Static.IInternal.Readonly => Readonly();

		Readonly.IInternal IInternal.Readonly => Readonly();

	}

	internal partial struct ModifiersBuilder :
		IPrivate,
		Sealed.IPrivate,
		Sealed.Readonly.IPrivate,
		Static.IPrivate,
		Readonly.IPrivate
	{
		Sealed.IPrivate IPrivate.Sealed => Sealed();

		Sealed.Readonly.IPrivate Sealed.IPrivate.Readonly => Readonly();

		Static.IPrivate IPrivate.Static => Static();

		Readonly.IPrivate Static.IPrivate.Readonly => Readonly();

		Readonly.IPrivate IPrivate.Readonly => Readonly();

	}

	internal partial struct ModifiersBuilder :
		IProtected,
		Sealed.IProtected,
		Sealed.Readonly.IProtected,
		Static.IProtected,
		Readonly.IProtected
	{
		Sealed.IProtected IProtected.Sealed => Sealed();

		Sealed.Readonly.IProtected Sealed.IProtected.Readonly => Readonly();

		Static.IProtected IProtected.Static => Static();

		Readonly.IProtected Static.IProtected.Readonly => Readonly();

		Readonly.IProtected IProtected.Readonly => Readonly();

	}

	internal partial struct ModifiersBuilder :
		IProtectedInternal,
		Sealed.IProtectedInternal,
		Sealed.Readonly.IProtectedInternal,
		Static.IProtectedInternal,
		Readonly.IProtectedInternal
	{
		Sealed.IProtectedInternal IProtectedInternal.Sealed => Sealed();

		Sealed.Readonly.IProtectedInternal Sealed.IProtectedInternal.Readonly => Readonly();

		Static.IProtectedInternal IProtectedInternal.Static => Static();

		Readonly.IProtectedInternal Static.IProtectedInternal.Readonly => Readonly();

		Readonly.IProtectedInternal IProtectedInternal.Readonly => Readonly();

	}
}