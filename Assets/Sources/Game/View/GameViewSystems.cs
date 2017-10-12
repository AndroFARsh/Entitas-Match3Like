using Game.View.Systems;

namespace Game.View
{
    public class GameViewSystems : Feature
    {
        public GameViewSystems(Contexts contexts)
        {
            Add(new InitCameraPositionSystem(contexts));
            
            Add(new RemoveViewSystem(contexts));
            Add(new AddViewSystem(contexts));
            Add(new SetPositionSystem(contexts));
            Add(new SetSpriteSystem(contexts));
            
            Add(new SelectViewSystem(contexts));
            Add(new SwitchItemSystem(contexts));
            Add(new AnimatePositionSystem(contexts));

            Add(new SelectItemAudioSystem(contexts));
            Add(new RemoveItemAudioSystem(contexts));
            
            Add(new BusySystem(contexts));
        }
    }
}