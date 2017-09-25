using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Game
{   
     
    [Game, Unique]
    public class BoardComponent : IComponent
    {
        public LevelBlueprint value;
    }
    
    [Game, Unique]
    public class IntializeComponent : IComponent
    {
    }
    
    [Game, Unique]
    public class GameOverComponent : IComponent
    {
    }
}