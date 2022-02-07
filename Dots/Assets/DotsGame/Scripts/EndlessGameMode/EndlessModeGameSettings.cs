using System;
using DotsGame.Common;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace DotsGame.EndlessMode
{
    [CreateAssetMenu(fileName = "EndlessModeGameSettings", menuName = "Game/EndlessModeGameSettings")]
    public class EndlessModeGameSettings : ScriptableObjectInstaller
    {
        [SerializeField] private GameTheme[] _themes;
        [SerializeField] private GridSettings _gridSettings;
        [SerializeField] private byte _maxDotsColors;

        public GridSettings GridSettings => _gridSettings;
        public byte MaxDotsColors => _maxDotsColors;

        public GameTheme GetRandomTheme() => _themes[Random.Range(0, _themes.Length)];
    }

    [Serializable]
    public class GridSettings
    {
        [SerializeField] [Range(0, byte.MaxValue)] private byte _columns;
        [SerializeField] [Range(0, byte.MaxValue)] private byte _rows;
        [SerializeField] private float _spacing = 5f;
        [SerializeField] private Sprite _contentSizeTemplate;

        public byte Columns => _columns;
        public byte Rows => _rows;

        public float CalcCellSide() => _contentSizeTemplate.bounds.size.x + _spacing * 2;
    }
}