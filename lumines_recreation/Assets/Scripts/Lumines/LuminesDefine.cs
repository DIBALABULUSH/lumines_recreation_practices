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
    public enum PieceType : byte
    {
        AAAA = 0,
        AAAB = 1,
        AABA = 2,
        AABB = 3,
        ABAB = 4,
        ABBA = 5,
        ABBB = 6,
        BAAA = 7,
        BAAB = 8,
        BABA = 9,
        BABB = 10,
        BBAA = 11,
        BBAB = 12,
        BBBA = 13,
        BBBB = 14,
    }
}
