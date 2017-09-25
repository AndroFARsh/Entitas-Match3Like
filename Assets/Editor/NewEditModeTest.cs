using System;
using NUnit.Framework;
using System.Collections.Generic;
using Smooth.Slinq;
using UnityEngine;


public class NewEditModeTest
{
    [Test]
    public void WhereBufferTest()
    {
        var array = new [] {2, 2, 1, 4, 4, 4, 4, 5, 5, 5, 5, 5, 3, 3, 3};
        for (var it = 0; it < 1000000; it++)
        {
            array
                .Slinq()
                .OrderBy((i1, i2) => i1 - i2)
                .BufferWhere((i1, i2) => i1 == i2)
                .Concat(array
                    .Slinq()
                    .OrderBy((i1, i2) => i2 - i1)
                    .BufferWhere((i1, i2) => i1 == i2))
                .ForEach(i =>
                {
                    Assert.IsNotEmpty(i);
                    Assert.AreEqual(i.Count, i[0], $"items type:{i[0]} count:{i.Count}");
                });

            array
                .Slinq()
                .OrderBy((i1, i2) => i1 - i2)
                .BufferWhere((i1, i2) => i1 == i2)
                .Concat(array
                    .Slinq()
                    .OrderBy((i1, i2) => i2 - i1)
                    .BufferWhere((i1, i2) => i1 == i2))
                .ForEach(i =>
                {
                    Assert.IsNotEmpty(i);
                    Assert.AreEqual(i.Count, i[0], $"items type:{i[0]} count:{i.Count}");
                });
        }
    }

//    [Test]
//    public void NewEditModeTestSimplePasses()
//    {
//        var matrix = new List<TestItem>();
//        for (var i = 0; i < 100; i++)
//        {
//            for (var j = 0; j < 10; j++)
//            {
//                matrix.Add(new TestItem(j, i));
//            }
//        }
//
//        var start = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
//        for (var i = 0; i < 1000; ++i)
//        {  
//            matrix.Slinq()
//                .OrderBy(OrderByXY)
//                .BufferWhere((prev, cur) => prev.x == cur.x)
//                .SelectMany(ls => ls.Slinq())
//                .Concat(matrix.Slinq()
//                    .OrderBy(OrderByYX)
//                    .BufferWhere((prev, cur) => prev.y == cur.y)
//                    .SelectMany(ls => ls.Slinq())
//                )
//                .ForEach(e =>
//                {
//                    var res = e.x + e.y;
//                });
//            
//            matrix.Slinq()
//                .OrderBy(OrderByXY)
//                //   .BufferWhere((prev, cur) => prev.x == cur.x)
////                .SelectMany(ls => ls.Slinq())
//                .Concat(matrix.Slinq()
//                        .OrderBy(OrderByYx)
//                    .BufferWhere((prev, cur) => prev.y == cur.y)
//                    .SelectMany(ls => ls.Slinq())
//                )
//                .ForEach(e =>
//                {
//                    var res = e.x + e.y;
//                });
//              
//            matrix.Slinq()
//                .OrderBy(OrderByYx)
////                .BufferWhere((prev, cur) => prev.y == cur.y)
////                .SelectMany(ls => ls.Slinq())
//                .Concat(
//                    matrix.Slinq()
//                        .OrderBy(OrderByXY)
////                        .BufferWhere((prev, cur) => prev.x == cur.x)
////                        .SelectMany(ls => ls.Slinq())
//                )
//                .ForEach(e =>
//                {
//                    var res = e.x + e.y;
//                });
//        }
//        var end = DateTimeOffset.Now.Ticks / TimeSpan.TicksPerMillisecond;
//        Debug.Log($"duration: {end - start}");
//    }

    static int OrderByXY(TestItem i1, TestItem i2)
    {
        var res = i1.x - i2.x;
        if (res == 0)
        {
            res = i1.y - i2.y;
        }
        return res;
    }

    int OrderByYX(TestItem i1, TestItem i2)
    {
        var res = i1.y - i2.y;
        if (res == 0)
        {
            res = i1.x - i2.x;
        }
        return res;
    }
}

internal class TestItem
{
    internal int x;
    internal int y;

    internal TestItem(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}