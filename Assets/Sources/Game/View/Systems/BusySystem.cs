using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace  Game.View.Systems
{
    public class BusySystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _group;

        public BusySystem(Contexts contexts) :base(contexts.game)
        {
            _context = contexts.game;
            _group = _context.GetGroup(GameMatcher.Animating);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Animating, GroupEvent.AddedOrRemoved);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _context.isBusy = _group.count != 0;
        }
    }
}