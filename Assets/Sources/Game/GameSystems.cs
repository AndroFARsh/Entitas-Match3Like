using Game.Board;
using Game.Board.Systems;
using Game.State;
using Game.View;

namespace Game
{
    public class GameSystems: Feature
    {
        public GameSystems(Contexts contexts)
        {
            Add(new GameBoardSystems(contexts));
            Add(new GameViewSystems(contexts));
            Add(new GameStatesSystems(contexts));
        }
    }
}