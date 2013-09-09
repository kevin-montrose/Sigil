
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// Pops a value off the stack and branches to the label at the index of that value in the given labels.
        /// 
        /// If the value is out of range, execution falls through to the next instruction.
        /// </summary>
        public Emit Switch(params Label[] labels)
        {
            InnerEmit.Switch(labels);
            return this;
        }

        /// <summary>
        /// Pops a value off the stack and branches to the label at the index of that value in the given label names.
        /// 
        /// If the value is out of range, execution falls through to the next instruction.
        /// </summary>
        public Emit Switch(params string[] names)
        {
            InnerEmit.Switch(names);
            return this;
        }
    }
}
