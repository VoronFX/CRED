using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsCodeGenerator
{
	// ReSharper disable once InconsistentNaming
    public static class IOExtension
    {
	    public static void EnsureFileDirectoryCreated(string filePath)
	    {
			var outFileDir = Path.GetDirectoryName(filePath);
		    if (!string.IsNullOrWhiteSpace(outFileDir))
			    Directory.CreateDirectory(outFileDir);
		}

	    public static string NormalizeExpandDirectoryPath(string directoryPath) 
			=> Path.GetFullPath(directoryPath+Path.DirectorySeparatorChar);
    }
}
