using GAIA.BT.Data.Save;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace GAIA.BT.Windows
{
    public class BTEditorWindow : EditorWindow
    {
        private BTGraphView GraphView;

        public void Initialize(BTGraphSO graphSO)
        {
            AddGraphView();
            AddStyles();
            GraphView.LoadSO(graphSO);
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