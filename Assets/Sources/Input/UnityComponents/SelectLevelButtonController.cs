using Game;
using Smooth.Slinq;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Like
{
    public class SelectLevelButtonController : MonoBehaviour
    {
        public LevelBlueprint _levelBlueprint;

        public void Set(LevelBlueprint levelBlueprint, int order)
        {
            _levelBlueprint = levelBlueprint;
            transform.position = transform.position + Vector3.down * 50 * order;
            GetComponentInChildren<Text>().text = $"Level {order + 1}";
        }

        public void OnButtonPressed()
        {
            Contexts.sharedInstance.game.ReplaceBoard(_levelBlueprint);
        }

    }
}