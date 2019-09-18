
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Leave an exception or catch block, branching to the given label.</para>
        /// <para>This instruction empties the stack.</para>
        /// </summary>
        public Emit Leave(Label label)
        {
            InnerEmit.Leave(label);
            return this;
        }

        /// <summary>
        /// <para>Leave an exception or catch block, branching to the label with the given name.</para>
        /// <para>This instruction empties the stack.</para>
        /// </summary>
        public Emit Leave(string name)
        {
            InnerEmit.Leave(name);
            return this;
        }
    }
}
