using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;
using static GameMatcher;

namespace Game.View.Systems
{
    public class RemoveItemAudioSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public RemoveItemAudioSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Remove, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.isIntialized 
                   && _context.hasConfig 
                   && !_context.isGameOver  
                   && entity.hasView ;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _context.gameControllerEntity
                .ToOption()
                .Where(e => e.hasView)
                .SelectMany(e => e.view.value.TryGetComponent<AudioSource>())
                .ForEach(_context.config.value.AudioClips.RemoveItems);
        }
    }
}