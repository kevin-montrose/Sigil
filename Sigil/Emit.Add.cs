using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void Add(EmitLocal loadVal2)
        {
            LoadLocal(loadVal2);
            Add();
        }

        public void Add(EmitLocal loadVal1, EmitLocal loadVal2)
        {
            LoadLocal(loadVal1);
            LoadLocal(loadVal2);
            Add();
        }

        public void Add(EmitLocal loadVa1, EmitLocal loadVal2, EmitLocal storeVal)
        {
            LoadLocal(loadVa1);
            LoadLocal(loadVal2);
            Add();
            StoreLocal(storeVal);
        }

        public void Add()
        {
            var args = Stack.Top(2);

            if (args == null)
            {
                throw new SigilException("Add requires 2 arguments be on the stack", Stack);
            }

            var val2 = args[0];
            var val1 = args[1];
            
            // See: http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.add.aspx
            //   For legal arguments table
            if (val1 == TypeOnStack.Get<int>())
            {
                if (val2 == TypeOnStack.Get<int>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<int>(), pop: 2);

                    return;
                }

                if (val2 == TypeOnStack.Get<NativeInt>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<NativeInt>(), pop: 2);

                    return;
                }

                if (val2.IsReference || val2.IsPointer)
                {
                    UpdateState(OpCodes.Add, val2, pop: 2);

                    return;
                }

                throw new SigilException("Adding to an int32 expects an int32, native int, reference, or pointer as second value; found " + val2, Stack);
            }

            if (val1 == TypeOnStack.Get<long>())
            {
                if (val2 == TypeOnStack.Get<long>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<long>(), pop: 2);

                    return;
                }

                throw new SigilException("Adding to an int64 expects an in64 as second value; found " + val2, Stack);
            }

            if (val1 == TypeOnStack.Get<NativeInt>())
            {
                if (val2 == TypeOnStack.Get<int>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<NativeInt>(), pop: 2);

                    return;
                }

                if (val2 == TypeOnStack.Get<NativeInt>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<NativeInt>(), pop: 2);

                    return;
                }

                if (val2.IsReference || val2.IsPointer)
                {
                    UpdateState(OpCodes.Add, val2, pop: 2);

                    return;
                }

                throw new SigilException("Adding to a native int expects an int32, native int, reference, or pointer as second value; found " + val2, Stack);
            }

            if (val1 == TypeOnStack.Get<StackFloat>())
            {
                if (val2 == TypeOnStack.Get<StackFloat>())
                {
                    UpdateState(OpCodes.Add, TypeOnStack.Get<StackFloat>(), pop: 2);

                    return;
                }

                throw new SigilException("Adding to a float expects a float as second value; found " + val2, Stack);
            }

            if (val1.IsReference)
            {
                if (val2 == TypeOnStack.Get<int>() || val2 == TypeOnStack.Get<NativeInt>())
                {
                    UpdateState(OpCodes.Add, val1, pop: 2);

                    return;
                }

                throw new SigilException("Adding to a reference expects an int32, or a native int as second value; found " + val2, Stack);
            }

            if (val1.IsPointer)
            {
                if (val2 == TypeOnStack.Get<int>() || val2 == TypeOnStack.Get<NativeInt>())
                {
                    UpdateState(OpCodes.Add, val1, pop: 2);

                    return;
                }

                throw new SigilException("Adding to a pointer expects an int32, or a native int as second value; found " + val2, Stack);
            }

            throw new SigilException("Add expects an int32, int64, native int, float, reference, or pointer as first value; found " + val1, Stack);
        }
    }
}
