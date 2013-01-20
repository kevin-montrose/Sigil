using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private List<Action<ILGenerator, StringBuilder>> Buffer = new List<Action<ILGenerator, StringBuilder>>();

        private Type DelegateType;

        public BufferedILGenerator(Type delegateType)
        {
            DelegateType = delegateType;
        }

        public string UnBuffer(ILGenerator il)
        {
            var log = new StringBuilder();

            foreach (var x in Buffer)
            {
                x(il, log);
            }

            return log.ToString();
        }

        public int ByteDistance(int start, int stop)
        {
            var invoke = DelegateType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes);
            var il = dynMethod.GetILGenerator();

            var ignored = new StringBuilder();

            foreach (var x in Buffer.Skip(start).Take(stop - start))
            {
                x(il, ignored);
            }

            return il.ILOffset;
        }

        public void Insert(int ix, OpCode op)
        {
            if (ix < 0 || ix > Buffer.Count)
            {
                throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + Buffer.Count);
            }

            Buffer.Insert(ix, (il, log) => { il.Emit(op); log.AppendLine(op.ToString()); });
        }

        public void Emit(OpCode op)
        {
            Buffer.Add((il, log) => { il.Emit(op); log.AppendLine(op.ToString()); });
        }

        public void Emit(OpCode op, int i)
        {
            Buffer.Add((il, log) => { il.Emit(op, i); log.AppendLine(op + " " + i); });
        }

        public void Emit(OpCode op, long l)
        {
            Buffer.Add((il, log) => { il.Emit(op, l); log.AppendLine(op + " " + l); });
        }

        public void Emit(OpCode op, float f)
        {
            Buffer.Add((il, log) => { il.Emit(op, f); log.AppendLine(op + " " + f); });
        }

        public void Emit(OpCode op, double d)
        {
            Buffer.Add((il, log) => { il.Emit(op, d); log.AppendLine(op + " " + d); });
        }

        public void Emit(OpCode op, MethodInfo method)
        {
            Buffer.Add((il, log) => { il.Emit(op, method); log.AppendLine(op + " " + method); });
        }

        public void Emit(OpCode op, ConstructorInfo cons)
        {
            Buffer.Add((il, log) => { il.Emit(op, cons); log.AppendLine(op + " " + cons); });
        }

        public void Emit(OpCode op, Type type)
        {
            Buffer.Add((il, log) => { il.Emit(op, type); log.AppendLine(op + " " + type); });
        }

        public void Emit(OpCode op, FieldInfo field)
        {
            Buffer.Add((il, log) => { il.Emit(op, field); log.AppendLine(op + " " + field); });
        }

        public void Emit(OpCode op, string str)
        {
            Buffer.Add((il, log) => { il.Emit(op, str); log.AppendLine(op + " '" + str + "'"); });
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
                (il, log) => 
                    {
                        var l = label(il);
                        il.Emit(localOp, l);
                        
                        log.AppendLine(op + " " + l);
                    }
            );
        }

        public void Emit(OpCode op, DeclareLocallDelegate local)
        {
            Buffer.Add(
                (il, log) => 
                    {
                        var l = local(il);
                        il.Emit(op, l);

                        log.AppendLine(op + " " + l);
                    }
            );
        }

        public DefineLabelDelegate BeginExceptionBlock()
        {
            ILGenerator forIl = null;
            Label? l = null;

            DefineLabelDelegate ret =
                il =>
                {
                    if (forIl != null && forIl != il)
                    {
                        l = null;
                    }

                    if (l != null) return l.Value;

                    forIl = il;
                    l = forIl.BeginExceptionBlock();

                    return l.Value;
                };

            Buffer.Add(
                (il, log) => 
                { 
                    ret(il);

                    log.AppendLine("--BeginExceptionBlock--");
                }
            );

            return ret;
        }

        public void BeginCatchBlock(Type exception)
        {
            Buffer.Add(
                (il, log) =>
                {
                    il.BeginCatchBlock(exception);

                    log.AppendLine("--BeginCatchBlock(" + exception + ")--");
                }
            );
        }

        public void EndExceptionBlock()
        {
            Buffer.Add(
                (il, log) =>
                {
                    il.EndExceptionBlock();

                    log.AppendLine("--EndExceptionBlock--");
                }
            );
        }

        public void BeginFinallyBlock()
        {
            Buffer.Add(
                (il, log) =>
                {
                    il.BeginFinallyBlock();

                    log.AppendLine("--BeginFinallyBlock--");
                }
            );
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

            Buffer.Add(
                (il, log) => 
                { 
                    ret(il);
                }
            );

            return ret;
        }

        public void MarkLabel(DefineLabelDelegate label)
        {
            Buffer.Add(
                (il, log) =>
                {
                    var l = label(il);
                    il.MarkLabel(l);

                    log.AppendLine("--MarkLabel(" + l + ")--");
                }
            );
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

            Buffer.Add(
                (il, log) => 
                { 
                    ret(il); 
                }
            );

            return ret;
        }
    }
}
