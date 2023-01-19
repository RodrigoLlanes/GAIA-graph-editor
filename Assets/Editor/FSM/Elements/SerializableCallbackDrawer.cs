using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace GAIA.Utils.Editor
{
    [CustomPropertyDrawer(typeof(SerializableCallback))]
    public class ModularJointDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20f * 2;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 90;
            rect.height /= 2;
            
            SerializedProperty goProperty = property.FindPropertyRelative("go");
            SerializedProperty scriptProperty = property.FindPropertyRelative("script");
            SerializedProperty methodNameProperty = property.FindPropertyRelative("methodName");
            
            
            GameObject go = (GameObject) EditorGUI.ObjectField(rect, "Game Object", goProperty.objectReferenceValue, typeof(GameObject), true);
            goProperty.objectReferenceValue = go;
            
            if (go == null)
            {
                scriptProperty.objectReferenceValue = null;
                methodNameProperty.stringValue = "";
                return; 
            }
            
            rect.y += rect.height;

            string className = null;
            if (scriptProperty.objectReferenceValue != null)
            {
                className = ((MonoBehaviour) scriptProperty.objectReferenceValue).GetType().FullName;
            }
            
            int selected = -1;
            List<string> methodNames = new List<string>();
            List<MethodInfo> methodInfos = new List<MethodInfo>();
            List<MonoBehaviour> methodComponents = new List<MonoBehaviour>();
            foreach (MonoBehaviour script in go.GetComponents<MonoBehaviour>())
            {
                MethodInfo[] infos = script.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (MethodInfo info in infos)
                {
                    if (info.ReturnType == typeof(bool) && info.GetParameters().Length == 0)
                    {
                        if (info.Name == methodNameProperty.stringValue && script.GetType().FullName == className)
                        {
                            selected = methodNames.Count;
                        }
                        methodNames.Add("(" + info.DeclaringType.Name + ") " + info.Name);
                        methodInfos.Add(info);
                        methodComponents.Add(script);
                    }
                }
            }
            selected = EditorGUI.Popup(rect, "Method", selected, methodNames.ToArray());
            if (selected != -1)
            {
                MethodInfo methodInfo = methodInfos[selected];
                
                scriptProperty.objectReferenceValue = methodComponents[selected];
                methodNameProperty.stringValue = methodInfo.Name;
            }
            else
            {
                scriptProperty.objectReferenceValue = null;
                methodNameProperty.stringValue = "";
            }
        }
    }
}