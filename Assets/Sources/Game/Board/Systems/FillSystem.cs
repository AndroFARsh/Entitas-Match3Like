using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Board.Systems
{
    public class FillItemSystem : ReactiveSystem<GameEntity>, ICleanupSystem
    {
        private readonly GameContext _context;
        private readonly IList<GameEntity> _buffer;

        public FillItemSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
            _buffer = new List<GameEntity>();
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Item, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return !_context.isGameOver &&
                   _context.hasBoard &&
                   !_context.boardEntity.isIntialize;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var size = _context.board.value.Size;
            var pieces = _context.board.value.Items;
            for (var col = 0; col < size.Columns; ++col)
            {
                for (var row = 0; row < size.Rows; ++row)
                {
                    // search empty element;
                    var pos = new IntVector2(col, row);
                    if (_context.GetEntitiesWithPosition(pos).Count != 0) continue;

                    _context.CreateItem(pos, pieces);
                }
            }
        }

        public void Cleanup()
        {   
            _buffer
                .Slinq()
                .ForEach(e => e.Destroy());
            _buffer.Clear();
        }
    }
}