using System.Collections.Generic;
using Entitas;
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
            return true;
        }

        protected override void Execute(List<InputEntity> entities)
        {
            entities
                .Slinq()
                .Select(e => Physics2D.Raycast(e.buttonDown.value, Vector2.zero, 100))
                .Where(hit => hit.collider != null)
                .Select(hit => hit.collider.transform.position)
                .SelectMany(pos => _context.GetEntitiesWithPosition(pos).Slinq())
                .Where(e => e.isInteractive)
                .ForEach(e =>
                {
                    var value = e.isSelected;
                    e.isSelected = !value;
                });
        }
    }
}