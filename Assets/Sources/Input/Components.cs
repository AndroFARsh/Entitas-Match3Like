using Entitas;
using Entitas.CodeGeneration.Attributes;
using Game;
using UnityEngine;

namespace Input {
    
    [Input, Unique]
    public class LeftMouseButtonComponent : IComponent
    {
    }

    [Input, Unique]
    public class MiddleMouseButtonComponent : IComponent
    {
    }

    [Input, Unique]
    public class RightMouseButtonComponent : IComponent
    {
    }
    
    [Input]
    public class ButtonDownComponent : IComponent
    {
        public Vector2 value;
    }

    [Input]
    public class ButtonPressedComponent : IComponent
    {
        public Vector2 value;
    }
    
    [Input]
    public class ButtonUpComponent : IComponent 
    {
        public Vector2 value;
    }
    
    [Input]
    public class LevelButtonComponent : IComponent 
    {
        public LevelBlueprint value;
    }
}
