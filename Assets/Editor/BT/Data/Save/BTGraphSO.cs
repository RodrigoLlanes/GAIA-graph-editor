using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace GAIA.AI.Data.Save
{
    using Windows;
    
    [CreateAssetMenuAttribute(fileName = "BehaviorTree", menuName = "GAIA/AI/Behavior Tree")]
    public class BTGraphSO : ScriptableObject
    {
        [field: SerializeField] public List<BTNodeSO> Nodes  = new List<BTNodeSO>();

        [OnOpenAssetAttribute(1)]
        public static bool OpenBTGraphWindow(int instanceID, int line)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            
            BTEditorWindow window = EditorWindow.CreateWindow<BTEditorWindow>(path);
            window.Focus();
            window.Initialize(path);
            
            return true;
        }

        public string GetName()
        {
            string path = AssetDatabase.GetAssetPath(this);
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
