using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace GAIA.Utils
{
    
    [Serializable]
    public class SerializableCallback
    {
        public GameObject go;
        public MonoBehaviour script;
        public string methodName = "";

        public bool Call()
        {
            MethodInfo methodInfo = script.GetType().GetMethod(methodName);
            return (bool) methodInfo.Invoke(script, null);
        }
    }
}