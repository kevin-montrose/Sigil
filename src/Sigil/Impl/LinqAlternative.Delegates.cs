
namespace Sigil.Impl
{
    internal delegate void SigilAction();
    internal delegate void SigilAction<A, B, C>(A a, B b, C c);
    
    internal delegate R SigilFunc<R>();
    internal delegate R SigilFunc<A, R>(A a);
    internal delegate R SigilFunc<A, B, R>(A a, B b);
}
