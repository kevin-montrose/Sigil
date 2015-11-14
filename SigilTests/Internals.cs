using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    // These tests aren't about the interface, but about proving some internal details are robust.
    // Anything used in these tests isn't guaranteed to work between versions, and shouldn't be relied upon.
    [TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Internals
    {
        public static double _OrderSelect(Tuple<int, double> i)
        {
            return i.Item2;
        }
        internal static Delegate CreateDelegate(Type delegateType, System.Reflection.MethodInfo method)
        {
#if COREFX
            return method.CreateDelegate(delegateType);
#else
            return Delegate.CreateDelegate(delegateType, method);
#endif
        }
        [TestMethod]
        public void OrderBy()
        {
            var sigilTypes = GetAssemblyTypes(typeof(Emit<>));
            var linq = sigilTypes.Single(t => t.Name == "LinqAlternative");

            var sigilListGeneric = sigilTypes.Single(t => t.Name == "LinqList`1");
            var sigilListT = sigilListGeneric.MakeGenericType(typeof(Tuple<int, double>));
            var sigilListCons = sigilListT.GetConstructor(new[] { typeof(List<Tuple<int, double>>) });

            var sigilFuncGeneric = sigilTypes.Single(t => t.Name == "SigilFunc`2");
            var sigilFunc = sigilFuncGeneric.MakeGenericType(typeof(Tuple<int, double>), typeof(double));
            var asEnumerable = sigilListT.GetMethod("AsEnumerable");
            Assert.IsNotNull(asEnumerable);
            var orderByGeneric = sigilListT.GetMethod("OrderBy");
            Assert.IsNotNull(orderByGeneric);
            var orderBy = orderByGeneric.MakeGenericMethod(typeof(double));

            var toSort = new List<Tuple<int, double>>();

            var rand = new Random();

            for (var i = 0; i < 1000; i++)
            {
                toSort.Add(Tuple.Create(i, rand.NextDouble()));
            }

            Func<Tuple<int, double>, double> p1 = _OrderSelect;
            var p2 = CreateDelegate(sigilFunc, this.GetType().GetMethod("_OrderSelect"));

            var asSigilList = sigilListCons.Invoke(new object[] { toSort });

            var sigilOrderedInternal = orderBy.Invoke(asSigilList, new object[] { p2 });
            var sigilOrdered = (IEnumerable<Tuple<int, double>>)asEnumerable.Invoke(sigilOrderedInternal, new object[0]);
            var linqOrdered = toSort.ToList().OrderBy(p1);

            var sigilList = sigilOrdered.ToList();
            var linqList = linqOrdered.ToList();

            Assert.AreEqual(linqList.Count, sigilList.Count);

            for (var i = 0; i < linqList.Count; i++)
            {
                Assert.AreEqual(linqList[i], sigilList[i]);
            }
        }

        static Type[] GetAssemblyTypes(Type type)
        {
#if COREFX
            return type.GetTypeInfo().Assembly.GetTypes();
#else
            return type.Assembly.GetTypes();
#endif
        }
        [TestMethod]
        public void OrderByDescending()
        {
            var sigilTypes = GetAssemblyTypes(typeof(Emit<>));
            var linq = sigilTypes.Single(t => t.Name == "LinqAlternative");

            var sigilListGeneric = sigilTypes.Single(t => t.Name == "LinqList`1");
            var sigilListT = sigilListGeneric.MakeGenericType(typeof(Tuple<int, double>));
            var sigilListCons = sigilListT.GetConstructor(new[] { typeof(List<Tuple<int, double>>) });

            var sigilFuncGeneric = sigilTypes.Single(t => t.Name == "SigilFunc`2");
            var sigilFunc = sigilFuncGeneric.MakeGenericType(typeof(Tuple<int, double>), typeof(double));

            var asEnumerable = sigilListT.GetMethod("AsEnumerable");

            var orderByGeneric = sigilListT.GetMethod("OrderByDescending");
            var orderBy = orderByGeneric.MakeGenericMethod(typeof(double));

            var toSort = new List<Tuple<int, double>>();

            var rand = new Random();

            for (var i = 0; i < 1000; i++)
            {
                toSort.Add(Tuple.Create(i, rand.NextDouble()));
            }

            Func<Tuple<int, double>, double> p1 = _OrderSelect;
            var p2 = CreateDelegate(sigilFunc, this.GetType().GetMethod("_OrderSelect"));

            var asSigilList = sigilListCons.Invoke(new object[] { toSort });

            var sigilOrderedInternal = orderBy.Invoke(asSigilList, new object[] { p2 });
            var sigilOrdered = (IEnumerable<Tuple<int, double>>)asEnumerable.Invoke(sigilOrderedInternal, new object[0]);
            var linqOrdered = toSort.ToList().OrderByDescending(p1);

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
