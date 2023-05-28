﻿using DedicatedUnityCloudBuild.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DedicatedUnityCloudBuild.Log
{
    // TODO: implement log file
    internal class Logger
    {
        // singleton pattern
        public static Logger Instance { get; private set; }

        private static int numberOfLineBreaks;

        // constructor
        public Logger()
        {
            // check if there is already instance of Logger
            if (Instance != null)
            {
                throw new Exception("Logger already exists!");
            }
            else
            {
                // set number of line breaks to variable value - 1 because writeLine already prints a new line
                numberOfLineBreaks = ProgramVariables.numberOfLineBreaks - 1;

                // else set current object as Instance
                Instance = this;
            }
        }

        private string CurrTime()
        {
            return DateTime.Now.ToString("HH:mm:ss") + ": ";
        }

        private void CreateLogFile()
        {
            // TODO: Create Log File
            throw new NotImplementedException();
        }

        // log message
        public void Log(string message)
        {
            Console.WriteLine(CurrTime() + message);
        }

        public void LogBlock(string title, string message)
        {
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.WriteLine(CurrTime());
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(title);
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.ResetColor();
        }

        // log error
        public void LogError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(CurrTime() + "ERROR: " + message);
            Console.ResetColor();
        }

        public void LogErrorBlock(string title, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.WriteLine(CurrTime());
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine("ERROR: " + title);
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.ResetColor();
        }

        public void LogErrorBlock(Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.WriteLine(CurrTime());
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine("ERROR: " + e.Message);
            Console.WriteLine();
            Console.WriteLine(e.StackTrace);
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.ResetColor();
        }

        // log warning
        public void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(CurrTime() + "WARNING: " + message);
            Console.ResetColor();
        }

        public void LogWarningBlock(string title, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.WriteLine(CurrTime());
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine("WARNING: " + title);
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.ResetColor();
        }

        // log info
        public void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(CurrTime() + "INFO: " + message);
            Console.ResetColor();
        }

        public void LogInfoBlock(string title, string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.WriteLine(CurrTime());
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine("INFO: " + title);
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine(new String('-', Console.WindowWidth));
            Console.WriteLine(new String('\n', numberOfLineBreaks));
            Console.ResetColor();
        }
    }
}