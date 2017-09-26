using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
using Smooth.Slinq.Context;
using Tools;
using UnityEngine;
using static System.Math;
using Random = UnityEngine.Random;
using Tuple = Smooth.Algebraics.Tuple;

namespace Game.Board.Systems
{
    public class InitBoardSystem : ReactiveSystem<GameEntity>
    {
        private readonly GameContext _context;

        public InitBoardSystem(Contexts contexts) : base(contexts.game)
        {
            _context = contexts.game;
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Board);
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .Select(e => e.board.value)
                .ForEach(config =>
                {
                    _context.isIntialize = true;
                    var items = Utils.Range(config.Size.Columns).Slinq()
                        .SelectMany(column => Utils.Range(config.Size.Rows).Slinq().Select(Tuple.Create, column))
                        .Select((t, c) => InitItem(new Item(t.Item1, t.Item2), c), config)
                        .ToList();

                    do
                    {
                        // check if no match3 lines on start and alow at list one turn   
                    } while (!CheckBoardGeneration(items, config));

                    items.Slinq()
                        .ForEach(i =>
                        {
                            _context
                                .CreateItem(i.position.x, i.position.y)
                                .Match()
                                .Where(e => i.interactive)
                                .Do(e => e.InitPiece(i.sprite))
                                .Else(e => e.InitBlock(i.sprite))
                                .Exec();
                        });
                    _context.isIntialize = false;
                });
        }

        private static bool CheckBoardGeneration(IList<Item> items, LevelBlueprint config)
        {
            return 
                // Search if match3 line exist
                !SearchItemInChaine(items, 3, Axis.X, Axis.X, Axis.Y).SelectMany(ls => ls.Slinq())
                    .Concat(SearchItemInChaine(items, 2, Axis.Y, Axis.Y, Axis.X).SelectMany(ls => ls.Slinq()))
                    .Select(InitItem, config)
                    .Aggregate(false, (b, item) => true) &&

                // Search if steps avaliable
                !SearchItemInChaine(items, 2, Axis.X, Axis.X, Axis.Y)
                    .Where((ls, _items) => IsStepAllowed(_items, ls[0].sprite,
                                               ls[0].position + Vector2.down,
                                               Vector2.right,
                                               Vector2.down,
                                               Vector2.left) ||
                                           IsStepAllowed(_items, ls[ls.Count - 1].sprite,
                                               ls[ls.Count - 1].position + Vector2.up,
                                               Vector2.left,
                                               Vector2.up,
                                               Vector2.right), items)
                    .SelectMany(ls => ls.Slinq())
                    .Concat(SearchItemInChaine(items, 2, Axis.Y, Axis.Y, Axis.X)
                        .Where((ls, _items) => IsStepAllowed(_items, ls[0].sprite,
                                                   ls[0].position + Vector2.left,
                                                   Vector2.left,
                                                   Vector2.up,
                                                   Vector2.down) ||
                                               IsStepAllowed(_items, ls[ls.Count - 1].sprite,
                                                   ls[ls.Count - 1].position + Vector2.right,
                                                   Vector2.right,
                                                   Vector2.up,
                                                   Vector2.down), items)
                        .SelectMany(ls => ls.Slinq()))
                    .Concat(items
                        .Slinq()
                        .OrderBy((i1, i2) => OrderBy(i1, i2, Axis.Y, Axis.X))
                        .Where(
                            (i, t) => IsAllowSpetMiddlePattern(t.Item1, i, t.Item2, Axis.Y, Vector2.up,
                                Vector2.down),
                            Tuple.Create(items, new List<Item>())))
                    .Concat(items.Slinq().OrderBy((i1, i2) => OrderBy(i1, i2, Axis.X, Axis.Y))
                        .Where(
                            (i, t) => IsAllowSpetMiddlePattern(t.Item1, i, t.Item2, Axis.X, Vector2.left,
                                Vector2.right),
                            Tuple.Create(items, new List<Item>())))
                    .Any()
                    .ToSome()
                    .Where(v => !v)
                    .ForEach((v, _items) => _items
                            .SlinqWithIndex()
                            .Where(t => t.Item2 % 3 == 0)
                            .Select(t => t.Item1)
                            .ForEach((i, c) => InitItem(i, c), config)
                        , items)
                    .ValueOr(false);
        }

        private static
            Slinq<List<Item>, PredicateContext<List<Item>, BufferPredicateContext<Item, LinkedContext<Item>>, int>>
            SearchItemInChaine(IList<Item> items, int count, Axis bufferBy, params Axis[] orderBy)
        {
            return items
                .Slinq()
                .OrderBy((i1, i2) => OrderBy(i1, i2, orderBy))
                .BufferWhere((i1, i2) => Abs(Odds(i1, i2, bufferBy)) < 0.01f && SpriteEquals(i1, i2))
                .Where((ls, minCount) => ls.Count >= minCount && ls.Slinq().First().interactive, count);
        }

        private static bool IsAllowSpetMiddlePattern(IList<Item> items, Item i, IList<Item> buffer, Axis axis,
            params Vector2[] offsets)
        {
            return Tuple.Create(buffer, i)
                .MatchTo<Tuple<IList<Item>, Item>, Tuple<IList<Item>, Item>>()
                .Where(_ => _.Item1.Count > 0 &&
                            Abs(_.Item1[_.Item1.Count - 1].position[(int) axis] - _.Item2.position[(int) axis]) > 0.01f)
                .Return(_ =>
                {
                    // next row||col
                    _.Item1.Clear();
                    return _;
                })
                .Else(_ => _)
                .Result()
                .MatchTo<Tuple<IList<Item>, Item>, bool>()
                .Where(_ => _.Item1.Count == 0 ||
                            (_.Item1.Count == 1 && !SpriteEquals(_.Item1[0], _.Item2)))
                .Return(_ => UpdateBuffer(_.Item2, _.Item1))
                .Where(_ => (_.Item1.Count == 1 && SpriteEquals(_.Item1[0], _.Item2)) ||
                            (_.Item1.Count == 2 && !SpriteEquals(_.Item1[0], _.Item2)))
                .Return(_ => UpdateBuffer(_.Item2, _.Item1, 0))
                .Where(_ => _.Item1.Count == 2 && SpriteEquals(_.Item1[0], _.Item2))
                .Return(_ =>
                {
                    UpdateBuffer(_.Item2, _.Item1, 0);
                    return _.Item1[1].interactive && IsStepAllowed(items,
                        _.Item1[1].sprite,
                        _.Item1[0], offsets);
                })
                .Else(_ => false)
                .Result();
        }

        private static bool UpdateBuffer(Item item, IList<Item> buffer, int removeIndex = -1, bool result = false)
        {
            if (removeIndex >= 0 && removeIndex < buffer.Count) buffer.RemoveAt(removeIndex);
            if (item != null) buffer.Add(item);
            return result;
        }

        private static bool SpriteEquals(Item i1, Item i2)
        {
            if (i1?.sprite == null || i2?.sprite == null) return false;

            return i1.sprite.Equals(i2.sprite);
        }

        private static int OrderBy(Item i1, Item i2, params Axis[] index)
        {
            var result = 0.0f;
            foreach (var t in index)
            {
                result = Odds(i1, i2, t);
                if (Abs(result) > 0.01f) return (int) result;
            }
            return (int) result;
        }

        private static float Odds(Item i1, Item i2, Axis axis)
        {
            if (i1 != null && i2 != null) return i1.position[(int) axis] - i2.position[(int) axis];
            if (i1 == null && i2 != null) return 1;
            if (i2 == null && i1 != null) return -1;
            return 0;
        }

        private static bool IsStepAllowed(IList<Item> items, Sprite sprite, Vector2 pos, params Vector2[] offsets)
        {
            var item = items.Slinq().Where(i => pos.Equals(i.position)).FirstOrDefault();
            return item != null && IsStepAllowed(items, sprite, item, offsets);
        }

        private static bool IsStepAllowed(IList<Item> items, Sprite sprite, Item item, params Vector2[] offsets)
        {
            return offsets
                .Slinq()
                .Where((o, allow) => allow, item.interactive)
                .Select((offset, position) => position + offset, item.position)
                .SelectMany(p => items.Slinq().Where(i => p.Equals(i.position)))
                .Any((i, t) => !t.Item1.Equals(i.position) && t.Item2.Equals(i.sprite),
                    Tuple.Create(item.position, sprite));
        }

        private static Item InitItem(Item item, LevelBlueprint config)
        {
            return item.MatchTo<Item, Item>()
                .Where(i => Random.value > config.Items.Distribution)
                .Return(i =>
                {
                    i.interactive = false;
                    i.sprite = config.Items.NotInteractive.GetRandomItem();
                    return i;
                })
                .Else(i =>
                {
                    i.interactive = true;
                    i.sprite = config.Items.Interactive.GetRandomItem();
                    return i;
                })
                .Result();
        }
    }

    internal class Item
    {
        internal readonly Vector2 position;
        internal Sprite sprite;
        internal bool interactive;

        internal Item(int x, int y)
        {
            position = new Vector2(x, y);
        }

        public virtual string ToString()
        {
            return $"Item[{(int) position.x}, {(int) position.y}]";
        }
    }
}