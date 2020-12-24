namespace ByteDance.Foundation
{
    using System;
    using System.Collections.Generic;

    /*
      * 
      * In C# 2, generating arbitrary sequences became much more convenient than it 
      * used to be in C# 1. Instead of implementing two classes, the IEnumerable<T> and 
      * the IEnumerator<T>, you can implement a single method that yields items using 
      * the iterator block syntax (i.e. the yield statements).
      * 
      * However, I still try to avoid creating a method just to generate a simple 
      * sequence, particularly if I use that sequence only in one place in my program. 
      * The Generate operator below accepts a delegate which generates the sequence 
      * element by element. To signal the end of the sequence, the generator returns 
      * null.
      * 
      * Since value types cannot be null, we need one overload for reference types, 
      * and another overload that uses a nullable wrapper to handle value types:
      * 
      * public static IEnumerable<T> Generate<T>(Func<T> generator)    where T : class
      * 
      * public static IEnumerable<T> Generate<T>(Func<Nullable<T>> generator)    where T : struct
      * 
      * To give a usage example, the ReadLinesFromConsole operator I mentioned above 
      * could be implemented as follows:
      * 
      * public static IEnumerable<string> ReadLinesFromConsole()
      * {
      *      return ExtendedEnumerable.Generate(() => Console.ReadLine());
      * }
      * 
      * As another example, this code sample generates an infinite sequence of random 
      * integers:
      * 
      * Random rand = new Random();var randomSeq = ExtendedEnumerable.Generate(() => (int?)rand.Next());
      * 
      *  This Generate operator has two disadvantages. First, it cannot be used to 
      *  generate sequences that contain null values, because null is the terminator 
      *  of the sequence. Second, it is a bit annoying to have to use the cast in the 
      *  value-type overload (see the cast to int? in the random-sequence example). 
      *  These are minor disadvantages, though, and I much prefer using the Generate 
      *  operator over implementing a new method each time I need to generate a simple 
      *  sequence.        
      */
    public static class ExtensionLinq
    {
        /// <summary>
        /// The Foreach.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="action">The action<see cref="System.Action{T}"/>.</param>
        public static void Foreach<T>(this IEnumerable<T> items, System.Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// The Aggregate.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="seed">The seed<see cref="T"/>.</param>
        /// <param name="accumulator">The accumulator<see cref="System.Func{T, T, T}"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public static T Aggregate<T>(this IEnumerable<T> items, T seed, System.Func<T, T, T> accumulator)
        {
            T value = seed;
            foreach (var item in items)
                value = accumulator(value, item);
            return value;
        }

        /// <summary>
        /// The All.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="func">The func<see cref="System.Func{T, bool}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool All<T>(this IEnumerable<T> items, System.Func<T, bool> func)
        {
            foreach (var item in items)
                if (!func(item))
                    return false;
            return true;
        }

        /// <summary>
        /// The Any.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="func">The func<see cref="System.Func{T, bool}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool Any<T>(this IEnumerable<T> items, System.Func<T, bool> func)
        {
            foreach (var item in items)
                if (func(item))
                    return true;
            return false;
        }

        /// <summary>
        /// The FirstOrDefault.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="func">The func<see cref="System.Func{T, bool}"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public static T FirstOrDefault<T>(this IEnumerable<T> items, System.Func<T, bool> func)
        {
            foreach (var item in items)
            {
                if (func != null)
                {
                    if (func(item))
                        return item;
                }
                else
                    return item;
            }

            return default(T);
        }

        /// <summary>
        /// The Find.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items<see cref="IEnumerable{T}"/>.</param>
        /// <param name="func">The func<see cref="System.Func{T, bool}"/>.</param>
        /// <returns>The <see cref="T"/>.</returns>
        public static T Find<T>(this IEnumerable<T> items, System.Func<T, bool> func)
        {
            foreach (var item in items)
                if (func(item))
                    return item;
            return default(T);
        }

        /// <summary>
        /// Executes the action in the middle of a query.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="source">.</param>
        /// <param name="action">.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T elem in source)
            {
                action(elem);
                yield return elem;
            }
        }

        /// <summary>
        /// Selects the specified function.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <typeparam name="Result">The type of the esult.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="func">The function.</param>
        /// <returns>.</returns>
        public static IEnumerable<Result> Select<T, Result>(this IEnumerable<T> items, System.Func<T, Result> func)
        {
            Stack<Result> results = new Stack<Result>();

            foreach (var item in items)
                results.Push(func(item));
            return results;
        }

        /// <summary>
        /// Distincts the specified items.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>.</returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> items)
        {
            Stack<T> result = new Stack<T>();
            foreach (var item in items)
                if (!result.Contains(item))
                    result.Push(item);
            return result;
        }

        /// <summary>
        /// Excepts the specified second.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="second">The second.</param>
        /// <returns>.</returns>
        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, IEnumerable<T> second)
        {
            Stack<T> results = new Stack<T>();
            HashSet<T> hashSet = second.ToHashSet();
            foreach (var item in items)
                if (!hashSet.Contains(item))
                    results.Push(item);
            return results;
        }

        /// <summary>
        /// Wheres the specified function.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="func">The function.</param>
        /// <returns>.</returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> items, System.Func<T, bool> func)
        {
            Stack<T> results = new Stack<T>();
            foreach (var item in items)
            {
                if (func(item))
                    results.Push(item);
            }
            return results;
        }

        /// <summary>
        /// Combines two sequences into one. The first element of sequence 1 and the
        /// first element of sequence 2 combine into a new element for the output
        /// sequence
        /// Ex. sumSeq = seq1.Combine(seq2, (a, b) => a + b);.
        /// </summary>
        /// <typeparam name="TIn1">.</typeparam>
        /// <typeparam name="TIn2">.</typeparam>
        /// <typeparam name="TOut">.</typeparam>
        /// <param name="in1">.</param>
        /// <param name="in2">.</param>
        /// <param name="func">.</param>
        /// <returns>.</returns>
        public static IEnumerable<TOut> Combine<TIn1, TIn2, TOut>(this IEnumerable<TIn1> in1, IEnumerable<TIn2> in2,
            Func<TIn1, TIn2, TOut> func)
        {
            if (in1 == null) throw new ArgumentNullException("in1");
            if (in2 == null) throw new ArgumentNullException("in2");
            if (func == null) throw new ArgumentNullException("func");

            using (var e1 = in1.GetEnumerator())
            {
                using (var e2 = in2.GetEnumerator())
                {
                    while (e1.MoveNext() && e2.MoveNext())
                    {
                        yield return func(e1.Current, e2.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Performs a Do While loop on <paramref name="action"/>, using <paramref name="compareFunc"/> to determine the results.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="source">.</param>
        /// <param name="action">assignment action.</param>
        /// <param name="compareFunc">compare function. when result is False, DoWhileAssignment will exit.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> DoWhile<T>(this IEnumerable<T> source, Action<T> action, Func<bool> compareFunc)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            // loop through each and perform the do while
            foreach (T elem in source)
            {
                do
                {
                    // perform action
                    action(elem);

                } while (compareFunc());

                // return result
                yield return elem;
            }
        }

        /// <summary>
        /// Converts to hashset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            HashSet<T> hashSet = new HashSet<T>();

            foreach (var item in items)
                hashSet.Add(item);
            return hashSet;
        }
    }
}
