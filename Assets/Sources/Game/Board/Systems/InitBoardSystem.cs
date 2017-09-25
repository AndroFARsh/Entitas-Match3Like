
using System.Collections.Generic;
using Entitas;
using Smooth.Algebraics;
using Smooth.Foundations.PatternMatching.GeneralMatcher;
using Smooth.Slinq;
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
                .ForEach(config => {
                    _context.isIntialize = true;
                    var items = Utils.Range(config.Size.Columns).Slinq()
                        .SelectMany(column => Utils.Range(config.Size.Rows).Slinq().Select(Tuple.Create, column))
                        .Select((t, c) => InitItem(new Item(t.Item1, t.Item2), c), config)
                        .ToList();

                    do
                    {
                      // check if no match3 lines on start and alow at list one turn   
                    } while (CheckBoardGeneration(items, config));

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

        private bool CheckBoardGeneration(IList<Item> items, LevelBlueprint config)
        {
            // Search if match3 line exist
            var oneMoreRun = items.Slinq()
                    .OrderBy((i1, i2) => OrderBy(i1, i2, 0, 1))
                    .BufferWhere((i1, i2) => Abs(Odds(i1, i2, 0)) < 0.01f && SpriteEquals(i1, i2))
                    .Where(ls => ls.Count > 2 && ls.Slinq().First().interactive)
                    .SelectMany(ls => ls.Slinq())
                    .Concat(items.Slinq()
                        .OrderBy((i1, i2) => OrderBy(i1, i2, 1, 0))
                        .BufferWhere((i1, i2) => Abs(Odds(i1, i2, 1)) < 0.01f && SpriteEquals(i1, i2))
                        .Where(ls => ls.Count > 2 && ls.Slinq().First().interactive)
                        .SelectMany(ls => ls.Slinq()))
                    .Select(InitItem, config)
                    .Any();
            
                
                // Search if steps avaliable
                // Search vertical lines xoox
                var vSearch = items
                    .Slinq()
                    .OrderBy((i1, i2) => OrderBy(i1, i2, 0, 1))
                    .BufferWhere((i1, i2) => Abs(Odds(i1, i2, 0)) < 0.01f && SpriteEquals(i1, i2))
                    .Where(ls => ls.Count == 2 && ls.Slinq().First().interactive)
                    .Where(ls => IsStepAllowed(ls[0].sprite,
                                     ls[0].position + Vector2.down,
                                     Vector2.right,
                                     Vector2.down,
                                     Vector2.left) ||
                                 IsStepAllowed(ls[ls.Count - 1].sprite,
                                     ls[ls.Count - 1].position + Vector2.up,
                                     Vector2.left,
                                     Vector2.up,
                                     Vector2.right))
                    .SelectMany(ls => ls.Slinq());

                // Search horizontal lines xoox
                var hSearch = items
                    .Slinq()
                    .OrderBy((i1, i2) => OrderBy(i1, i2, 1, 0))
                    .BufferWhere((i1, i2) => Abs(Odds(i1, i2, 1)) < 0.01f && SpriteEquals(i1, i2))
                    .Where(collection => collection.Count == 2 && collection.Slinq().First().interactive)
                    .Where(ls => IsStepAllowed(ls[0].sprite,
                                     ls[0].position + Vector2.left,
                                     Vector2.left,
                                     Vector2.up,
                                     Vector2.down) ||
                                 IsStepAllowed(ls[ls.Count - 1].sprite,
                                     ls[ls.Count - 1].position + Vector2.right,
                                     Vector2.right,
                                     Vector2.up,
                                     Vector2.down))
                    .SelectMany(ls => ls.Slinq());
                
                // Search horizontal lines oxo
                var hSearchMP = items
                    .Slinq()
                    .OrderBy((i1, i2) => OrderBy(i1, i2, 1, 0))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.Y, Vector2.up, Vector2.down),
                        new List<Item>());

                // Search vertical lines oxo
                var vSearchMP = items
                    .Slinq()
                    .OrderBy((i1, i2) => OrderBy(i1, i2, 0, 1))
                    .Where((e, buffer) => IsAllowSpetMiddlePattern(e, buffer, Axis.X, Vector2.left, Vector2.right),
                        new List<Item>());

                vSearch
                    .Concat(hSearch)
                    .Concat(vSearchMP)
                    .Concat(hSearchMP)
                    .Any()
                    .ToSome()
                    .Where(v => v)
                    .ForEach(v => items
                        .SlinqWithIndex()
                        .Where(t => t.Item2 % 3 == 0)
                        .Select(t => t.Item1)
                        .ForEach((i, c) => InitItem(i, c), config)
                    )
                    .ForEach(v => oneMoreRun = true);

            return oneMoreRun;
        }

        private bool IsAllowSpetMiddlePattern(Item i, IList<Item> buffer, Axis axis, params Vector2[] offsets)
        {
            return Tuple.Create(buffer, i)
                .MatchTo<Smooth.Algebraics.Tuple<IList<Item>, Item>, Smooth.Algebraics.Tuple<IList<Item>, Item>>()
                .Where(_ =>  _.Item1.Count > 0 && Abs(_.Item1[_.Item1.Count - 1].position[(int)axis] - _.Item2.position[(int)axis])  > 0.01f)
                .Return(_ =>
                {
                    // next row||col
                    _.Item1.Clear();
                    return _;
                })
                .Else(_ => _)
                .Result()
                .MatchTo<Smooth.Algebraics.Tuple<IList<Item>, Item>, bool>()
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
                    return IsStepAllowed(
                        _.Item1[1].sprite,
                        _.Item1[0].position, offsets);
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

        private static int OrderBy(Item i1, Item i2,  params int[] index)
        {
            var result = 0.0f;
            foreach (var t in index)
            {
                result = Odds(i1, i2, t);
                if (Abs(result) > 0.01f) return (int)result;
            }
            return (int)result;
        }
        
        private static float Odds(Item i1, Item i2, int index)
        {
            if (i1 != null && i2 != null) return i1.position[index] - i2.position[index];
            if (i1 == null && i2 != null) return 1;
            if (i2 == null && i1 != null) return -1;
            return 0;
        }

        private bool IsStepAllowed(Sprite sprite, Vector2 pos, params Vector2[] offsets)
        {          
            return offsets
                .Slinq()
                .Select((offset, position) => position + offset, pos)
                .SelectMany((p, context) => context.GetEntitiesWithPosition(p).Slinq(), _context)
                .Any((e, t) => !t.Item1.Equals(e.position.value) && t.Item2.Equals(e.sprite.value), Tuple.Create(pos, sprite));
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
            return $"Item[{(int)position.x}, {(int)position.y}]";
        }
    }
}