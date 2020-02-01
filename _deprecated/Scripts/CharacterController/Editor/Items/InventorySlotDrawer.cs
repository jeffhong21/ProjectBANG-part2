//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace CharacterController
//{
//    [CustomPropertyDrawer(typeof(InventorySlot))]
//    public class InventorySlotDrawer : PropertyDrawer
//    {
//        private SerializedProperty item;
//        private SerializedProperty quantity;
//        private SerializedProperty isActive;

//        private Rect itemRect = new Rect();
//        private Rect quantityRect = new Rect();

  

//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            //base.OnGUI(position, property, label);
//            EditorGUI.BeginProperty(position, label, property);

//            Rect rect = position;
//            itemRect.Set(rect.x, rect.y, rect.width, rect.height);
//            itemRect.x += rect.width * 0.4f;
//            itemRect.width = rect.width * 0.36f;

//            quantityRect.Set(rect.x, rect.y, rect.width, rect.height);
//            quantityRect.x = rect.width - 36;
//            quantityRect.width = 36;


//            item = property.FindPropertyRelative("item");
//            quantity = property.FindPropertyRelative("quantity");
//            isActive = property.FindPropertyRelative("isActive");

//            EditorGUI.ObjectField(itemRect, item.objectReferenceValue, typeof(Item), false);
//            EditorGUI.IntField(quantityRect, quantity.intValue);


//            EditorGUI.EndProperty();
//        }



//        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        //{
//        //    return base.GetPropertyHeight(property, label);
//        //}

//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            return Screen.width < 333 ? (16f + 18f) : 16f;
//        }
//    }

//}
