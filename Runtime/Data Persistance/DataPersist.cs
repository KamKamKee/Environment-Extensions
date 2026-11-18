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
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using EExtensions.ListExpressions;
using UnityEngine;


namespace EExtensions
{
    namespace DataPersistance
    {
        public class SimplePath
        {
            public string[] pathComponents { get; private set; }

            public SimplePath(string path)
            {
                path = path.Replace("\\", "/");
                pathComponents = path.Split("/");
                CleanupParts();
            }

            public SimplePath(string[] pathParts)
            {
                pathComponents = pathParts;
                CleanupParts();
            }

            public SimplePath(List<string> pathParts)
            {
                pathComponents = pathParts.ToArray();
                CleanupParts();
            }

            public override string ToString()
            {
                return pathComponents.ToList().Join("/");
            }

            public string Last()
            {
                return this[-1];
            }

            private void CleanupParts()
            {
                List<string> editableParts = pathComponents.ToList();

                editableParts.Map(element => element.Replace("\\", "").Replace("/", ""));
                editableParts.Filter(element => !string.IsNullOrEmpty(element));

                pathComponents = editableParts.ToArray();
            }

            public string this[int index]
            {
                get { if (index == -1) return pathComponents.Last(); return pathComponents[index]; }
            }

            public static string Fix(string path)
            {
                return new SimplePath(path).ToString();
            }

            public static implicit operator SimplePath(string path)
            {
                return new SimplePath(path);
            }

            public static SimplePath operator +(SimplePath pathLeft, SimplePath pathRight)
            {
                List<string> pathComponents = pathLeft.pathComponents.ToList();
                pathComponents.AddRange(pathRight.pathComponents);
                return new SimplePath(pathComponents);
            }

            public static SimplePath operator +(SimplePath path, string other)
            {
                List<string> pathComponents = path.pathComponents.ToList();
                pathComponents.Add(other);
                return new SimplePath(pathComponents);
            }

            public static SimplePath operator -(SimplePath path, int amount)
            {
                if (amount < 0)
                    return path;

                if (amount >= path.pathComponents.Length)
                    return new SimplePath("");

                List<string> pathComponents = path.pathComponents.ToList();
                pathComponents.GetRange(0, pathComponents.Count - amount);
                pathComponents.RemoveAt(pathComponents.Count - 1);
                return new SimplePath(pathComponents);
            }

            public static SimplePath operator --(SimplePath path)
            {
                if (path.pathComponents.Length == 0)
                    return path;

                List<string> pathComponents = path.pathComponents.ToList();
                pathComponents.RemoveAt(pathComponents.Count - 1);
                return new SimplePath(pathComponents);
            }
        }

        public static class DataPersistance
        {
            public static void SaveClass<T>(T classToSave, SimplePath path)
            {
                BinaryFormatter converter = new BinaryFormatter();

                FileStream stream = new FileStream(path.ToString(), FileMode.OpenOrCreate, FileAccess.Write);

                converter.Serialize(stream, classToSave);

                stream.Close();
            }

            public static void SaveClassToJson<T>(T classToSave, SimplePath path)
            {
                string data = JsonUtility.ToJson(classToSave);

                SaveJson(data, path);
            }

            public static void SaveJson(string data, SimplePath path)
            {
                BinaryFormatter converter = new BinaryFormatter();

                FileStream stream = new FileStream(path.ToString(), FileMode.OpenOrCreate, FileAccess.Write);

                converter.Serialize(stream, data);

                stream.Close();
            }

            public static T LoadClass<T>(SimplePath path)
            {
                BinaryFormatter converter = new BinaryFormatter();

                FileStream stream = new FileStream(path.ToString(), FileMode.Open, FileAccess.Read);

                T data = (T)converter.Deserialize(stream);

                stream.Close();

                return data;
            }

            public static T LoadClassFromJson<T>(SimplePath path)
            {
                string data = LoadJson(path);

                return JsonUtility.FromJson<T>(data);
            }

            public static string LoadJson(SimplePath path)
            {
                BinaryFormatter converter = new BinaryFormatter();

                FileStream stream = new FileStream(path.ToString(), FileMode.Open, FileAccess.Read);

                string data = converter.Deserialize(stream) as string;

                stream.Close();

                return data;
            }
        }

        /// <summary>
        /// References a file and stores/retrieves data with a given type from that file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class CustomPersistence<T>
        {
            public T data { get; private set; }

            public SimplePath path { get; private set; }

            public CustomPersistence(SimplePath path, T defaultConfig)
            {
                this.path = path;

                if (!DataFileExists())
                {
                    Debug.Log($"File at path doesn't Exits: '{path}'");
                    Debug.Log($"Creating...");
                    data = defaultConfig;
                    Directory.CreateDirectory((this.path - 1).ToString());
                    DataPersistance.SaveClassToJson(data, path);
                    Debug.Log($"Created path and file");
                }

                LoadData();
            }

            /// <summary>
            /// Provides the current data as a parameter.
            /// Edit the values of e.g. your class inside the function block.
            /// After executing your function block the current data will be persisted in the file.
            /// </summary>
            /// <param name="updateFunctionNoReturn"></param>
            public void Update(Action<T> updateFunctionNoReturn)
            {
                updateFunctionNoReturn(data);
                Persist();
            }

            /// <summary>
            /// Provides the current data as a parameter.
            /// Return the new value of the data.
            /// After executing your function block and setting the data, it will be persisted in the file.
            /// </summary>
            /// <param name="updateFunction"></param>
            public void Update(Func<T, T> updateFunction)
            {
                data = updateFunction(data);
                Persist();
            }

            private void Persist()
            {
                DataPersistance.SaveClassToJson(data, path);
            }

            private void LoadData()
            {
                data = DataPersistance.LoadClassFromJson<T>(path);
            }

            /// <summary>
            /// Returns true if the target directory exists.
            /// </summary>
            /// <returns></returns>
            public bool ParentFolderExists()
            {
                return Directory.Exists((path - 1).ToString());
            }

            /// <summary>
            /// Returns true if the target file exists.
            /// </summary>
            /// <returns></returns>
            public bool DataFileExists()
            {
                return File.Exists(path.ToString());
            }
        }

        /*public class PreferencePersistence
        {
            private class Preference
            {
                public string key;
                public object value;

                public Preference(string key, object value)
                {
                    this.key = key;
                    this.value = value;
                }

                public override string ToString()
                {
                    return $"{key}: {value}";
                }
            }

            private class PreferenceCollection
            {
                // TODO Here replace this by two lists of key and value. May work tbh ORRR maybe the problem is just the object. that i cant store objects on json idk
                public List<Preference> collection;
                public List<int> something;

                public PreferenceCollection()
                {
                    this.collection = new();
                    something = new() { 1, 2, 3 };
                }

                public PreferenceCollection(List<Preference> collection)
                {
                    this.collection = collection;
                    something = new() { 1, 2, 3 };
                }

                public object this[string key]
                {
                    get
                    {
                        object foundValue = null;

                        foreach (Preference pref in collection)
                        {
                            if (pref.key == key)
                            {
                                foundValue = pref.value;
                                break;
                            }
                        }

                        return foundValue;
                    }

                    set
                    {
                        bool foundKey = false;

                        foreach (Preference pref in collection)
                        {
                            if (pref.key == key)
                            {
                                pref.value = value;
                                foundKey = true;
                                break;
                            }
                        }

                        if (!foundKey)
                        {
                            collection.Add(new Preference(key, value));
                        }
                    }
                }

                public bool Pop(string key)
                {
                    foreach (Preference pref in collection)
                    {
                        if (pref.key == key)
                        {
                            collection.Remove(pref);
                            return true;
                        }
                    }

                    return false;
                }

                public static implicit operator PreferenceCollection(List<Preference> collection)
                {
                    return new PreferenceCollection(collection);
                }
            }

            public Path path { get; private set; }
            private PreferenceCollection cachedCollection;

            public PreferencePersistence(Path path)
            {
                this.path = path;

                if (!DataFileExists())
                {
                    Debug.Log("Doesn't Exits");
                    cachedCollection = new();
                    Directory.CreateDirectory((this.path - 1).ToString());
                    DataPersistance.SaveClass(cachedCollection, path);
                }

                LoadData();
            }

            public T Get<T>(string name, T defaultValue)
            {
                object valueFound = cachedCollection["name"];

                if (valueFound != null)
                    return (T)valueFound;

                else
                    return defaultValue;
            }

            public void Persist<T>(string name, T valueToPersist)
            {
                cachedCollection[name] = valueToPersist;

                Debug.LogWarning(cachedCollection.collection.Printable());

                DataPersistance.SaveClass(cachedCollection, path);
            }

            private void LoadData()
            {
                cachedCollection = DataPersistance.LoadClass<PreferenceCollection>(path);
            }

            public bool ParentFolderExists()
            {
                return Directory.Exists((path - 1).ToString());
            }

            public bool DataFileExists()
            {
                return File.Exists(path.ToString());
            }
        }*/

        /*public class PersistedData<T>
        {
            public Path path { get; private set; }
            public bool useJson { get; private set; }

            public PersistedData(Path path, T defaultValue, bool useJson = true)
            {
                this.path = path;
                this.useJson = useJson;

                if (!DataFileExists())
                {
                    Debug.Log("Doesn't Exits");
                    Directory.CreateDirectory((this.path - 1).ToString());
                    Persist(defaultValue);
                }
            }

            public T Get()
            {
                if (useJson)
                    return DataPersistance.LoadClassFromJson<T>(path);

                else
                    return DataPersistance.LoadClass<T>(path);
            }

            public void Persist(T newData)
            {
                if (useJson)
                    DataPersistance.SaveClassToJson(newData, path);

                else
                    DataPersistance.SaveClass(newData, path);
            }

            public bool ParentFolderExists()
            {
                return Directory.Exists((path - 1).ToString());
            }

            public bool DataFileExists()
            {
                return File.Exists(path.ToString());
            }
        }*/
    }
}