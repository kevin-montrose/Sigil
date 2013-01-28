using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    /// <summary>
    /// Keeps track of the types on the stack, allowing for type checking.
    /// </summary>
    internal class StackState
    {
        private StackState Previous;

        public TypeOnStack Value { get; private set; }

        public bool IsRoot { get; private set; }

        public StackState()
        {
            IsRoot = true;
        }

        private StackState(StackState prev, TypeOnStack val)
        {
            Previous = prev;
            Value = val;
        }

        public StackState Unique()
        {
            return
                new StackState
                {
                    Previous = Previous,
                    Value = Value,
                    IsRoot = IsRoot
                };
        }

        public StackState Push(TypeOnStack val)
        {
            return new StackState(this, val);
        }

        public StackState Pop()
        {
            if (IsRoot) throw new Exception("Internal state invalid, tried to pop root");

            return Previous;
        }

        public StackState Pop(OpCode op, int num, bool firstParamIsThis)
        {
            var ret = this;

            while (num > 0)
            {
                if (ret.IsRoot) throw new Exception("Internal state invalid, tried to pop root");

                num--;

                // keep track of what ops used what values on the stack
                ret.Value.Mark(op, num, isThis: (firstParamIsThis && num == 0));

                ret = ret.Previous;
            }

            if (!ret.IsRoot) ret = ret.Unique();

            return ret;
        }

        public TypeOnStack[] Top(int n = 1)
        {
            var ret = new TypeOnStack[n];

            int i = 0;
            var cur = this;

            while (i < n)
            {
                if (cur.IsRoot) return null;

                ret[i] = cur.Value;
                cur = cur.Previous;
                i++;
            }

            return ret;
        }

        public bool AreEquivalent(object obj)
        {
            var other = obj as StackState;
            if (other == null) return false;

            var cur = this;

            while (!cur.IsRoot && !other.IsRoot)
            {
                if (cur.Value != other.Value)
                    return false;

                cur = cur.Previous;
                other = other.Previous;
            }

            return cur.IsRoot && other.IsRoot;
        }

        public int Count()
        {
            var i = 0;

            var cur = this;

            while (!cur.IsRoot)
            {
                i++;
                cur = cur.Previous;
            }

            return i;
        }
    }
}