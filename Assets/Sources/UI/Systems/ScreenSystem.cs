using Entitas;

namespace Ui.Systems
{
    public class ScreenSystem: IInitializeSystem
    {
        private readonly UiContext _context;

        public ScreenSystem(Contexts contexts)
        {
            _context = contexts.ui;

            contexts.game.GetGroup(GameMatcher.Board).OnEntityAdded +=
                (group, entity, index, component) => GoToScreen(Screen.Game);
            contexts.game.GetGroup(GameMatcher.GameOver).OnEntityAdded +=
                (group, entity, index, component) => GoToScreen(Screen.GameOver);
            contexts.game.GetGroup(GameMatcher.GameOver).OnEntityRemoved +=
                (group, entity, index, component) => GoToScreen(Screen.SelectLevel);
        }
        
        private void GoToScreen(Screen screen)
        {
            _context.ReplaceScreen(screen);
        }

        public void Initialize()
        {
            GoToScreen(Screen.SelectLevel);
        }
    }
}