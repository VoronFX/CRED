using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using Retyped.semantic_ui;

namespace CRED.Client.Helpers
{
    internal static class SemanticUiExtensionMethods
    {
	    public static JQuery SemanticUi(this Element element)
	    {
		    // ReSharper disable once SuspiciousTypeConversion.Global
		    return (JQuery) element;
	    }
    }
}
