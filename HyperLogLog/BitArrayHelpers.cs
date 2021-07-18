﻿using System;
using System.Collections;

namespace HyperLogLog
{
    public static class BitArrayHelpers
    {
        public static byte[] ToBytes(this BitArray arr)
        {
            if (arr.Count != 8)
            {
                throw new ArgumentException("Not enough bits to make a byte!");
            }
            var bytes = new byte[(arr.Length - 1) / 8 + 1];
            arr.CopyTo(bytes, 0);
            return bytes;
        }
    }
}
