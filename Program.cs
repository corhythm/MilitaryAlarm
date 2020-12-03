namespace Alarm
{
	static class Program
	{	
		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		private static extern System.IntPtr GetConsoleWindow();
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);
		
		[System.STAThread]
		public static void Main(string[] args)
		{
			System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(
				System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToString());
		
			if(process.Length > 1)
			{
				System.Windows.Forms.MessageBox.Show("프로세스가 이미 실행중입니다!!", "Error", System.Windows.Forms.MessageBoxButtons.OK);	
				return;
			}
			else
			{
				System.IntPtr hWnd = GetConsoleWindow();
				if(hWnd != System.IntPtr.Zero)
				{
					//ShowWindow(hWnd, 0); // hide
					System.Windows.Forms.Application.EnableVisualStyles();
					System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(true);
					System.Windows.Forms.Application.Run(new AlarmForm());
			
					//System.Threading.Thread.Sleep(5000);
					//ShowWindow(hWnd, 1); // visible
				}
				//System.Console.ReadKey();
			}
			
			/*System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
			System.Windows.Forms.Application.Run(new AlarmForm());*/
		}
	







	}
}