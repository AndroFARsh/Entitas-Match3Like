using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using Random = UnityEngine.Random;

namespace Game.Board.Systems
{
    public class FillItemSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;
        
        public FillItemSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Reomve, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.hasBoard;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var pieces = _context.board.value.Items;
            entities
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                .Select(e => e.Match())
                .ForEach(m =>
                {
                    m.Where(e => Random.value > pieces.Distribution)
                        .Do(e => e.InitBlock(pieces.NotInteractive))
                        .Else(e => e.InitPiece(pieces.Interactive))
                        .Exec();
                });
        }
    }
}