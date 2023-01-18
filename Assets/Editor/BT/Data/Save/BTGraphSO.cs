using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace GAIA.BT.Data.Save
{
    using Windows;
    
    [CreateAssetMenuAttribute(fileName = "BehaviorTree", menuName = "GAIA/Behavior Tree")]
    public class BTGraphSO : ScriptableObject
    {
        [field: SerializeField] public List<BTNodeSO> Nodes  = new List<BTNodeSO>();

        [OnOpenAssetAttribute(1)]
        public static bool OpenBTGraphWindow(int instanceID, int line)
        {
            string path = AssetDatabase.GetAssetPath(instanceID);
            BTGraphSO graph = AssetDatabase.LoadAssetAtPath<BTGraphSO>(path);

            if (graph is null) { return false; }
            
            BTEditorWindow window = EditorWindow.CreateWindow<BTEditorWindow>(path);
            window.Focus();
            window.Initialize(graph);
            
            return true;
        }

        public string GetName()
        {
            string path = AssetDatabase.GetAssetPath(this);
            return Path.GetFileNameWithoutExtension(path);
        }
    }
}
