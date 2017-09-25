using Game;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace Match3Like
{
    public class SelectLevelController: MonoBehaviour
    {
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private LevelBlueprint[] _levelBlueprints;
        
        private void Awake()
        {
            _levelBlueprints
                .SlinqWithIndex()
                .ForEach(t =>
                {
                    Instantiate(_buttonPrefab, transform)
                        .TryGetComponent<SelectLevelButtonController>()
                        .ForEach(c => c.Set(t.Item1, t.Item2));
                });
        }
    }
}