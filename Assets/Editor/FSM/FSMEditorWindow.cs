
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


namespace GAIA.FSM.Windows
{
    public class FSMEditorWindow : EditorWindow
    {
        private FSMGraphView GraphView;

        public void Initialize(FSM fsm)
        {
            AddGraphView();
            AddStyles();
            GraphView.Load(fsm);
            titleContent.text = GraphView.fsm.transform.name + " FSM";
        }

        private void AddGraphView()
        {
            GraphView = new FSMGraphView();
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