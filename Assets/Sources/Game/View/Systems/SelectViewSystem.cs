using System.Collections.Generic;
using DG.Tweening;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using UnityEngine;
using static GameMatcher;

namespace Game.View.Systems
{
    public class SelectViewSystem : ReactiveSystem<GameEntity>
    {
        public SelectViewSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Selected, GroupEvent.AddedOrRemoved);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(entity => entity.Match()
                     .Where(e => e.isSelected)
                     .Do(e => e.view.value.transform.DOScale(Vector3.one * 0.8f, 0.2f))
                     .Else(e => e.view.value.transform.DOScale(Vector3.one * 1f, 0.2f))   
                     .Exec());
        }
    }
}