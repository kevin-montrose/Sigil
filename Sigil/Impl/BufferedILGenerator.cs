using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace Sigil.Impl
{
    internal class BufferedILInstruction
    {
        public bool DefinesLabel { get; internal set; }
        public Sigil.Label MarksLabel { get; internal set; }

        public bool StartsExceptionBlock { get; internal set; }
        public bool EndsExceptionBlock { get; internal set; }
        
        public bool StartsCatchBlock { get; internal set; }
        public bool EndsCatchBlock { get; internal set; }
        
        public bool StartsFinallyBlock { get; internal set; }
        public bool EndsFinallyBlock { get; internal set; }
        
        public bool DeclaresLocal { get; internal set; }

        public OpCode? IsInstruction { get; internal set; }

        public Type MethodReturnType { get; internal set; }
        public LinqRoot<Type> MethodParameterTypes { get; internal set; }
#if !COREFX // see https://github.com/dotnet/corefx/issues/4543 item 4
        public bool TakesTypedReference()
        {
            var instr = this;

            if (instr.MethodReturnType == typeof(TypedReference)) return true;

            return instr.MethodParameterTypes.Any(p => p == typeof(TypedReference));
        }
#endif

        public bool TakesManagedPointer()
        {
            var instr = this;

            if (instr.MethodReturnType.IsPointer) return true;

            return instr.MethodParameterTypes.Any(p => p.IsPointer);
        }

        internal bool TakesByRefArgs()
        {
            var instr = this;

            return instr.MethodParameterTypes.Any(p => p.IsByRef);
        }
    }

    internal class BufferedILGenerator<DelegateType>
    {
        public BufferedILInstruction this[int ix]
        {
            get
            {
                return TraversableBuffer[ix];
            }
        }

        public int Index { get { return Buffer.Count; } }

        private LinqList<SigilAction<ILGenerator, bool, StringBuilder>> Buffer = new LinqList<SigilAction<ILGenerator, bool, StringBuilder>>();
        private LinqList<BufferedILInstruction> TraversableBuffer = new LinqList<BufferedILInstruction>();
        internal LinqList<Operation<DelegateType>> Operations = new LinqList<Operation<DelegateType>>();
        private LinqList<SigilFunc<int>> InstructionSizes = new LinqList<SigilFunc<int>>();

        public BufferedILGenerator()
        {
        }

        public string UnBuffer(ILGenerator il)
        {
            var log = new StringBuilder();

            // First thing will always be a Mark for tracing purposes; no reason to actually do it
            for(var i = 2; i < Buffer.Count; i++)
            {
                var x = Buffer[i];

                x(il, false, log);
            }

            return log.ToString();
        }

        private Dictionary<int, int> LengthCache = new Dictionary<int, int>();

        private int LengthTo(int end)
        {
            if (end == 0)
            {
                return 0;
            }

            int cached;
            if (LengthCache.TryGetValue(end, out cached))
            {
                return cached;
            }

            int runningTotal = 0;

            for (var i = 0; i < end; i++)
            {
                var s = InstructionSizes[i];

                runningTotal += s();

                LengthCache[i + 1] = runningTotal;
            }

            cached = LengthCache[end];

            return cached;
        }

        internal string[] Instructions(LinqList<Local> locals)
        {
            var ret = new List<string>();

            var invoke = typeof(DelegateType).GetMethod("Invoke");
            var returnType = invoke.ReturnType;
            var parameterTypes = LinqAlternative.Select(invoke.GetParameters(), s => s.ParameterType).ToArray();

            var dynMethod = new DynamicMethod(Guid.NewGuid().ToString(), returnType, parameterTypes);
            var il = dynMethod.GetILGenerator();

            var instrs = new StringBuilder();

            for(var i = 0; i < Buffer.Count; i++)
            {
                var x = Buffer[i];

                x(il, true, instrs);
                var line = instrs.ToString().TrimEnd();

                if (line.StartsWith(OpCodes.Ldloc_0.ToString()) ||
                    line.StartsWith(OpCodes.Stloc_0.ToString()))
                {
                    line += " // " + GetInScopeAt(locals, i)[0];
                }

                if (line.StartsWith(OpCodes.Ldloc_1.ToString()) ||
                    line.StartsWith(OpCodes.Stloc_1.ToString()))
                {
                    line += " // " + GetInScopeAt(locals, i)[1];
                }

                if (line.StartsWith(OpCodes.Ldloc_2.ToString()) ||
                    line.StartsWith(OpCodes.Stloc_2.ToString()))
                {
                    line += " // " + GetInScopeAt(locals, i)[2];
                }

                if (line.StartsWith(OpCodes.Ldloc_3.ToString()) ||
                    line.StartsWith(OpCodes.Stloc_3.ToString()))
                {
                    line += " // " + GetInScopeAt(locals, i)[3];
                }

                if (line.StartsWith(OpCodes.Ldloc_S.ToString()) ||
                    line.StartsWith(OpCodes.Stloc_S.ToString()))
                {
                    line += " // " + ExtractLocal(line, locals, i);
                }

                ret.Add(line);
                instrs.Length = 0;
            }

            return ret.ToArray();
        }

        private static LinqDictionary<int, Local> GetInScopeAt(LinqList<Local> allLocals, int ix)
        {
            return
                allLocals
                    .Where(
                        l =>
                            l.DeclaredAtIndex <= ix &&
                            (l.ReleasedAtIndex == null || l.ReleasedAtIndex > ix)
                    ).ToDictionary(d => (int)d.Index, d => d);
        }

#if DOTNET5_2
        private static Regex _ExtractLocal = new Regex(@"\s+(?<locId>\d+)");
#else
        private static Regex _ExtractLocal = new Regex(@"\s+(?<locId>\d+)", RegexOptions.Compiled);
#endif

        private static Local ExtractLocal(string from, LinqList<Local> locals, int ix)
        {
            var match = _ExtractLocal.Match(from);

            var locId = match.Groups["locId"].Value;

            var lid = int.Parse(locId);

            return GetInScopeAt(locals, ix)[lid];
        }

        public int ByteDistance(int start, int stop)
        {
            var toStart = LengthTo(start);
            var toStop = LengthTo(stop);

            return toStop - toStart;
        }

        public void Remove(int ix)
        {
            if (ix < 0 || ix >= Buffer.Count)
            {
                throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + Buffer.Count);
            }

            LengthCache.Clear();

            InstructionSizes.RemoveAt(ix);

            Buffer.RemoveAt(ix);

            TraversableBuffer.RemoveAt(ix);

            Operations.RemoveAt(ix);
        }

        public void Insert(int ix, OpCode op)
        {
            if (ix < 0 || ix > Buffer.Count)
            {
                throw new ArgumentOutOfRangeException("ix", "Expected value between 0 and " + Buffer.Count);
            }

            LengthCache.Clear();

            InstructionSizes.Insert(ix, () => InstructionSize.Get(op));

            Buffer.Insert(
                ix, 
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op);
                    }

                    if (ExtensionMethods.IsPrefix(op))
                    {
                        log.Append(op.ToString());
                    }
                    else
                    {
                        log.AppendLine(op.ToString());
                    }
                }
            );

            TraversableBuffer.Insert(
                ix,
                new BufferedILInstruction
                {
                    IsInstruction = op
                }
            );

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[0] });
        }

        public void Emit(OpCode op)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op);
                    }

                    if (ExtensionMethods.IsPrefix(op))
                    {
                        log.Append(op.ToString());
                    }
                    else
                    {
                        log.AppendLine(op.ToString()); 
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[0] });
        }

        public void Emit(OpCode op, byte b)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.Emit(op, b);
                    }

                    if (ExtensionMethods.IsPrefix(op))
                    {
                        log.Append(op + "" + b + ".");
                    }
                    else
                    {
                        log.AppendLine(op + " " + b);
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { b } });
        }

        public void Emit(OpCode op, short s)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.Emit(op, s);
                    }

                    if (ExtensionMethods.IsPrefix(op))
                    {
                        log.Append(op + "" + s + ".");
                    }
                    else
                    {
                        log.AppendLine(op + " " + s);
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { s } });
        }

        public void Emit(OpCode op, int i)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, i);
                    }

                    if (ExtensionMethods.IsPrefix(op))
                    {
                        log.Append(op + "" + i + ".");
                    }
                    else
                    {
                        log.AppendLine(op + " " + i);
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { i } });
        }

        public void Emit(OpCode op, uint ui)
        {
            int asInt;
            unchecked
            {
                asInt = (int)ui;
            }

            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, asInt);
                    }
                    
                    log.AppendLine(op + " " + ui);
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { ui } });
        }

        public void Emit(OpCode op, long l)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, l);
                    }
                    
                    log.AppendLine(op + " " + l); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { l } });
        }

        public void Emit(OpCode op, ulong ul)
        {
            long asLong;
            unchecked
            {
                asLong = (long)ul; 
            }

            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, asLong);
                    }
                    
                    log.AppendLine(op + " " + ul); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { ul } });
        }

        public void Emit(OpCode op, float f)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, f);
                    }
                    
                    log.AppendLine(op + " " + f); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { f } });
        }

        public void Emit(OpCode op, double d)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, d);
                    }
                    
                    log.AppendLine(op + " " + d);
                });

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { d } });
        }

        public void Emit(OpCode op, MethodInfo method, IEnumerable<Type> parameterTypes)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, method);
                    }

                    var mtdString = method is MethodBuilder ? method.Name : method.ToString();

                    log.AppendLine(op + " " + mtdString); 
                }
            );

            var parameters = new LinqList<Type>(parameterTypes);
            if(!method.IsStatic)
            {
                var declaring = method.DeclaringType;

                if (TypeHelpers.IsValueType(declaring))
                {
                    declaring = declaring.MakePointerType();
                }

                parameters.Insert(0, declaring);
            }

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = method.ReturnType, MethodParameterTypes = parameters });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { method } });
        }

        public void Emit(OpCode op, ConstructorInfo cons, IEnumerable<Type> parameterTypes)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.Emit(op, cons);
                    }

                    var mtdString = cons is ConstructorBuilder ? cons.Name : cons.ToString();

                    log.AppendLine(op + " " + mtdString);
                }
            );

            var parameters = new LinqList<Type>(parameterTypes);
            var declaring = cons.DeclaringType;

            if (TypeHelpers.IsValueType(declaring))
            {
                declaring = declaring.MakePointerType();
            }

            parameters.Insert(0, declaring);


            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = typeof(void), MethodParameterTypes = parameters });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { cons } });
        }

        public void Emit(OpCode op, ConstructorInfo cons)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, cons);
                    }
                    
                    log.AppendLine(op + " " + cons); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { cons } });
        }

        public void Emit(OpCode op, Type type)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, type);
                    }

                    log.AppendLine(op + " " + type); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { type } });
        }

        public void Emit(OpCode op, FieldInfo field)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, field);
                    }

                    log.AppendLine(op + " " + field); 
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { field } });
        }

        public void Emit(OpCode op, string str)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        il.Emit(op, str);
                    }
                    
                    log.AppendLine(op + " '" + str.Replace("'", @"\'") + "'");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { str } });
        }

        public void Emit(OpCode op, Sigil.Label label, out UpdateOpCodeDelegate update)
        {
            var localOp = op;

            update =
                newOpcode =>
                {
                    LengthCache.Clear();

                    localOp = newOpcode;
                };

            InstructionSizes.Add(() => InstructionSize.Get(localOp));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        var l = label.LabelDel(il);
                        il.Emit(localOp, l);
                    }

                    log.AppendLine(localOp + " " + label);
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { label } });
        }

        public void Emit(OpCode op, Sigil.Label[] labels, out UpdateOpCodeDelegate update)
        {
            var localOp = op;

            update =
                newOpcode =>
                {
                    LengthCache.Clear();

                    localOp = newOpcode;
                };

            InstructionSizes.Add(() => InstructionSize.Get(localOp, labels));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        var ls = LinqAlternative.Select(labels, l => l.LabelDel(il)).ToArray();
                        il.Emit(localOp, ls);
                    }

                    log.AppendLine(localOp + " " + Join(", ", ((LinqArray<Label>)labels).AsEnumerable()));
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = labels });
        }

        internal static string Join<T>(string delimiter, IEnumerable<T> parts) where T: class
        {
            using (var iter = parts.GetEnumerator())
            {
                if (!iter.MoveNext()) return "";
                var sb = new StringBuilder();
                var next = iter.Current;
                if (next != null) sb.Append(next);
                while (iter.MoveNext())
                {
                    sb.Append(delimiter);
                    next = iter.Current;
                    if (next != null) sb.Append(next);
                }
                return sb.ToString();
            }
        }
        public void Emit(OpCode op, Sigil.Local local)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                    {
                        if (!logOnly)
                        {
                            var l = local.LocalDel(il);
                            il.Emit(op, l);
                        }

                        log.AppendLine(op + " " + local);
                    }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = new object[] { local } });
        }

        public void Emit(OpCode op, CallingConventions callConventions, Type returnType, Type[] parameterTypes)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.EmitCalli(op, callConventions, returnType, parameterTypes, null);
                    }

                    log.AppendLine(op + " " + callConventions + " " + returnType + " " + Join(" ", ((LinqArray<Type>)parameterTypes).AsEnumerable()));
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = returnType, MethodParameterTypes = (LinqArray<Type>)parameterTypes  });

            var paras = new List<object> { callConventions, returnType };
            paras.AddRange(parameterTypes);

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = paras.ToArray() });
        }

        public void EmitCall(OpCode op, MethodInfo method, IEnumerable<Type> parameterTypes, Type[] arglist)
        {
            InstructionSizes.Add(() => InstructionSize.Get(op));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.EmitCall(op, method, arglist);
                    }

                    var mtdString = method is MethodBuilder ? method.Name : method.ToString();

                    log.AppendLine(op + " " + mtdString + " __arglist(" + Join(", ", arglist) + ")");
                }
            );

            var parameters = new LinqList<Type>(parameterTypes);
            if (!method.IsStatic)
            {
                var declaring = method.DeclaringType;

                if (TypeHelpers.IsValueType(declaring))
                {
                    declaring = declaring.MakePointerType();
                }

                parameters.Insert(0, declaring);
            }

            parameters.AddRange(arglist);

            var paras = new List<object> { method };
            paras.AddRange(arglist);

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = op, MethodReturnType = method.ReturnType, MethodParameterTypes = parameters });

            Operations.Add(new Operation<DelegateType> { OpCode = op, Parameters = paras.ToArray() });
        }

        public void EmitCalli(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] arglist)
        {
            InstructionSizes.Add(() => InstructionSize.Get(OpCodes.Calli));

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.EmitCalli(OpCodes.Calli, callingConvention, returnType, parameterTypes, arglist);
                    }

                    log.AppendLine(OpCodes.Calli + " " + callingConvention + " " + returnType + " " + Join(" ", (IEnumerable<Type>)parameterTypes) + " __arglist(" + Join(", ", arglist) + ")");
                }
            );

            var ps = new LinqList<Type>(parameterTypes);
            ps.AddRange(arglist);

            TraversableBuffer.Add(new BufferedILInstruction { IsInstruction = OpCodes.Calli, MethodReturnType = returnType, MethodParameterTypes = ps });

            var paras = new List<object>() { callingConvention, returnType };
            paras.AddRange(parameterTypes);
            paras.AddRange(arglist);

            Operations.Add(new Operation<DelegateType> { OpCode = OpCodes.Calli, Parameters = paras.ToArray() });
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

            InstructionSizes.Add(() => InstructionSize.BeginExceptionBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        ret(il);
                    }

                    log.AppendLine("--BeginExceptionBlock--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { StartsExceptionBlock = true });

            Operations.Add(null);

            return ret;
        }

        public void BeginCatchBlock(Type exception)
        {
            InstructionSizes.Add(() => InstructionSize.BeginCatchBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.BeginCatchBlock(exception);
                    }

                    log.AppendLine("--BeginCatchBlock(" + exception + ")--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { StartsCatchBlock = true });

            Operations.Add(null);
        }

        public void EndExceptionBlock()
        {
            InstructionSizes.Add(() => InstructionSize.EndExceptionBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.EndExceptionBlock();
                    }

                    log.AppendLine("--EndExceptionBlock--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { EndsExceptionBlock = true });

            Operations.Add(null);
        }

        public void EndCatchBlock()
        {
            InstructionSizes.Add(() => InstructionSize.EndCatchBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    log.AppendLine("--EndCatchBlock--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { EndsCatchBlock = true });

            Operations.Add(null);
        }

        public void BeginFinallyBlock()
        {
            InstructionSizes.Add(() => InstructionSize.BeginFinallyBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        il.BeginFinallyBlock();
                    }

                    log.AppendLine("--BeginFinallyBlock--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { StartsFinallyBlock = true });

            Operations.Add(null);
        }

        public void EndFinallyBlock()
        {
            InstructionSizes.Add(() => InstructionSize.EndFinallyBlock());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    log.AppendLine("--EndFinallyBlock--");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { EndsFinallyBlock = true });

            Operations.Add(null);
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

            InstructionSizes.Add(() => InstructionSize.DefineLabel());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) => 
                {
                    if (!logOnly)
                    {
                        ret(il);
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { DefinesLabel = true });

            Operations.Add(null);

            return ret;
        }

        public void MarkLabel(Sigil.Label label)
        {
            InstructionSizes.Add(() => InstructionSize.MarkLabel());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly, log) =>
                {
                    if (!logOnly)
                    {
                        var l = label.LabelDel(il);
                        il.MarkLabel(l);
                    }

                    log.AppendLine();
                    log.AppendLine(label + ":");
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { MarksLabel = label });

            Operations.Add(null);
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

            InstructionSizes.Add(() => InstructionSize.DeclareLocal());

            LengthCache.Clear();

            Buffer.Add(
                (il, logOnly,log) => 
                {
                    if (!logOnly)
                    {
                        ret(il);
                    }
                }
            );

            TraversableBuffer.Add(new BufferedILInstruction { DeclaresLocal = true });

            Operations.Add(null);

            return ret;
        }
    }
}
