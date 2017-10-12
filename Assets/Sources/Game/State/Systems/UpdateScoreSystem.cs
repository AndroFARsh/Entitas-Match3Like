using System.Collections.Generic;
using Entitas;
using static Entitas.GroupEvent;
using static GameMatcher;

namespace Game.State.Systems
{
    public class UpdateScoreSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public UpdateScoreSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Remove, Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.hasBoard
                   && _context.isIntialized
                   && !_context.isGameOver;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _context.ReplaceScore(_context.score.value + entities.Count);
        }
    }
}