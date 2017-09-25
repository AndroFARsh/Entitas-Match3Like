using System;
using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Smooth.Slinq;

namespace Game.View.Systems
{
    public class AnimatePositionSystem : ReactiveSystem<GameEntity>
    {
        private readonly IGroup<GameEntity> _group;

        public AnimatePositionSystem(Contexts contexts) : base(contexts.game)
        {
            _group = contexts.game.GetGroup(GameMatcher.Animating);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        { 
            return context.CreateCollector(GameMatcher.Position, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasPosition && entity.hasView ;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .Where(e => Math.Abs(e.view.value.transform.position.x - e.position.value.x) > 0.01f ||
                            Math.Abs(e.view.value.transform.position.y - e.position.value.y) > 0.01f)
                .ForEach(e => 
                    e.view.value.transform.DOMove(e.position.value, 0.3f)
                        .OnStart(() => e.isAnimating = true )
                        .OnComplete(() => e.isAnimating = false )
                );
        }
    }
}