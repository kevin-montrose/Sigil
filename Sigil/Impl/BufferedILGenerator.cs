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
        public delegate System.Reflection.Emit.Label DefineLabelDelegate(ILGenerator il);
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

        private int LengthTo(int end, out string instructions)
        {
            var invoke = DelegateType.GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = invoke.GetParameters().Select(s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes);
            var il = dynMethod.GetILGenerator();

            var instrs = new StringBuilder();

            foreach (var x in Buffer.Take(end))
            {
                x(il, instrs);
            }

            instructions = instrs.ToString();

            return il.ILOffset;
        }

        internal string Instructions()
        {
            string instrs;
            LengthTo(Buffer.Count, out instrs);

            return instrs;
        }

        public int ByteDistance(int start, int stop)
        {
            string ignored;
            var toStart = LengthTo(start, out ignored);
            var toStop = LengthTo(stop, out ignored);

            return toStop - toStart;
        }

        public void Insert(int ix, OpCode op)
        {
            if (ix < 0 || ix > Buffer.Count)
            {
                throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + Buffer.Count);
            }

            Buffer.Insert(
                ix, 
                (il, log) => 
                { 
                    il.Emit(op);
                    if (new[] { OpCodes.Volatile, OpCodes.Readonly, OpCodes.Tailcall, OpCodes.Unaligned }.Contains(op))
                    {
                        log.Append(op.ToString());
                    }
                    else
                    {
                        log.AppendLine(op.ToString());
                    }
                }
            );
        }

        public void Emit(OpCode op)
        {
            Buffer.Add(
                (il, log) => 
                { 
                    il.Emit(op);

                    if (new[] { OpCodes.Volatile, OpCodes.Readonly, OpCodes.Tailcall, OpCodes.Unaligned }.Contains(op))
                    {
                        log.Append(op.ToString());
                    }
                    else
                    {
                        log.AppendLine(op.ToString()); 
                    }
                }
            );
        }

        public void Emit(OpCode op, int i)
        {
            Buffer.Add(
                (il, log) => 
                { 
                    il.Emit(op, i);

                    if (new[] { OpCodes.Volatile, OpCodes.Readonly, OpCodes.Tailcall, OpCodes.Unaligned }.Contains(op))
                    {
                        log.Append(op + "" + i + ".");
                    }
                    else
                    {
                        log.AppendLine(op + " " + i);
                    }
                }
            );
        }

        public void Emit(OpCode op, uint ui)
        {
            int asInt;
            unchecked
            {
                asInt = (int)ui;
            }

            Buffer.Add((il, log) => { il.Emit(op, asInt); log.AppendLine(op + " " + ui); });
        }

        public void Emit(OpCode op, long l)
        {
            Buffer.Add((il, log) => { il.Emit(op, l); log.AppendLine(op + " " + l); });
        }

        public void Emit(OpCode op, ulong ul)
        {
            long asLong;
            unchecked
            {
                asLong = (long)ul; 
            }

            Buffer.Add((il, log) => { il.Emit(op, asLong); log.AppendLine(op + " " + ul); });
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
            Buffer.Add((il, log) => { il.Emit(op, str); log.AppendLine(op + " '" + str.Replace("'", @"\'") + "'"); });
        }

        public void Emit(OpCode op, Sigil.Label label, out UpdateOpCodeDelegate update)
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
                    var l = label.LabelDel(il);
                    il.Emit(localOp, l);

                    log.AppendLine(op + " " + label);
                }
            );
        }

        public void Emit(OpCode op, Sigil.Label[] labels, out UpdateOpCodeDelegate update)
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
                    var ls = labels.Select(l => l.LabelDel(il)).ToArray();
                    il.Emit(localOp, ls);

                    log.AppendLine(op + " " + string.Join(", ", labels.AsEnumerable()));
                }
            );
        }

        public void Emit(OpCode op, Sigil.Local local)
        {
            Buffer.Add(
                (il, log) => 
                    {
                        var l = local.LocalDel(il);
                        il.Emit(op, l);

                        log.AppendLine(op + " " + l);
                    }
            );
        }

        public void Emit(OpCode op, CallingConventions callConventions, Type returnType, Type[] parameterTypes)
        {
            Buffer.Add(
                (il, log) =>
                {
                    il.EmitCalli(op, callConventions, returnType, parameterTypes, null);

                    log.AppendLine(op + " " + callConventions + " " + returnType + " " + string.Join(" ", (IEnumerable<Type>)parameterTypes));
                }
            );
        }

        public DefineLabelDelegate BeginExceptionBlock()
        {
            ILGenerator forIl = null;
            System.Reflection.Emit.Label? l = null;

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
            System.Reflection.Emit.Label? l = null;

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

        public void MarkLabel(Sigil.Label label)
        {
            Buffer.Add(
                (il, log) =>
                {
                    var l = label.LabelDel(il);
                    il.MarkLabel(l);

                    log.AppendLine();
                    log.AppendLine(label + ":");
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
