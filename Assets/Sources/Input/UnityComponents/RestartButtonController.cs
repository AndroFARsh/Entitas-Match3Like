using Smooth.Algebraics;
using UnityEngine;

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