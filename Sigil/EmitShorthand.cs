using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sigil
{
    public class EmitShorthand<DelegateType>
    {
        private readonly Emit<DelegateType> InnerEmit;

        internal EmitShorthand(Emit<DelegateType> inner)
        {
            InnerEmit = inner;
        }

        public Local DeclareLocal<Type>()
        {
            return DeclareLocal(typeof(Type));
        }

        public Local DeclareLocal(Type type)
        {
            return InnerEmit.DeclareLocal(type);
        }

        public Local DeclareLocal<Type>(string name)
        {
            return DeclareLocal(typeof(Type), name);
        }

        public Local DeclareLocal(Type type, string name)
        {
            return InnerEmit.DeclareLocal(type, name);
        }

        public Label DefineLabel()
        {
            return InnerEmit.DefineLabel();
        }

        public Label DefineLabel(string name)
        {
            return InnerEmit.DefineLabel(name);
        }

        public void MarkLabel(Label label)
        {
            InnerEmit.MarkLabel(label);
        }

        public ExceptionBlock BeginExceptionBlock()
        {
            return InnerEmit.BeginExceptionBlock();
        }

        public CatchBlock BeginCatchBlock<ExceptionType>(ExceptionBlock forTry)
        {
            return BeginCatchBlock(forTry, typeof(ExceptionType));
        }

        public CatchBlock BeginCatchBlock(ExceptionBlock forTry, Type exceptionType)
        {
            return InnerEmit.BeginCatchBlock(forTry, exceptionType);
        }

        public void EndCatchBlock(CatchBlock forCatch)
        {
            InnerEmit.EndCatchBlock(forCatch);
        }

        public FinallyBlock BeginFinallyBlock(ExceptionBlock forTry)
        {
            return InnerEmit.BeginFinallyBlock(forTry);
        }

        public void EndFinallyBlock(FinallyBlock forFinally)
        {
            InnerEmit.EndFinallyBlock(forFinally);
        }

        public void EndExceptionBlock(ExceptionBlock forTry)
        {
            InnerEmit.EndExceptionBlock(forTry);
        }

        public DelegateType CreateDelegate()
        {
            return InnerEmit.CreateDelegate();
        }

        public void Add()
        {
            InnerEmit.Add();
        }

        public void Add_Ovf()
        {
            InnerEmit.AddOverflow();
        }

        public void Add_Ovf_Un()
        {
            InnerEmit.UnsignedAddOverflow();
        }

        public void And()
        {
            InnerEmit.And();
        }

        public void Beq(Label label)
        {
            InnerEmit.BranchIfEqual(label);
        }

        public void Bge(Label label)
        {
            InnerEmit.BranchIfGreaterOrEqual(label);
        }

        public void Bge_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreaterOrEqual(label);
        }

        public void Bgt(Label label)
        {
            InnerEmit.BranchIfGreater(label);
        }

        public void Bgt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfGreater(label);
        }

        public void Ble(Label label)
        {
            InnerEmit.BranchIfLessOrEqual(label);
        }

        public void Ble_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLessOrEqual(label);
        }

        public void Blt(Label label)
        {
            InnerEmit.BranchIfLess(label);
        }

        public void Blt_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfLess(label);
        }

        public void Bne_Un(Label label)
        {
            InnerEmit.UnsignedBranchIfNotEqual(label);
        }

        public void Box<ValueType>()
        {
            Box(typeof(ValueType));
        }

        public void Box(Type valueType)
        {
            InnerEmit.Box(valueType);
        }

        public void Br(Label label)
        {
            InnerEmit.Branch(label);
        }

        public void Break()
        {
            InnerEmit.Break();
        }

        public void Brfalse(Label label)
        {
            InnerEmit.BranchIfFalse(label);
        }

        public void Brtrue(Label label)
        {
            InnerEmit.BranchIfTrue(label);
        }

        public void Call(MethodInfo method)
        {
            InnerEmit.Call(method);
        }

        public void Calli(CallingConventions callingConvention, Type returnType, params Type[] parameterTypes)
        {
            InnerEmit.CallIndirect(callingConvention, returnType, parameterTypes);
        }

        public void Callvirt(MethodInfo method, Type constrained = null)
        {
            InnerEmit.CallVirtual(method, constrained);
        }

        public void Castclass<ReferenceType>()
        {
            Castclass(typeof(ReferenceType));
        }

        public void Castclass(Type referenceType)
        {
            InnerEmit.CastClass(referenceType);
        }

        public void Ceq()
        {
            InnerEmit.CompareEqual();
        }

        public void Cgt()
        {
            InnerEmit.CompareGreaterThan();
        }

        public void Cgt_Un()
        {
            InnerEmit.UnsignedCompareGreaterThan();
        }

        public void Ckfinite()
        {
            InnerEmit.CheckFinite();
        }

        public void Clt()
        {
            InnerEmit.CompareLessThan();
        }

        public void Clt_Un()
        {
            InnerEmit.UnsignedCompareLessThan();
        }

        public void Conv_Ovf_I()
        {
            InnerEmit.ConvertToNativeIntOverflow();
        }

        public void Conv_Ovf_I_Un()
        {
            InnerEmit.UnsignedConvertToNativeIntOverflow();
        }

        public void Conv_Ovf_I1()
        {
            InnerEmit.ConvertToSByteOverflow();
        }

        public void Conv_Ovf_I1_Un()
        {
            InnerEmit.UnsignedConvertToSByteOverflow();
        }

        public void Conf_Ovf_I2()
        {
            InnerEmit.ConvertToInt16Overflow();
        }

        public void Conf_Ovf_I2_Un()
        {
            InnerEmit.UnsignedConvertToInt16Overflow();
        }

        public void Conf_Ovf_I4()
        {
            InnerEmit.ConvertToInt32Overflow();
        }

        public void Conf_Ovf_I4_Un()
        {
            InnerEmit.UnsignedConvertToInt32Overflow();
        }

        public void Conf_Ovf_I8()
        {
            InnerEmit.ConvertToInt64Overflow();
        }

        public void Conf_Ovf_I8_Un()
        {
            InnerEmit.UnsignedConvertToInt64Overflow();
        }

        public void Conf_Ovf_U()
        {
            InnerEmit.ConvertToUnsignedNativeIntOverflow();
        }

        public void Conf_Ovf_U_Un()
        {
            InnerEmit.UnsignedConvertToUnsignedNativeIntOverflow();
        }

        public void Conv_Ovf_U1()
        {
            InnerEmit.ConvertToByteOverflow();
        }

        public void Conv_Ovf_U1_Un()
        {
            InnerEmit.UnsignedConvertToByteOverflow();
        }

        public void Conv_Ovf_U2()
        {
            InnerEmit.ConvertToUInt16Overflow();
        }

        public void Conv_Ovf_U2_Un()
        {
            InnerEmit.UnsignedConvertToUInt16Overflow();
        }

        public void Conv_Ovf_U4()
        {
            InnerEmit.ConvertToUInt32Overflow();
        }

        public void Conv_Ovf_U4_Un()
        {
            InnerEmit.UnsignedConvertToUInt32Overflow();
        }

        public void Conv_Ovf_U8()
        {
            InnerEmit.ConvertToUInt64Overflow();
        }

        public void Conv_Ovf_U8_Un()
        {
            InnerEmit.UnsignedConvertToUInt64Overflow();
        }

        public void Conv_R_Un()
        {
            InnerEmit.UnsignedConvertToFloat();
        }

        public void Conv<PrimitiveType>()
        {
            Conv(typeof(PrimitiveType));
        }

        public void Conv(Type primitiveType)
        {
            InnerEmit.Convert(primitiveType);
        }

        public void Conv_Ovf<PrimitType>()
        {
            Conv_Ovf(typeof(PrimitType));
        }

        public void Conv_Ovf(Type primitiveType)
        {
            InnerEmit.ConvertOverflow(primitiveType);
        }

        public void Cpblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.CopyBlock(isVolatile, unaligned);
        }

        public void Cpobj<ValueType>()
        {
            Cpobj(typeof(ValueType));
        }

        public void Cpobj(Type valueType)
        {
            InnerEmit.CopyObject(valueType);
        }

        public void Div()
        {
            InnerEmit.Divide();
        }

        public void Div_Un()
        {
            InnerEmit.UnsignedDivide();
        }

        public void Dup()
        {
            InnerEmit.Duplicate();
        }

        public void Initblk(bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.InitializeBlock(isVolatile, unaligned);
        }

        public void Initobj<ValueType>()
        {
            Initobj(typeof(ValueType));
        }

        public void Initobj(Type valueType)
        {
            InnerEmit.InitializeObject(valueType);
        }

        public void Isinst<Type>()
        {
            Isinst(typeof(Type));
        }

        public void Isinst(Type type)
        {
            InnerEmit.IsInstance(type);
        }

        public void Jmp(MethodInfo method)
        {
            InnerEmit.Jump(method);
        }

        public void Ldarg(int index)
        {
            InnerEmit.LoadArgument(index);
        }

        public void Ldarga(int index)
        {
            InnerEmit.LoadArgumentAddress(index);
        }

        public void Ldc(bool b)
        {
            InnerEmit.LoadConstant(b);
        }

        public void Ldc(float f)
        {
            InnerEmit.LoadConstant(f);
        }

        public void Ldc(double d)
        {
            InnerEmit.LoadConstant(d);
        }

        public void Ldc(uint u)
        {
            InnerEmit.LoadConstant(u);
        }

        public void Ldc(int i)
        {
            InnerEmit.LoadConstant(i);
        }

        public void Ldc(long l)
        {
            InnerEmit.LoadConstant(l);
        }

        public void Ldc(ulong u)
        {
            InnerEmit.LoadConstant(u);
        }

        public void Ldelem()
        {
            InnerEmit.LoadElement();
        }

        public void Ldelema<ElementType>()
        {
            Ldelema(typeof(ElementType));
        }

        public void Ldelema(Type elementType)
        {
            InnerEmit.LoadElementAddress(elementType);
        }

        public void Ldfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadField(field, isVolatile, unaligned);
        }

        public void Ldflda(FieldInfo field)
        {
            InnerEmit.LoadFieldAddress(field);
        }

        public void Ldftn(MethodInfo method)
        {
            InnerEmit.LoadFunctionPointer(method);
        }

        public void Ldind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Ldind(typeof(Type), isVolatile, unaligned);
        }

        public void Ldind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadIndirect(type, isVolatile, unaligned);
        }

        public void Ldlen()
        {
            InnerEmit.LoadLength();
        }

        public void Ldloc(Local local)
        {
            InnerEmit.LoadLocal(local);
        }

        public void Ldloca(Local local)
        {
            InnerEmit.LoadLocalAddress(local);
        }

        public void Ldnull()
        {
            InnerEmit.LoadNull();
        }

        public void Ldobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Ldobj(typeof(ValueType), isVolatile, unaligned);
        }

        public void Ldobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.LoadObject(valueType, isVolatile, unaligned);
        }

        public void Ldstr(string str)
        {
            InnerEmit.LoadConstant(str);
        }

        public void Ldtoken(FieldInfo field)
        {
            InnerEmit.LoadConstant(field);
        }

        public void Ldtoken(MethodInfo method)
        {
            InnerEmit.LoadConstant(method);
        }

        public void Ldtoken<Type>()
        {
            Ldtoken(typeof(Type));
        }

        public void Ldtoken(Type type)
        {
            InnerEmit.LoadConstant(type);
        }

        public void Ldvirtftn(MethodInfo method)
        {
            InnerEmit.LoadVirtualFunctionPointer(method);
        }

        public void Leave(Label label)
        {
            InnerEmit.Leave(label);
        }

        public void Localloc()
        {
            InnerEmit.LocalAllocate();
        }

        public void Mul()
        {
            InnerEmit.Multiply();
        }

        public void Mul_Ovf()
        {
            InnerEmit.MultiplyOverflow();
        }

        public void Mul_Ovf_Un()
        {
            InnerEmit.UnsignedMultiplyOverflow();
        }

        public void Neg()
        {
            InnerEmit.Negate();
        }

        public void Newarr<ElementType>()
        {
            Newarr(typeof(ElementType));
        }

        public void Newarr(Type elementType)
        {
            InnerEmit.NewArray(elementType);
        }

        public void Newobj(ConstructorInfo constructor)
        {
            InnerEmit.NewObject(constructor);
        }

        public void Nop()
        {
            InnerEmit.Nop();
        }

        public void Not()
        {
            InnerEmit.Not();
        }

        public void Or()
        {
            InnerEmit.Or();
        }

        public void Pop()
        {
            InnerEmit.Pop();
        }

        public void Rem()
        {
            InnerEmit.Remainder();
        }

        public void Rem_Un()
        {
            InnerEmit.UnsignedRemainder();
        }

        public void Ret()
        {
            InnerEmit.Return();
        }

        public void Rethrow()
        {
            InnerEmit.ReThrow();
        }

        public void Shl()
        {
            InnerEmit.ShiftLeft();
        }

        public void Shr()
        {
            InnerEmit.ShiftRight();
        }

        public void Shr_Un()
        {
            InnerEmit.UnsignedShiftRight();
        }

        public void Sizeof<ValueType>()
        {
            Sizeof(typeof(ValueType));
        }

        public void Sizeof(Type valueType)
        {
            InnerEmit.SizeOf(valueType);
        }

        public void Starg(int index)
        {
            InnerEmit.StoreArgument(index);
        }

        public void Stelem()
        {
            InnerEmit.StoreElement();
        }

        public void Stfld(FieldInfo field, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreField(field, isVolatile, unaligned);
        }

        public void Stind<Type>(bool isVolatile = false, int? unaligned = null)
        {
            Stind(typeof(Type), isVolatile, unaligned);
        }

        public void Stind(Type type, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreIndirect(type, isVolatile, unaligned);
        }

        public void Stloc(Local local)
        {
            InnerEmit.StoreLocal(local);
        }

        public void Stobj<ValueType>(bool isVolatile = false, int? unaligned = null)
        {
            Stobj(typeof(ValueType), isVolatile, unaligned);
        }

        public void Stobj(Type valueType, bool isVolatile = false, int? unaligned = null)
        {
            InnerEmit.StoreObject(valueType, isVolatile, unaligned);
        }

        public void Sub()
        {
            InnerEmit.Subtract();
        }

        public void Sub_Ovf()
        {
            InnerEmit.SubtractOverflow();
        }

        public void Sub_Ovf_Un()
        {
            InnerEmit.UnsignedSubtractOverflow();
        }

        public void Switch(params Label[] labels)
        {
            InnerEmit.Switch(labels);
        }

        public void Throw()
        {
            InnerEmit.Throw();
        }

        public void Unbox<ValueType>()
        {
            Unbox(typeof(ValueType));
        }

        public void Unbox(Type valueType)
        {
            InnerEmit.Unbox(valueType);
        }

        public void Unbox_Any<ValueType>()
        {
            Unbox_Any(typeof(ValueType));
        }

        public void Unbox_Any(Type valueType)
        {
            InnerEmit.UnboxAny(valueType);
        }

        public void Xor()
        {
            InnerEmit.Xor();
        }
    }
}
