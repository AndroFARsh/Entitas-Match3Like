using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;
using static GameMatcher;

namespace Game.View.Systems
{
    public class SelectItemAudioSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public SelectItemAudioSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Selected, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.hasConfig && entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            var config = _context.config.value.AudioClips;
            entities
                .Slinq()
                .ForEach(e => e.view.value
                    .TryGetComponent<AudioSource>()
                    .ForEach(config.SelectItem));
        }
    }
}