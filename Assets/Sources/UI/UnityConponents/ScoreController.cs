using Smooth.Algebraics;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Like
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private Text _label;
        
        private void Awake()
        {
            if (_label == null) _label = GetComponent<Text>();
        }

        private void Start()
        {
            var contexts = Contexts.sharedInstance;

            contexts.ui.CreateEntity().ToOption()
                .ForEach(e => e.AddScoreListener(OnScoreChanged));
        }

        private void OnScoreChanged(int score)
        {
            _label.text = $"Score: {score}";
        }
    }
}