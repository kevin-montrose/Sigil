
namespace Sigil.Impl
{
    internal delegate void LocalReusableDelegate(Local local);

    internal delegate void UpdateOpCodeDelegate(System.Reflection.Emit.OpCode newOpcode);
    internal delegate System.Reflection.Emit.Label DefineLabelDelegate(System.Reflection.Emit.ILGenerator il);
    internal delegate System.Reflection.Emit.LocalBuilder DeclareLocallDelegate(System.Reflection.Emit.ILGenerator il);
}
