using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LowEntryNetworkCSharp
{
    /// <summary>
    /// This has the low levelWrite for LowEntry TCP network byte array manipulation.
    /// This lets a standalone C# server talk to and from UE4 games via TCP/IP sockets.
    /// 
    /// In general this utility is written in the same style of the LowEntry C++ code.
    /// 
    /// This class acts as an indexed writer on a byte buffer.
    /// 
    /// LowEntry is Big-endian encoding, but has some bit flags for Int32 that are low value.
    /// Much like ProtoBuf from Google.
    /// </summary>
    public class LowEntryByteWriter
    {
        public List<byte> buf;

        public LowEntryByteWriter()
        {
            buf = new List<byte>();
        }

        /// <summary>
        /// Length of internal buffer.
        /// </summary>
        public int Length => buf.Count;

        public void Reset()
        {
            buf.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRawByte(byte b)
        {
            buf.Add(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRawBytes(byte[] bb, int start = 0, int length = -1)
        {
            if (length < 0)
            {
                buf.AddRange(bb);
            }
            else
            {
                buf.AddRange(bb.AsSpan(start, length).ToArray());
            }
        }

        public void AddByte(byte b)
        {
            AddRawByte(b);
        }
        public void AddInteger(Int32 Value)
        {
            AddRawByte((byte)(Value >> 24));
            AddRawByte((byte)(Value >> 16));
            AddRawByte((byte)(Value >> 8));
            AddRawByte((byte)(Value));
        }

        public void AddUinteger(Int32 Value)
        {
            if (Value <= 0)
            {
                AddRawByte(0);
            }
            else if (Value < 128)
            {
                AddRawByte((byte)Value);
            }
            else
            {
                AddRawByte((byte)((Value >> 24) | (1 << 7)));
                AddRawByte((byte)(Value >> 16));
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
        }

        public void AddPositiveInteger1(Int32 Value)
        {
            if (Value <= 0)
            {
                AddRawByte(0);
            }
            else if (Value < 128)
            {
                AddRawByte((byte)Value);
            }
            else
            {
                AddRawByte((byte)((Value >> 24) | (1 << 7)));
                AddRawByte((byte)(Value >> 16));
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
        }

        public void AddPositiveInteger2(Int32 Value)
        {
            if (Value <= 0)
            {
                AddRawByte(0);
                AddRawByte(0);
            }
            else if (Value < 32768)
            {
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
            else
            {
                AddRawByte((byte)((Value >> 24) | (1 << 7)));
                AddRawByte((byte)(Value >> 16));
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
        }

        public void AddPositiveInteger3(Int32 Value)
        {
            if (Value <= 0)
            {
                AddRawByte(0);
                AddRawByte(0);
                AddRawByte(0);
            }
            else if (Value < 8388608)
            {
                AddRawByte((byte)(Value >> 16));
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
            else
            {
                AddRawByte((byte)((Value >> 24) | (1 << 7)));
                AddRawByte((byte)(Value >> 16));
                AddRawByte((byte)(Value >> 8));
                AddRawByte((byte)(Value));
            }
        }

        public void AddLongBytes(UInt64 Value)
        {
            AddRawByte((byte)(Value >> 56));
            AddRawByte((byte)(Value >> 48));
            AddRawByte((byte)(Value >> 40));
            AddRawByte((byte)(Value >> 32));
            AddRawByte((byte)(Value >> 24));
            AddRawByte((byte)(Value >> 16));
            AddRawByte((byte)(Value >> 8));
            AddRawByte((byte)(Value));
        }

        public void AddFloat(float Value)
        {
            if (BitConverter.IsLittleEndian)
            {
                AddRawBytes(BitConverter.GetBytes(Value).Reverse().ToArray());
                return;
            }
            AddRawBytes(BitConverter.GetBytes(Value));
        }

        public void AddDoubleBytes(double Value)
        {
            if (BitConverter.IsLittleEndian)
            {
                AddRawBytes(BitConverter.GetBytes(Value).Reverse().ToArray());
                return;
            }
            AddRawBytes(BitConverter.GetBytes(Value));
        }

        public void AddBoolean(bool Value)
        {
            if (Value)
            {
                AddRawByte(0x01);
            }
            else
            {
                AddRawByte(0x00);
            }
        }
        public void AddStringUtf8(string Value)
        {
            AddByteArray(UTF8Encoding.UTF8.GetBytes(Value));
        }


        public void AddByteArray(byte[] Value)
        {
            AddUinteger(Value.Length);
            AddRawBytes(Value);
        }
        public void AddIntegerArray(Int32[] Value)
        {
            AddUinteger(Value.Length);
            foreach (Int32 V in Value)
            {
                AddInteger(V);
            }
        }
        public void AddPositiveInteger1Array(Int32[] Value)
        {
            AddUinteger(Value.Length);
            foreach (Int32 V in Value)
            {
                System.Diagnostics.Debug.Assert(V >= 0);
                AddPositiveInteger1(V);
            }
        }
        public void AddPositiveInteger2Array(Int32[] Value)
        {
            AddUinteger(Value.Length);
            foreach (Int32 V in Value)
            {
                System.Diagnostics.Debug.Assert(V >= 0);
                AddPositiveInteger2(V);
            }
        }
        public void AddPositiveInteger3Array(Int32[] Value)
        {
            AddUinteger(Value.Length);
            foreach (Int32 V in Value)
            {
                System.Diagnostics.Debug.Assert(V >= 0);
                AddPositiveInteger3(V);
            }
        }
        public void AddLongBytesArray(UInt64[] Value)
        {
            AddUinteger(Value.Length);
            foreach (UInt64 V in Value)
            {
                AddLongBytes(V);
            }
        }
        public void AddFloatArray(float[] Value)
        {
            AddUinteger(Value.Length);
            foreach (float V in Value)
            {
                AddFloat(V);
            }
        }
        public void AddDoubleBytesArray(double[] Value)
        {
            AddUinteger(Value.Length);
            foreach (double V in Value)
            {
                AddDoubleBytes(V);
            }
        }
        public void AddBooleanArray(bool[] Value)
        {
            AddUinteger(Value.Length);
            byte B = 0;
            Int32 BIndex = 0;
            foreach (bool V in Value)
            {
                if (V)
                {
                    B |= (byte)(1 << (7 - BIndex));
                }
                BIndex++;
                if (BIndex == 8)
                {
                    AddRawByte(B);
                    B = 0;
                    BIndex = 0;
                }
            }
            if (BIndex > 0)
            {
                AddRawByte(B);
            }
        }
        public void AddStringUtf8Array(string[] Value)
        {
            AddUinteger(Value.Length);
            foreach (string V in Value)
            {
                AddStringUtf8(V);
            }
        }
    }
}
