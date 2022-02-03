using UnityEngine;

namespace Dots.Extras
{
    public static class ColorExtensions
    {
        private const float RedGreyScaleWeight = 0.299f;
        private const float GreenGreyScaleWeight = 0.587f;
        private const float BlueGreyScaleWeight = 0.144f;
        private const float DefaultDarkScale = 0.6f;

        public static Color Desaturated(this Color colour, float percent = 1f)
        {
            var greyLevel = colour.r * RedGreyScaleWeight + colour.g * GreenGreyScaleWeight +
                            colour.b * BlueGreyScaleWeight;
            var greyColor = new Color(greyLevel, greyLevel, greyLevel, colour.a);

            return percent < 1f
                ? Color.Lerp(colour, greyColor, percent)
                : greyColor;
        }

        public static Color DarkOut(this Color colour, float scaleFactor = DefaultDarkScale)
        {
            var darkerColor = new Color(colour.r * scaleFactor, colour.g * scaleFactor, colour.b * scaleFactor,
                colour.a);

            return darkerColor;
        }

        public static string ToHex(this Color colour)
        {
            return string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(colour));
        }

        public static Color SetAlpha(this Color color, float newAlpha)
        {
            return new Color(color.r, color.g, color.b, newAlpha);
        }

        // This creates a color that should be the same as rendering the destColor before the color,
        // with the standard blend mode of (src * srcAlpha + dest * (1 - srcAlpha)).
        public static Color PreAlphaBlend(this Color color, Color destColor)
        {
            var a1 = color.a;
            var a2 = destColor.a;
            var aa = a1 * a2;
            var a3 = a1 + a2 - aa;
            var blendedColor = (color * a1 + destColor * a2 - destColor * aa) / a3;
            blendedColor.a = a3;

            return blendedColor;
        }
    }
}

    