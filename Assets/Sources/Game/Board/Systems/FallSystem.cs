using System.Collections.Generic;
using Entitas;
using Smooth.Slinq;

namespace Game.Board.Systems
{
    public class FallItemSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public FallItemSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Item, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return !_context.isGameOver && _context.hasBoard && !_context.boardEntity.isIntialize;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var size = _context.board.value.Size;
            for (var col=0; col < size.Columns; ++col) {
                for (var row = 0; row < size.Rows; ++row)
                {
                    // search empty element;
                    var pos = new IntVector2(col, row);
                    if (_context.GetEntitiesWithPosition(pos).Count != 0) continue;
                    // search next not empty element;
                    
                    if (!MoveNotEmptyElementDown(pos, size.Rows)) break;
                }
            }
        }

        private bool MoveNotEmptyElementDown(IntVector2 emptyPos, int max)
        {
            var pos = emptyPos;
            while (pos.Y < max)
            {
                pos = pos + IntVector2.Up;
                var items = _context.GetEntitiesWithPosition(pos);
                if (items.Count != 0)
                {
                    items.Slinq().ToList().Slinq()
                        .ForEach((e, p) => e.ReplacePosition(p), emptyPos);
                    return true;
                }
            }
            return false;
        }
    }
}