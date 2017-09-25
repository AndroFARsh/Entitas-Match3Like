using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace  Game.View.Systems
{
    public class SetSpriteSystem : ReactiveSystem<GameEntity>
    {
        public SetSpriteSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Sprite);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasSprite && entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(e =>
                    {
                        e.view.value
                            .TryGetComponent<SpriteRenderer>()
                            .Or(() => e.view.value.AddComponent<SpriteRenderer>().ToSome())
                            .ForEach(sr =>
                            {
                                var color = sr.color;
                                color.a = 1.0f;
                                sr.material.DOColor(color, 0.6f);
           
                                sr.sprite = e.sprite.value;
                            });
//                        e.view.value
//                            .UnityComponentToOption<Transform>()
//                            .ForEach(tr =>
//                            {
//                                tr.localScale = Vector3.zero;
//                                tr.DOScale(Vector3.one, 0.6f);
//                            });
                    }
                );
        }
    }
}