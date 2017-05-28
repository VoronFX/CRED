using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResourcePacker
{
    internal static class ExtensionMethods
    {
	    public static string ToPascalCaseIdentifier(this string name)
		    => Regex.Replace(Regex.Replace("-" + name, "(?si)[^A-Za-z0-9]+", "-"), "(?si)-+([A-Za-z0-9]?)",
			    x => x.Groups[1].Value.ToUpperInvariant());

	    public static string ToLiteral(this string input)
	    {
		    using (var writer = new StringWriter())
		    using (var provider = CodeDomProvider.CreateProvider("CSharp"))
		    {
			     provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
			    //provider.GenerateCodeFromMember(
				   // new CodeMemberField()
				   // {
					  //  Attributes = MemberAttributes.Const,
					  //  Type = new CodeTypeReference(typeof(string)),
					  //  Comments = { new CodeCommentStatement("sdasdas", true) }
				   // }, writer, null);
			    return writer.ToString();
		    }
	    }
	}
}
