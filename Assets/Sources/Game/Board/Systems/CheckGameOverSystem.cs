using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using Smooth.Slinq.Context;
using UnityEngine;
using static System.Math;

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
            return context.CreateCollector(GameMatcher.Animating, GroupEvent.Removed);
        }

        protected override bool Filter(GameEntity entity)
        {
            return _animGroup.count == 0;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            _context.isGameOver = IsGameOver();
        }

        private bool IsGameOver()
        {
            return !Search2ItemInChaine(Axis.X, Axis.X, Axis.Y)
                .Where(ls => IsStepAllowed(ls[0].sprite.value,
                                 SearchItemAtPosition(ls[0].position.value + Vector2.down),
                                 Vector2.right,
                                 Vector2.down,
                                 Vector2.left) ||
                             IsStepAllowed(ls[ls.Count - 1].sprite.value,
                                 SearchItemAtPosition(ls[ls.Count - 1].position.value + Vector2.up),
                                 Vector2.left,
                                 Vector2.up,
                                 Vector2.right))
                .SelectMany(ls => ls.Slinq())
                .Concat(Search2ItemInChaine(Axis.Y, Axis.Y, Axis.X)
                    .Where(ls => IsStepAllowed(ls[0].sprite.value,
                                     SearchItemAtPosition(ls[0].position.value + Vector2.left),
                                     Vector2.left,
                                     Vector2.up,
                                     Vector2.down) ||
                                 IsStepAllowed(ls[ls.Count - 1].sprite.value,
                                     SearchItemAtPosition(ls[ls.Count - 1].position.value + Vector2.right),
                                     Vector2.right,
                                     Vector2.up,
                                     Vector2.down))
                    .SelectMany(ls => ls.Slinq()))
                .Concat(_itemGroup.GetEntities()
                    .Slinq()
                    .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.Y, Axis.X))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.Y, Vector2.up, Vector2.down), new List<GameEntity>()))
                .Concat(_itemGroup.GetEntities()
                    .Slinq()
                    .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.X, Vector2.left, Vector2.right), new List<GameEntity>()))
                .Any();
        }

        private GameEntity SearchItemAtPosition(Vector2 position)
        {
            return _context.GetEntitiesWithPosition(position).Slinq().FirstOrDefault();
        }

        private Slinq<List<GameEntity>, PredicateContext<List<GameEntity>,
            BufferPredicateContext<GameEntity, LinkedContext<GameEntity>>>> Search2ItemInChaine(Axis bufferBy,
            params Axis[] orderBy)
        {
            return _itemGroup.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, orderBy))
                .BufferWhere((e1, e2) => Abs(e1.Odds(e2, bufferBy)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(ls => ls.Count == 2 && ls.Slinq().First().isInteractive);
        }

        private bool IsAllowSpetMiddlePattern(GameEntity e, IList<GameEntity> buffer, Axis axis,
            params Vector2[] offsets)
        {
            return Tuple.Create(buffer, e)
                .MatchTo<Tuple<IList<GameEntity>, GameEntity>, Tuple<IList<GameEntity>, GameEntity>>()
                .Where(_ => _.Item1.Count > 0 &&
                            Abs(_.Item1[_.Item1.Count - 1].position.value[(int) axis] -
                                _.Item2.position.value[(int) axis]) > 0.01f)
                .Return(_ =>
                {
                    // next row||col
                    _.Item1.Clear();
                    return _;
                })
                .Else(_ => _)
                .Result()
                .MatchTo<Tuple<IList<GameEntity>, GameEntity>, bool>()
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
                    return _.Item1[1].isInteractive && IsStepAllowed(_.Item1[1].sprite.value, _.Item1[0], offsets);
                })
                .Else(_ => false)
                .Result();
        }

        private bool IsStepAllowed(Sprite sprite, GameEntity item, params Vector2[] offsets)
        {
            return item.MatchTo<GameEntity, bool>()
                .Where(i => i != null)
                .Return(i => offsets
                    .Slinq()
                    .Where((offset, allow) => allow, i.isInteractive)
                    .Select((offset, position) => position + offset, i.position.value)
                    .SelectMany((p, context) => context.GetEntitiesWithPosition(p).Slinq(), _context)
                    .Any((e, t) => !t.Item1.Equals(e.position.value) && t.Item2.Equals(e.sprite.value),
                        Tuple.Create(i.position.value, sprite))
                )
                .Else(i => false)
                .Result();
        }

        private static bool UpdateBuffer(GameEntity entity, IList<GameEntity> buffer, int removeIndex = -1,
            bool result = false)
        {
            if (removeIndex >= 0 && removeIndex < buffer.Count) buffer.RemoveAt(removeIndex);
            if (entity != null) buffer.Add(entity);
            return result;
        }
    }
}