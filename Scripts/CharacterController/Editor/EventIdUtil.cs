//using UnityEngine;
//using UnityEditor;
//using System;
//using System.Reflection;
//using System.Linq;

//namespace CharacterController
//{
//    public static class EventIdUtil
//    {



//        public static string[] GetAllEventIDs()
//        {

//            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
//            Type t = typeof(EventIDs);
//            FieldInfo[] fields = t.GetFields(bindingFlags);
//            string[] eventIDs = new string[fields.Length];
//            for (int i = 0; i < fields.Length; i++) {
//                eventIDs[i] = fields[i].GetValue(null).ToString();
//            }

//            return eventIDs;
//        }


//    }

//}
