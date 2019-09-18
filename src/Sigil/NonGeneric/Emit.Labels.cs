
namespace Sigil.NonGeneric
{
    public partial class Emit
    {
        /// <summary>
        /// <para>Defines a new label.</para>
        /// <para>This label can be used for branching, leave, and switch instructions.</para>
        /// <para>A label must be marked exactly once after being defined, using the MarkLabel() method.        </para>
        /// </summary>
        public Label DefineLabel(string name = null)
        {
            return InnerEmit.DefineLabel(name);
        }

        /// <summary>
        /// <para>Defines a new label.</para>
        /// <para>This label can be used for branching, leave, and switch instructions.</para>
        /// <para>A label must be marked exactly once after being defined, using the MarkLabel() method.        </para>
        /// </summary>
        public Emit DefineLabel(out Label label, string name = null)
        {
            InnerEmit.DefineLabel(out label, name);

            return this;
        }

        /// <summary>
        /// <para>Marks a label in the instruction stream.</para>
        /// <para>When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.</para>
        /// <para>Labels can only be marked once, and *must* be marked before creating a delegate.</para>
        /// <para>
        /// Logically after a Branch or Leave instruction, a stack assertion is required to continue emiting.  The stack
        /// is assumed to match that state in these cases.
        /// </para>
        /// <para>In all other cases, a stack assertion is merely checked (and if failing, a verification exception is thrown).</para>
        /// <para>In the assertion, the top of the stack is the first (0-indexed, left-most) parameter.</para>
        /// </summary>
        public Emit MarkLabel(Label label)
        {
            InnerEmit.MarkLabel(label);
            return this;
        }

        /// <summary>
        /// <para>Marks a label with the given name in the instruction stream.</para>
        /// <para>When branching, leaving, or switching with a label control will be transfered to where it was *marked* not defined.</para>
        /// <para>Labels can only be marked once, and *must* be marked before creating a delegate.</para>
        /// </summary>
        public Emit MarkLabel(string name)
        {
            InnerEmit.MarkLabel(name);
            return this;
        }
    }
}
