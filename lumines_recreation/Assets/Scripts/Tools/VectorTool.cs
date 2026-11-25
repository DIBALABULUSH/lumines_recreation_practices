using UnityEngine;

namespace Tools
{
    public static class VectorTool
    {
        public static Vector2 ToVec2(this Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }
        
        public static Vector3 ToVec3(this Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }
        
        public static Vector3 ToVec3(this Vector2Int vec)
        {
            return new Vector3(vec.x, vec.y, 0);
        }
    }
}