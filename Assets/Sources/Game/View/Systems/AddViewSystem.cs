using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using UnityEngine;

namespace  Game.View.Systems
{
    public class AddViewSystem : IExecuteSystem
    {
        private const string NAME = "Board";
        
        private readonly Transform _container = new GameObject(NAME).transform;
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _group;

        public AddViewSystem(Contexts contexts)
        {
            _context = contexts.game;
            _group = _context.GetGroup(GameMatcher.Item);
        }

        private bool IsExecute()
        {
            return _context.hasBoard && !_context.boardEntity.isIntialize && _group.count > 0;
        }

        public void Execute()
        {
            if (!IsExecute()) return;
            
            _group.GetEntities()
                .Slinq()
                .Where(e => !e.hasView)
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