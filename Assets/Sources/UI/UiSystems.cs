using Ui.Systems;

namespace Ui
{
    public class UiSystems : Feature
    {
        public UiSystems(Contexts contexts)
        {
            Add(new ScreenSystem(contexts));
            
            Add(new NotifyScoreChangedSystem(contexts));
            Add(new NotifyScreenChangedSystem(contexts));
        }
    }
}