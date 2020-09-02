////*************************Copyright © 2008 Feng 豐**************************	
// Created    : 10/16/2009 2:09:34 PM 
// Description: CollectionExtension.cs  
// Revisions  :            		
// **************************************************************************** 
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Support.Net.Util
{
    public static class CollectionExtension
    {
        public static bool Exists<T>(this IEnumerable<T> collection, Predicate<T> func)
        {
            foreach (var item in collection)
            {
                if (func(item)) return true;
            }
            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
        public static T Find<T>(this IEnumerable<T> collection, Predicate<T> func)
        {
            foreach (var item in collection)
            {
                if(func(item))    return item; 
            }
            return default(T);
        }
        public static List<T> FindAll<T>(this IEnumerable<T> collection, Predicate<T> func)
        {            
            List<T> list = new List<T>();
            foreach (var item in collection)
            {
                if (func(item))
                    list.Add(item);
            }
            return list;
        }
        public static bool Contains<T>(this IEnumerable<T> collection,IEnumerable<T> targetCollection)
        {
            foreach (var item in targetCollection)
            {
                if (!collection.Contains(item))
                    return false;
            }
            return true;
        }     
        public static int RemoveAll<T>(this IList<T> collection, Predicate<T> match)
        {
            int ct = 0;
            foreach (var item in collection)
            {
                if (match(item))
                {
                    if (collection.Remove(item)) ct++;
                }
            }
            return ct;
        }
    }
}
