﻿/*
Oxygen Flow library
*/

using System;
using OpenQA.Selenium.Remote;

namespace Oxygen
{
    public partial class Flow
    {
        const string LogLine = "*************************************************************************";


        /// <summary>
        /// Trace output to stdout.
        /// </summary>
        /// <param name="message"></param>
        public static string O(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {message}");
            return message;
        }

        /// <summary>
        /// Output message in flow step
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Func<Context, Context> Trace(string message) => (Context context) =>
         {
             O(message);
             return context;
         };


        public static string LogError(string msg, Exception x = null)
        {
            var result = "FAILED " + msg;
            if (x != null) result += ": " + x.ToString();

            O(LogLine);
            O(result);
            O(LogLine);

            return result;
        }

        /// <summary>
        /// Saves screenshot in D:\Temp
        /// </summary>
        /// <param name="title"></param>
        public static void SaveScreenshot(RemoteWebDriver drv, string title)
        {
            if (drv == null) return;

            var path = $"D:\\Temp\\{title}-{Guid.NewGuid().ToString("N")}.png";

            O($"Screenshot {path}");

            drv.GetScreenshot().SaveAsFile(path);

        }
    }
}