using System;
using Smooth.Algebraics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    [CreateAssetMenu(fileName = "LevelBlueprint",menuName = "Match3Like/Level Blueprint")]
    public class LevelBlueprint : ScriptableObject
    {
        [SerializeField] private BoardSize _boardSize = new BoardSize();
        [SerializeField] private Items _items = new Items();
       
        public BoardSize Size => _boardSize;
        
        public Items Items => _items;
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            _boardSize
                .ToOption()
                .ForEachOr(v => v.OnValidate(), () => { throw new ArgumentNullException(); });
            _items
                .ToOption()
                .ForEachOr(v => v.OnValidate(), () => { throw new ArgumentNullException(); });
        }
        #endif
    }

    [Serializable]
    public class BoardSize
    {
        [SerializeField] private int _rows = 3;
        [SerializeField] private int _columns = 3;
        
        public int Columns => _columns;

        public int Rows => _rows;

        public int Items => Columns * Rows;
        
        #if UNITY_EDITOR
        internal void OnValidate()
        {
            if (_rows < 1)
                throw new ArgumentOutOfRangeException($"number of rows should be > 1: {_rows}");
            if (_columns < 1)
                throw new ArgumentOutOfRangeException($"number of columns should be > 1: {_columns}");
            if (_rows * _columns < 9)
                throw new ArgumentOutOfRangeException($"number of items should be > 9: {_rows * _columns}");
        }
        #endif
    }
    
    [Serializable]
    public class Items
    {
        [SerializeField] private GameObject _prefab;
        
        [SerializeField] private Sprite[] _interactive;
        [SerializeField] private Sprite[] _notInteractive;
        
        [Range(0,1)]
        [SerializeField] private float _distribution = 1f;
        
        public GameObject Prefab => _prefab;
        public Sprite[] Interactive => _interactive;
        public Sprite[] NotInteractive => _notInteractive;
        public float Distribution => _distribution;

        public int GetRandomInteractive() => Random.Range(0, Interactive.Length);
        public int GetRandomNotInteractive() => Random.Range(0, NotInteractive.Length);
        
        #if UNITY_EDITOR
        internal void OnValidate()
        {
            _prefab.ToOption().Or(() =>{ throw new ArgumentNullException("Prefab not set"); });
        }
        #endif
    }
}