using System.Collections.Generic;
using Entitas;
using static Entitas.GroupEvent;

namespace Game.State.Systems
{
    public class InitScoreSystem: ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public InitScoreSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Board, Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _context.ReplaceScore(0);
        }
    }
}