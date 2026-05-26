using System;
using System.Collections.Generic;

namespace KanbanBoardSystem.App.Infrastructure
{
    public static class GenericAlgorithms
    {
       
        public static void ForEach<T>(IEnumerable<T> source, Action<T> action)
        {
            if (source == null || action == null) return;
            foreach (var item in source)
            {
                action(item);
            }
        }

       
        public static IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null || selector == null) yield break;
            foreach (var item in source)
            {
                yield return selector(item);
            }
        }

        
        public static TAggregate Reduce<TSource, TAggregate>(IEnumerable<TSource> source, TAggregate seed, Func<TAggregate, TSource, TAggregate> accumulator)
        {
            if (source == null || accumulator == null) return seed;
            TAggregate result = seed;
            foreach (var item in source)
            {
                result = accumulator(result, item);
            }
            return result;
        }
    }
}