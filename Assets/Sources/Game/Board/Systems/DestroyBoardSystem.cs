using Entitas;
using Smooth.Algebraics;

namespace Game.Board.Systems
{
    public class DestroyBoardSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly GameContext _context;

        public DestroyBoardSystem(Contexts contexts)
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(GameMatcher.Item);
        }
        
        public void Cleanup()
        {
            if (!_context.isGameOver && _group.count != 0) return;
            
            _context.boardEntity.ToOption().ForEach(e => e.Destroy());
        }
    }
}