using Sigil.Impl;
using System;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Unconditionally branches to the given label.
        /// </summary>
        public Emit<DelegateType> Branch(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return Branch(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Br, label, Wrap(StackTransition.None(), "Branch"), out update);

            var valid = CurrentVerifiers.UnconditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("Branch", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Br, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Br);

            MustMark = true;

            return this;
        }

        /// <summary>
        /// Unconditionally branches to the label with the given name.
        /// </summary>
        public Emit<DelegateType> Branch(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return Branch(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the given label.
        /// </summary>
        public Emit<DelegateType> BranchIfEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(WildcardType), typeof(WildcardType) }, TypeHelpers.EmptyTypes)
                };

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Beq, label, Wrap(transitions, "BranchIfEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Beq, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Beq);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if both are equal branches to the label with the given name.
        /// </summary>
        public Emit<DelegateType> BranchIfEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfEqual(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the given label.
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfNotEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return UnsignedBranchIfNotEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            var transitions =
                new[] 
                {
                    new StackTransition(new [] { typeof(WildcardType), typeof(WildcardType) }, TypeHelpers.EmptyTypes)
                };

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Bne_Un, label, Wrap(transitions, "UnsignedBranchIfNotEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("UnsignedBranchIfNotEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Bne_Un, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Bne_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, if they are not equal (when treated as unsigned values) branches to the label with the given name.
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfNotEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return UnsignedBranchIfNotEqual(Labels[name]);
        }

        private TransitionWrapper BranchComparableTransitions(string name)
        {
            return
                Wrap(
                    new[]
                    {
                        new StackTransition(new [] { typeof(int), typeof(int) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(int), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(int) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(NativeIntType), typeof(NativeIntType) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(long), typeof(long) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(float), typeof(float) }, TypeHelpers.EmptyTypes),
                        new StackTransition(new [] { typeof(double), typeof(double) }, TypeHelpers.EmptyTypes)
                    },
                    name
                );
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreaterOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfGreaterOrEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Bge, label, BranchComparableTransitions("BranchIfGreaterOrEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfGreaterOrEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Bge, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Bge);


            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreaterOrEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfGreaterOrEqual(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreaterOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return UnsignedBranchIfGreaterOrEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Bge_Un, label, BranchComparableTransitions("UnsignedBranchIfGreaterOrEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("UnsignedBranchIfGreaterOrEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Bge_Un, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Bge_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreaterOrEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return UnsignedBranchIfGreaterOrEqual(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreater(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfGreater(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Bgt, label, BranchComparableTransitions("BranchIfGreater"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfGreater", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Bgt, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Bgt);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfGreater(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfGreater(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreater(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return UnsignedBranchIfGreater(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Bgt_Un, label, BranchComparableTransitions("UnsignedBranchIfGreater"), out update);

            var valid = CurrentVerifiers.UnconditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("UnsignedBranchIfGreater", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Bgt_Un, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Bgt_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is greater than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfGreater(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return UnsignedBranchIfGreater(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLessOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfLessOrEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Ble, label, BranchComparableTransitions("BranchIfLessOrEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfLessOrEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Ble, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Ble);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLessOrEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfLessOrEqual(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLessOrEqual(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return UnsignedBranchIfLessOrEqual(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Ble_Un, label, BranchComparableTransitions("UnsignedBranchIfLessOrEqual"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("UnsignedBranchIfLessOrEqual", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Ble_Un, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Ble_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than or equal to the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLessOrEqual(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return UnsignedBranchIfLessOrEqual(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLess(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfLess(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Blt, label, BranchComparableTransitions("BranchIfLess"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfLess", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Blt, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Blt);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value.
        /// </summary>
        public Emit<DelegateType> BranchIfLess(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfLess(Labels[name]);
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the given label if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLess(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return UnsignedBranchIfLess(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Blt_Un, label, BranchComparableTransitions("UnsignedBranchIfLess"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("UnsignedBranchIfLess", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Blt_Un, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Blt_Un);

            return this;
        }

        /// <summary>
        /// Pops two arguments from the stack, branches to the label with the given name if the second value is less than the first value (when treated as unsigned values).
        /// </summary>
        public Emit<DelegateType> UnsignedBranchIfLess(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return UnsignedBranchIfLess(Labels[name]);
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public Emit<DelegateType> BranchIfFalse(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfFalse(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            var transitions = 
                new []
                {
                    new StackTransition(new [] { typeof(WildcardType) }, TypeHelpers.EmptyTypes)
                };

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Brfalse, label, Wrap(transitions, "BranchIfFalse"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfFalse", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Brfalse, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Brfalse);

            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the label with the given name if the value is false.
        /// 
        /// A value is false if it is zero or null.
        /// </summary>
        public Emit<DelegateType> BranchIfFalse(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfFalse(Labels[name]);
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the given label if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public Emit<DelegateType> BranchIfTrue(Label label)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }

            if (((IOwned)label).Owner != this)
            {
                if (((IOwned)label).Owner is DisassembledOperations<DelegateType>)
                {
                    return BranchIfTrue(label.Name);
                }

                FailOwnership(label);
            }

            UnusedLabels.Remove(label);

            var transitions =
                new[]
                {
                    new StackTransition(new [] { typeof(WildcardType) }, TypeHelpers.EmptyTypes)
                };

            UpdateOpCodeDelegate update;
            UpdateState(OpCodes.Brtrue, label, Wrap(transitions, "BranchIfTrue"), out update);

            var valid = CurrentVerifiers.ConditionalBranch(label);
            if (!valid.Success)
            {
                throw new SigilVerificationException("BranchIfTrue", valid, IL.Instructions(AllLocals));
            }

            Branches.Add(SigilTuple.Create(OpCodes.Brtrue, label, IL.Index));

            BranchPatches[IL.Index] = SigilTuple.Create(label, update, OpCodes.Brtrue);

            return this;
        }

        /// <summary>
        /// Pops one argument from the stack, branches to the label with the given name if the value is true.
        /// 
        /// A value is true if it is non-zero or non-null.
        /// </summary>
        public Emit<DelegateType> BranchIfTrue(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            return BranchIfTrue(Labels[name]);
        }
    }
}
