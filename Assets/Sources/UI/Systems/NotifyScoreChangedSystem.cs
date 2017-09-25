using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Slinq;
using static Entitas.GroupEvent;
using static GameMatcher;
using static UiMatcher;

namespace Ui.Systems
{
    public class NotifyScoreChangedSystem: ReactiveSystem<GameEntity>
    {
        private readonly IGroup<UiEntity> _group;

        public NotifyScoreChangedSystem(Contexts contexts) : base(contexts.game)
        {
            _group = contexts.ui.GetGroup(ScoreListener);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Score, Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .Select(e => e.score.value)
                .SelectMany(s => _group.GetEntities()
                    .Slinq()
                    .Select(e => e.scoreListener.value)
                    .Select(Tuple.Create, s))
                .ForEach(t => t.Item1(t.Item2));
        }
    }
}