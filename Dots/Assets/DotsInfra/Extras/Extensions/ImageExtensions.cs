using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Dots.Extras
{
    public static class ImageExtensions
    {
        public static void SetAlpha(this Image img, float alpha)
        {
            img.color = img.color.SetAlpha(alpha);
        }

        public static Vector4 GetAdjustedBorders(this Image img, Vector4 border, Rect adjustedRect)
        {
            var rect = img.rectTransform.rect;

            for (var i = 0; i <= 1; ++i)
            {
                if (rect.size[i] != 0.0)
                {
                    var adjustedRatio = adjustedRect.size[i] / rect.size[i];
                    border[i] *= adjustedRatio;
                    border[i + 2] *= adjustedRatio;
                }

                var borderSize = border[i] + border[i + 2];
                if (adjustedRect.size[i] < borderSize && borderSize != 0.0f)
                {
                    var rectToBorderRatio = adjustedRect.size[i] / borderSize;
                    border[i] *= rectToBorderRatio;
                    border[i + 2] *= rectToBorderRatio;
                }
            }

            return border;
        }

        public static Rect GetAspectAdjustedPixelRect(this Image img)
        {
            if (!img.preserveAspect || !img.sprite)
            {
                return img.GetPixelAdjustedRect();
            }

            var pixelRect = img.GetPixelAdjustedRect();
            var imageAspect = pixelRect.width / pixelRect.height;
            var spriteAspect = img.sprite.textureRect.width / img.sprite.textureRect.height;

            if (imageAspect < spriteAspect)
            {
                var oldHeight = pixelRect.height;
                pixelRect.height = pixelRect.width / img.sprite.textureRect.width * img.sprite.textureRect.height;
                pixelRect.y += (oldHeight - pixelRect.height) * img.rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = pixelRect.width;
                pixelRect.width = pixelRect.height / img.sprite.textureRect.height * img.sprite.textureRect.width;
                pixelRect.x += (oldWidth - pixelRect.width) * img.rectTransform.pivot.x;
            }

            return pixelRect;
        }

        public static void SetSpriteOrHideIfNull(this Image img, Sprite sprite)
        {
            img.sprite = sprite;
            img.gameObject.SetActive(sprite);
        }

        public static Rect CalculateSpriteRect(this Image img, Rect pixelRect, Sprite sprite = null, bool cropSprite = false)
        {
            if (!sprite)
            {
                if (!img.sprite)
                {
                    return new Rect();
                }

                sprite = img.sprite;
            }

            var position = sprite.textureRect.position;
            var size = sprite.textureRect.size;

            if (cropSprite)
            {
                var sourceAspect = size.x / size.y;
                var pixelAspect = pixelRect.width / pixelRect.height;

                if (pixelAspect > sourceAspect)
                {
                    size.y = size.x / pixelAspect;
                    position.y += (sprite.textureRect.height - size.y) * .5f;
                }
                else
                {
                    size.x = size.y * pixelAspect;
                    position.x += (sprite.textureRect.width - size.x) * .5f;
                }
            }

            var spriteRect = new Rect();
            spriteRect.position = new Vector2(position.x / sprite.texture.width, position.y / sprite.texture.height);
            spriteRect.size = new Vector2(size.x / sprite.texture.width, size.y / sprite.texture.height);

            return spriteRect;
        }

        public static Vector4 GetAdjustedBorders(this Image img, Vector4 border, Vector2 rectSize, int quadrantRotation = 0, bool mirror = false)
        {
            var rect = img.rectTransform.rect;

            for (var i = 0; i <= 1; ++i)
            {
                if (rect.size[i] != 0.0)
                {
                    var adjustedRatio = rectSize[i] / rect.size[i];
                    border[i] *= adjustedRatio;
                    border[i + 2] *= adjustedRatio;
                }

                var borderSize = border[i] + border[i + 2];
                if (rectSize[i] < borderSize && borderSize != 0.0f)
                {
                    var rectToBorderRatio = rectSize[i] / borderSize;
                    border[i] *= rectToBorderRatio;
                    border[i + 2] *= rectToBorderRatio;
                }
            }

            return border;
        }

        public static Vector4 GetDrawingDimensions(this Image img, bool shouldPreserveAspect)
        {
            var sprite = img.overrideSprite
                             ? img.overrideSprite
                             : img.sprite;
            var pixelAdjustedRect = img.GetPixelAdjustedRect();
            if (!sprite)
            {
                return new Vector4(pixelAdjustedRect.xMin, pixelAdjustedRect.yMin, pixelAdjustedRect.xMax, pixelAdjustedRect.yMax);
            }

            var paddingDimensions = DataUtility.GetPadding(sprite);
            var spriteDimensions = new Vector2(sprite.rect.width, sprite.rect.height);
            var xAsInt = Mathf.RoundToInt(spriteDimensions.x);
            var yAsInt = Mathf.RoundToInt(spriteDimensions.y);

            var coordinates = new Vector4(
                paddingDimensions.x / xAsInt,
                paddingDimensions.y / yAsInt,
                (xAsInt - paddingDimensions.z) / xAsInt,
                (yAsInt - paddingDimensions.w) / yAsInt);

            if (shouldPreserveAspect && spriteDimensions.sqrMagnitude > 0.0)
            {
                var spriteAspect = spriteDimensions.x / spriteDimensions.y;
                var pixelRectAspect = pixelAdjustedRect.width / pixelAdjustedRect.height;

                if (spriteAspect > pixelRectAspect)
                {
                    var height = pixelAdjustedRect.height;
                    pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / spriteAspect);
                    pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * img.rectTransform.pivot.y;
                }
                else
                {
                    var width = pixelAdjustedRect.width;
                    pixelAdjustedRect.width = pixelAdjustedRect.height * spriteAspect;
                    pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * img.rectTransform.pivot.x;
                }
            }

            coordinates = new Vector4(
                pixelAdjustedRect.x + pixelAdjustedRect.width * coordinates.x,
                pixelAdjustedRect.y + pixelAdjustedRect.height * coordinates.y,
                pixelAdjustedRect.x + pixelAdjustedRect.width * coordinates.z,
                pixelAdjustedRect.y + pixelAdjustedRect.height * coordinates.w);

            return coordinates;
        }

        public static Vector4 GetPixelRectAdjustForAspectAndPadding(this Image img, Vector4 paddingAsPercentageV4Rect, float aspect)
        {
            var pixelAdjustedRect = img.GetPixelAdjustedRect();

            if (aspect > 0f)
            {
                if (aspect > pixelAdjustedRect.Aspect())
                {
                    var height = pixelAdjustedRect.height;
                    pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / aspect);
                    pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * img.rectTransform.pivot.y;
                }
                else
                {
                    var width = pixelAdjustedRect.width;
                    pixelAdjustedRect.width = pixelAdjustedRect.height * aspect;
                    pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * img.rectTransform.pivot.x;
                }
            }

            return paddingAsPercentageV4Rect.ScaleMinMax(pixelAdjustedRect.size).OffsetMinMax(pixelAdjustedRect.min);
        }
    }

    public static class RectTransformExtensions
    {
        public static void SetPosition(this RectTransform transform, UIPosition position)
        {
            transform.anchorMin = position.AnchorMin;
            transform.anchorMax = position.AnchorMax;
            transform.anchoredPosition = position.AnchoredPosition;
            transform.sizeDelta = position.SizeDelta;
        }

        public static void FitDimensions(
            this RectTransform rt,
            Vector2 dimensions,
            bool fitHeight,
            bool fitWidth,
            bool zeroPosition)
        {
            if (!fitHeight && !fitWidth && !zeroPosition)
            {
                return;
            }

            var newObjDimensions = new Vector2(
                fitWidth
                    ? dimensions.x
                    : rt.sizeDelta.x,
                fitHeight
                    ? dimensions.y
                    : rt.sizeDelta.y);

            rt.sizeDelta = newObjDimensions;

            if (zeroPosition)
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
            }
        }

        public static Vector2 GetDimensions(this RectTransform rt)
        {
            return new Vector2(rt.rect.width, rt.rect.height);
        }

        public static void SetHeight(this RectTransform rt, float height)
        {
            var width = rt.sizeDelta.x;

            rt.sizeDelta = new Vector2(width, height);
        }

        public static void SetWidth(this RectTransform rt, float width)
        {
            var height = rt.sizeDelta.y;

            rt.sizeDelta = new Vector2(width, height);
        }

        public static float Aspect(this Rect rect)
        {
            return rect.width / rect.height;
        }

        public static bool ContainsInDimension(this Rect container, Rect containee, int dimension, float tolerance = 0f)
        {
            return container.min[dimension] <= containee.min[dimension] + tolerance &&
                   container.max[dimension] >= containee.max[dimension] - tolerance;
        }

        public static void SetDimensionPosition(this Rect rect, int index, float val)
        {
            switch (index)
            {
                case 0:
                    rect.x = val;
                    break;
                case 1:
                    rect.y = val;
                    break;
                case 2:
                    rect.xMax = val;
                    break;
                case 3:
                    rect.yMax = val;
                    break;
            }
        }

        public static float GetDimensionPosition(this Rect rect, int index)
        {
            return index < 2
                       ? rect.min[index & 1]
                       : rect.max[index & 1];
        }

        public static Vector3 GetLocalPosition(this RectTransform transformA, RectTransform transformB)
        {
            if (transformA.parent == transformB.parent)
            {
                return transformB.localPosition - transformA.localPosition;
            }

            return transformA.worldToLocalMatrix.MultiplyPoint(transformB.localToWorldMatrix.MultiplyPoint(Vector3.zero));
        }

        public static void SetAnchoredPositionY(this RectTransform transform, float y)
        {
            var position = transform.anchoredPosition;
            position.y = y;
            transform.anchoredPosition = position;
        }

        public static void SetAnchoredPositionX(this RectTransform transform, float x)
        {
            var position = transform.anchoredPosition;
            position.x = x;
            transform.anchoredPosition = position;
        }
    }
}
