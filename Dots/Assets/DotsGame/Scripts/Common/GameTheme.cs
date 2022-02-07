using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DotsGame.Common
{
    [CreateAssetMenu(fileName = "GameTheme", menuName = "Game/GameTheme")]
    public class GameTheme : ScriptableObject
    {
        [SerializeField] private Color _backgroundColor = Color.white;
        [SerializeField] private Color[] _dotsColors;

        //convenient to play with the game mechanic
        private int _maxDotsColorAmount = Int32.MaxValue;

        public Color BackgroundColor => _backgroundColor;

        public Color GetRandomDotColor()
        {
            var i = Random.Range(0, Mathf.Min(_dotsColors.Length, _maxDotsColorAmount));
            return _dotsColors[i];
        }

        public void SetMaxDotsColorAmount(int number)
        {
            if (_maxDotsColorAmount > _dotsColors.Length)
                Debug.LogWarning($"Theme doesn't supported {_maxDotsColorAmount} colors. Max amount: {_dotsColors.Length}");

            _maxDotsColorAmount = number;
        }
    }
}