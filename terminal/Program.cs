using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HaiFeng
{
	class Program
	{
		private static string _errLog = string.Empty;
		[STAThread]
		static void Main(string[] args)
		{
			_errLog = "err_" + Application.ProductName + ".log";

			Console.Title = "AT  v" + Application.ProductVersion;
			DisableCloseButton(Console.Title);
			//Application.Run(new Form1() { Text = Console.Title });

			//你在主线程捕获全部异常就行，如下代码： 
			//WINFORM未处理异常之捕获 
			//处理未捕获的异常 
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			//处理UI线程异常 
			Application.ThreadException += Application_ThreadException;
			//处理非UI线程异常 
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			using (Form f = new Form())
			{
				using (FormLogin fl = new FormLogin())
					if (fl.ShowDialog() == DialogResult.OK)
					{
						f.Text = "AT  v" + Application.ProductVersion;
						Plat plat = new Plat(fl.Trade, fl.Quote);
						plat.Dock = DockStyle.Fill;
						f.Height = plat.Height;
						f.Width = plat.Width;
						f.Controls.Add(plat);
						f.ShowDialog();
						fl.Trade.ReqUserLogout();
						fl.Quote.ReqUserLogout();
					}
			}
			Environment.Exit(0); //正常关闭
		}

		#region 禁用关闭按钮
		[DllImport("User32.dll", EntryPoint = "FindWindow")]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

		[DllImport("user32.dll", EntryPoint = "RemoveMenu")]
		static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		///<summary>
		/// 禁用关闭按钮
		///</summary>
		///<param name="consoleName">控制台名字</param>
		public static void DisableCloseButton(string title)
		{
			//线程睡眠，确保closebtn中能够正常FindWindow，否则有时会Find失败。。
			//Thread.Sleep(100);

			IntPtr windowHandle = FindWindow(null, title);
			IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
			uint SC_CLOSE = 0xF060;
			RemoveMenu(closeMenu, SC_CLOSE, 0x0);
		}
		public static bool IsExistsConsole(string title)
		{
			IntPtr windowHandle = FindWindow(null, title);
			if (windowHandle.Equals(IntPtr.Zero)) return false;

			return true;
		}
		#endregion


		#region 处理未捕获异常的挂钩函数

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			Exception error = e.Exception;
			if (error != null)
			{
				using (StreamWriter sw = new StreamWriter(_errLog, true))
				{
					sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + string.Format("出现应用程序未处理的异常 异常类型：{0} 异常消息：{1} 异常位置：{2} ", error.GetType().Name, error.Message, error.StackTrace));
				}
			}
			else
			{
				using (StreamWriter sw = new StreamWriter(_errLog, true))
				{
					sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + string.Format("Application ThreadError:{0}", e));
				}
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception error = e.ExceptionObject as Exception;
			if (error != null)
			{
				using (StreamWriter sw = new StreamWriter(_errLog, true))
				{
					sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + string.Format("Application UnhandledException:{0}; 堆栈信息:{1}", error.Message, error.StackTrace));
				}
			}
			else
			{
				using (StreamWriter sw = new StreamWriter(_errLog, true))
				{
					sw.WriteLine(DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "\t" + string.Format("Application UnhandledError:{0}", e));
				}
			}
		}
		#endregion
	}
}
