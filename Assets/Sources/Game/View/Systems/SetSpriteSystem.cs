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
        private readonly GameContext _context;

        public SetSpriteSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.View);
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
                        var sprite = e.isInteractive
                            ? _context.board.value.Items.Interactive[e.sprite.value]
                            : _context.board.value.Items.NotInteractive[e.sprite.value];
                        
                        e.view.value
                            .TryGetComponent<SpriteRenderer>()
                            .Or(() => e.view.value.AddComponent<SpriteRenderer>().ToSome())
                            .ForEach(sr =>
                            {
                                sr.sprite = sprite;
                                
                                var color = sr.color;
                                color.a = 1.0f;
                                sr.material.DOColor(color, 0.6f);
                            });
                    }
                );
        }
    }
}