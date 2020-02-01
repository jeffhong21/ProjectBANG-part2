using UnityEngine;
using System;

public static class Serialization
{


    public class GroupAttribute : PropertyAttribute
    {
        public string name;
        public bool groupAll;

        public GroupAttribute(string name, bool groupAll = false)
        {
            this.name = name;
            this.groupAll = groupAll;
        }
    }




}





