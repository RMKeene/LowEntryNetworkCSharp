using System;
using System.Collections.Generic;
using System.Text;

namespace LowEntryNetworkCSharp
{
    public class LowEntryExtendedStandardLibrary
    {
        /// <summary>
        /// Returns a copy of the ByteArray from Index for Length bytes. If Index is negative then returns Length + Index (remeber, Index is negative)
        /// bytes starting from 0. If Index + Length is off the ned then returns what it can up to the end of the BytesArray.
        /// In any case if Length ends up being 0 then returns an empty array.
        /// If the resulting return array is all of ByteArray then simply return ByteArray, not a copy.
        /// </summary>
        /// <param name="ByteArray"></param>
        /// <param name="Index"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static byte[] BytesSubArray(byte[] ByteArray, Int32 Index, Int32 Length)
        {
            if (ByteArray.Length <= 0)
            {
                return new byte[0];
            }

            if (Index < 0)
            {
                Length += Index;
                Index = 0;
            }
            if (Length > ByteArray.Length - Index)
            {
                Length = ByteArray.Length - Index;
            }
            if (Length <= 0)
            {
                return new byte[0];
            }

            if ((Index == 0) && (Length == ByteArray.Length))
            {
                return ByteArray;
            }
            byte[] ReturnArray = new byte[Length];
            Array.Copy(ByteArray, Index, ReturnArray, 0, Length);
            return ReturnArray;
        }
    }
}
