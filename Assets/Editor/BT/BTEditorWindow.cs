using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace GAIA.AI.Windows
{
    public class BTEditorWindow : EditorWindow
    {
        [MenuItem("Window/GAIA/AI/Behavior Tree")]
        public static void Open()
        {
            BTEditorWindow wnd = GetWindow<BTEditorWindow>("Behavior Tree");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView()
        {
            BTGraphView graphView = new BTGraphView();

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
        
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("BehaviorTrees/BTVariables.uss");

            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}