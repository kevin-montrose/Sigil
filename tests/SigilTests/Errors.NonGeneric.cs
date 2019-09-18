using Sigil.NonGeneric;
using System;
using Xunit;

namespace SigilTests
{
    public partial class Errors
    {
        [Fact]
        public void NotDelegateNonGeneric()
        {
            var emit = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            var ex = Assert.Throws<ArgumentException>(() => emit.CreateDelegate(typeof(string)));
            Assert.Equal("delegateType must be a delegate, found System.String", ex.Message);
        }

        [Fact]
        public void WrongReturnTypeNonGeneric()
        {
            var emit = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes);

            emit.LoadConstant("hello world");
            emit.Return();

            var ex = Assert.Throws<ArgumentException>(() => emit.CreateDelegate(typeof(Func<int>)));
            Assert.Equal("Expected delegateType to return System.String, found System.Int32", ex.Message);
        }

        [Fact]
        public void WrongParameterTypesNonGeneric()
        {
            {
                var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int), typeof(string) });
                emit.Return();
                var ex = Assert.Throws<ArgumentException>(() => emit.CreateDelegate(typeof(Action<int, int>)));
                Assert.Equal("Expected delegateType's parameter at index 1 to be a System.String, found System.Int32", ex.Message);
            }

            {
                var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int) });
                emit.Return();
                var ex = Assert.Throws<ArgumentException>(() => emit.CreateDelegate(typeof(Action<int, int>)));
                Assert.Equal("Expected delegateType to take 1 parameters, found 2", ex.Message);
            }
        }
    }
}
