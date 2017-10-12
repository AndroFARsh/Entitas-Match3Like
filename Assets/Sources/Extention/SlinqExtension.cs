using System;
using System.Collections.Generic;
using Smooth.Algebraics;
using Smooth.Delegates;
using Smooth.Pools;
using Smooth.Slinq.Context;

namespace Smooth.Slinq
{
    public static class SlinqExtension
    {
        public static Slinq<List<T>, BufferPredicateContext<T, C>> BufferWhere<T, C>(this Slinq<T, C> slinq,
            DelegateFunc<T, T, bool> predicate)
        {
            return BufferPredicateContext<T, C>.BufferWhere(slinq, predicate);
        }
    }
}

namespace Smooth.Slinq.Context
{
    #region No parameter

    public struct BufferPredicateContext<T, C>
    {
        #region Slinqs

        public static Slinq<List<T>, BufferPredicateContext<T, C>> BufferWhere(Slinq<T, C> slinq,
            DelegateFunc<T, T, bool> predicate)
        {
            return new Slinq<List<T>, BufferPredicateContext<T, C>>(
                skip,
                remove,
                dispose,
                new BufferPredicateContext<T, C>(slinq, predicate));
        }

        #endregion

        #region Context

        private bool needsMove;
        private Slinq<T, C> chained;
        private readonly DelegateFunc<T, T, bool> predicate;
        private readonly List<List<T>> release;
        private List<T> acc;

        #pragma warning disable 0414
        private BacktrackDetector bd;
        #pragma warning restore 0414

        private BufferPredicateContext(Slinq<T, C> slinq, DelegateFunc<T, T, bool> pred)
        {
            needsMove = false;
            chained = slinq;
            predicate = pred;
            acc = null;
            release = ListPool<List<T>>.Instance.Borrow();
            bd = BacktrackDetector.Borrow();
        }

        #endregion

        #region BufferWhere

        private static readonly Mutator<List<T>, BufferPredicateContext<T, C>> skip = Skip;
        private static readonly Mutator<List<T>, BufferPredicateContext<T, C>> remove = Remove;

        private static void Skip(ref BufferPredicateContext<T, C> context, out Option<List<T>> next)
        {
            context.bd.DetectBacktrack();
            if (context.acc == null)
            {
                var acc = ListPool<T>.Instance.Borrow();
                context.acc = acc;
                context.release.Add(acc);
            }

            while (context.chained.current.isSome && (context.acc.Count == 0 ||
                                                      context.predicate(context.acc[context.acc.Count - 1],
                                                          context.chained.current.value)))
            {
                context.acc.Add(context.chained.current.value);
                context.chained.skip(ref context.chained.context, out context.chained.current);
            }

            next = context.acc.Count > 0 ? context.acc.ToOption() : Option<List<T>>.None;
            context.acc = null;

            if (!next.isSome)
            {
                context.release.Slinq().ForEach(ListPool<T>.Instance.Release);
                ListPool<List<T>>.Instance.Release(context.release);
                context.bd.Release();
            }
        }

        private static readonly Mutator<List<T>, BufferPredicateContext<T, C>> dispose = Dispose;

        private static void Remove(ref BufferPredicateContext<T, C> context, out Option<List<T>> next)
        {
            throw new NotSupportedException();
        }

        private static void Dispose(ref BufferPredicateContext<T, C> context, out Option<List<T>> next)
        {
            next = new Option<List<T>>();

            context.release.Slinq().ForEach(ListPool<T>.Instance.Release);
            ListPool<List<T>>.Instance.Release(context.release);

            context.bd.Release();
        }

        #endregion
    }

    #endregion
}