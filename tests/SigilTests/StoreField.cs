using Sigil;
using System;
using Xunit;

namespace SigilTests
{
    public partial class StoreField
    {
        private class StoreFieldClass
        {
            public static int Static;
            public int Instance;

            public StoreFieldClass()
            {
                Static = Instance = 314159;
            }
        }

        [Fact]
        public void Simple()
        {
            {
                var e1 = Emit<Action<StoreFieldClass>>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Static");
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate();

                StoreFieldClass.Static = 2;
                d1(null);

                Assert.Equal(12, StoreFieldClass.Static);
            }

            {
                var e1 = Emit<Action<StoreFieldClass>>.NewDynamicMethod();
                var f = typeof(StoreFieldClass).GetField("Instance");
                e1.LoadArgument(0);
                e1.LoadConstant(12);
                e1.StoreField(f);
                e1.Return();

                var d1 = e1.CreateDelegate();

                var x = new StoreFieldClass();
                d1(x);

                Assert.Equal(12, x.Instance);
            }
        }

        private struct _ValueType
        {
#pragma warning disable 0649
            public string A;
#pragma warning restore 0649
        }

        [Fact]
        public void ValueType()
        {
            var field = typeof(_ValueType).GetField("A");

            var e1 = Emit<Func<_ValueType>>.NewDynamicMethod();
            using (var loc = e1.DeclareLocal<_ValueType>())
            {
                e1.LoadLocalAddress(loc);           // _ValueType*
                e1.InitializeObject<_ValueType>();  // --empty--
                e1.LoadLocalAddress(loc);           // _ValueType*
                e1.LoadConstant("hello world");     // _ValueType* string
                e1.StoreField(field);               // --empty--
                e1.LoadLocal(loc);                  // ValueType
                e1.Return();                        // --empty--
            }

            var d1 = e1.CreateDelegate();

            var x = d1();
            Assert.Equal("hello world", x.A);
        }
    }
}
