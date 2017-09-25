using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using UniInput = UnityEngine.Input;
using UnityEngine;
using static InputMatcher;

namespace Input.Components.Systems
{
    public class EmitMouseInputSystem : IInitializeSystem, IExecuteSystem, ICleanupSystem
    {
        private readonly InputContext _context;
        private readonly List<InputEntity> _entities = new List<InputEntity>();
        private readonly IGroup<InputEntity> _group;

        public EmitMouseInputSystem(Contexts contexts)
        {
            _context = contexts.input;
            _group = _context.GetGroup(AnyOf(ButtonDown, ButtonPressed, ButtonUp));
        }

        public void Initialize()
        {
            _context.isLeftMouseButton = true;
            _entities.Add(_context.leftMouseButtonEntity);
            
//            _context.isRightMouseButton = true;
//            _entities.Add(_context.rightMouseButtonEntity);
//            
//            _context.isMiddleMouseButton = true;
//            _entities.Add(_context.middleMouseButtonEntity);
        }

        public void Execute()
        {
            _entities
                .SlinqWithIndex()
                .ForEach(value =>
                {
                    value.Match()
                        .Where(v => UniInput.GetMouseButtonDown(v.Item2))
                        .Do( v => value.Item1.ReplaceButtonDown(Camera.main.ScreenToWorldPoint(UniInput.mousePosition)))
                        .Where(v => UniInput.GetMouseButton(v.Item2))
                        .Do( v => value.Item1.ReplaceButtonPressed(Camera.main.ScreenToWorldPoint(UniInput.mousePosition)))
                        .Where(v => UniInput.GetMouseButtonUp(v.Item2))
                        .Do( v => value.Item1.ReplaceButtonUp(Camera.main.ScreenToWorldPoint(UniInput.mousePosition)))
                        .IgnoreElse()
                        .Exec();
              });
        }

        public void Cleanup()
        {
          //  _group.GetEntities().Slinq().ForEach(e => e.Destroy());
        }
    }
}