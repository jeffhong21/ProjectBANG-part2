namespace CharacterController
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class EventHandler
    {


        private static Dictionary<object, Dictionary<string, List<object>>> m_RegisterdEvents =
            new Dictionary<object, Dictionary<string, List<object>>>();



        public static void LogAllRegistered()
        {
            foreach (var obj in m_RegisterdEvents)
            {
                foreach (var item in obj.Value)
                {
                    Debug.LogFormat("object: {0} | eventName {1}", obj.Key, item.Key);
                }
            }
        }



        #region Register Events

        public static void RegisterEvent(object obj, string eventName, Action handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                //  object is registered.  Now check if it is registered to event name.
                if (!m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    //  object is registered, but event is not registered to object.
                    m_RegisterdEvents[obj].Add(eventName, new List<object>());
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
                else
                {
                    //  object is registered and event is registered, add the handler.
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
            }
            //  object is not registered.
            else
            {
                var callback = new Dictionary<string, List<object>>();  //  Initialize callback dictionary.
                callback.Add(eventName, new List<object>());            //  Add defaults to dictionary.
                callback[eventName].Add(handler);
                m_RegisterdEvents.Add(obj, callback);
            }
        }

        public static void RegisterEvent<T>(object obj, string eventName, Action<T> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                //  object is registered.  Now check if it is registered to event name.
                if (!m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    //  object is registered, but event is not registered to object.
                    m_RegisterdEvents[obj].Add(eventName, new List<object>());
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
                else
                {
                    //  object is registered and event is registered, add the handler.
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
            }
            //  object is not registered.
            else
            {
                var callback = new Dictionary<string, List<object>>();  //  Initialize callback dictionary.
                callback.Add(eventName, new List<object>());            //  Add defaults to dictionary.
                callback[eventName].Add(handler);
                m_RegisterdEvents.Add(obj, callback);
            }
        }


        public static void RegisterEvent<T, U>(object obj, string eventName, Action<T, U> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                //  object is registered.  Now check if it is registered to event name.
                if (!m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    //  object is registered, but event is not registered to object.
                    m_RegisterdEvents[obj].Add(eventName, new List<object>());
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
                else
                {
                    //  object is registered and event is registered, add the handler.
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
            }
            //  object is not registered.
            else
            {
                var callback = new Dictionary<string, List<object>>();  //  Initialize callback dictionary.
                callback.Add(eventName, new List<object>());            //  Add defaults to dictionary.
                callback[eventName].Add(handler);
                m_RegisterdEvents.Add(obj, callback);
            }
            //Debug.LogFormat("Registering {0} from {1}", obj.ToString(), eventName);
        }


        public static void RegisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                //  object is registered.  Now check if it is registered to event name.
                if (!m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    //  object is registered, but event is not registered to object.
                    m_RegisterdEvents[obj].Add(eventName, new List<object>());
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
                else
                {
                    //  object is registered and event is registered, add the handler.
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
            }
            //  object is not registered.
            else
            {
                var callback = new Dictionary<string, List<object>>();  //  Initialize callback dictionary.
                callback.Add(eventName, new List<object>());            //  Add defaults to dictionary.
                callback[eventName].Add(handler);
                m_RegisterdEvents.Add(obj, callback);
            }
            //Debug.LogFormat("Registering {0} from {1}", obj.ToString(), eventName);
        }


        public static void RegisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                //  object is registered.  Now check if it is registered to event name.
                if (!m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    //  object is registered, but event is not registered to object.
                    m_RegisterdEvents[obj].Add(eventName, new List<object>());
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
                else
                {
                    //  object is registered and event is registered, add the handler.
                    m_RegisterdEvents[obj][eventName].Add(handler);
                }
            }
            //  object is not registered.
            else
            {
                var callback = new Dictionary<string, List<object>>();  //  Initialize callback dictionary.
                callback.Add(eventName, new List<object>());            //  Add defaults to dictionary.
                callback[eventName].Add(handler);
                m_RegisterdEvents.Add(obj, callback);
            }
            //Debug.LogFormat("Registering {0} from {1}", obj.ToString(), eventName);
        }

        #endregion



        #region Execute Events


        public static void ExecuteEvent(object obj, string eventName)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action callback = (Action)m_RegisterdEvents[obj][eventName][i];
                        callback();
                    }
                }
            }
        }

        public static void ExecuteEvent<T>(object obj, string eventName, T arg1)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T> callback = (Action<T>)m_RegisterdEvents[obj][eventName][i];
                        callback(arg1);
                    }

                }
            }
        }

        public static void ExecuteEvent<T, U>(object obj, string eventName, T arg1, U arg2)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U> callback = (Action<T, U>)m_RegisterdEvents[obj][eventName][i];
                        callback(arg1, arg2);
                    }

                }
            }
        }

        public static void ExecuteEvent<T, U, V>(object obj, string eventName, T arg1, U arg2, V arg3)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U, V> callback = (Action<T, U, V>)m_RegisterdEvents[obj][eventName][i];
                        callback(arg1, arg2, arg3);
                    }

                }

            }
        }


        public static void ExecuteEvent<T, U, V, W>(object obj, string eventName, T arg1, U arg2, V arg3, W arg4)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U, V, W> callback = (Action<T, U, V, W>)m_RegisterdEvents[obj][eventName][i];
                        callback(arg1, arg2, arg3, arg4);
                        //Debug.LogFormat("Executing {1} for {0}", obj.ToString(), eventName);
                    }

                }
            }
        }


        #endregion



        #region Unregister Events

        public static void UnregisterEvent(object obj, string eventName, Action handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action callback = (Action)m_RegisterdEvents[obj][eventName][i];
                        if (callback == handler)
                        {
                            m_RegisterdEvents[obj][eventName].Remove(handler);
                            //Debug.LogFormat("Unregistering {0} ({1}) from {2}", eventName, handler.ToString(), obj.ToString());
                        }
                    }
                }
            }
        }

        public static void UnregisterEvent<T>(object obj, string eventName, Action<T> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T> callback = (Action<T>)m_RegisterdEvents[obj][eventName][i];
                        if (callback == handler)
                        {
                            m_RegisterdEvents[obj][eventName].Remove(handler);
                            //Debug.LogFormat("Unregistering {0} ({1}) from {2}", eventName, handler.ToString(), obj.ToString());
                        }
                    }
                }
            }
        }


        public static void UnregisterEvent<T, U>(object obj, string eventName, Action<T, U> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U> callback = (Action<T, U>)m_RegisterdEvents[obj][eventName][i];
                        if (callback == handler)
                        {
                            m_RegisterdEvents[obj][eventName].Remove(handler);
                            //Debug.LogFormat("Unregistering {0} ({1}) from {2}", eventName, handler.ToString(), obj.ToString());
                        }
                    }
                }
            }
        }


        public static void UnregisterEvent<T, U, V>(object obj, string eventName, Action<T, U, V> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U, V> callback = (Action<T, U, V>)m_RegisterdEvents[obj][eventName][i];
                        if (callback == handler)
                        {
                            m_RegisterdEvents[obj][eventName].Remove(handler);
                            //Debug.LogFormat("Unregistering {0} ({1}) from {2}", eventName, handler.ToString(), obj.ToString());
                        }
                    }
                }
            }
        }


        public static void UnregisterEvent<T, U, V, W>(object obj, string eventName, Action<T, U, V, W> handler)
        {
            if (m_RegisterdEvents.ContainsKey(obj))
            {
                if (m_RegisterdEvents[obj].ContainsKey(eventName))
                {
                    for (int i = 0; i < m_RegisterdEvents[obj][eventName].Count; i++)
                    {
                        Action<T, U, V, W> callback = (Action<T, U, V, W>)m_RegisterdEvents[obj][eventName][i];
                        if (callback == handler)
                        {
                            m_RegisterdEvents[obj][eventName].Remove(handler);
                            //Debug.LogFormat("Unregistering {0} ({1}) from {2}", eventName, handler.ToString(), obj.ToString());
                        }
                    }
                }
            }
        }


        #endregion







    }



}
