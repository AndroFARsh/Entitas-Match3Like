using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Smooth.Slinq;
using UnityEngine;

namespace Game.View.Systems
{
    public class SetPositionSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public SetPositionSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        { 
            return context.CreateCollector(GameMatcher.View, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasPosition && entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var boardSize = _context.board.value.Size;
            entities
                .Slinq()
                .ForEach(e =>
                {
                    e.view.value.name = "IITEM_" + e.position.value;
                    e.view.value.transform.position = new Vector2(e.position.value.x, boardSize.Rows);
                    e.view.value.transform.DOMove(e.position.value, 0.3f)
                        .OnStart(() => e.isAnimating = true)
                        .OnComplete(() => e.isAnimating = false);
                });
        }
    }
}