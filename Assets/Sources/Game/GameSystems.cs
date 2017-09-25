using Game.Board;
using Game.State;
using Game.View;

namespace Game
{
    public class GameSystems: Feature
    {
        public GameSystems(Contexts contexts)
        {
            Add(new GameBoardSystems(contexts));
            Add(new GameStatesSystems(contexts));
            Add(new GameViewSystems(contexts));
        }
    }
}