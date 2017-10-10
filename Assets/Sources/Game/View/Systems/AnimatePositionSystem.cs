using System;
using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Smooth.Slinq;
using UnityEngine;

namespace Game.View.Systems
{
    public class AnimatePositionSystem : ReactiveSystem<GameEntity>
    {
        public AnimatePositionSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Position)
                .NoneOf(GameMatcher.Animating));
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasPosition && entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .Where(e => Math.Abs(e.view.value.transform.position.x - e.position.value.X) > 0.01f ||
                            Math.Abs(e.view.value.transform.position.y - e.position.value.Y) > 0.01f)
                .ForEach(e =>
                    e.view.value.transform.DOMove(new Vector3
                        {
                            x = e.position.value.X,
                            y = e.position.value.Y,
                            z = e.view.value.transform.position.z
                        }, 0.3f)
                        .OnStart(() => e.isAnimating = true)
                        .OnComplete(() => e.isAnimating = false)
                );
        }
    }
}