using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    [TestClass]
    public class VerifiableTrackerT
    {
        [TestMethod]
        public void One()
        {
            var tracker = new VerifiableTracker();
            var x = tracker.Transition(new [] { new StackTransition(Type.EmptyTypes, new[] { typeof(string) }) });

            Assert.IsTrue(x);
        }

        [TestMethod]
        public void Two()
        {
            var tracker = new VerifiableTracker();
            var x = tracker.Transition(new[] { new StackTransition(Type.EmptyTypes, new[] { typeof(int) }) });
            var y = tracker.Transition(new[] { new StackTransition(Type.EmptyTypes, new[] { typeof(int) }) });
            var z = tracker.Transition(new[] { new StackTransition(new [] { typeof(int), typeof(int) }, new[] { typeof(int) }) });

            Assert.IsTrue(x);
            Assert.IsTrue(y);
            Assert.IsTrue(z);
        }

        [TestMethod]
        public void Three()
        {
            var tracker = new VerifiableTracker();
            var x = tracker.Transition(new[] { new StackTransition(Type.EmptyTypes, new[] { typeof(int) }) });
            var y = tracker.Transition(new[] { new StackTransition(new[] { typeof(string) }, new[] { typeof(string) }) });

            Assert.IsTrue(x);
            Assert.IsFalse(y);
        }

        [TestMethod]
        public void Four()
        {
            var tracker = new VerifiableTracker(true);
            var x = tracker.Transition(new[] { new StackTransition(new[] { typeof(string) }, new[] { typeof(string) }) });

            var other = new VerifiableTracker(false);
            var y = other.Transition(new[] { new StackTransition(Type.EmptyTypes, new[] { typeof(int) }) });
            var z = other.Transition(new[] { new StackTransition(new [] { typeof(int) }, new[] { typeof(string) }) });

            var aleph = tracker.Incoming(other);

            Assert.IsTrue(x);
            Assert.IsTrue(y);
            Assert.IsTrue(z);
            Assert.IsTrue(aleph);
        }

        [TestMethod]
        public void Five()
        {
            var tracker = new VerifiableTracker(true);
            var x = tracker.Transition(new[] { new StackTransition(new[] { typeof(string) }, new[] { typeof(string) }) });

            var other = new VerifiableTracker(false);
            var y = other.Transition(new[] { new StackTransition(Type.EmptyTypes, new[] { typeof(int) }) });
            var z = other.Transition(new[] { new StackTransition(new[] { typeof(int) }, new[] { typeof(string) }) });
            var aleph = other.Transition(new[] { new StackTransition(new[] { typeof(string) }, new[] { typeof(object) }) });

            var bet = tracker.Incoming(other);

            Assert.IsTrue(x);
            Assert.IsTrue(y);
            Assert.IsTrue(z);
            Assert.IsTrue(aleph);
            Assert.IsFalse(bet);
        }
    }
}
