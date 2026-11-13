using UnityEngine;

namespace Lumines
{
    /// <summary>
    /// The standard Lumines piece is a 2x2 cube fill with 2 types of block.
    ///     AB
    ///     BA
    /// The 2x2 piece above is present as ABBA
    /// We can use the binary to store these.
    /// </summary>
    public enum EPieceType : byte
    {
        AAAA = 0b0000,
        AAAB = 0b0001,
        AABA = 0b0010,
        AABB = 0b0011,
        ABAA = 0b0100,
        ABAB = 0b0101,
        ABBA = 0b0110,
        ABBB = 0b0111,
        BAAA = 0b1000,
        BAAB = 0b1001,
        BABA = 0b1010,
        BABB = 0b1011,
        BBAA = 0b1100,
        BBAB = 0b1101,
        BBBA = 0b1110,
        BBBB = 0b1111,
    }
}
