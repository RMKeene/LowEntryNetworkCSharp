using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
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
    public class LowEntryByteReader
    {
        public byte[] Bytes;
        public int Position = 0;

        public LowEntryByteReader()
        {
        }

        public LowEntryByteReader(byte[] Bytes, Int32 Index = 0, Int32 Length = Int32.MaxValue)
        {
            this.Bytes = LowEntryExtendedStandardLibrary.BytesSubArray(Bytes, Index, Length);
        }

        public static byte[] ReversedSubArray(byte[] b, int start, int length)
        {
            byte[] ret = b.AsSpan(start, length).ToArray();
            return ret.Reverse().ToArray();
        }

        public bool IsAtEnd => Position >= Bytes.Length;

        /// <summary>
        /// Get the current position, then increas Position by Increasment.
        /// Return the position from before Incresement was added.
        /// </summary>
        /// <param name="Increasement"></param>
        /// <returns></returns>
        public Int32 GetAndIncreasePosition(Int32 Increasement)
        {
            Int32 Pos = Position;
            if ((Bytes.Length - Increasement) <= Position)
            {
                Position = Bytes.Length;
            }
            else
            {
                Position += Increasement;
            }
            return Pos;
        }

        public Int32 MaxElementsRemaining(Int32 MinimumSizePerElement)
        {
            Int32 RemainingCount = Remaining();
            if (RemainingCount <= 0)
            {
                return 0;
            }
            if (MinimumSizePerElement <= 1)
            {
                return RemainingCount;
            }
            return (RemainingCount / MinimumSizePerElement) + 1;
        }

        public Int32 GetPosition()
        {
            return Position;
        }

        public void SetPosition(Int32 Position)
        {
            this.Position = Position;
        }

        public void Reset()
        {
            Position = 0;
        }

        public void Empty()
        {
            Position = Bytes.Length;
        }

        public Int32 Remaining()
        {
            return Bytes.Length - Position;
        }


        public byte GetByte()
        {
            Int32 Pos = GetAndIncreasePosition(1);
            if (Bytes.Length <= Pos)
            {
                return 0;
            }
            return Bytes[Pos];
        }

        public Int32 GetInteger()
        {
            int sz = 4;
            Int32 Pos = GetAndIncreasePosition(sz);
            if (Bytes.Length <= Pos + sz)
            {
                return 0;
            }
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToInt32(ReversedSubArray(Bytes, Pos, sz));
            return BitConverter.ToInt32(Bytes, Pos);
        }

        public Int32 GetUinteger()
        {
            Int32 Pos = GetAndIncreasePosition(1);
            if (Bytes.Length <= Pos)
            {
                return 0;
            }
            byte B = Bytes[Pos];
            if ((B & 0x080) == 0)
            {
                return B;
            }

            Pos = GetAndIncreasePosition(3);
            if (Bytes.Length <= (Pos + 2))
            {
                return 0;
            }
            Int32 Value = ((B & ~(1 << 7)) << 24) | (Bytes[Pos + 0] << 16) | (Bytes[Pos + 1] << 8) | Bytes[Pos + 2];
            if (Value < 128)
            {
                return 0;
            }
            return Value;
        }

        public Int32 GetPositiveInteger1()
        {
            Int32 Pos = GetAndIncreasePosition(1);
            if (Bytes.Length <= Pos)
            {
                return 0;
            }
            byte B = Bytes[Pos];
            if (((B >> 7) & 1) == 0)
            {
                return B;
            }

            Pos = GetAndIncreasePosition(3);
            if (Bytes.Length <= (Pos + 2))
            {
                return 0;
            }
            Int32 Value = ((B & ~(1 << 7)) << 24) | (Bytes[Pos + 0] << 16) | (Bytes[Pos + 1] << 8) | Bytes[Pos + 2];
            if (Value < 128)
            {
                return 0;
            }
            return Value;
        }

        public Int32 GetPositiveInteger2()
        {
            Int32 Pos = GetAndIncreasePosition(2);
            if (Bytes.Length <= (Pos + 1))
            {
                return 0;
            }
            byte B1 = Bytes[Pos + 0];
            byte B2 = Bytes[Pos + 1];
            if (((B1 >> 7) & 1) == 0)
            {
                return (B1 << 8) | B2;
            }

            Pos = GetAndIncreasePosition(2);
            if (Bytes.Length <= (Pos + 1))
            {
                return 0;
            }
            Int32 Value = ((B1 & ~(1 << 7)) << 24) | (B2 << 16) | (Bytes[Pos + 0] << 8) | Bytes[Pos + 1];
            if (Value < 32768)
            {
                return 0;
            }
            return Value;
        }

        public Int32 GetPositiveInteger3()
        {
            Int32 Pos = GetAndIncreasePosition(3);
            if (Bytes.Length <= (Pos + 2))
            {
                return 0;
            }
            byte B1 = Bytes[Pos + 0];
            byte B2 = Bytes[Pos + 1];
            byte B3 = Bytes[Pos + 2];
            if (((B1 >> 7) & 1) == 0)
            {
                return (B1 << 16) | (B2 << 8) | B3;
            }

            Pos = GetAndIncreasePosition(1);
            if (Bytes.Length <= Pos)
            {
                return 0;
            }
            Int32 Value = ((B1 & ~(1 << 7)) << 24) | (B2 << 16) | (B3 << 8) | Bytes[Pos];
            if (Value < 8388608)
            {
                return 0;
            }
            return Value;
        }

        public UInt64 GetLongBytes()
        {
            int sz = 8;
            Int32 Pos = GetAndIncreasePosition(sz);
            if (Bytes.Length <= Pos + sz)
            {
                return 0;
            }
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToUInt64(ReversedSubArray(Bytes, Pos, sz), 0);
            return BitConverter.ToUInt64(Bytes, Pos);
        }

        public float GetFloat()
        {
            int sz = 4;
            Int32 Pos = GetAndIncreasePosition(sz);
            if (Bytes.Length <= Pos + sz)
            {
                return 0;
            }
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToSingle(ReversedSubArray(Bytes, Pos, sz));
            return BitConverter.ToSingle(Bytes, Pos);
        }

        public double GetDoubleBytes()
        {
            int sz = 8;
            Int32 Pos = GetAndIncreasePosition(sz);
            if (Bytes.Length <= Pos + sz)
            {
                return 0.0;
            }
            if (BitConverter.IsLittleEndian)
                return BitConverter.ToDouble(ReversedSubArray(Bytes, Pos, sz), 0);
            return BitConverter.ToDouble(Bytes, Pos);
        }

        public bool GetBoolean()
        {
            Int32 Pos = GetAndIncreasePosition(1);
            if (Bytes.Length <= Pos)
            {
                return false;
            }

            return Bytes[Pos] == 0 ? false : true;
        }

        public string GetStringUtf8()
        {
            Int32 Length = GetUinteger();
            if (Length <= 0)
            {
                return "";
            }
            Int32 Pos = GetAndIncreasePosition(Length);
            if (Bytes.Length <= Pos)
            {
                return "";
            }

            return UTF8Encoding.UTF8.GetString(Bytes, Pos, Length);
        }

        public byte[] GetByteArray()
        {
            Int32 Length = GetUinteger();
            if (Length <= 0)
            {
                return new byte[0];
            }
            Int32 Pos = GetAndIncreasePosition(Length);
            if (Bytes.Length <= Pos)
            {
                return new byte[0];
            }
            return LowEntryExtendedStandardLibrary.BytesSubArray(Bytes, Pos, Length);
        }

        public Int32[] GetIntegerArray()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(4));
            if (Length <= 0)
            {
                return new Int32[0];
            }
            Int32[] Array = new Int32[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetInteger();
            }
            return Array;
        }

        public Int32[] GetPositiveInteger1Array()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(1));
            if (Length <= 0)
            {
                return new Int32[0];
            }
            Int32[] Array = new Int32[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetPositiveInteger1();
            }
            return Array;
        }

        public Int32[] GetPositiveInteger2Array()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(2));
            if (Length <= 0)
            {
                return new Int32[0];
            }
            Int32[] Array = new Int32[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetPositiveInteger2();
            }
            return Array;
        }

        public Int32[] GetPositiveInteger3Array()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(3));
            if (Length <= 0)
            {
                return new Int32[0];
            }
            Int32[] Array = new Int32[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetPositiveInteger3();
            }
            return Array;
        }

        public UInt64[] GetLongBytesArray()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(8));
            if (Length <= 0)
            {
                return new UInt64[0];
            }
            UInt64[] Array = new UInt64[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetLongBytes();
            }
            return Array;
        }

        public float[] GetFloatArray()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(4));
            if (Length <= 0)
            {
                return new float[0];
            };
            float[] Array = new float[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetFloat();
            }
            return Array;
        }

        public double[] GetDoubleBytesArray()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(8));
            if (Length <= 0)
            {
                return new double[0];
            }
            double[] Array = new double[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetDoubleBytes();
            }
            return Array;
        }

        public bool[] GetBooleanArray()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, SafeMultiply(MaxElementsRemaining(1), 8));
            if (Length <= 0)
            {
                return new bool[0];
            }
            bool[] Array = new bool[Length];
            for (Int32 i = 0; i < Length; i += 8)
            {
                byte B = GetByte();
                for (Int32 BIndex = 0; BIndex < 8; BIndex++)
                {
                    Int32 Index = i + BIndex;
                    if (Index >= Length)
                    {
                        return Array;
                    }
                    Array[Index] = (((B >> (7 - BIndex)) & 1) != 0);
                }
            }
            return Array;
        }

        public string[] GetStringUtf8Array()
        {
            Int32 Length = GetUinteger();
            Length = Math.Min(Length, MaxElementsRemaining(1));
            if (Length <= 0)
            {
                return new string[0];
            }
            string[] Array = new string[Length];
            for (Int32 i = 0; i < Length; i++)
            {
                Array[i] = GetStringUtf8();
            }
            return Array;
        }


        public Int32 SafeMultiply(Int32 A, Int32 B)
        {
            Int64 Result = (Int64)A * (Int64)B;
            Int64 Max = Int32.MaxValue;
            Int64 Min = Int32.MinValue;
            if (Result >= Max)
            {
                return Int32.MaxValue;
            }
            if (Result <= Min)
            {
                return Int32.MinValue;
            }
            return (Int32)Result;
        }

    }
}
