using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using Random = UnityEngine.Random;

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
            if (!_context.hasBoard || !_context.boardEntity.isIntialize)
                return;

            if (_group.count == 0)
            {
                _context.boardEntity.isIntialize = false;
                return;
            }

            var pieces = _context.board.value.Items;
            _group.GetEntities()
                .Slinq()
                .Select(entity => {
                    var pos = entity.position.value;
                    entity.Destroy();
                    return pos;
                })
                .ForEach(pos =>
                {
                    _context.CreateItem(pos, pieces);
                });
        }
    }
}