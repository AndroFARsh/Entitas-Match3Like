using System;
using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using UnityEngine;
using static GameMatcher;

namespace Game.View.Systems
{
    public class SwitchItemSystem : ReactiveSystem<GameEntity>
    {
        private const int MATCH_3 = 3;
        
        private readonly IGroup<GameEntity> _group;
        private readonly GameContext _context;


        public SwitchItemSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
            _group = contexts.game.GetGroup(Selected);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Selected, GroupEvent.Added);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasView;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            if (_group.count < 2) return;
            
            _group.GetEntities()
                .Match()
                .Where(IsAllowToSwitch)
                .Do(SwitchItem) 
                .Else(UnselectItem)
                .Exec();
        }

        private bool IsAllowToSwitch(GameEntity[] entities)
        {
            if (entities.Length != 2) return false;
            
            var i1 = entities[0];
            var i2 = entities[1];
            var p1 = i1.position.value;
            var p2 = i2.position.value;
            
            var allow = (Math.Abs(p1.y - p2.y) < 0.01f &&
                    (Math.Abs(p1.x - (p2.x - 1)) < 0.01f ||
                     Math.Abs(p1.x - (p2.x + 1)) < 0.01f)) ||
                   
                   (Math.Abs(p1.x - p2.x) < 0.01f &&
                    (Math.Abs(p1.y - (p2.y - 1)) < 0.01f ||
                     Math.Abs(p1.y - (p2.y + 1)) < 0.01f));
            if (allow)
            {
                return IsMatch3(i1, p2, true) ||
                       IsMatch3(i1, p2, false) ||
                       IsMatch3(i2, p1, true) ||
                       IsMatch3(i2, p1, false);
            }
            return false;
        }

        private bool IsMatch3(GameEntity entity, Vector2 newPos, bool horizontal)
        {
            var index = horizontal ? 0 : 1;
            var sprite = entity.sprite.value;

            var chainLength = 1;
            // Look forward
            IsMatch3(sprite, entity.position.value, newPos, index, 1, ref chainLength);
            // Look backward
            IsMatch3(sprite, entity.position.value, newPos, index, -1, ref chainLength);
            
            return chainLength >= MATCH_3;
        }

        private void IsMatch3(Sprite sprite, Vector2 pos, Vector2 newPos, int index, int step, ref int chainLength)
        {
            do
            {
                newPos[index] += step;
                if (pos.Equals(newPos)) return;
                    
                var count = _context.GetEntitiesWithPosition(newPos)
                    .Slinq()
                    .Where(e => e.isInteractive)
                    .Select(e => e.sprite.value)
                    .Where(sprite.Equals)
                    .Count();
                
                if (count == 0) return;
                chainLength += count;
            } while (chainLength < MATCH_3);
        }


        private static void SwitchItem(GameEntity[] entities)
        {
            var i1 = entities[0];
            var i2 = entities[1];
            var p1 = i1.position.value;
            var p2 = i2.position.value;
            
            i1.ReplacePosition(p2);
            i1.isSelected = false;
            
            i2.ReplacePosition(p1);
            i2.isSelected = false;
        }
        
        private static void UnselectItem(GameEntity[] entities)
        {
            entities.Slinq().ForEach(e => e.isSelected = false);
        }
    }
}