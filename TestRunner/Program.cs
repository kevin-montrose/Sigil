using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            var classes = Assembly.Load(new AssemblyName("SigilTests")).GetTypes().Where(t => t.GetCustomAttribute<TestClassAttribute>() != null).ToList();

            bool success = true;

            foreach (var clazz in classes.OrderBy(o => o.Name))
            {
                var inst = clazz.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

                foreach (var test in clazz.GetMethods().Where(m => m.GetCustomAttribute<TestMethodAttribute>() != null).OrderBy(o => o.Name))
                {
                    Console.Write(clazz.Name + "." + test.Name + "...");

                    try
                    {
                        test.Invoke(inst, new object[0]);
                        Console.WriteLine(" Passed");
                    }
                    catch (Exception failure)
                    {
                        success = false;

                        Console.WriteLine("Failure!");
                        Console.WriteLine();
                        Console.WriteLine(failure.ToString());

                        Console.ReadKey();
                    }
                }
            }

            return success ? 0 : -1;
        }
    }
}
