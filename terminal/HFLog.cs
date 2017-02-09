using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;

namespace HaiFeng
{
	public static class HFLog
	{
		static private Logger _logger = NLog.LogManager.GetCurrentClassLogger();
		
		static public void Log(string pMsg, NLog.LogLevel pLevel = null, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0, [CallerFilePathAttribute]string pFileName = "")
		{
			_logger.Log(pLevel ?? NLog.LogLevel.Debug, "{0,4}|{1,-20}|{2,-12}|{3}", pLineNumber, pCallName, new FileInfo(pFileName).Name, pMsg);
		}
		static public void LogTrace(string pMsg, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0 /*,[CallerFilePathAttribute]string pFileName = "" */)
		{
			_logger.Log(NLog.LogLevel.Trace, "{0,4}|{1,-20}|{2}", /*new FileInfo(pFileName).Name,*/ pLineNumber, pCallName, pMsg);
		}

		static public void LogInfo(string pMsg, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0 /*,[CallerFilePathAttribute]string pFileName = "" */)
		{
			_logger.Log(NLog.LogLevel.Info, "{0,4}|{1,-20}|{2}", /*new FileInfo(pFileName).Name,*/ pLineNumber, pCallName, pMsg);
		}

		static public void LogDebug(string pMsg, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0 /*,[CallerFilePathAttribute]string pFileName = "" */)
		{
			_logger.Log(NLog.LogLevel.Debug, "{0,4}|{1,-20}|{2}", /*new FileInfo(pFileName).Name,*/ pLineNumber, pCallName, pMsg);
		}

		static public void LogWarn(string pMsg, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0 /*,[CallerFilePathAttribute]string pFileName = "" */)
		{
			_logger.Log(NLog.LogLevel.Warn, "{0,4}|{1,-20}|{2}", /*new FileInfo(pFileName).Name,*/ pLineNumber, pCallName, pMsg);
		}

		static public void LogError(string pMsg, [CallerMemberName]string pCallName = "", [CallerLineNumber] int pLineNumber = 0 /*,[CallerFilePathAttribute]string pFileName = "" */)
		{
			_logger.Log(NLog.LogLevel.Error, "{0,4}|{1,-20}|{2}", /*new FileInfo(pFileName).Name,*/ pLineNumber, pCallName, pMsg);
			if (File.Exists("Alert.wav"))
				new System.Media.SoundPlayer("Alert.wav").Play();
		}
	}
}
