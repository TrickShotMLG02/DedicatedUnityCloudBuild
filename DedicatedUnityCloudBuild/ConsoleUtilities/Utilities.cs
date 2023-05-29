using DedicatedUnityCloudBuild.Log;
using System;

namespace DedicatedUnityCloudBuild.CmdUtil
{
    internal class Utilities
    {
        // ask user for confirmation
        public static bool AskConfirm(string title, string promt)
        {
            Logger.Instance.LogWarningBlock(title, promt);
            ConsoleKey response;
            do
            {
                response = Console.ReadKey(false).Key;
                if (response != ConsoleKey.Enter)
                {
                    Console.WriteLine();
                }
            } while (response != ConsoleKey.Y && response != ConsoleKey.N);
            Console.WriteLine();
            return (response == ConsoleKey.Y);
        }
    }
}