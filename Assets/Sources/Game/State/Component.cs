using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Game
{
    [Game, Unique]
    public class ScoreComponent : IComponent
    {
        public int value;
    }
}