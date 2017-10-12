using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace Game.View.Systems
{
    public class RemoveViewSystem : ReactiveSystem<GameEntity>
    {
        public RemoveViewSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Remove,GameMatcher.View)
                                                      .NoneOf(GameMatcher.Animating));
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
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
                .Select(e => e.view.value)
                .ForEach(go =>
                {
                    go.transform.DOScale(Vector3.one * 1.2f, 0.5f)
                        .OnComplete(() =>
                        {
                            go.Unlink();
                            Object.Destroy(go);
                            entity.Destroy();
                        });
                    go.TryGetComponent<SpriteRenderer>()
                        .ForEach(sr =>
                        {
                            var color = sr.color;
                            color.a = 0.0f;
                            sr.material.DOColor(color, 0.5f)
                                .OnStart(() => entity.isAnimating = true)
                                .OnComplete(() => entity.isAnimating = false);
                        });
                });
        }
    }
}