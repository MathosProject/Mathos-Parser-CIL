using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var mp = new Mathos.ILParser.ILMathParser();

            mp.LocalVariables.Add("x");
            mp.LocalVariables.Add("y");

            string a = mp.Parse("(x(3x))");

            var s = mp.OutputIL;

            var sw = new System.IO.StreamWriter("c:\\out\\il.txt");
            sw.Write(a);
            sw.Close();



            decimal result = Mathos.ILFunction.Parse(3, 2);
           // decimal result = Mathos.ILFunction.Parse(2, 3);

           // decimal result = Mathos.ILParser.
            //var DLL = Assembly.LoadFile(@"C:\out\file.exe");

            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    var c = Activator.CreateInstance(type);
            //    var ab = type.InvokeMember("Parse", BindingFlags.InvokeMethod, null, c, new object[] { 3, 2 });

            //}

            
      
            //Console.ReadLine();

        }
    }
}
