using Game.State.Systems;

namespace Game.State
{
    public class GameStatesSystems: Feature
    {
        public GameStatesSystems(Contexts contexts)
        {
            Add(new InitScoreSystem(contexts));
            Add(new UpdateScoreSystem(contexts));
        }
    }
}