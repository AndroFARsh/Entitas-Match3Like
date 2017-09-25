using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace  Game.View.Systems
{
    public class AddViewSystem : ReactiveSystem<GameEntity>
    {
        private const string NAME = "Board";
        
        private readonly Transform _container = new GameObject(NAME).transform;
        private readonly GameContext _context;
        
        public AddViewSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Sprite);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _context.hasBoard && 
                   !entity.hasView && 
                   !entity.isReomve;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .ForEach(e =>
                    {
                        Object.Instantiate(_context.board.value.Items.Prefab)
                            .ToOption()
                            .ForEachOr(go =>
                            {
                                go.transform.SetParent(_container, false);
                                e.AddView(go);
                                go.Link(e, _context);
                            }, () => { Debug.LogError("Prefab not found"); });
                    }
                );
        }
    }
}