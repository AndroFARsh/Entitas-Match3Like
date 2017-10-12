using System.Collections.Generic;
using Entitas;
using Smooth.Slinq;
using Tools;

namespace Game.Board.Systems
{
    public class InitBoardSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public InitBoardSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Board);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(entity =>
                {
                    var config = entity.board.value;
                    Utils.Range(config.Size.Columns).Slinq()
                        .SelectMany(column => Utils.Range(config.Size.Rows)
                            .Slinq()
                            .Select((r, c) => new IntVector2(c, r), column))
                        .ForEach(pos => _context.CreateItem(pos, config.Items));
                });
        }
    }
}