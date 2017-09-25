using Game;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Like
{
    public class RestartButtonController : MonoBehaviour
    {
        public void OnButtonPressed()
        {
            Contexts
                .sharedInstance
                .game
                .gameOverEntity
                .ToOption()
                .ForEach(e => e.Destroy());
        }

    }
}