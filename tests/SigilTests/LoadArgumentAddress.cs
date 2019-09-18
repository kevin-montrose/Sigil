using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SigilTests
{
    public partial class LoadArgumentAddress
    {
        private delegate int LotsOfParams(
            int a1, int b1, int c1, int d1, int e1, int f1, int g1, int h1, int i1, int j1, int k1, int l1, int m1, int n1, int o1, int p1, int q1, int r1, int s1, int t1, int u1, int v1, int w1, int x1, int y1, int z1,
            int a2, int b2, int c2, int d2, int e2, int f2, int g2, int h2, int i2, int j2, int k2, int l2, int m2, int n2, int o2, int p2, int q2, int r2, int s2, int t2, int u2, int v2, int w2, int x2, int y2, int z2,
            int a3, int b3, int c3, int d3, int e3, int f3, int g3, int h3, int i3, int j3, int k3, int l3, int m3, int n3, int o3, int p3, int q3, int r3, int s3, int t3, int u3, int v3, int w3, int x3, int y3, int z3,
            int a4, int b4, int c4, int d4, int e4, int f4, int g4, int h4, int i4, int j4, int k4, int l4, int m4, int n4, int o4, int p4, int q4, int r4, int s4, int t4, int u4, int v4, int w4, int x4, int y4, int z4,
            int a5, int b5, int c5, int d5, int e5, int f5, int g5, int h5, int i5, int j5, int k5, int l5, int m5, int n5, int o5, int p5, int q5, int r5, int s5, int t5, int u5, int v5, int w5, int x5, int y5, int z5,
            int a6, int b6, int c6, int d6, int e6, int f6, int g6, int h6, int i6, int j6, int k6, int l6, int m6, int n6, int o6, int p6, int q6, int r6, int s6, int t6, int u6, int v6, int w6, int x6, int y6, int z6,
            int a7, int b7, int c7, int d7, int e7, int f7, int g7, int h7, int i7, int j7, int k7, int l7, int m7, int n7, int o7, int p7, int q7, int r7, int s7, int t7, int u7, int v7, int w7, int x7, int y7, int z7,
            int a8, int b8, int c8, int d8, int e8, int f8, int g8, int h8, int i8, int j8, int k8, int l8, int m8, int n8, int o8, int p8, int q8, int r8, int s8, int t8, int u8, int v8, int w8, int x8, int y8, int z8,
            int a9, int b9, int c9, int d9, int e9, int f9, int g9, int h9, int i9, int j9, int k9, int l9, int m9, int n9, int o9, int p9, int q9, int r9, int s9, int t9, int u9, int v9, int w9, int x9, int y9, int z9,
            int a0, int b0, int c0, int d0, int e0, int f0, int g0, int h0, int i0, int j0, int k0, int l0, int m0, int n0, int o0, int p0, int q0, int r0, int s0, int t0, int u0, int v0, int w0, int x0, int y0, int z0);

        [Fact]
        public void All()
        {
            var e1 = Emit<LotsOfParams>.NewDynamicMethod();

            for (ushort i = 0; i < 260; i++)
            {
                e1.LoadArgumentAddress(i);
                e1.LoadIndirect<int>();
            }

            for (var i = 0; i < 259; i++)
            {
                e1.Add();
            }

            e1.Return();

            var d1 = e1.CreateDelegate();

            var rand = new Random();
            var args = new List<int>();
            for (var i = 0; i < 260; i++)
            {
                args.Add(rand.Next(10));
            }

            var ret = (int)d1.DynamicInvoke(args.Cast<object>().ToArray());

            Assert.Equal(args.Sum(), ret);
        }
    }
}
