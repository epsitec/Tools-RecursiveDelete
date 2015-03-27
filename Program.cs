//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecursiveDelete
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				System.Console.Error.WriteLine ("Specify one single path and it will be deleted recursively...");
				System.Environment.Exit (1);
			}

			var path = System.IO.Path.GetFullPath (args[0]);

			Program.Delete (path, path);

			System.Console.WriteLine ();
			System.Console.WriteLine ("Longest path found had {0} characters", Program.maxPath.Length);
			System.Console.WriteLine (Program.maxPath);
		}

		private static void Delete(string path, string displayPath)
		{
			var fullDisplayPath = displayPath;

			if (fullDisplayPath.Length > Program.maxPath.Length)
			{
				Program.maxPath = fullDisplayPath;
			}

			if (displayPath.Length > 64)
			{
				displayPath = "..." + displayPath.Substring (displayPath.Length-61, 61);
			}

			if (System.IO.File.Exists (path))
			{
				System.Console.WriteLine ("Delete file {0}", displayPath);
				System.IO.File.Delete (path);
				return;
			}

			if (System.IO.Directory.Exists (path))
			{
				var shortPath = Program.ShortenDir (path);

				System.Console.WriteLine ("Analyzing   {0}", displayPath);
				
				var entries = System.IO.Directory.GetFileSystemEntries (shortPath);

				foreach (var entry in entries)
				{
					Program.Delete (entry, System.IO.Path.Combine (fullDisplayPath, System.IO.Path.GetFileName (entry)));
				}

				System.Console.WriteLine ("Delete dir  {0}", displayPath);

				Program.DeleteDir (shortPath);
			}
		}

		private static void DeleteDir(string newPath)
		{
			try
			{
				System.IO.Directory.Delete (newPath);
			}
			catch (System.IO.IOException ex)
			{
				System.Console.WriteLine (ex.Message);
			}
		}


		private static string ShortenDir(string path)
		{
			var root = System.IO.Path.GetDirectoryName (path);
			var name = System.IO.Path.GetFileName (path);

			var oldPath = System.IO.Path.Combine (root, name);
			var newPath = System.IO.Path.Combine (root, "@");

			if (oldPath != newPath)
			{
				try
				{
					System.IO.Directory.Move (oldPath, newPath);
				}
				catch (System.IO.IOException ex)
				{
					System.Console.WriteLine (ex.Message);
					return oldPath;
				}
			}
			
			return newPath;
		}
		
		static string maxPath = "";
	}
}
