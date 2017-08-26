using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace CRED.BuildTasks
{
    internal static class FileUtilities
    {
	    public static string GetHashForFile(string path)
	    {
		    using (var sha256 = SHA256.Create())
		    {
			    using (var readStream = new FileStream(path, FileMode.Open))
			    {
				    var hash = sha256.ComputeHash(readStream);
				    return WebEncoders.Base64UrlEncode(hash);
			    }
		    }
	    }

		public static void ThrowIfOutsideOfDirectoryTree(string directoryTreeRoot, params string[] files)
	    {
		    var external = files
				.Where(x => !x.StartsWith(directoryTreeRoot))
			    .ToArray();

		    if (external.Any())
			    throw new Exception(string.Join(Environment.NewLine,
				    new[] { $"Next files are outside of directory tree root ({directoryTreeRoot}):" }
					    .Concat(external)));
	    }
	}
}
