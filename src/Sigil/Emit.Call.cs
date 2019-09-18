using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        private void InjectTailCall()
        {
            for (var i = 0; i < IL.Index; i++)
            {
                var instr = IL[i];
                BufferedILInstruction call = null;

                if (instr.IsInstruction == OpCodes.Ret)
                {
                    int callIx = -1;

                    for (var j = i - 1; j >= 0; j--)
                    {
                        var atJ = IL[j];

                        if (atJ.MarksLabel != null)
                        {
                            break;
                        }

                        if (atJ.IsInstruction.HasValue)
                        {
                            if (ExtensionMethods.IsTailableCall(atJ.IsInstruction.Value))
                            {
                                callIx = j;
                                call = atJ;
                            }

                            break;
                        }
                    }

                    if (callIx == -1) continue;
                    if (call.TakesManagedPointer()) continue;
                    if (call.TakesTypedReference()) continue;
                    if (call.TakesByRefArgs()) continue;

                    var callReturns = call.MethodReturnType;
                    var delegateReturns = ReturnType.Type;

                    // the method's return types not matching
                    //   means we can't just turn the call into a jump
                    //   since _something_ has to preceed or survive the call to
                    //   make the ret legal
                    if (!ExtensionMethods.IsAssignableFrom(delegateReturns, callReturns)) continue;

                    // there's one case not being handled explicitly here,
                    //   which is the call must consume the _entire_ stack.
                    // we don't have to asset it because the return type
                    //   comparison is sufficient:
                    //     - if the types match, the stack must be empty 
                    //         or the following ret will fail to verify (since 
                    //         there's an extra item of the corret type on the stack)
                    //     - if the types _don't_ match, we've already bailed on the
                    //         tail injection

                    InsertInstruction(callIx, OpCodes.Tailcall);
                    i++;
                }
            }
        }

        /// <summary>
        /// <para>Calls the method being constructed by the given emit.  Emits so used must have been constructed with BuildMethod or related methods.</para>
        /// <para>Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.</para>
        /// <para>If the given method is an instance method, the `this` reference should appear before any parameters.</para>
        /// <para>Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.</para>
        /// <para>To call overrides of instance methods, use CallVirtual.</para>
        /// <para>Recursive calls can only be performed with DynamicMethods, other passed in Emits must already have their methods created.</para>
        /// <para>When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.</para>
        /// </summary>
        public Emit<DelegateType> Call<MethodEmit>(Emit<MethodEmit> emit, Type[] arglist = null)
        {
            if (emit == null)
            {
                throw new ArgumentNullException("emit");
            }

            MethodInfo methodInfo = emit.MtdBuilder ?? (MethodInfo)emit.DynMethod;
            if (methodInfo == null)
            {
                throw new InvalidOperationException("emit must be building a method");
            }

            if (HasFlag(emit.CallingConventions, CallingConventions.VarArgs) && !HasFlag(emit.CallingConventions, CallingConventions.Standard))
            {
                if (arglist == null)
                {
                    throw new InvalidOperationException("When calling a VarArgs method, arglist must be set");
                }
            }

            var expectedParams = ((LinqArray<Type>)emit.ParameterTypes).Select(s => TypeOnStack.Get(s)).ToList();

            if (arglist != null)
            {
                expectedParams.AddRange(((LinqArray<Type>)arglist).Select(t => TypeOnStack.Get(t)));
            }

            // Instance methods expect this to preceed parameters
            var declaring = methodInfo.DeclaringType;
            if (declaring != null)
            {
                if (HasFlag(emit.CallingConventions, CallingConventions.HasThis))
                {

                    if (TypeHelpers.IsValueType(declaring))
                    {
                        declaring = declaring.MakePointerType();
                    }

                    expectedParams.Insert(0, TypeOnStack.Get(declaring));
                }
            }

            var resultType = emit.ReturnType == TypeOnStack.Get(typeof(void)) ? null : emit.ReturnType;

            var firstParamIsThis =
                HasFlag(emit.CallingConventions, CallingConventions.HasThis) ||
                HasFlag(emit.CallingConventions, CallingConventions.ExplicitThis);

            IEnumerable<StackTransition> transitions;
            if (resultType != null)
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.Reverse().AsEnumerable(), new [] { resultType })
                    };
            }
            else
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.Reverse().AsEnumerable(), new TypeOnStack[0])
                    };
            }

            UpdateState(OpCodes.Call, methodInfo, emit.ParameterTypes, Wrap(transitions, "Call"), firstParamIsThis: firstParamIsThis, arglist: arglist);

            return this;
        }

        /// <summary>
        /// <para>Calls the given method.  Pops its arguments in reverse order (left-most deepest in the stack), and pushes the return value if it is non-void.</para>
        /// <para>If the given method is an instance method, the `this` reference should appear before any parameters.</para>
        /// <para>Call does not respect overrides, the implementation defined by the given MethodInfo is what will be called at runtime.</para>
        /// <para>To call overrides of instance methods, use CallVirtual.</para>
        /// <para>When calling VarArgs methods, arglist should be set to the types of the extra parameters to be passed.</para>
        /// </summary>
        public Emit<DelegateType> Call(MethodInfo method, Type[] arglist = null)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (HasFlag(method.CallingConvention, CallingConventions.VarArgs) && !HasFlag(method.CallingConvention, CallingConventions.Standard))
            {
                if (arglist == null)
                {
                    throw new InvalidOperationException("When calling a VarArgs method, arglist must be set");
                }
            }

            var expectedParams = ((LinqArray<ParameterInfo>)method.GetParameters()).Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            if (arglist != null)
            {
                expectedParams.AddRange(((LinqArray<Type>)arglist).Select(t => TypeOnStack.Get(t)));
            }

            // Instance methods expect this to preceed parameters
            if (HasFlag(method.CallingConvention, CallingConventions.HasThis))
            {
                var declaring = method.DeclaringType;

                if (TypeHelpers.IsValueType(declaring))
                {
                    declaring = declaring.MakePointerType();
                }

                expectedParams.Insert(0, TypeOnStack.Get(declaring));
            }

            var resultType = method.ReturnType == typeof(void) ? null : TypeOnStack.Get(method.ReturnType);

            var firstParamIsThis =
                HasFlag(method.CallingConvention, CallingConventions.HasThis) ||
                HasFlag(method.CallingConvention, CallingConventions.ExplicitThis);

            IEnumerable<StackTransition> transitions;
            if (resultType != null)
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.Reverse().AsEnumerable(), new [] { resultType })
                    };
            }
            else
            {
                transitions =
                    new[]
                    {
                        new StackTransition(expectedParams.Reverse().AsEnumerable(), new TypeOnStack[0])
                    };
            }

            UpdateState(OpCodes.Call, method, ((LinqArray<ParameterInfo>)method.GetParameters()).Select(s => s.ParameterType).AsEnumerable(), Wrap(transitions, "Call"), firstParamIsThis: firstParamIsThis, arglist: arglist);

            return this;
        }

        private bool IsLegalConstructoCall(ConstructorInfo cons)
        {
            var consDeclaredIn = cons.DeclaringType;

            var curType = ConstructorDefinedInType ?? (ConstrBuilder.DeclaringType);
            var baseType = curType.BaseType;

            var inCurrentType = curType == consDeclaredIn;
            var inBaseType = baseType != null && consDeclaredIn == baseType;

            return inCurrentType || inBaseType;
        }

        /// <summary>
        /// <para>Calls the given constructor.  Pops its arguments in reverse order (left-most deepest in the stack).</para>
        /// <para>The `this` reference should appear before any parameters.</para>
        /// </summary>
        public Emit<DelegateType> Call(ConstructorInfo cons)
        {
            if (cons == null)
            {
                throw new ArgumentNullException("cons");
            }

            if (HasFlag(cons.CallingConvention, CallingConventions.VarArgs) && !HasFlag(cons.CallingConvention, CallingConventions.Standard))
            {
                throw new NotSupportedException("Calling constructors with VarArgs is currently not supported.");
            }

            if (!IsBuildingConstructor)
            {
                throw new SigilVerificationException("Constructors may only be called directly from within a constructor, use NewObject to allocate a new object with a specific constructor.", IL.Instructions(AllLocals));
            }

            if (!IsLegalConstructoCall(cons))
            {
                throw new SigilVerificationException("Only constructors defined in the current class or it's base class may be called", IL.Instructions(AllLocals));
            }

            var expectedParams = ((LinqArray<ParameterInfo>)cons.GetParameters()).Select(s => TypeOnStack.Get(s.ParameterType)).ToList();

            var declaring = cons.DeclaringType;

            if (TypeHelpers.IsValueType(declaring))
            {
                declaring = declaring.MakePointerType();
            }

            expectedParams.Insert(0, TypeOnStack.Get(declaring));

            var transitions = 
                new[]
                {
                    new StackTransition(expectedParams.Reverse().AsEnumerable(), new TypeOnStack[0])
                };

            UpdateState(OpCodes.Call, cons, ((LinqArray<ParameterInfo>)cons.GetParameters()).Select(s => s.ParameterType).AsEnumerable(), Wrap(transitions, "Call"));

            return this;
        }
    }
}
