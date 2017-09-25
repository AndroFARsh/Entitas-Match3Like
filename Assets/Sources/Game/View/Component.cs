using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Game
{  
    [Game]
    public class AnimatingComponent : IComponent
    {
    }
    
    [Game]
    public class ItemComponent : IComponent
    {
    }
    
    [Game]
    public class ReomveComponent : IComponent
    {
    }
    
    [Game]
    public class ViewComponent : IComponent
    {
        public GameObject value;
    }
    
    [Game]
    public class PositionComponent : IComponent
    {
        [EntityIndex]
        public Vector2 value;
    }

    [Game]
    public class SpriteComponent : IComponent
    {
        public Sprite value;
    }
    
    [Game]
    public class InteractiveComponent : IComponent {}
    
    [Game]
    public class MovableComponent : IComponent {}
    
    [Game]
    public class SelectedComponent : IComponent {}
}