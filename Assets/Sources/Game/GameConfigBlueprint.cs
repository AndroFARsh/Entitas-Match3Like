using System;
using Smooth.Algebraics;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameConfigBlueprint",menuName = "Match3Like/Game Config Blueprint")]
    public class GameConfigBlueprint : ScriptableObject
    {
        [SerializeField] private AudioClips _audioClips = new AudioClips();
       
        public AudioClips AudioClips => _audioClips;
    }

    [Serializable]
    public class AudioClips
    {
        [SerializeField] private AudioClip _selectItem;
        [SerializeField] private AudioClip _removeItems;

        public void SelectItem(AudioSource source)
        {
            source.ToOption()
                .Where(s => _selectItem != null)
                .ForEach(s => s.PlayOneShot(_selectItem));
        }
        
        public void RemoveItems(AudioSource source)
        {
            source.ToOption()
                .Where(s => _removeItems != null)
                .ForEach(s => s.PlayOneShot(_removeItems));
        }
    }
}