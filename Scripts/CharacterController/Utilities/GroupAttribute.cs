using UnityEngine;
using System.Collections;

namespace CharacterController
{

    public class GroupAttribute : PropertyAttribute
    {
        public string Name { get; private set; }
        public bool Expanded { get; set; }

        public GroupAttribute(string name, bool expanded = true)
        {
            Name = name;
            Expanded = expanded;
        }
    }




}


