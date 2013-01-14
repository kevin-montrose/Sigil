using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Sigil.Impl
{
    internal class BufferedILGenerator
    {
        public delegate void UpdateOpCodeDelegate(OpCode newOpcode);
        public delegate Label DefineLabelDelegate(ILGenerator il);
        public delegate LocalBuilder DeclareLocallDelegate(ILGenerator il);

        public int Index { get { return Buffer.Count; } }

        private List<Action<ILGenerator>> Buffer = new List<Action<ILGenerator>>();

        private Type DelegateType;

        public BufferedILGenerator(Type delegateType)
        {
            DelegateType = delegateType;
        }

        public void UnBuffer(ILGenerator il)
        {
            foreach (var x in Buffer)
            {
                x(il);
            }
        }

        public int ByteDistance(int start, int stop)
        {
            var invoke = DelegateType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes);
            var il = dynMethod.GetILGenerator();

            foreach (var x in Buffer.Skip(start).Take(stop - start))
            {
                x(il);
            }

            return il.ILOffset;
        }

        public void Emit(OpCode op)
        {
            Buffer.Add(il => il.Emit(op));
        }

        public void Emit(OpCode op, int i)
        {
            Buffer.Add(il => il.Emit(op, i));
        }

        public void Emit(OpCode op, long l)
        {
            Buffer.Add(il => il.Emit(op, l));
        }

        public void Emit(OpCode op, float f)
        {
            Buffer.Add(il => il.Emit(op, f));
        }

        public void Emit(OpCode op, double d)
        {
            Buffer.Add(il => il.Emit(op, d));
        }

        public void Emit(OpCode op, DefineLabelDelegate label, out UpdateOpCodeDelegate update)
        {
            var localOp = op;

            update =
                newOpcode =>
                {
                    localOp = newOpcode;
                };

            Buffer.Add(
                il => 
                    {
                        il.Emit(localOp, label(il));
                    }
            );
        }

        public void Emit(OpCode op, DeclareLocallDelegate local)
        {
            Buffer.Add(il => il.Emit(op, local(il)));
        }

        public DefineLabelDelegate DefineLabel()
        {
            ILGenerator forIl = null;
            Label? l = null;

            DefineLabelDelegate ret =
                il =>
                {
                    if(forIl != null && forIl != il)
                    {
                        l = null;
                    }

                    if (l != null) return l.Value;
                    
                    forIl = il;
                    l = forIl.DefineLabel();

                    return l.Value;
                };

            Buffer.Add(il => { ret(il); });

            return ret;
        }

        public void MarkLabel(DefineLabelDelegate label)
        {
            Buffer.Add(il => il.MarkLabel(label(il)));
        }

        public DeclareLocallDelegate DeclareLocal(Type type)
        {
            ILGenerator forIl = null;
            LocalBuilder l = null;

            DeclareLocallDelegate ret =
                il =>
                {
                    if(forIl != null && il != forIl)
                    {
                        l = null;
                    }

                    if (l != null) return l;

                    forIl = il;
                    l = forIl.DeclareLocal(type);

                    return l;
                };

            Buffer.Add(il => { ret(il); });

            return ret;
        }
    }
}
