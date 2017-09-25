using Game.Board.Systems;

namespace Game.Board
{
    public class GameBoardSystems : Feature
    {
        public GameBoardSystems(Contexts contexts)
        {
            Add(new InitCameraPositionSystem(contexts));
            Add(new DestroyBoardSystem(contexts));
            Add(new InitBoardSystem(contexts));
            Add(new MatchLineSystem(contexts));
            Add(new CheckGameOverSystem(contexts));
            Add(new FallItemSystem(contexts));
            Add(new FillItemSystem(contexts));
        }
    }
}