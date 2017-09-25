using System.Linq;
using Smooth.Algebraics;
using Smooth.Slinq;
using Tools;
using UnityEngine;

namespace Match3Like
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private Ui.Screen _screen;
        [SerializeField] private Transform[] _content;

#if  UNITY_EDITOR
        private void OnValidate()
        {
            if (_content == null) Debug.LogWarning("Content not set");
        }
#endif
        private void Start()
        {
            var contexts = Contexts.sharedInstance;

            contexts.ui.CreateEntity().ToOption()
                .ForEach(e => e.AddScreenListener(OnScreenChanged));
        }

        private void OnScreenChanged(Ui.Screen screen)
        {       
            _content.ToOption()
                .Select(v => v.Length != 0 ? v : GetChildren())
                .ValueOr(GetChildren)
                .Slinq()
                .ForEach((t, v) => t.gameObject.SetActive(v), _screen == screen);
        }

        private Transform[] GetChildren()
        {
            return Utils.Range(transform.childCount).Select(transform.GetChild).ToArray();
        }
    }
}