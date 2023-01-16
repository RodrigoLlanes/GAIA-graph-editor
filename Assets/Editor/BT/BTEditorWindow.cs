using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace GAIA.AI.Windows
{
    public class BTEditorWindow : EditorWindow
    {
        private BTGraphView GraphView;

        public void Initialize(string path)
        {
            AddGraphView();
            AddStyles();
            GraphView.LoadSO(path);
            titleContent.text = GraphView.graphSO.GetName();
        }

        private void AddGraphView()
        {
            GraphView = new BTGraphView();
            GraphView.StretchToParentSize();
            GraphView.Window = this;
            rootVisualElement.Add(GraphView);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("BehaviorTrees/BTVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}