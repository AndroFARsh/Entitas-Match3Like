using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;

namespace Game.Board.Systems
{
    public class RegenBoardItemSystem : IExecuteSystem
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _group;

        public RegenBoardItemSystem(Contexts contexts)
        {
            _context = contexts.game;
            _group = _context.GetGroup(GameMatcher.Remove);
        }

        public void Execute()
        {
            if (!_context.hasBoard || _context.isIntialized)
                return;

            _group.count
                .Match()
                .Where(count => count != 0)
                .Do(_ => _group.GetEntities()
                    .Slinq()
                    .Select(GetPositionAndDestroy)
                    .ForEach(pos => _context.CreateItem(pos, _context.board.value.Items)))
                .Else(_ => _context.isIntialized = true)
                .Exec();
        }

        private static IntVector2 GetPositionAndDestroy(GameEntity entity)
        {
            var pos = entity.position.value;
            entity.Destroy();
            return pos;
        }
    }
}