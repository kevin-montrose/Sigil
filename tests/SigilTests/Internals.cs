using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SigilTests
{
    // These tests aren't about the interface, but about proving some internal details are robust.
    // Anything used in these tests isn't guaranteed to work between versions, and shouldn't be relied upon.
    public class Internals
    {
        public static double _OrderSelect(Tuple<int, double> i)
        {
            return i.Item2;
        }

        internal static Delegate CreateDelegate(Type delegateType, System.Reflection.MethodInfo method)
        {
            return Delegate.CreateDelegate(delegateType, method);
        }

        [Fact]
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
            Assert.NotNull(asEnumerable);
            var orderByGeneric = sigilListT.GetMethod("OrderBy");
            Assert.NotNull(orderByGeneric);
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

            Assert.Equal(linqList.Count, sigilList.Count);

            for (var i = 0; i < linqList.Count; i++)
            {
                Assert.Equal(linqList[i], sigilList[i]);
            }
        }

        private static Type[] GetAssemblyTypes(Type type)
        {
            return type.Assembly.GetTypes();
        }

        [Fact]
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

            Assert.Equal(linqList.Count, sigilList.Count);

            for (var i = 0; i < linqList.Count; i++)
            {
                Assert.Equal(linqList[i], sigilList[i]);
            }
        }
    }
}
