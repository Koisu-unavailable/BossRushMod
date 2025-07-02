using System;
using Microsoft.Xna.Framework;

namespace BossRush.Utils
{
    public static class VectorUtils
    {
        public static double GetDistanceVector2(this Vector2 vector1, Vector2 vector2)
        {
            // distance formula
            return Math.Sqrt(
                Math.Pow((vector1.X - vector2.X), 2) + Math.Pow((vector1.Y - vector2.Y), 2)
            );
        }
    }
}