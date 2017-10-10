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
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.View)
                , GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasPosition && entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(e =>
                {
                    e.view.value.name = "IITEM_" + e.position.value;
                    e.view.value.transform.position = new Vector2(e.position.value.X, e.position.value.Y+1);
                    e.view.value.transform.DOMove(new Vector3
                        {
                            x = e.position.value.X,
                            y = e.position.value.Y,
                            z = e.view.value.transform.position.z
                        }, 0.3f)
                        .OnStart(() => e.isAnimating = true)
                        .OnComplete(() => e.isAnimating = false);
                });
        }
    }
}