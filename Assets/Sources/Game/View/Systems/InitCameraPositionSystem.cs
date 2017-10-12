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

namespace Game.View.Systems
{
    public class InitCameraPositionSystem : ReactiveSystem<GameEntity>
    {
        public InitCameraPositionSystem(Contexts contexts) : base(contexts.game)
        {
        }

        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Board);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.hasBoard;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            entities
                .Slinq()
                .Select(e => e.board.value.Size)
                .Select(size => new Vector3
                    {
                        x = (size.Columns - 1) * 0.5f,
                        y = (size.Rows - 1) * 0.5f,
                        z = Camera.main.transform.position.z
                    }
                )
                .ForEach(pos => Camera.main.transform.position = pos);
        }
    }
}