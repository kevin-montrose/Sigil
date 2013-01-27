#Sigil

A fail-fast, validating helper for [DynamicMethod](http://msdn.microsoft.com/en-us/library/system.reflection.emit.dynamicmethod.aspx) and [ILGenerator](http://msdn.microsoft.com/en-us/library/system.reflection.emit.ilgenerator.aspx).

##Usage

Sigil is a roughly 1-to-1 replacement for ILGenerator.  Rather than calling ILGenerator.Emit(OpCode, ...), you call Emit<DelegateType>.OpCode(...).

Unlike ILGenerator, Sigil will fail as soon as an error is detected in the emitted IL.

###Creating an Emit

Sigil currently only supports creating DynamicMethods.

To create an `Emit<DelegateType>`:
```
var emiter = Emit<Func<int>>.NewDynamicMethod("MyMethod");
```

###Instructions and Validation

There are methods on Emit<DelegateType> for each legal CIL opcode.  Note that not all CIL opcodes are legal within DynamicMethods.

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

Sigil validates the CIL stream as each instruction is added, and throws a `SigilException` as soon as an illegal program is detected.

```
var emiter = Emit<Func<int, string, int>>.NewDynamicMethod("MyMethod");
emiter.LoadArgument(0);
emiter.LoadArgument(1);
emiter.Add();   // Throws a SigilException, indicating that Add() isn't defined for [int, string]
emiter.Return();
```

SigilExceptions include the types on the stack when thrown, to aid in debugging.

###Locals

Sigil exposes `DeclareLocal<Type>()` for creating new locals, and a number of OpCodes take locals as operands.

If a local is unused in a method body, a SigilException will be thrown when `CreateDelegate()` is called.

###Labels, Try, Catch, and Finally

Sigil exposes `DefineLabel`, `MarkLabel`, `BeginExceptionBlock`, `EndExceptionBlock`, `BeginCatchBlock`, `BeginCatchAllBlock`, `EndCatchBlock`, `BeginFinallyBlock`, and `EndFinallyBlock`.

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

emiter.MarkLabel(inputIsNull);
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

###Automated OpCode Choice

Many methods in Sigil map to multiple OpCodes, the ideal one is chosen automatically.

For example, `Br_S` is chosen when `Branch(Label)` is called if the offset needed is small enough for `Br_S` to be used instead of `Br`.
Similarly, `LoadIndirect<Type>()` chooses the correct version of `Ldind_*` based on it's generic parameter.

The `tailcall` prefix is also inserted automatically, but not the `volatile` and `unaligned` prefixes.

#Sigil is a WORK IN PROGRESS

Use at your own risk, there are almost certainly serious bugs at the moment.