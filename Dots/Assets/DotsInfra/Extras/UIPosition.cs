using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Dots.Extras
{
    [Serializable]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Justification = "This class needs to extend System.Object, not object, so that it renders in the inspector.")]
    public class UIPosition : object
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private Vector2 sizeDelta;

        [SerializeField]
        private Vector2 anchoredPosition;

        [SerializeField]
        private Vector2 anchorMin;

        [SerializeField]
        private Vector2 anchorMax;

        public string Name => name;

        public Vector2 SizeDelta => sizeDelta;

        public Vector2 AnchoredPosition => anchoredPosition;

        public Vector2 AnchorMin => anchorMin;

        public Vector2 AnchorMax => anchorMax;
    }
}