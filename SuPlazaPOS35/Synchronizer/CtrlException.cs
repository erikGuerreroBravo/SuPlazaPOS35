using System;
using System.IO;

namespace SuPlazaPOS35.Synchronizer
{
	public class CtrlException
	{
		private static string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\error.log";

		public static void SetError(string msg)
		{
			if (!File.Exists(fileName))
			{
				File.Create(fileName).Dispose();
			}
			TextWriter textWriter = new StreamWriter(fileName, append: true);
			textWriter.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), msg));
			textWriter.Close();
		}
	}
}
