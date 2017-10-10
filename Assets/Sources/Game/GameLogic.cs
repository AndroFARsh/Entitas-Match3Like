using System;
using System.Collections.Generic;
using Entitas;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using Smooth.Slinq.Context;
using Tools;
using UnityEngine;

namespace Game
{
    internal static class GameContextExtention
    {
        internal static GameEntity CreateItem(this GameContext context, IntVector2 position, Items config)
        {
            return UnityEngine.Random.value > config.Distribution
                ? CreateBlockItem(context, position, config)
                : CreatePieceItem(context, position, config);
        }
        
        internal static GameEntity CreateBlockItem(this GameContext context, IntVector2 position, Items config)
        {
            var entity = context.CreateEntity();
            entity.isItem = true;
            entity.AddPosition(position);

            entity.isInteractive = false;

            entity.AddSprite(config.GetRandomNotInteractive());
            
            return entity;
        }
        
        internal static GameEntity CreatePieceItem(this GameContext context, IntVector2 position, Items config)
        {
            var entity = context.CreateEntity();
            entity.isItem = true;
            entity.AddPosition(position);

            entity.isInteractive = true;

            entity.AddSprite(config.GetRandomInteractive());
            return entity;
        }
    }
    
    public enum Axis {
        X=0, 
        Y=1
    }
    
    internal static class GameEntytyExtention
    {
        public static int OrderBy(this GameEntity e1, GameEntity e2,  params Axis[] axises)
        {
            if (!e1.hasPosition && !e2.hasPosition)
                return 0;
            if (!e1.hasPosition)
                return -1;
            if (!e2.hasPosition)
                return 1;
            
            var result = 0.0f;
            foreach (var axis in axises)
            {
                result = e1.position.value[axis] - e2.position.value[axis];
                if (Math.Abs(result) > 0.01f) return (int)result;
            }
            return (int)result;
        }
        
        public static float Odds(this GameEntity e1, GameEntity e2, Axis axis)
        {
            return e1.position.value[axis] - e2.position.value[axis];
        }
    }
    
    internal static class GameOverExtention
    {
        public static bool IsGameOver(this IGroup<GameEntity> items, GameContext context)
        {
            return !items.Search2ItemInChaine(Axis.X, Axis.X, Axis.Y)
                .Where(ls => IsStepAllowed(context, ls[0].sprite.value,
                                 context.SearchItemAtPosition(ls[0].position.value + IntVector2.Down),
                                 IntVector2.Right,
                                 IntVector2.Down,
                                 IntVector2.Left) ||
                             IsStepAllowed(context, ls[ls.Count - 1].sprite.value,
                                 context.SearchItemAtPosition(ls[ls.Count - 1].position.value + IntVector2.Up),
                                 IntVector2.Left,
                                 IntVector2.Up,
                                 IntVector2.Right))
                .SelectMany(ls => ls.Slinq())
                .Concat(items.Search2ItemInChaine(Axis.Y, Axis.Y, Axis.X)
                    .Where(ls => IsStepAllowed(context, ls[0].sprite.value,
                                     context.SearchItemAtPosition(ls[0].position.value + IntVector2.Left),
                                     IntVector2.Left,
                                     IntVector2.Up,
                                     IntVector2.Down) ||
                                 IsStepAllowed(context, ls[ls.Count - 1].sprite.value,
                                     context.SearchItemAtPosition(ls[ls.Count - 1].position.value + IntVector2.Right),
                                     IntVector2.Right,
                                     IntVector2.Up,
                                     IntVector2.Down))
                    .SelectMany(ls => ls.Slinq()))
                .Concat(items.GetEntities()
                    .Slinq()
                    .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.Y, Axis.X))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(context, e, buffer, Axis.Y, IntVector2.Up, IntVector2.Down), new List<GameEntity>()))
                .Concat(items.GetEntities()
                    .Slinq()
                    .OrderBy((e1, e2) => e1.OrderBy(e2, Axis.X, Axis.Y))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(context, e, buffer, Axis.X, IntVector2.Left, IntVector2.Right), new List<GameEntity>()))
                .Any();
        }

        private static GameEntity SearchItemAtPosition(this GameContext context, IntVector2 position)
        {
            return context.GetEntitiesWithPosition(position).Slinq().FirstOrDefault();
        }

        private static Slinq<List<GameEntity>, PredicateContext<List<GameEntity>,
            BufferPredicateContext<GameEntity, LinkedContext<GameEntity>>>> Search2ItemInChaine(this IGroup<GameEntity> items, Axis bufferBy,
            params Axis[] orderBy)
        {
            return items.GetEntities()
                .Slinq()
                .OrderBy((e1, e2) => e1.OrderBy(e2, orderBy))
                .BufferWhere((e1, e2) => Mathf.Abs(e1.Odds(e2, bufferBy)) < 0.01f && e1.sprite.value.Equals(e2.sprite.value))
                .Where(ls => ls.Count == 2 && ls[0].isInteractive);
        }

        private static bool IsAllowSpetMiddlePattern(GameContext context, GameEntity e, IList<GameEntity> buffer, Axis axis,
            params IntVector2[] offsets)
        {
            return Tuple.Create(buffer, e)
                .MatchTo<Tuple<IList<GameEntity>, GameEntity>, Tuple<IList<GameEntity>, GameEntity>>()
                .Where(_ => _.Item1.Count > 0 &&
                            Mathf.Abs(_.Item1[_.Item1.Count - 1].position.value[(int) axis] -
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
                    return _.Item1[1].isInteractive && IsStepAllowed(context,_.Item1[1].sprite.value, _.Item1[0], offsets);
                })
                .Else(_ => false)
                .Result();
        }

        private static bool IsStepAllowed(GameContext context, int sprite, GameEntity item, params IntVector2[] offsets)
        {
            return item.MatchTo<GameEntity, bool>()
                .Where(i => i != null)
                .Return(i => offsets
                    .Slinq()
                    .Where((offset, allow) => allow, i.isInteractive)
                    .Select((offset, position) => position + offset, i.position.value)
                    .SelectMany((p, c) => c.GetEntitiesWithPosition(p).Slinq(), context)
                    .Any((e, t) => t.Item1 != e.position.value && t.Item2 == e.sprite.value,
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