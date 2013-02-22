using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;

namespace SigilTests
{
    [TestClass]
    public class WriteLine
    {
        [TestMethod]
        public void WriteLineSimple()
        {
            var el = Emit<Func<string>>.NewDynamicMethod();
            var guid = Guid.NewGuid().ToString();

            el.DeclareLocal<MemoryStream>("MemoryStream");
            el.DeclareLocal<StreamWriter>("StreamWriter");
            el.DeclareLocal<byte[]>("arr");
            
            el.NewObject<MemoryStream>();
            el.StoreLocal("MemoryStream");
            el.LoadLocal("MemoryStream");
            el.NewObject(typeof(StreamWriter), new[] { typeof(Stream) });
            el.StoreLocal("StreamWriter");
            el.LoadLocal("StreamWriter");
            el.Call(typeof (Console).GetMethod("SetOut"));
            
            el.WriteLine(guid);
            
            el.LoadLocal("StreamWriter");
            el.Call(typeof (StreamWriter).GetMethod("Flush"));
            el.LoadLocal("MemoryStream");
            el.Call(typeof (MemoryStream).GetMethod("ToArray"));
            el.StoreLocal("arr");
            el.Call(typeof (Encoding).GetMethod("get_UTF8"));
            el.LoadLocal("arr");
            el.Call(typeof (Encoding).GetMethod("GetString", new[] { typeof(byte[]) }));
            el.Return();

            var del = el.CreateDelegate();
            var val = del();

            Assert.AreEqual(guid + Environment.NewLine, val);
        }
    }
}