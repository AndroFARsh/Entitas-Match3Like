using System.Collections.Generic;
using Entitas;
using Game;
using Smooth.Slinq;
using UnityEngine;
using static InputMatcher;

namespace Input.Components.Systems
{
    public class SelectItemSystem: ReactiveSystem<InputEntity>
    {
        private readonly GameContext _context;
        
        public SelectItemSystem(Contexts contexts) : base(contexts.input)
        {
            _context = contexts.game;
        }

        protected override ICollector<InputEntity> GetTrigger(IContext<InputEntity> context)
        {
            return context.CreateCollector(AllOf(LeftMouseButton, ButtonDown));
        }

        protected override bool Filter(InputEntity entity)
        {
            return _context.hasBoard;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            entities
                .Slinq()
                .Select(e =>
                {
                    var result = Physics2D.Raycast(e.buttonDown.value, Vector2.zero, 100);
                    return result;
                })
                .Where(hit => hit.collider != null)
                .Select(hit => hit.collider.transform.position)
                .Select(pos => new IntVector2((int)pos.x, (int)pos.y))
                .SelectMany(pos => _context.GetEntitiesWithPosition(pos).Slinq())
                .Where(e => e.isInteractive)
                .ForEach(e => e.isSelected = !e.isSelected);
        }
    }
}