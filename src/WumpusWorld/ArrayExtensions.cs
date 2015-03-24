using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WumpusWorld
{
  public static partial class ArrayExtensions
  {
    static object syncRoot = new object();

    static ArrayExtensions()
    {

    }

    public static T Read<T>(this T[] array, int index)
    {
      T retval = default(T);
      if (-1 < index && index < array.Length)
      {
        lock (syncRoot)
        {
          retval = array[index];
        }
      }
      return retval;
    }

    public static void Write<T>(this T[] array, int index, T value)
    {
      if (-1 < index && index < array.Length)
      {
        lock (syncRoot)
        {
          array[index] = value;
        }
      }
    }

    public static int BinarySearch<T>(this T[] array, T value)
    {
      return Array.BinarySearch(array, value);
    }

    public static int BinarySearch<T>(this T[] array, T value, IComparer<T> comparer)
    {
      return Array.BinarySearch(array, value, comparer);
    }

    public static int BinarySearch<T>(this T[] array, int index, int length, T value)
    {
      return Array.BinarySearch(array, index, length, value);
    }

    public static int BinarySearch<T>(this T[] array, int index, int length, T value, IComparer<T> comparer)
    {
      return Array.BinarySearch(array, index, length, value, comparer);
    }

    public static void Clear<T>(this T[] array, int index, int length)
    {
      Array.Clear(array, index, length);
    }

    public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
    {
      return Array.ConvertAll(array, converter);
    }

    public static bool Exists<T>(this T[] array, Predicate<T> match)
    {
      return Array.Exists(array, match);
    }

    public static T Find<T>(this T[] array, Predicate<T> match)
    {
      return Array.Find(array, match);
    }

    public static T[] FindAll<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindAll(array, match);
    }

    public static int FindIndex<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindIndex(array, match);
    }

    public static int FindIndex<T>(this T[] array, int startIndex, Predicate<T> match)
    {
      return Array.FindIndex(array, startIndex, match);
    }

    public static int FindIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match)
    {
      return Array.FindIndex(array, startIndex, count, match);
    }

    public static T FindLast<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindLast(array, match);
    }

    public static int FindLastIndex<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindLastIndex(array, match);
    }

    public static int FindLastIndex<T>(this T[] array, int startIndex, Predicate<T> match)
    {
      return Array.FindLastIndex(array, startIndex, match);
    }

    public static int FindLastIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match)
    {
      return Array.FindLastIndex(array, startIndex, count, match);
    }

    public static void ForEach<T>(this T[] array, Action<T> action)
    {
      Array.ForEach(array, action);
    }

    public static int IndexOf<T>(this T[] array, T value)
    {
      return Array.IndexOf(array, value);
    }

    public static int IndexOf<T>(this T[] array, T value, int startIndex)
    {
      return Array.IndexOf(array, value, startIndex);
    }

    public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
      return Array.IndexOf(array, value, startIndex, count);
    }

    public static int LastIndexOf<T>(this T[] array, T value)
    {
      return Array.LastIndexOf(array, value);
    }

    public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
    {
      return Array.LastIndexOf(array, value, startIndex);
    }

    public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
      return Array.LastIndexOf(array, value, startIndex, count);
    }

    public static void Sort<T>(this T[] array)
    {
      Array.Sort(array);
    }

    public static void Sort<T>(this T[] array, Comparison<T> comparison)
    {
      Array.Sort(array, comparison);
    }

    public static void Sort<T>(this T[] array, IComparer<T> comparer)
    {
      Array.Sort(array, comparer);
    }

    public static void Sort<T>(this T[] array, int index, int length)
    {
      Array.Sort(array, index, length);
    }

    public static void Sort<T>(this T[] array, int index, int length, IComparer<T> comparer)
    {
      Array.Sort(array, index, length, comparer);
    }

    public static T[,] To2D<T>(this T[] src, int sizeX, int sizeY)
    {
      int x, y;
      T[,] retval = new T[sizeX, sizeY];
      for (x = 0; x < sizeX; ++x)
        for (y = 0; y < sizeY; ++y)
          retval[x, y] = src[x + y * sizeX];
      return retval;
    }

    public static T[] To1D<T>(this T[,] src, int sizeX, int sizeY)
    {
      int x, y;
      T[] retval = new T[sizeX * sizeY];
      for (x = 0; x < sizeX; ++x)
        for (y = 0; y < sizeY; ++y)
          retval[x + y * sizeX] = src[x, y];
      return retval;
    }
  }
}
