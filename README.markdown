#Sigil

A fail-fast, validating helper for [DynamicMethod](http://msdn.microsoft.com/en-us/library/system.reflection.emit.dynamicmethod.aspx) and [ILGenerator](http://msdn.microsoft.com/en-us/library/system.reflection.emit.ilgenerator.aspx).

##Usage

Sigil is a roughly 1-to-1 replacement for ILGenerator.  Rather than calling ILGenerator.Emit(OpCode, ...), you call Emit<DelegateType>.OpCode(...).

Unlike ILGenerator, Sigil will fail as soon as an error is detected in the emitted IL.

###Creating an Emit

Sigil is oriented mostly towards DynamicMethod, but does support creating methods with TypeBuilder.

To create an `Emit<DelegateType>`:
```
var emiter = Emit<Func<int>>.NewDynamicMethod("MyMethod");
```

To build a static method with Sigil:
```
TypeBuilder myBuilder = ...;
var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Static", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard);
```

To build an instance method with Sigil:  
```
TypeBuilder myBuilder = ...;
var emiter = Emit<Func<int, string>>.BuildMethod(myBuilder, "Instance", MethodAttributes.Public, CallingConventions.Standard | CallingConventions.HasThis);
// Technically this is a Func<myBuilder, int string>; but because myBuilder isn't complete
//   the generic parameters skip the `this` reference.  myBuilder will still be available as the
//   first argument to the method
```

Call `CreateDelegate()` and `CreateMethod()` to finish building with DynamicMethod and TypeBuilder respectively.

###Instructions and Validation

There are methods on `Emit<DelegateType>` for each legal CIL opcode.  Note that not all CIL opcodes are legal within DynamicMethods.

DynamicMethods are created using a different Module with the current assembly's trust level, if loaded under full trust unverifiable instructions are legal.

```
// Create a delegate that sums two integers
var emiter = Emit<Func<int, int, int>>.NewDynamicMethod("MyMethod");
emiter.LoadArgument(0);
emiter.LoadArgument(1);
emiter.Add();
emiter.Return();
var del = emiter.CreateDelegate();

// prints "473"
Console.WriteLine(del(314, 159));
```

Sigil validates the CIL stream as each instruction is added, and throws a `SigilVerificationException` as soon as an illegal program is detected.

```
var emiter = Emit<Func<int, string, int>>.NewDynamicMethod("MyMethod");
emiter.LoadArgument(0);
emiter.LoadArgument(1);
emiter.Add();   // Throws a SigilVerificationException, indicating that Add() isn't defined for [int, string]
emiter.Return();
```

SigilVerificationExceptions include the types on the stack when thrown, to aid in debugging.

###Locals

Sigil exposes `DeclareLocal<Type>()` for creating new locals, and a number of OpCodes take locals as operands.

If a local is unused in a method body, a SigilVerificationException will be thrown when `CreateDelegate()` is called.

Locals implement IDisposable, letting you free locals up for Sigil to reuse.  This can result in more compact code, and a smaller stack frame.

The following code only allocates a single local  
```
var e1 = Emit<Func<int>>.NewDynamicMethod();

using (var a = e1.DeclareLocal<int>("a"))
{
	e1.LoadLocal(a);
	e1.LoadConstant(1);
	e1.Add();
}

// reuses the definition of "a", since it's available and the types match
using (var b = e1.DeclareLocal<int>("b"))
{
	e1.StoreLocal(b);
	e1.LoadLocal(b);
	e1.Return();
}
```

###Labels and Branches

The methods `DefineLabel` and `MarkLabel`, and the instruction family `Branch*` and `Leave` are provided to specify control flow.

For example:
```
var emiter = Emit<Func<int>>.NewDynamicMethod("Unconditional");

var label1 = emiter.DefineLabel("label1");
var label2 = emiter.DefineLabel("label2");
var label3 = emiter.DefineLabel("label3");

emiter.LoadConstant(1);
emiter.Branch(label1);

emiter.MarkLabel(label2);
emiter.LoadConstant(2);
emiter.Branch(label3);

emiter.MarkLabel(label1);
emiter.Branch(label2);

emiter.MarkLabel(label3); // the top of the stack is the first element
emiter.Add();
emiter.Return();

var d = emiter.CreateDelegate();
d();  // returns 3
```

###Try, Catch, and Finally

Sigil exposes `BeginExceptionBlock`, `EndExceptionBlock`, `BeginCatchBlock`, `BeginCatchAllBlock`, `EndCatchBlock`, `BeginFinallyBlock`, and `EndFinallyBlock`.

An example of dynamically building a try/catch block:
```
MethodInfo mayFail = ...;
MethodInfo alwaysCall = ...;
var emiter = Emit<Func<string, bool>>.NewDynamicMethod("TryCatchFinally");

var inputIsNull = emiter.DefineLabel("ifNull");  // names are purely for ease of debugging, and are optional
var tryCall = emiter.DefineLabel("tryCall");

emiter.LoadArgument(0);
emiter.LoadNull();
emiter.BranchIfEqual(inputIsNull);
emiter.Branch(tryCall);

emiter.MarkLabel(inputIsNull, Type.EmptyTypes);
emiter.LoadConstant(false);
emiter.Return();

emiter.MarkLabel(tryCall);

var succeeded = emiter.DeclareLocal<bool>("succeeded");
var t = emiter.BeginExceptionBlock();
emiter.Call(mayFail);
emiter.LoadConstant(true);
emiter.StoreLocal(succeeded);

var c = emiter.BeginCatchAllBlock(t);
emiter.Pop();   // Remove exception
emiter.LoadConstant(false);
emiter.StoreLocal(succeeded);
emiter.EndCatchBlock(c);

var f = emiter.BeginFinallyBlock(t);
emiter.Call(alwaysCall);
emiter.EndFinallyBlock(f);

emiter.EndExceptionBlock(t);

emiter.LoadLocal(succeeded);
emiter.Return();

var del = emiter.CreateDelegate();
```

The above is equivalent to:
```
Func<string, bool> del =
   s => 
   {
      if(s == null) return false;

	  bool succeeded;
	  try
	  {
	     mayFail();
		 succeeded = true;
	  }
	  catch
	  {
	     succeeded = false;
	  }
	  finally
	  {
	     alwaysCall();
	  }

	  return succeeded;
   };
```

###Better Names And Call Chaining

Don't know all the IL instructions offhand?  The interface on `Emit<DelegateType>` has longer, more descriptive, names for all supported OpCodes.

Already really familiar with `ILGenerator` and `OpCodes`?  `Emit<DelegateType>.AsShorthand()` gives you a proxy to the same functions 
as `Emit<DelegateType>` but with shorter, more familiar names.

Loading a constant with `Emit<DelegateType>`:
```
Emit<DelegateType> emit = ...;
emit.LoadConstant(123);
```

Loading a constant with `Emit<DelegateType>.AsShorthand()`:
```
Emit<DelegateType> emit = ...;
var e = emit.AsShorthand();
e.Ldc(123);
```

Furthermore, most Sigil methods return `this` to allow for call chaining.  Those methods (like `DeclareLocal<T>()`) that would normally return
a value also have an override which places results in out parameters.

###Automated OpCode Choice

Many methods in Sigil map to multiple OpCodes, the ideal one is chosen automatically.

For example, `Br_S` is chosen when `Branch(Label)` is called if the offset needed is small enough for `Br_S` to be used instead of `Br`.
Similarly, `LoadIndirect<Type>()` chooses the correct version of `Ldind_*` based on it's generic parameter.

The `tailcall` and `readonly` prefixes are also inserted automatically.

The `volatile` prefix is inserted automatically when using `LoadField()` with non FieldBuilder FieldInfos.  An override is available as well.

The `unaligned` prefix is not automatically inserted, but is available as an optional parameters on all operations where it is legal.

While generally 1-to-1, Sigil does provide single methods for "families" of opcodes, all methods that map to multiple opcodes are listed below:

 - `Branch(Label)`, `BranchIfGreater(Label)`, etc -&gt; the _S variants are used if possible  
 - `Convert(...)` -&gt; Conv_* opcodes depending on the type  
 - `ConvertOverflow(...)` -&gt; Conv_Ovf_* opcodes depending on the type  
 - `UnsignedConvertOverflow(...)` -&gt; Conv_Ovf_*_Un opcodes depending on the type  
   - note that `UnsignedConvertToFloat()` is separate from the three above, as their is no overflow checking for Conv_R_Un  
 - `Leave(Label)` -&gt; Leave_S is used if possible  
 - `LoadArgument(int)` -&gt; Ldarg_0 through Ldarg_3, and Ldarg_S are used if possible  
 - `LoadArgumentAddress(int)` -&gt; Ldarga_S is used if possible  
 - `LoadConstant(...)` -&gt; Ldc_I4_M1 through Ldc_I4_8 are used if possible; Ldstr, Ldc_R4, Ldc_R8, and Ldtoken are used depending on the override  
 - `LoadElement(...)` -&gt; Ldelem_*, depending on the passed type  
 - `LoadField(FieldInfo)` -&gt; Ldfld or Ldsfld depending on the FieldInfo  
 - `LoadFieldAddress(FieldInfo)` -&gt; Ldflda or Ldsflda depending on the FieldIfno  
 - `LoadIndirect(...)` -&gt; Ldind_* depending on the passed type  
 - `LoadLocal(Local)` -&gt; Ldloc_0 through Ldloc_3, and Ldloc_S are used if possible  
 - `LoadLocalAddress(...)` -&gt; Ldloca_S is used if possible  
 - `StoreArgument(...)` -&gt; Starg_S is used if possible  
 - `StoreElement(...)` -&gt; Stelem or Stelem_* depending on the passed type  
 - `StoreField(FieldInfo)` -&gt; Stfld or Stsfld depending on the FieldInfo  
 - `StoreIndirect(...)` -&gt; Stind_* depending on the type  
 - `StoreLocal(Local)` -&gt; Stloc_0 through Stloc_3, and Stloc_S are used if possible  

###Debugging

In addition to failing fast, Sigil also exposes some additional information to aid in debugging.

Emit exposes an Instructions method which returns a string represention of the IL stream.  SigilVerificationException, in addition to a useful
Message property, also has a GetDebugInfo method which returns additional details (like the state of the stack, or an invalid code path).

Be aware that these features are meant as debugging aids, and their contents and the formatting of said contents may change at any time.

###Disassembly

The `Disassembler<DelegateType>` adds limited support for disassembling .NET delegates starting with Sigil 3.0.0.

The `Disassembler<DelegateType>.Disassemble(...)` method returns a `DisassembleOperations` object which can be used to inspect 
the inner workings of the delegate, and re-emit it under certain circumstances.

For example:
```
Func<string, int> del =
    str =>
    {
        var i = int.Parse(str);
        return (int)Math.Pow(2, i);
    };

var ops = Sigil.Disassembler<Func<string, int>>.Disassemble(del);
var methods = ops.Where(o => new[] { OpCodes.Call, OpCodes.Callvirt }.Contains(o.OpCode)).ToList();
```
Will find all calls to methods, which in this case would be `Int32.Parse(String)` and `Math.Pow(Double, Double)`.  The appropriate MethodInfos
will be in the Parameters property on Operation.

`DisassembledOperations` also provides usage information, like in `Emit<DelegateType>.TraceOperationUsage()`, which allows you to trace the flow
of values through a delegate.

The `DisassembledOperations<DelegateType>.EmitAll(...)` method emits decompiled operations into a new emit, the standard Sigil optimizations
will be applied so the CIL generated will not necessarily be exactly the same.  Also be aware that Sigil cannot emit delegates that close over
variables, you can check the `CanEmit` property on `DisassembledOperations<DelegateType>` to distiguish these delegates.

###Unsupported Operations

Fault blocks are not supported because of their rarity (there is no C# equivalent) and because they are forbidden in dynamic methods.

Sigil does not support Calli when disassembling delegates, as the C# compilers will not emit that instruction it is currently untestable.

###Performance

Since Sigil performs a great deal of verification it is necessarily slower than using ILGenerator directly.  That being said, Sigil should be adequately performant for most purposes.

Sigil may be too slow for practical use if you need:

  - More than ~100 labels and branches
  - Methods with more than ~10,000 instructions

Some costly optimizations can be disabled via the OptimizationOptions enumeration, and some validation steps can be deferred via the ValidationOptions enumeration.