using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using GAIA.FSM.Data;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;


namespace GAIA.FSM.Windows
{
    using Elements;
    
    public class FSMGraphView : GraphView
    {
        public EditorWindow Window;
        public FSM fsm;
        
        public FSMGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
        }
        
        private void Save()
        {
            FSMNodeDH root = null;
            Dictionary<string, FSMNodeDH> instantiatedNodes = new Dictionary<string, FSMNodeDH>();

            foreach (FSMNode node in nodes)
            {
                FSMNodeDH nodeDH = new FSMNodeDH();

                nodeDH.OnEnter = node.OnEnter;
                nodeDH.OnExit = node.OnExit;
                nodeDH.Action = node.Action;
                
                nodeDH.name = node.name;
                nodeDH.Id = node.Id;
                nodeDH.position = node.GetPosition().position;
                
                instantiatedNodes[node.Id] = nodeDH;
                if (node.IsRoot) { root = nodeDH; }
            }
            
            foreach (FSMEdge edge in edges)
            {
                FSMEdgeDH edgeDH = new FSMEdgeDH();
                edgeDH.Target = instantiatedNodes[((FSMNode) edge.input.node).Id];
                edgeDH.Event = edge.Event;
                
                instantiatedNodes[((FSMNode) edge.output.node).Id].Edges.Add(edgeDH);
            }

            fsm.root = root;
        }
        
        public void Load(FSM graph)
        {
            fsm = graph;
            Dictionary<string, FSMNode> instantiatedNodes = new Dictionary<string, FSMNode>();

            HashSet<FSMNodeDH> visited = new HashSet<FSMNodeDH>();
            Stack<FSMNodeDH> stack = new Stack<FSMNodeDH>();
            stack.Push(fsm.root);
            while (stack.Count > 0)
            {
                FSMNodeDH nodeDH = stack.Pop();
                visited.Add(nodeDH);

                foreach (FSMEdgeDH edge in nodeDH.Edges)
                {
                    if (!visited.Contains(edge.Target))
                    {
                        stack.Push(edge.Target);
                    }
                }

                FSMNode node = CreateNode(nodeDH.position, nodeDH.name, nodeDH == fsm.root);
                node.OnEnter = nodeDH.OnEnter;
                node.OnExit = nodeDH.OnExit;
                node.Action = nodeDH.Action;
                
                instantiatedNodes[nodeDH.Id] = node;
                AddElement(node);
            }
            
            foreach (FSMNodeDH node in visited)
            {
                foreach (FSMEdgeDH edge in node.Edges)
                {
                    FSMEdge Edge = instantiatedNodes[node.Id].OutputPort.ConnectTo<FSMEdge>(instantiatedNodes[edge.Target.Id].InputPort);
                    AddElement(Edge);
                }
            }
        }
        
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port => {
                if (startPort == port) { return; }
                if (startPort.node == port.node) { return; }
                if (startPort.direction == port.direction) { return; }
                compatiblePorts.Add(port);
            });
            
            return compatiblePorts;
        }

        private FSMNode CreateNode(Vector2 position, string nodeName = null, bool isRoot = false)
        {
            FSMNode node = new FSMNode();

            node.Initialize(position, nodeName, isRoot);
            node.Draw();

            return node;
        }

        private void AddManipulators()
        {
            // TODO: Force use FSMEdge instead of Edge
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            
            this.AddManipulator(CreateNodeContextualMenu());
        }

        private IManipulator CreateNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => {
                    menuEvent.menu.AppendAction("Add Node", actionEvent => AddElement(CreateNode(actionEvent.eventInfo.localMousePosition)));
                    menuEvent.menu.AppendAction("Save", actionEvent => Save());
                });

            return contextualMenuManipulator;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
        
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load("BehaviorTrees/BTGraphViewStyles.uss");

            styleSheets.Add(styleSheet);
        }
    }
}

