
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Leave an exception or catch block, branching to the given label.
        /// 
        /// This instruction empties the stack.
        /// </summary>
        public Emit Leave(Label label)
        {
            InnerEmit.Leave(label);
            return this;
        }

        /// <summary>
        /// Leave an exception or catch block, branching to the label with the given name.
        /// 
        /// This instruction empties the stack.
        /// </summary>
        public Emit Leave(string name)
        {
            InnerEmit.Leave(name);
            return this;
        }
    }
}
