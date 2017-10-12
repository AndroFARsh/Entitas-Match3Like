using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Game
{   
    [Game, Unique]
    public class ConfigComponent : IComponent
    {
        public GameConfigBlueprint value;
    }
    
    [Game, Unique]
    public class GameControllerComponent : IComponent {}
  
     
    [Game, Unique]
    public class BoardComponent : IComponent
    {
        public LevelBlueprint value;
    }
    
    [Game, Unique]
    public class IntializedComponent : IComponent
    {
    }
    
    [Game, Unique]
    public class BusyComponent : IComponent
    {
    }
    
    [Game, Unique]
    public class GameOverComponent : IComponent
    {
    }
}