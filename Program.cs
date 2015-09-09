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
				var programName = System.IO.Path.GetFileName (System.Environment.GetCommandLineArgs ()[0]);

				System.Console.Error.WriteLine ("Specify the path of a directory and it will be deleted recursively.");
				System.Console.Error.WriteLine ("You can also drop a folder on the program to delete it.");
				System.Console.Error.WriteLine ();
				System.Console.Error.WriteLine ("Usage:");
				System.Console.Error.WriteLine ("  {0} \"path to folder\"", programName);
				System.Console.Error.WriteLine ();
				System.Console.Error.WriteLine ("No confirmation will be asked. The folder, all its files and subfolders");
				System.Console.Error.WriteLine ("will be deleted, not moved to the trash. The operation is irreversible.");
				
				System.Console.ReadLine ();
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

			if (Program.ExecuteOperation (() => System.IO.File.Exists (path)))
			{
				System.Console.WriteLine ("Delete file {0}", displayPath);
				
				Program.DeleteFile (path);

				return;
			}

			if (Program.ExecuteOperation (() => System.IO.Directory.Exists (path)))
			{
				var shortPath = Program.ShortenDir (path);

				System.Console.WriteLine ("Analyzing   {0}", displayPath);
				
				var entries = Program.ExecuteOperation (() => System.IO.Directory.GetFileSystemEntries (shortPath));

				if (entries != null)
				{
					foreach (var entry in entries)
					{
						Program.Delete (entry, System.IO.Path.Combine (fullDisplayPath, System.IO.Path.GetFileName (entry)));
					}
				}

				System.Console.WriteLine ("Delete dir  {0}", displayPath);

				Program.DeleteDir (shortPath);
			}
		}

		private static void DeleteFile(string path)
		{
			Program.ExecuteOperation (() => System.IO.File.Delete (path));
		}
		
		private static void DeleteDir(string path)
		{
			Program.ExecuteOperation (() => System.IO.Directory.Delete (path));
		}

		private static T ExecuteOperation<T>(System.Func<T> operation)
		{
			T result = default (T);
			
			Program.ExecuteOperation (() =>
			{
				result = operation ();
			});

			return result;
		}

		private static void ExecuteOperation(System.Action operation)
		{
			try
			{
				operation ();
			}
			catch (System.IO.IOException ex)
			{
				Program.Display (ex);
			}
			catch (System.UnauthorizedAccessException ex)
			{
				Program.Display (ex);
			}
		}

		private static void Display(System.Exception ex)
		{
			var color = System.Console.ForegroundColor;
			
			System.Console.ForegroundColor = System.ConsoleColor.Red;
			System.Console.Error.WriteLine (ex.Message);
			System.Console.ForegroundColor = color;
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
