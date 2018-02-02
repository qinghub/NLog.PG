using System;

namespace NLog.PG.ConsoleTest
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            _logger.Trace("Sample trace message");

            Console.ReadLine();
        }
    }
}
