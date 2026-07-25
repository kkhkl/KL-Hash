using System.Windows.Forms;

namespace KLHash;

internal static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
		ApplicationConfiguration.Initialize();
		var files = ParseFileArguments(args);
		if (files.Count > 0)
		{
			Application.Run(new MainForm(files.ToArray()));
			return;
		}
		Application.Run(new MainForm());
	}

	private static List<string> ParseFileArguments(string[] args)
	{
		var files = new List<string>();
		if (args == null || args.Length == 0) return files;
		string combinedArg = string.Join(" ", args);
		string cleanPath = combinedArg.Trim('"', ' ');
		if (File.Exists(cleanPath))
		{
			files.Add(cleanPath);
			return files;
		}
		foreach (string arg in args)
		{
			if (string.IsNullOrWhiteSpace(arg)) continue;
			string p = arg.Trim('"', ' ');
			if (File.Exists(p) && !files.Contains(p))
			{
				files.Add(p);
			}
		}
		return files;
	}
}
