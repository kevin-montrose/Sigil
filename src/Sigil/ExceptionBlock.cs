using Sigil.Impl;

namespace Sigil
{
    /// <summary>
    /// <para>Represents an ExceptionBlock, which is roughly analogous to a try + catch + finally block in C#.</para>
    /// <para>To create an ExceptionBlock call BeginExceptionBlock().</para>
    /// </summary>
    public class ExceptionBlock : IOwned
    {
        object IOwned.Owner { get { return ((IOwned)Label).Owner; } }

        /// <summary>
        /// <para>A label which marks the end of the ExceptionBlock.</para>
        /// <para>
        /// This Label is meant to be targetted by Leave() from anywhere except a FinallyBlock
        /// in the ExceptionBlock.
        /// </para>
        /// <para>Remember that it is illegal to branch from within an ExceptionBlock to outside.</para>
        /// </summary>
        public Label Label { get; private set; }

        internal ExceptionBlock(Label label)
        {
            Label = label;
        }
    }
}
