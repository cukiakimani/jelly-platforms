using UnityEngine;

namespace SweetLibs
{
    public static class DebugHelpers
    {
        public static void BreakIf(bool breakOnTrue)
        {
            if (breakOnTrue)
                Debug.Break();
        }
        
        public static void DrawCircle(Vector3 position, Color color, float radius = 1f)
        {
            var inc = 36f;
            var angle = 360f / inc;
            var a = position + Vector3.up * radius;

            for (int i = 0; i < inc; i++)
            {
                var b = a - position;
                
                var x = b.x * Mathf.Cos(angle * Mathf.Deg2Rad) - b.y * Mathf.Sin(angle * Mathf.Deg2Rad);
                var y = b.x * Mathf.Sin(angle * Mathf.Deg2Rad) + b.y * Mathf.Cos(angle * Mathf.Deg2Rad);

                b.x = x;
                b.y = y;
                
                b += position;
                Debug.DrawLine(a, b, color);
                a = b;
            }
        }

        public static void DebugPoint(Vector3 position, Color color, float size = 1f, float duration = 0f)
        {
            Vector3 top = position + Vector3.up * size * 0.5f;
            Vector3 bottom = position + Vector3.down * size * 0.5f;
            Vector3 right = position + Vector3.right * size * 0.5f;
            Vector3 left = position + Vector3.left * size * 0.5f;

            Debug.DrawLine(top, bottom, color, duration);
            Debug.DrawLine(right, left, color, duration);
        }
    }
}