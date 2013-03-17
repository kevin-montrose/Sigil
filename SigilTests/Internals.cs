using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    // These tests aren't about the interface, but about proving some internal details are robust.
    // Anything used in these tests isn't guaranteed to work between versions, and shouldn't be relied upon.
    [TestClass]
    public class Internals
    {
        [TestMethod]
        public void Sort()
        {
            var sigilTypes = typeof(Emit<>).Assembly.GetTypes();
            var linq = sigilTypes.Single(t => t.Name == "LinqAlternative");

            var orderByGeneric = linq.GetMethod("OrderBy");
            var orderBy = orderByGeneric.MakeGenericMethod(typeof(Tuple<int, double>), typeof(double));

            var toSort = new List<Tuple<int, double>>();

            var rand = new Random();

            for (var i = 0; i < 1000; i++)
            {
                toSort.Add(Tuple.Create(i, rand.NextDouble()));
            }

            Func<Tuple<int, double>, double> p = x => x.Item2;

            var sigilOrdered = (IEnumerable<Tuple<int, double>>)orderBy.Invoke(null, new object[] { toSort.ToList(), p });
            var linqOrdered = toSort.ToList().OrderBy(p);

            var sigilList = sigilOrdered.ToList();
            var linqList = linqOrdered.ToList();

            Assert.AreEqual(linqList.Count, sigilList.Count);

            for (var i = 0; i < linqList.Count; i++)
            {
                Assert.AreEqual(linqList[i], sigilList[i]);
            }
        }
    }
}
