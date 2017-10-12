using Entitas;
using Smooth.Slinq;

namespace Game.Board.Systems
{
    public class CheckGameOverSystem : IExecuteSystem
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _group;

        public CheckGameOverSystem(Contexts contexts) 
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(GameMatcher.Item);
        }

        private bool IsNotScip()
        {
            return _context.hasBoard 
                   && _context.isIntialized
                   && !_context.isBusy
                   && _group.count == _context.board.value.Size.Items
                   && !_group.GetEntities().Slinq().Where(e => !e.hasView).Any()
                   && !_group.GetEntities().Slinq().Where(e => e.isRemove).Any();
        }

        public void Execute()
        {
            if (!IsNotScip()) return;
            
            _context.isGameOver = _group.IsGameOver(_context);
            if (_context.isGameOver)
                _group.GetEntities().Slinq().ForEach(e => e.isRemove = true);
        }
    }
}