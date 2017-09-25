using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace  Game.View.Systems
{
    public class RemoveViewSystem : ReactiveSystem<GameEntity>
    {
        public RemoveViewSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Reomve, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(DestroyView);
        }

        private static void DestroyView(GameEntity entity)
        {
            entity.ToOption()
                .Select(e => Tuple.Create(e.view.value, e))
                .ForEach(t => t.Item1.transform
                    .DOScale(Vector3.one * 1.2f, 0.2f)
                    .OnComplete(() =>
                    {
                        t.Item1.Unlink();
                        Object.Destroy(t.Item1);
                    }))
                .Select(t => Tuple.Create(t.Item1.GetComponent<SpriteRenderer>(), t.Item2))
                .ForEach(t =>
                {
                    var color = t.Item1.color;
                    color.a = 0.0f;
                    t.Item1.material.DOColor(color, 0.2f)
                        .OnStart(() => t.Item2.isAnimating = true)
                        .OnComplete(() => t.Item2.isAnimating = false);
                });
            
            entity.RemoveView();
        }
    }
}