using Domain.Contracts;
using System;

namespace Domain.Loggers
{
    public class Logger : ILogger
    {
        public void LogException(Exception ex)
        {
            Console.WriteLine($"[ERROR] Excpetion message: {ex.Message}.");
            Console.WriteLine($"[ERROR] Exception type: {ex.GetType().Name}");
            Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}.");
        }

        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] Log: {message}");
        }
    }
}
