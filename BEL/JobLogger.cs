﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace BEL
{
    using System;
    using System.Linq;
    using System.Text;
    [Obsolete("Not used any more", true)]
    public class JobLoggerObsolete
    {

        private static bool _logToFile;
        private static bool _logToConsole;
        private static bool _logMessage;
        private static bool _logWarning;
        private static bool _logError;
        private static bool LogToDatabase;
        private bool _initialized;

        public JobLoggerObsolete(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
        {
            _logError = logError;
            _logMessage = logMessage;
            _logWarning = logWarning;
            LogToDatabase = logToDatabase;
            _logToFile = logToFile;
            _logToConsole = logToConsole;
        }

        public static void LogMessage(string message, bool isMessage, bool warning, bool error)
        {
            message.Trim();
            if (message == null || message.Length == 0)
            {
                return;
            }
            if (!_logToConsole && !_logToFile && !LogToDatabase)
            {
                throw new Exception("Invalid configuration");
            }
            if ((!_logError && !_logMessage && !_logWarning) || (!isMessage && !warning && !error))
            {
                throw new Exception("Error or Warning or Message must be specified");
            }

            //var x = ConfigurationManager.AppSettings["UrlToPing"].ToString();
            //var y = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

            SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            connection.Open();
            int t = 0;
            if (isMessage && _logMessage)
            {
                t = 1;
            }
            if (error && _logError)
            {
                t = 2;
            }
            if (warning && _logWarning)
            {
                t = 3;
            }
            SqlCommand command = new SqlCommand("Insert into Log Values('" + message + "', " + t.ToString() + ")");
            command.ExecuteNonQuery();

            string l = string.Empty;
            //var rootFolder = ConfigurationManager.AppSettings["LogFileDirectory"].ToString();
            if (!File.Exists(ConfigurationManager.AppSettings["LogFileDirectory"].ToString() + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"))
            {
                //l = File.ReadAllText(ConfigurationManager.AppSettings["LogFileDirectory"].ToString() + "LogFile" +".txt");
                l = File.ReadAllText(ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" +".txt");
            }
            if (error && _logError)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }
            if (warning && _logWarning)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }
            if (isMessage && _logMessage)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }

            File.WriteAllText(ConfigurationManager.AppSettings["LogFileDirectory"].ToString() + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l);

            if (error && _logError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (warning && _logWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (isMessage && _logMessage)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(DateTime.Now.ToShortDateString() + message);
        }
    }
}