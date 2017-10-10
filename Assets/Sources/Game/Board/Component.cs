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
    
    [Game]
    public class IntializeComponent : IComponent
    {
    }
    
    [Game, Unique]
    public class GameOverComponent : IComponent
    {
    }
}