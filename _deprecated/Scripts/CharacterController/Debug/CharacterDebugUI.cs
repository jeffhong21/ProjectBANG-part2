//namespace CharacterController
//{
//    using UnityEngine;
//    using UnityEngine.UI;
//    using UnityEngine.EventSystems;
//    using System.Collections.Generic;
//    using System.Text;


//    public class CharacterDebugUI : MonoBehaviour
//    {
//        private static CharacterDebugUI _instance;
//        public static CharacterDebugUI Instance
//        {
//            get; private set;
//        }
//        //public static CharacterDebugUI Instance
//        //{
//        //    get
//        //    {
//        //        if(_instance == null){
//        //            var prefab = Resources.Load<CharacterDebugUI>("CharacterDebugUI");
//        //            _instance = Instantiate(prefab);
//        //        }
//        //        return _instance;
//        //    }
//        //    private set
//        //    {
//        //        _instance = value;
//        //    }
//        //}


//        [System.Serializable]
//        public struct RectTransformInfo
//        {
//            public string name;
//            public Rect rect;
//            public Vector2 anchorMin;
//            public Vector2 anchorMax;
//            public Vector2 pivot;

//            public RectTransformInfo(string rectName)
//            {
//                this.name = rectName;
//                this.rect = new Rect();
//                this.anchorMin = Vector2.zero;
//                this.anchorMax = Vector2.zero;
//                this.pivot = Vector2.zero;
//            }

//            public RectTransformInfo(Rect rect)
//            {
//                this.name = "UI Text";
//                this.rect = rect;
//                this.anchorMin = Vector2.zero;
//                this.anchorMax = Vector2.zero;
//                this.pivot = Vector2.zero;
//            }
//        }



//        private Canvas canvas;
//        //private RigidbodyCharacterControllerBase rigidCharController;
//        [SerializeField]
//        private Text charProperties;
//        [SerializeField]
//        private Text charActions;

//        [SerializeField]
//        private RectTransformInfo propertiesRectInfo = new RectTransformInfo("PropertiesLog")
//        {
//            rect = new Rect(-936, -56, 400, 752),
//            anchorMin = new Vector2(0.5f, 0.5f),
//            anchorMax = new Vector2(0.5f, 0.5f),
//            pivot = new Vector2(0, 0.5f)
//        };

//        [SerializeField]
//        private RectTransformInfo actionsRectInfo = new RectTransformInfo("ActionsLog")
//        {
//            rect = new Rect(24, 36, 400, 860),
//            anchorMin = new Vector2(0, 0),
//            anchorMax = new Vector2(0, 1),
//            pivot = new Vector2(0, 0.5f)
//        };


//        private void Awake()
//        {
//            if (Instance != null && Instance != this){
//                Debug.LogWarningFormat("Another instance of {0} has already been registered for this scene, destroying this one", this);
//                Destroy(this.gameObject);
//            }
//            Instance = this;

//            if (FindObjectOfType<EventSystem>() == null)
//            {
//                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
//            }

//            canvas = GetComponent<Canvas>();
//        }


//        private void Start()
//        {
//            //charProperties = CreateUITextObject(propertiesRectInfo, propertiesRectInfo.name);

//        }



//        private Text CreateUITextObject(RectTransformInfo rectTransform, string objectName)
//        {
//            Text textObject = new GameObject(objectName, typeof(Text)).GetComponent<Text>();
//            textObject.rectTransform.parent = this.transform;
//            //textObject.rectTransform.position = rectTransform.rect.position;
//            textObject.rectTransform.anchorMin = rectTransform.anchorMin;
//            textObject.rectTransform.anchorMax = rectTransform.anchorMax;
//            textObject.rectTransform.pivot = rectTransform.pivot;
//            textObject.rectTransform.anchoredPosition = rectTransform.rect.position;
//            textObject.rectTransform.sizeDelta = rectTransform.rect.size;


//            textObject.color = Color.white;
//            textObject.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
//            textObject.fontStyle = FontStyle.Bold;
//            textObject.fontSize = 18;
//            textObject.supportRichText = true;
//            textObject.horizontalOverflow = HorizontalWrapMode.Overflow;
//            textObject.verticalOverflow = VerticalWrapMode.Truncate;
//            textObject.resizeTextForBestFit = true;
//            textObject.resizeTextMinSize = 10;
//            textObject.resizeTextMaxSize = 28;

//            return textObject;
//        }



//        private void LateUpdate()
//        {
//            if(CharacterDebug.ActiveCharacter != null)
//            {
//                if (CharacterDebug.ActiveCharacter.Debugger.debugMode)
//                {
//                    if (charProperties != null)
//                    {
//                        charProperties.text = CharacterDebug.Write();
//                    }
//                }

//            }
//            else
//            {
//                charProperties.text = "No active character.";
//            }


//        }
//    }




//    public class CharacterDebug
//    {
//        private static StringBuilder PropertiesLog = new StringBuilder(500);

//        public static RigidbodyCharacterControllerBase ActiveCharacter;

//        public static Dictionary<string, string> DebugProperties = new Dictionary<string, string>();



//        public static void Log(string property, string value)
//        {
//            if (DebugProperties.ContainsKey(property))
//            {
//                DebugProperties[property] = value;
//            }
//            else
//            {
//                DebugProperties.Add(property, value);
//            }
//        }

//        public static void Log<T>(string property, T value)
//        {
//            Log(property, value.ToString());
//        }


//        public static void Remove(string property)
//        {
//            if (DebugProperties.ContainsKey(property))
//            {
//                DebugProperties.Remove(property);
//            }
//        }


//        public static string Write()
//        {
//            if(ActiveCharacter == null)
//            {
//                return "<i>No active character.</i>";
//            }
//            //string message = string.Format("<b>{0} charProperties</b><\n>", ActiveCharacter.name);
//            PropertiesLog.Clear();

//            PropertiesLog.AppendFormat("<b>** {0} Properties **</b>\n", ActiveCharacter.name);

//            foreach (var property in DebugProperties)
//            {
//                //message += string.Format("<b>{0}:</b> {1}<\n>",property.Key, property.Value );
//                PropertiesLog.AppendFormat("<b> {0}:</b> {1}\n", property.Key, property.Value);

//            }


//            return PropertiesLog.ToString();
//        }

//    }
//}

