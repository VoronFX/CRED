using System.Linq;
using Bridge.NET.Test.Helpers;
using Bridge.React;

namespace Bridge.NET.Test.Components.Azure
{
		//private enum FxsContainerClasses { FxsPortal, DesktopNormal, FxsShowStartboard }
		public static class Fxs
		{
			// Check for typos
			//static Fxs()
			//{
			//	foreach (var property in typeof(Fxs).GetProperties())
			//	{
			//		if ((string)property.GetValue(typeof(Fxs)) != nameof(Fxs) + property.Name)
			//		{
			//			throw new ArgumentException($"Property name of \"{property.GetValue(typeof(Fxs))}\""
			//									  + $" not equal to \"{nameof(Fxs) + property.Name} \". Possible typo was made.");
			//		}
			//	}
			//}

			public static string Portal => Compose(nameof(Fxs), nameof(Portal));
			public static string DesktopNormal => Compose(nameof(Fxs), nameof(DesktopNormal));
			public static string ShowStartboard => Compose(nameof(Fxs), nameof(ShowStartboard));
			public static string ShowJourney => Compose(nameof(Fxs), nameof(ShowJourney));
			public static string Topbar => Compose(nameof(Fxs), nameof(Topbar));
			public static string Sidebar => Compose(nameof(Fxs), nameof(Sidebar));

			public static string PortalTip => Compose(nameof(Fxs), nameof(PortalTip));
			public static string PortalMain => Compose(nameof(Fxs), nameof(PortalMain));
			public static string PortalContent => Compose(nameof(Fxs), nameof(PortalContent));

			public static string Trim => Compose(nameof(Fxs), nameof(Trim));

			public static string ScrollbarTransparent => Compose(nameof(Fxs), nameof(ScrollbarTransparent));
			public static string ScrollbarDefaultHover => Compose(nameof(Fxs), nameof(ScrollbarDefaultHover));
			public static string Panorama => Compose(nameof(Fxs), nameof(Panorama));

			public static string SidebarCollapseButton => Compose(nameof(Fxs), nameof(SidebarCollapseButton));
			public static string HasHover => Compose(nameof(Fxs), nameof(HasHover));

			public static string SelectClasses(params string[] names)
				=> string.Join(" ", names.Select(x => x.ToLower()));

			public static Attributes ClassAttribute(params string[] names)
				=> new Attributes { ClassName = SelectClasses(names) };

			private static string Compose(params string[] names)
				=> names.Select(x => x.ToCssClassName())
				.ToJoinedString("-").ToCssClassName();
		}
}
