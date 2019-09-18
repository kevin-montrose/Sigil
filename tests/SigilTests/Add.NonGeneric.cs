using Sigil.NonGeneric;
using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Xunit;

namespace SigilTests
{
    public partial class Add
    {
        [Fact]
        public unsafe void PointerToPointerNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int*), new[] { typeof(int), typeof(int*) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (PointerToPointerDelegate)e1.CreateDelegate(typeof(PointerToPointerDelegate));

            int* ptr1 = (int*)Marshal.AllocHGlobal(64);

            var ptr2 = d1(4, ptr1);

            Marshal.FreeHGlobal((IntPtr)ptr1);

            Assert.Equal(((int)ptr1) + 4, (int)ptr2);
        }

        [Fact]
        public unsafe void ByRefToByRefNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { Type.GetType("System.Int32&"), typeof(int), Type.GetType("System.Int32&") });
            e1.LoadArgument(1);
            e1.LoadArgument(0);
            e1.Add();
            e1.StoreArgument(2);
            e1.Return();

            var d1 = (ByRefToByRefDelegate)e1.CreateDelegate(typeof(ByRefToByRefDelegate));

            int a = 2;
            d1(ref a, 4, ref a);

            Assert.Equal(2, a);
        }

        [Fact]
        public void BlogPostNonGeneric()
        {
            {
                var method = new DynamicMethod("AddOneAndTwo", typeof(int), Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Ldc_I4, 2);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Ret);

                var del = (Func<int>)method.CreateDelegate(typeof(Func<int>));

                Assert.Equal(3, del());
            }

            {
                var method = new DynamicMethod("AddOneAndTwo", typeof(int), Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Ret);

                var del = (Func<int>)method.CreateDelegate(typeof(Func<int>));

                var ex = Assert.Throws<InvalidProgramException>(() => del());
                Assert.Equal("Common Language Runtime detected an invalid program.", ex.Message);
            }

            {

                var ex = Assert.Throws<Sigil.SigilVerificationException>(() =>
                {
                    var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "AddOneAndTwo");
                    il.LoadConstant(1);
                    il.Add();
                    il.Return();

                    var del = (Func<int>)il.CreateDelegate(typeof(Func<int>));
                    del();
                });

                Assert.Equal("Add expects 2 values on the stack", ex.Message);
            }
        }

        [Fact]
        public void IntIntNonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes,"IntInt");
            il.LoadConstant(1);
            il.LoadConstant(2);
            il.Add();
            il.Return();

            var del = (Func<int>)il.CreateDelegate(typeof(Func<int>));
            Assert.Equal(3, del());
        }

        [Fact]
        public void IntNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.Convert<IntPtr>();
            e1.LoadConstant(3);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.Equal(4, d1());
        }

        [Fact]
        public void NativeIntIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.LoadConstant(3);
            e1.Convert<IntPtr>();
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.Equal(4, d1());
        }

        [Fact]
        public void NativeIntNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.Convert<IntPtr>();
            e1.LoadConstant(3);
            e1.Convert<IntPtr>();
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.Equal(4, d1());
        }

        [Fact]
        public void IntPointerNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadArgumentAddress(0);
            e1.LoadConstant(2);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            // Should be legal
            var x = d1(3);
            Assert.True(x != 0);
        }

        [Fact]
        public void PointerIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadConstant(2);
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            // Should be legal
            var x = d1(3);
            Assert.True(x != 0);
        }

        [Fact]
        public void PointerNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadConstant(2);
            e1.Convert<IntPtr>();
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            // Should be legal
            var x = d1(3);
            Assert.True(x != 0);
        }

        [Fact]
        public void LongLongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long), typeof(long) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<long, long, long>)e1.CreateDelegate(typeof(Func<long, long, long>));

            Assert.Equal(2 * ((long)uint.MaxValue), d1(uint.MaxValue, uint.MaxValue));
        }

        [Fact]
        public void FloatFloatNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(float), new [] { typeof(float), typeof(float) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<float, float, float>)e1.CreateDelegate(typeof(Func<float, float, float>));

            Assert.Equal(3.14f + 1.59f, d1(3.14f, 1.59f));
        }

        [Fact]
        public void DoubleDoubleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<double, double, double>)e1.CreateDelegate(typeof(Func<double, double, double>));

            Assert.Equal(3.14 + 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void OverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.AddOverflow();
            e1.Return();

            var d1 = (Func<int, int, int>)e1.CreateDelegate(typeof(Func<int, int, int>));

            Assert.Equal(4 + 5, d1(4, 5));
        }

        [Fact]
        public void UnsignedOverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedAddOverflow();
            e1.Return();

            var d1 = (Func<int, int, int>)e1.CreateDelegate(typeof(Func<int, int, int>));

            Assert.Equal(4 + 5, d1(4, 5));
        }
    }
}
