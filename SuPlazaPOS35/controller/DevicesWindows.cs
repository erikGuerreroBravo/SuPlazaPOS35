using System;
using System.Drawing.Printing;
using System.IO.Ports;
using System.Linq;

namespace SuPlazaPOS35.controller
{
    public class DevicesWindows
    {

		private DevicesWindows() { }

		public static string[] getPortsCOM()
		{
			return SerialPort.GetPortNames();
		}

		public static string[] getParitys()
		{
			return Enum.GetNames(typeof(Parity));
		}

		public static string[] getStopBits()
		{
			return Enum.GetNames(typeof(StopBits));
		}

		public static string[] getPrintersWindows()
		{
			return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
		}
	}
}
