using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Olive;

namespace MSharp.Build
{
    abstract class Builder
    {
        internal static bool ShouldLog;
        static DateTime Start = DateTime.Now;

        Dictionary<string, Action> Steps = new Dictionary<string, Action>();
        static List<KeyValuePair<string, string>> LogMessages = new List<KeyValuePair<string, string>>();

        protected abstract void AddTasks();

        protected void Add(Expression<Action> step)
        {
            var method = step.Body as MethodCallExpression;
            var name = method.Method.Name;

            Steps.Add(name, () => method.Method.Invoke(this, new object[0]));
        }

        protected void Add(string key, Action step) => Steps.Add(key, step);

        protected void Log(string message, [CallerMemberName] string step = "")
            => LogMessages.Add(KeyValuePair.Create(step, message));

        void ExecuteTasks()
        {
            AddTasks();

            foreach (var step in Steps)
            {
                try
                {
                    Console.Write("Running " + step.Key + "...");
                    step.Value();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Done. " + Math.Round(DateTime.Now.Subtract(Start).TotalSeconds, 1) + "s");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed: " + ex);
                    Console.ResetColor();
                    throw ex;
                }
            }
        }

        public int Execute()
        {
            try
            {
                ExecuteTasks();
                return 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToLogString());
                Console.ResetColor();
                return -1;
            }
            finally
            {
                if (ShouldLog) PrintLog();
            }
        }

        static void PrintLog()
        {
            foreach (var item in LogMessages.GroupBy(x => x.Key))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("-------Log: " + item.Key.PadRight(50, '-'));
                Console.ForegroundColor = ConsoleColor.DarkGray;

                foreach (var x in item)
                    Console.WriteLine(x.Value);
            }

            Console.ResetColor();
        }
    }
}