using System.Collections.Generic;
using Entitas;
using Smooth.Slinq;

namespace Game.Board.Systems
{
    public class FallItemSystem : ReactiveSystem<GameEntity>, ICleanupSystem
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _removed;

        public FallItemSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
            _removed = _context.GetGroup(GameMatcher.Reomve);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Reomve, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                .ForEach(ItemMoveDown, _context);
        }
        
        public void Cleanup()
        {
            _removed
                .GetEntities()
                .Slinq()
                .ForEach(e => e.isReomve = false);
        }

        internal static void ItemMoveDown(GameEntity e1, GameContext context)
        {
            var pos = e1.position.value;
            do
            {
                pos.y += 1;
                var ls =context.GetEntitiesWithPosition(pos)
                    .Slinq()
                    .Where(e => !e.isReomve)
                    .ToList();
                    
                    
                    ls.Slinq().ForEach(e2 =>
                    {
                        // shwitch position
                        var tmpPos = e1.position.value;
                        e1.ReplacePosition(pos);
                        e2.ReplacePosition(tmpPos);
                        pos = tmpPos;
                    });
            } while (pos.y < context.board.value.Size.Rows);
        }
    }
}