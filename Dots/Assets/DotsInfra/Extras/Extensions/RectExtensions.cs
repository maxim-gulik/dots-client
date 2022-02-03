using UnityEngine;

namespace Dots.Extras
{
    public static class RectExtensions
    {
        public static void StretchToParentSize(this RectTransform target)
        {
            var parent = target.transform.parent.GetComponent<RectTransform>();
            if (parent == null)
            {
                return;
            }

            target.anchoredPosition = parent.position;
            target.anchorMin = Vector2.zero;
            target.anchorMax = Vector2.one;
            target.pivot = Vector2.one * 0.5f;
            target.sizeDelta = Vector2.zero;
            target.localScale = Vector3.one;
            target.ForceUpdateRectTransforms();
        }
    }
}