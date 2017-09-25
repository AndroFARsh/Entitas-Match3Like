using System;
using Smooth.Slinq;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    internal static class GameContextExtention
    {
        internal static GameEntity CreateItem(this GameContext context, float x, float y)
        {
            var entity = context.CreateEntity();
            entity.isItem = true;
            entity.ReplacePosition(new Vector2(x, y));

            return entity;
        }
        
        internal static float GetNextEmptyRow(this GameContext context, Vector2 position) {
            position.y -= 1;
            while(position.y >= 0 && context.GetEntitiesWithPosition(position)
                      .Slinq()
                      .Where(e => e.isReomve)
                      .ToList()
                      .Count == 0) {
                position.y -= 1;
            }

            return position.y + 1;
        }
    }
    
    public enum Axis {
        X=0, 
        Y=1
    }
    
    internal static class GameEntytyExtention
    {
        public static GameEntity InitBlock(this GameEntity entity, Sprite[] sptries)
        {
            return InitBlock(entity, sptries.GetRandomItem());
        }
        
        public static GameEntity InitBlock(this GameEntity entity, Sprite sptries)
        {
            entity.isMovable = true;
            entity.isInteractive = false;

            entity.ReplaceSprite(sptries);
            return entity;
        }

        public static GameEntity InitPiece(this GameEntity entity, Sprite[] sptries)
        {
            return InitPiece(entity, sptries.GetRandomItem());
        }
        
        public static GameEntity InitPiece(this GameEntity entity, Sprite sptries)
        {
            entity.isMovable = true;
            entity.isInteractive = true;

            entity.ReplaceSprite(sptries);
            return entity;
        }

        public static int OrderBy(this GameEntity e1, GameEntity e2,  params Axis[] axises)
        {
            var result = 0.0f;
            foreach (var axis in axises)
            {
                result = e1.position.value[(int)axis] - e2.position.value[(int)axis];
                if (Math.Abs(result) > 0.01f) return (int)result;
            }
            return (int)result;
        }
        
        public static float Odds(this GameEntity e1, GameEntity e2, Axis axis)
        {
            return e1.position.value[(int)axis] - e2.position.value[(int)axis];
        }
    }
}