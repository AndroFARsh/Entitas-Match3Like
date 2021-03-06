﻿using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Smooth.Algebraics;
using Smooth.Slinq;
using Object = UnityEngine.Object;

namespace Game.Board.Systems
{
    public class DestroyBoardSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly GameContext _context;

        public DestroyBoardSystem(Contexts contexts)
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(GameMatcher.Item);
        }
        
        
        public void Cleanup()
        {
            if (!_context.isGameOver) return;
            
            _group
                .GetEntities()
                .Slinq()
                .Where(e => e.hasView)
                .ForEach(e =>
                {
                    var go = e.view.value;
                    go.Unlink();
                    Object.Destroy(go);
                    e.Destroy();
                });
            _context.boardEntity.ToOption().ForEach(e => e.Destroy());
        }
    }
}