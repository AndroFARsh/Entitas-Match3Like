using System.Collections.Generic;
using Entitas;
using Smooth.Slinq;

namespace Game.Board.Systems
{
    public class CheckBoardInitializationSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _group;
        private readonly GameContext _context;

        public CheckBoardInitializationSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Sprite, GameMatcher.Position));
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Sprite, GameMatcher.Position));
            
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.boardEntity.isIntialize;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var result = _group.IsGameOver(_context);
            if (result)
                _group.GetEntities()
                    .SlinqWithIndex()
                    .Where(t => t.Item2 % 2 == 0)
                    .Select(t => t.Item1)
                    .ForEach(e => e.isRemove = true);
        }
    }
}