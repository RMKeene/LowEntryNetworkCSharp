using LowEntryNetworkCSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LowEntryNetworkCSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string A = "Hello world!";
            string B = "This is a long phrase so we can see if we go to longer string length unlike the previous 0C on the start. This way we see if our integer code is correct.";
            bool[] C = { true, false, false, true, true, true };
            byte[] D = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };
            double[] E = { 3.333, 1.111, 2.222, 5.555, -7.777 };
            float[] F = { 3.333f, 1.111f, 2.222f, 5.555f, -7.777f };
            Int32[] G = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 200, 3800, 8389000 };
            UInt64[] H = { 100, 101, 102, 103 };
            string[] I = { "One", "Two", "Three", "Four" };

            LowEntryByteWriter bw = new LowEntryByteWriter();
            bw.AddStringUtf8(A);
            string s = BitConverter.ToString(bw.buf.ToArray());
            Assert.AreEqual(s, "0C-48-65-6C-6C-6F-20-77-6F-72-6C-64-21");

            bw.AddStringUtf8(B);
            s = BitConverter.ToString(bw.buf.ToArray());
            //                  SZ  H  e  l  l  o     W  o  r  l  d  ! SZ SZ SZ SZ  T  h  i  s     i  s     a     l  o  n  g     p  h  r  a  s  e...
            //                  0C hex = 12 decimal                  bit flag   99 hex = 153 decimal
            Assert.AreEqual(s, "0C-48-65-6C-6C-6F-20-77-6F-72-6C-64-21-80-00-00-99-54-68-69-73-20-69-73-20-61-20-6C-6F-6E-67-20-70-68-72-61-73-65-20-73-6F-20-77-65-20-63-61-6E-20-73-65-65-20-69-66-20-77-65-20-67-6F-20-74-6F-20-6C-6F-6E-67-65-72-20-73-74-72-69-6E-67-20-6C-65-6E-67-74-68-20-75-6E-6C-69-6B-65-20-74-68-65-20-70-72-65-76-69-6F-75-73-20-30-43-20-6F-6E-20-74-68-65-20-73-74-61-72-74-2E-20-54-68-69-73-20-77-61-79-20-77-65-20-73-65-65-20-69-66-20-6F-75-72-20-69-6E-74-65-67-65-72-20-63-6F-64-65-20-69-73-20-63-6F-72-72-65-63-74-2E");

            LowEntryByteReader br = new LowEntryByteReader(bw.buf.ToArray());
            string AA = br.GetStringUtf8();
            Assert.AreEqual(A, AA);
            string BB = br.GetStringUtf8();
            Assert.AreEqual(B, BB);

            bw.Reset();
            Assert.AreEqual(bw.buf.Count, 0);

            bw.AddBoolean(true);
            bw.AddBoolean(false);
            bw.AddBooleanArray(C);
            bw.AddByte(0xDE);
            bw.AddByte(0xAD);
            bw.AddByteArray(D);
            bw.AddDoubleBytes(3.1415926);
            bw.AddDoubleBytesArray(E);
            bw.AddFloat(6.6666f);
            bw.AddFloatArray(F);
            // 0xEDCB5433 = -0x1234ABCD  same bits
            bw.AddInteger(-0x1234ABCD);
            bw.AddInteger(1);
            bw.AddInteger(0);
            bw.AddInteger(-1);
            bw.AddIntegerArray(G);
            bw.AddLongBytes(0xDEADBEEF);
            bw.AddLongBytes(0xCAFEBABEDEADBEEF);
            bw.AddLongBytesArray(H);
            bw.AddPositiveInteger1(42);
            bw.AddPositiveInteger1(0);
            bw.AddPositiveInteger1(42424242);
            bw.AddPositiveInteger1Array(G);
            bw.AddPositiveInteger2(42);
            bw.AddPositiveInteger2(1025);
            bw.AddPositiveInteger2(42424242);
            bw.AddPositiveInteger2Array(G);
            bw.AddPositiveInteger3(42);
            bw.AddPositiveInteger3(8388609);
            bw.AddPositiveInteger3(42424242);
            bw.AddPositiveInteger3Array(G);
            bw.AddStringUtf8Array(I);
            bw.AddUinteger(1);

            s = BitConverter.ToString(bw.buf.ToArray());
            //                   T  F 6 100111     6                   [       3.1415926      ] 5 [       3.333          ,     1.111             ,       2.222           ,     5.555             ,           -7.777      ] [ 6.6666f ] 5 [ 3.333f   , 1.111f    , 2.222f    , 5.555f    , -7.777f   ]-0x1234ABCD|       1   |      0    |     -1    |             9,          8,          7,          6,          5,          4,          3,          2,          1,          0,        200,       3800,    8389000,  0xDEADBEEF (64bit)   ,   0xCAFEBABEDEADBEEF  ,                      100 ,                   101 ,                 102   ,                 103   ,42, 0,  42424242 ,    9, 8, 7, 6, 5, 4, 3, 2, 1, 0,        200,       3800,    8389000, etc...
            Assert.AreEqual(s, "01-00-06-9C-DE-AD-06-01-02-03-04-05-06-40-09-21-FB-4D-12-D8-4A-05-40-0A-A9-FB-E7-6C-8B-44-3F-F1-C6-A7-EF-9D-B2-2D-40-01-C6-A7-EF-9D-B2-2D-40-16-38-51-EB-85-1E-B8-C0-1F-1B-A5-E3-53-F7-CF-40-D5-54-CA-05-40-55-4F-DF-3F-8E-35-3F-40-0E-35-3F-40-B1-C2-8F-C0-F8-DD-2F-ED-CB-54-33-00-00-00-01-00-00-00-00-FF-FF-FF-FF-0D-00-00-00-09-00-00-00-08-00-00-00-07-00-00-00-06-00-00-00-05-00-00-00-04-00-00-00-03-00-00-00-02-00-00-00-01-00-00-00-00-00-00-00-C8-00-00-0E-D8-00-80-01-88-00-00-00-00-DE-AD-BE-EF-CA-FE-BA-BE-DE-AD-BE-EF-04-00-00-00-00-00-00-00-64-00-00-00-00-00-00-00-65-00-00-00-00-00-00-00-66-00-00-00-00-00-00-00-67-2A-00-82-87-57-B2-0D-09-08-07-06-05-04-03-02-01-00-80-00-00-C8-80-00-0E-D8-80-80-01-88-00-2A-04-01-82-87-57-B2-0D-00-09-00-08-00-07-00-06-00-05-00-04-00-03-00-02-00-01-00-00-C8-0E-D8-80-80-01-88-00-00-2A-80-80-00-01-82-87-57-B2-0D-00-00-09-00-00-08-00-00-07-00-00-06-00-00-05-00-00-04-00-00-03-00-00-02-00-00-01-00-00-00-C8-00-0E-D8-80-80-01-88-04-03-4F-6E-65-03-54-77-6F-05-54-68-72-65-65-04-46-6F-75-72-01");
            //                            0xDEAD      1, 2, 3, 4, 5, 6                                                                                                                                                                                                                                                                               13                                                                                                                                                                                                              4                                                                                               flag bit 0x80       11                    flag bit 0x80

            br = new LowEntryByteReader(bw.buf.ToArray());
            Assert.AreEqual(br.GetBoolean(), true);
            Assert.AreEqual(br.GetBoolean(), false);
            Assert.AreEqual(br.GetBooleanArray(), C);
            Assert.AreEqual(br.GetByte(), 0xDE);
            Assert.AreEqual(br.GetByte(), 0xAD);
            Assert.AreEqual(br.GetByteArray(), D);
            Assert.AreEqual(br.GetDoubleBytes(), 3.1415926);
            Assert.AreEqual(br.GetDoubleBytesArray(), E);
            Assert.AreEqual(br.GetFloat(), 6.6666f);
            Assert.AreEqual(br.GetFloatArray(), F);
            Assert.AreEqual(br.GetInteger(), -0x1234ABCD);
            Assert.AreEqual(br.GetInteger(), 1);
            Assert.AreEqual(br.GetInteger(), 0);
            Assert.AreEqual(br.GetInteger(), -1);
            Assert.AreEqual(br.GetIntegerArray(), G);
            Assert.AreEqual(br.GetLongBytes(), 0xDEADBEEF);
            Assert.AreEqual(br.GetLongBytes(), 0xCAFEBABEDEADBEEF);
            Assert.AreEqual(br.GetLongBytesArray(), H);
            Assert.AreEqual(br.GetPositiveInteger1(), 42);
            Assert.AreEqual(br.GetPositiveInteger1(), 0);
            Assert.AreEqual(br.GetPositiveInteger1(), 42424242);
            Assert.AreEqual(br.GetPositiveInteger1Array(), G);
            Assert.AreEqual(br.GetPositiveInteger2(), 42);
            Assert.AreEqual(br.GetPositiveInteger2(),1025);
            Assert.AreEqual(br.GetPositiveInteger2(),42424242);
            Assert.AreEqual(br.GetPositiveInteger2Array(),G);
            Assert.AreEqual(br.GetPositiveInteger3(),42);
            Assert.AreEqual(br.GetPositiveInteger3(),8388609);
            Assert.AreEqual(br.GetPositiveInteger3(),42424242);
            Assert.AreEqual(br.GetPositiveInteger3Array(),G);
            Assert.AreEqual(br.GetStringUtf8Array(),I);
            Assert.AreEqual(br.GetUinteger(), 1);
        }
    }
}
