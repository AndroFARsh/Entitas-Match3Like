using System;
using System.Collections.Generic;
using Entitas;
using Smooth.Slinq;
using UnityEngine;

namespace Game.Board.Systems
{
    public class MatchLineSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _group;
        private readonly GameContext _context;

        public MatchLineSystem(Contexts contexts)
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(GameMatcher.Item);
        }
        
        public void Execute()
        {
            if (_context.GetGroup(GameMatcher.Animating).count != 0)
                return;
            
            // Search vertical lines
            var vSearch = _group.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                .BufferWhere((e1, e2) =>
                    Math.Abs(e1.Odds(e2, Axis.X)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(Match3Predicat)
                .SelectMany(list => list.Slinq());

            // Search horizontal lines
            var hSearch = _group.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.Y, Axis.X))
                .BufferWhere((e1, e2) =>
                    Math.Abs(e1.Odds(e2, Axis.Y)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(Match3Predicat)
                .SelectMany(list => list.Slinq());

            hSearch
                .Concat(vSearch)
                .ForEach(item => item.isRemove = true);
        }

        private static bool Match3Predicat(ICollection<GameEntity> collection)
        {
            return collection.Count > 2 && collection.Slinq().First().isInteractive;
        }
    }
}