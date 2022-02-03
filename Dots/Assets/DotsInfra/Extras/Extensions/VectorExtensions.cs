using UnityEngine;

namespace Dots.Extras
{
    public static class VectorExtensions
    {
        public static Vector2 RotateByDirectionVector(this Vector2 vec2, Vector2 normalizedDirection)
        {
            return vec2.x * normalizedDirection.PerpendicularVector() + vec2.y * normalizedDirection;
        }

        public static Vector2 PerpendicularVector(this Vector2 vec2)
        {
            return new Vector2(vec2.y, -vec2.x);
        }

        public static Vector2 RadiansToVector2(float radians)
        {
            return new Vector2(Mathf.Sin(radians), Mathf.Cos(radians));
        }

        public static Vector2 DegreesToVector2(float degrees)
        {
            return RadiansToVector2(degrees * Mathf.Deg2Rad);
        }

        public static Vector2 RotatedByDegrees(this Vector2 vec2, float degrees)
        {
            return vec2.RotateByDirectionVector(DegreesToVector2(degrees));
        }

        public static Vector2 RotatedByRadians(this Vector2 vec2, float radians)
        {
            return vec2.RotateByDirectionVector(RadiansToVector2(radians));
        }

        public static float RadiansFromNormalizedVector(this Vector2 normalizedVector)
        {
            return Mathf.Atan2(normalizedVector.y, normalizedVector.x);
        }

        public static float DegreesFromNormalizedVector(this Vector2 normalizedVector)
        {
            return RadiansFromNormalizedVector(normalizedVector) * Mathf.Rad2Deg;
        }

        public static Vector2 AbsoluteValue(this Vector2 vec2)
        {
            return new Vector2(Mathf.Abs(vec2.x), Mathf.Abs(vec2.y));
        }

        public static Vector2 MinMaxMin(this Vector4 minMax)
        {
            return minMax;
        }

        public static Vector2 MinMaxMax(this Vector4 minMax)
        {
            return new Vector2(minMax.z, minMax.w);
        }

        public static Rect ToRectFromMinMax(this Vector4 vec4)
        {
            var rect = new Rect();
            rect.min = new Vector2(vec4.x, vec4.y);
            rect.max = new Vector2(vec4.z, vec4.w);
            return rect;
        }

        public static Vector4 OffsetMinMax(this Vector4 minMax, Vector2 offset)
        {
            return new Vector4(minMax.x + offset.x, minMax.y + offset.y, minMax.z + offset.x, minMax.w + offset.y);
        }

        public static Vector4 ScaleMinMax(this Vector4 minMax, Vector2 scale)
        {
            return new Vector4(minMax.x * scale.x, minMax.y * scale.y, minMax.z * scale.x, minMax.w * scale.y);
        }

        public static Vector4 ScaleMinMaxCentered(this Vector4 minMax, float scale)
        {
            var halfSize = minMax.MinMaxSize() * 0.5f;
            var scaledHalfSize = halfSize * scale;
            return new Vector4(
                minMax.x + halfSize.x - scaledHalfSize.x,
                minMax.y + halfSize.y - scaledHalfSize.y,
                minMax.z - halfSize.x + scaledHalfSize.x,
                minMax.w - halfSize.y + scaledHalfSize.y);
        }

        public static Vector4 DivideMinMax(this Vector4 minMax, Vector2 denominator)
        {
            return new Vector4(minMax.x / denominator.x, minMax.y / denominator.y, minMax.z / denominator.x, minMax.w / denominator.y);
        }

        public static Vector2 LerpMinMax(this Vector4 minMax, Vector2 t)
        {
            return new Vector2(
                Mathf.Lerp(minMax.x, minMax.z, t.x),
                Mathf.Lerp(minMax.y, minMax.w, t.y));
        }

        public static Vector2 InverseLerpMinMax(this Vector4 minMax, Vector2 t)
        {
            return new Vector2(
                Mathf.InverseLerp(minMax.x, minMax.z, t.x),
                Mathf.InverseLerp(minMax.y, minMax.w, t.y));
        }

        public static Vector2 MinMaxSize(this Vector4 minMax)
        {
            return new Vector2(minMax.z - minMax.x, minMax.w - minMax.y);
        }

        public static Vector4 ConstrainMinMaxTo(this Vector4 minMax, Vector4 constrainingMinMax)
        {
            return new Vector4(
                Mathf.Clamp(minMax.x, constrainingMinMax.x, constrainingMinMax.z),
                Mathf.Clamp(minMax.y, constrainingMinMax.y, constrainingMinMax.w),
                Mathf.Clamp(minMax.z, constrainingMinMax.x, constrainingMinMax.z),
                Mathf.Clamp(minMax.w, constrainingMinMax.y, constrainingMinMax.w));
        }
    }
}