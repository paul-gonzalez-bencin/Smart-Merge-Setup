
public static class Program
{
	[System.STAThread]
	public static void Main(string[] args)
	{
		if(args.Length == 0)
		{
			MainForm form = new MainForm();
			
			form.ShowDialog();
		}
		else
		{
			foreach(string str in args)
			{
				System.Console.WriteLine(str);
				System.Console.WriteLine();
			}
		}
	}
}
