using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OMLLogging
{
    public static class LogInfo
    {
        public static string LogName = @"OML";
        public static string LogMachine = @".";
        public static string LogPrefix = @"OML";

        public static void WriteErrorLog(string zsMsg)
        {
            WriteErrorLog(String.Empty, zsMsg);
        }

        public static void WriteErrorLog(string zsSource, string zsMsg)
        {
            WriteLog(LogName, zsSource, zsMsg, EventLogEntryType.Error);
        }

        public static void WriteStatusLog(string zsMsg)
        {
            WriteStatusLog(String.Empty, zsMsg);
        }

        public static void WriteStatusLog(string zsSource, string zsMsg)
        {
            WriteLog(LogName, zsSource, zsMsg, EventLogEntryType.Information);
        }

        private static void WriteLog(string zsModule, string zsSource, string zsMsg, EventLogEntryType eType)
        {
            try
            {
                zsSource = LogPrefix + zsSource;
                if (!EventLog.SourceExists(zsSource, LogMachine))
                {
                    EventSourceCreationData srcData = new EventSourceCreationData(zsSource, LogName);
                    EventLog.CreateEventSource(srcData);
                }
                EventLog eLog = null;
                try
                {
                    eLog = new EventLog(zsModule, LogMachine, zsSource);
                    eLog.WriteEntry(zsMsg, eType, 100);
                }
                finally
                {
                    if (eLog != null)
                    {
                        eLog.Dispose();
                    }
                }
            }
            catch
            {
                if (!EventLog.SourceExists(LogPrefix, LogMachine))
                {
                    EventSourceCreationData srcData = new EventSourceCreationData(LogPrefix, LogName);
                    EventLog.CreateEventSource(srcData);
                }
                EventLog eLog = new EventLog(LogName, LogMachine, LogPrefix);
                eLog.WriteEntry(@"Error trying to write to the log", EventLogEntryType.Error, 100);
                eLog.Dispose();
            }
        }
    }
}
