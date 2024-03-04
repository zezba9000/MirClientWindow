using System;
using System.Runtime.InteropServices;

namespace TestMirCSharp
{
	unsafe class MainClass
	{
		const string lib = "libmirclient";

		[DllImport(lib, EntryPoint = "mir_connect_sync", CharSet = CharSet.Ansi)]
		static extern IntPtr mir_connect_sync(string server, string app_name);

		public static void Main (string[] args)
		{
			var connection = mir_connect_sync ("TODO", "TODO");
			Console.WriteLine ("Hello World: " + handle.ToString());
			//mir_connection_release(connection);
		}
	}
}
