using System;
using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using UnityEngine;
using static GameMatcher;
using Tuple = Smooth.Algebraics.Tuple;

namespace Game.Board.Systems
{
    public class CheckGameOverSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;
        private readonly IGroup<GameEntity> _itemGroup;
        private readonly IGroup<GameEntity> _animGroup;

        public CheckGameOverSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
            _itemGroup = contexts.game.GetGroup(GameMatcher.Item);
            _animGroup = contexts.game.GetGroup(GameMatcher.Animating);
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(Animating, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _animGroup.count == 0;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            // Search vertical lines xoox
            var vSearch = _itemGroup.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                .BufferWhere((e1, e2) =>
                    Math.Abs(e1.Odds(e2, Axis.X)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(Match2Predicat)
                .Where(ls => IsStepAllowed(ls[0].sprite.value,
                                 ls[0].position.value + Vector2.down,
                                 Vector2.right,
                                 Vector2.down,
                                 Vector2.left) ||
                             IsStepAllowed(ls[ls.Count - 1].sprite.value,
                                 ls[ls.Count - 1].position.value + Vector2.up,
                                 Vector2.left,
                                 Vector2.up,
                                 Vector2.right))
                .SelectMany(ls => ls.Slinq());


            // Search horizontal lines xoox
            var hSearch = _itemGroup.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.Y, Axis.X))
                .BufferWhere((e1, e2) =>
                    Math.Abs(e1.Odds(e2, Axis.Y)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(Match2Predicat)
                .Where(ls => IsStepAllowed(ls[0].sprite.value,
                                 ls[0].position.value + Vector2.left,
                                 Vector2.left,
                                 Vector2.up,
                                 Vector2.down) ||
                             IsStepAllowed(ls[ls.Count - 1].sprite.value,
                                 ls[ls.Count - 1].position.value + Vector2.right,
                                 Vector2.right,
                                 Vector2.up,
                                 Vector2.down))
                .SelectMany(ls => ls.Slinq());

            // Search horizontal lines oxo
            var hSearchMP = _itemGroup.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.Y, Axis.X))
                .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.Y, Vector2.up, Vector2.down),
                    new List<GameEntity>());

            // Search vertical lines oxo
            var vSearchMP = _itemGroup.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.X, Vector2.left, Vector2.right),
                    new List<GameEntity>());

            _context.isGameOver = !vSearch
                .Concat(hSearch)
                .Concat(vSearchMP)
                .Concat(hSearchMP)
                .Any();
        }

        private bool IsAllowSpetMiddlePattern(GameEntity e, IList<GameEntity> buffer, Axis axis, params Vector2[] offsets)
        {
            return Tuple.Create(buffer, e)
                .MatchTo<Smooth.Algebraics.Tuple<IList<GameEntity>, GameEntity>, Smooth.Algebraics.Tuple<IList<GameEntity>, GameEntity>>()
                .Where(_ =>  _.Item1.Count > 0 && Math.Abs(_.Item1[_.Item1.Count - 1].position.value[(int)axis] - _.Item2.position.value[(int)axis])  > 0.01f)
                .Return(_ =>
                {
                    // next row||col
                    _.Item1.Clear();
                    return _;
                })
                .Else(_ => _)
                .Result()
                .MatchTo<Smooth.Algebraics.Tuple<IList<GameEntity>, GameEntity>, bool>()
                .Where(_ => _.Item1.Count == 0 ||
                              (_.Item1.Count == 1 && !_.Item1[0].sprite.value.Equals(_.Item2.sprite.value)))
                .Return(_ => UpdateBuffer(_.Item2, _.Item1))
                .Where(_ => (_.Item1.Count == 1 && _.Item1[0].sprite.value.Equals(_.Item2.sprite.value)) ||
                              (_.Item1.Count == 2 && !_.Item1[0].sprite.value.Equals(_.Item2.sprite.value)))
                .Return(_ => UpdateBuffer(_.Item2, _.Item1, 0))
                .Where(_ => _.Item1.Count == 2 && _.Item1[0].sprite.value.Equals(_.Item2.sprite.value))
                .Return(_ =>
                {
                    UpdateBuffer(_.Item2, _.Item1, 0);
                    return IsStepAllowed(
                        _.Item1[1].sprite.value,
                        _.Item1[0].position.value, offsets);
                })
                .Else(_ => false)
                .Result();
        }
        
        private bool IsStepAllowed(Sprite sprite, Vector2 pos, params Vector2[] offsets)
        {          
            return offsets
                .Slinq()
                .Select((offset, position) => position + offset, pos)
                .SelectMany((p, context) => context.GetEntitiesWithPosition(p).Slinq(), _context)
                .Any((e, t) => !t.Item1.Equals(e.position.value) && t.Item2.Equals(e.sprite.value), Tuple.Create(pos, sprite));
        }

        private static bool Match2Predicat(ICollection<GameEntity> collection)
        {
            return collection.Count == 2 && collection.Slinq().First().isInteractive;
        }

        private static bool UpdateBuffer(GameEntity entity, IList<GameEntity> buffer, int removeIndex = -1, bool result = false)
        {
            if (removeIndex >= 0 && removeIndex < buffer.Count) buffer.RemoveAt(removeIndex);
            if (entity != null) buffer.Add(entity);
            return result;
        }
    }
}