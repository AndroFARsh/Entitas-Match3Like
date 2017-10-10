using System;
using UnityEngine;

namespace Game
{
    public struct IntVector2
    {
        public static IntVector2 Zero { get; } = new IntVector2(0, 0);
        public static IntVector2 One { get; } = new IntVector2(1, 1);
        public static IntVector2 Up { get; } = new IntVector2(0, 1);
        public static IntVector2 Down { get; } = new IntVector2(0, -1);
        public static IntVector2 Right { get; } = new IntVector2(1, 0);
        public static IntVector2 Left { get; } = new IntVector2(-1, 0);

        private readonly int[] _value;

        public int X => this[0];
        
        public int Y => this[1];

        public int this[Axis index] => _value[(int) index];
        
        public int this[int index] => _value[index];

        public float Magnitude => (float) Math.Sqrt(X * X + Y * Y);

        public int SqrMagnitude => X * X + Y * Y;
        
        public IntVector2(int x, int y)  
        {
            _value = new[] {x, y};
        }

        public override bool Equals(object obj) => obj is IntVector2 && X == ((IntVector2)obj).X && Y == ((IntVector2)obj).Y;

        public override int GetHashCode() => X << 8 | Y;

        public override string ToString() => $"[{X}, {Y}]";
        
        public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.X + b.X, a.Y + b.Y);

        public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.X - b.X, a.Y - b.Y);

        public static IntVector2 operator -(IntVector2 a) => new IntVector2(-a.X, -a.Y);

        public static IntVector2 operator *(IntVector2 a, int d) => new IntVector2(a.X * d, a.Y * d);

        public static IntVector2 operator *(int d, IntVector2 a) =>  new IntVector2(a.X * d, a.Y * d);

        public static bool operator ==(IntVector2 lhs, IntVector2 rhs) => (lhs - rhs).SqrMagnitude == 0;
        
        public static bool operator !=(IntVector2 lhs, IntVector2 rhs) => !(lhs == rhs);
        
        // Cast operators
//        public static implicit operator Vector2(IntVector2 a) => new Vector2(a.X, a.Y);
//        
//        public static implicit operator Vector3(IntVector2 a) => new Vector3(a.X, a.Y, 0);        
//     
//        public static implicit operator IntVector2(Vector2 a) => new IntVector2((int)a.x, (int)a.y);
//        
//        public static implicit operator IntVector2(Vector3 a) => new IntVector2((int)a.x, (int)a.y);
    }
}