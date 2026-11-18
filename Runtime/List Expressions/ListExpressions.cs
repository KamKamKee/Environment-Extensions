/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System;
using System.Collections.Generic;

namespace EExtensions
{
    namespace ListExpressions
    {
        public static class ListExpressions
        {
            /// <summary>
            /// Joins every element of a list by connecting it with the inbetween string. Returns the joined list as a string.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="inbetween"></param>
            /// <returns></returns>
            public static string Join<T>(this List<T> list, string inbetween)
            {
                string result = "";

                list.ForEach(item =>
                {
                    if (item != null)
                        result += item.ToString() + inbetween;

                    else
                        result += "[NULL]" + inbetween;
                });

                if (result.Length > 0)
                {
                    result = result.Substring(0, result.Length - inbetween.Length);
                }

                return result;
            }

            /// <summary>
            /// Turns the list into a printable string format.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static string Printable<T>(this List<T> list)
            {
                if (list.Count == 0)
                    return "List { EMPTY }";
                else
                    return $"List {{ {list.Join(", ")} }}";
            }

            /// <summary>
            /// Turns the list into a printable string format and lables it with a name.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            public static string Printable<T>(this List<T> list, string displayName)
            {
                if (list.Count == 0)
                    return $"List {displayName} {{ EMPTY }}";
                else
                    return $"List {displayName} {{ {list.Join(", ")} }}";
            }

            /// <summary>
            /// Iterates through the list and executes a function for every element. 
            /// Provides access to break and continue. Return lower strings to use. "break", "continue" and "" at the end of the function aswell.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static List<T> ForEachR<T>(this List<T> list, Func<T, string> function)
            {
                foreach(T item in list)
                {
                    string result = function(item);

                    if (result == "break")
                        break;
                }

                return list;
            }

            /// <summary>
            /// Iterates through every element and chooses whether it should stay or get removed according to the function.
            /// The function takes in the element's value and should return true if the element should stay and false if not.
            /// Does modify the initial list.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static List<T> Filter<T>(this List<T> list, Func<T, bool> function)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (!function(list[i]))
                        list.RemoveAt(i);
                }

                return list;
            }

            /// <summary>
            /// /// Iterates through every element and chooses whether it should stay or get removed according to the function.
            /// The function takes in the element's value and should return true if the element should stay and false if not.
            /// Does not modify the initial list.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static List<T> Filtered<T>(this List<T> list, Func<T, bool> function)
            {
                List<T> filteredList = new List<T>(list);

                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (!function(filteredList[i]))
                        filteredList.RemoveAt(i);
                }

                return filteredList;
            }

            /// <summary>
            /// Iterates through every element and modifies it according to the function.
            /// The function takes in the element's value and should return the modified value.
            /// Does modify the initial list.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static List<T> Map<T>(this List<T> list, Func<T, T> function)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = function(list[i]);
                }

                return list;
            }

            /// <summary>
            /// Iterates through every element and modiefies it according to the function. 
            /// The function takes in the element's value and should return the modified value. 
            /// Does not modify the initial list.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="func"></param>
            /// <returns></returns>
            public static List<T> Mapped<T>(this List<T> list, Func<T, T> function)
            {
                List<T> mappedList = new List<T>(list);

                for (int i = 0; i < mappedList.Count; i++)
                {
                    mappedList[i] = function(mappedList[i]);
                }

                return mappedList;
            }

            /// <summary>
            /// Iterates through every element and transforms it according to the function. 
            /// The function takes in the element's value and should return the new transformed value of a different type. 
            /// Does not modify the initial list.
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="list"></param>
            /// <param name="transformationFunction"></param>
            /// <returns></returns>
            public static List<T2> SwitchMapped<T1, T2>(this List<T1> list, Func<T1, T2> transformationFunction)
            {
                List<T2> switchedList = new();

                list.ForEach(i =>
                {
                    switchedList.Add(transformationFunction(i));
                });

                return switchedList;
            }

            /// <summary>
            /// Taps from a list. The tap function has the list as a parameter.
            /// This is especially useful for additional callbacks during multiple modifications of a list (map, filter...) or debugging.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="tapFunction"></param>
            /// <returns></returns>
            public static List<T> Tap<T>(this List<T> list, Action<List<T>> tapFunction)
            {
                tapFunction.Invoke(list);
                return list;
            }
        }
    }
}
