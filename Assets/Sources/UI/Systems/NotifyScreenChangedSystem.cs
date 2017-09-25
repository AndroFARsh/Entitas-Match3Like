using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Slinq;
using static Entitas.GroupEvent;
using static UiMatcher;

namespace Ui.Systems
{
    public class NotifyScreenChangedSystem: ReactiveSystem<UiEntity>
    {
        private readonly IGroup<UiEntity> _group;

        public NotifyScreenChangedSystem(Contexts contexts) : base(contexts.ui)
        {
            _group = contexts.ui.GetGroup(ScreenListener);
        }

        protected override ICollector<UiEntity> GetTrigger(IContext<UiEntity> context)
        {
            return context.CreateCollector(UiMatcher.Screen, Added);
        }

        protected override bool Filter(UiEntity entity)
        {
            return true;
        }

        protected override void Execute(List<UiEntity> entities)
        {
            entities
                .Slinq()
                .Select(e => e.screen.value)
                .SelectMany(s => _group.GetEntities()
                    .Slinq()
                    .Select(e => e.screenListener.value)
                    .Select(Tuple.Create, s)
                )
                .ForEach(t => t.Item1(t.Item2));
        }
    }
}