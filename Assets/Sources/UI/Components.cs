using Entitas;
using Entitas.CodeGeneration.Attributes;
using Game;

namespace Ui
{
    public enum Screen
    {
        SelectLevel,
        Game,
        GameOver
    }

    [Ui, Unique]
    public class ScreenComponent : IComponent
    {
        public Screen value;
    }
    
    public delegate void OnScreenChangedDelegate(Screen next);
    
    [Ui]
    public class ScreenListenerComponent : IComponent
    {
        public OnScreenChangedDelegate value;
    }
    

    public delegate void OnScoreChangedDelegate(int score);
    
    [Ui]
    public class ScoreListenerComponent : IComponent
    {
        public OnScoreChangedDelegate value;
    }
    
}