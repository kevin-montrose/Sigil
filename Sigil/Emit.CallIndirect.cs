using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes a void return and no parameters.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(void), TypeHelpers.EmptyTypes);
        }

        #region Generic CallIndirect Finder Helpers

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and no parameters.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15));
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This helper assumes ReturnType as a return and parameters of the types given in ParameterType*.
        /// </summary>
#if NET45
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
        public Emit<DelegateType> CallIndirect<ReturnType, ParameterType1, ParameterType2, ParameterType3, ParameterType4, ParameterType5, ParameterType6, ParameterType7, ParameterType8, ParameterType9, ParameterType10, ParameterType11, ParameterType12, ParameterType13, ParameterType14, ParameterType15, ParameterType16>(CallingConventions callConventions)
        {
            return CallIndirect(callConventions, typeof(ReturnType), typeof(ParameterType1), typeof(ParameterType2), typeof(ParameterType3), typeof(ParameterType4), typeof(ParameterType5), typeof(ParameterType6), typeof(ParameterType7), typeof(ParameterType8), typeof(ParameterType9), typeof(ParameterType10), typeof(ParameterType11), typeof(ParameterType12), typeof(ParameterType13), typeof(ParameterType14), typeof(ParameterType15), typeof(ParameterType16));
        }

        #endregion

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// 
        /// This override allows an arglist to be passed for calling VarArgs methods.
        /// </summary>
        public Emit<DelegateType> CallIndirect(CallingConventions callConventions, Type returnType, Type[] parameterTypes, Type[] arglist = null)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var known = CallingConventions.Any | CallingConventions.ExplicitThis | CallingConventions.HasThis | CallingConventions.Standard | CallingConventions.VarArgs;
            known = ~known;

            if ((callConventions & known) != 0)
            {
                throw new ArgumentException("Unexpected value not in CallingConventions", "callConventions");
            }

            if (!AllowsUnverifiableCIL)
            {
                FailUnverifiable("CallIndirect");
            }

            if (HasFlag(callConventions, CallingConventions.VarArgs) && !HasFlag(callConventions, CallingConventions.Standard))
            {
                if (arglist == null)
                {
                    throw new InvalidOperationException("When calling a VarArgs method, arglist must be set");
                }
            }

            var takeExtra = 1;

            if (HasFlag(callConventions, CallingConventions.HasThis))
            {
                takeExtra++;
            }

            IEnumerable<StackTransition> transitions;
            if (HasFlag(callConventions, CallingConventions.HasThis))
            {
                var p = new List<Type>();
                p.Add(typeof(NativeIntType));
                p.AddRange(LinqAlternative.Reverse(parameterTypes).AsEnumerable());
                p.Add(typeof(WildcardType));

                if (returnType != typeof(void))
                {
                    transitions =
                        new[]
                        {
                            new StackTransition(p, new [] { returnType })
                        };
                }
                else
                {
                    transitions =
                        new[]
                        {
                            new StackTransition(p, TypeHelpers.EmptyTypes)
                        };
                }
            }
            else
            {
                var p = new List<Type>();
                p.Add(typeof(NativeIntType));
                p.AddRange(LinqAlternative.Reverse(parameterTypes).AsEnumerable());

                if (returnType != typeof(void))
                {
                    transitions =
                        new[]
                        {
                            new StackTransition(p, new [] { returnType })
                        };
                }
                else
                {
                    transitions =
                        new[]
                        {
                            new StackTransition(p, TypeHelpers.EmptyTypes)
                        };
                }
            }

            var onStack = CurrentVerifiers.InferStack(LinqAlternative.ElementAt(transitions, 0).PoppedFromStack.Length);
            if (onStack != null && onStack.Count > 0)
            {
                var funcPtr = onStack.First();

                if (funcPtr == TypeOnStack.Get<NativeIntType>() && funcPtr.HasAttachedMethodInfo)
                {
                    if (funcPtr.CallingConvention != callConventions)
                    {
                        throw new SigilVerificationException("CallIndirect expects method calling conventions to match, found " + funcPtr.CallingConvention + " on the stack", IL.Instructions(AllLocals));
                    }

                    if (HasFlag(callConventions, CallingConventions.HasThis))
                    {
                        var thisRef = onStack.Last();

                        if (!ExtensionMethods.IsAssignableFrom(funcPtr.InstanceType, thisRef))
                        {
                            throw new SigilVerificationException("CallIndirect expects a 'this' value assignable to " + funcPtr.InstanceType + ", found " + thisRef, IL.Instructions(AllLocals));
                        }
                    }

                    if (funcPtr.ReturnType != returnType)
                    {
                        throw new SigilVerificationException("CallIndirect expects method return types to match, found " + funcPtr.ReturnType + " on the stack", IL.Instructions(AllLocals));
                    }
                }
            }

            UpdateState(OpCodes.Calli, callConventions, returnType, parameterTypes, Wrap(transitions, "CallIndirect"), arglist);

            return this;
        }

        /// <summary>
        /// Pops a pointer to a method, and then all it's arguments (in reverse order, left-most parameter is deepest on the stack) and calls
        /// invokes the method pointer.  If the method returns a non-void result, it is pushed onto the stack.
        /// </summary>
        public Emit<DelegateType> CallIndirect(CallingConventions callConventions, Type returnType, params Type[] parameterTypes)
        {
            return CallIndirect(callConventions, returnType, parameterTypes, arglist: null);
        }
    }
}
