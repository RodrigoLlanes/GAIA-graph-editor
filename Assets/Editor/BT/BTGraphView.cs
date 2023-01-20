using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;


namespace GAIA.BT.Windows
{
    using Elements;
    using Data.Save;
    using Enumerations;
    
    public class BTGraphView : GraphView
    {
        public EditorWindow Window;
        public BTGraphSO graphSO;
        
        public BTGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        public void ExportToXML()
        {
            string path = AssetDatabase.GetAssetPath(graphSO);
            path = path.Substring(0, path.Length - Path.GetFileName(path).Length);
            path = EditorUtility.SaveFilePanel("Export Behavior Tree as XML", path, graphSO.GetName(), "xml");
            
            StreamWriter stream = new StreamWriter(path + ".xml");
            XmlWriterSettings sts = new XmlWriterSettings() { Indent = true };
            XmlWriter writer = XmlWriter.Create(stream, sts);
            
            writer.WriteStartDocument();
            writer.WriteStartElement("BT");
            writer.WriteElementString("BTid", graphSO.GetName());
            writer.WriteStartElement("Bt");
            writer.WriteStartElement("Trees");
            
            foreach (Node n in nodes)
            {
                BTNode node = (BTNode) n;
                if (node.NodeType == BTNodeType.Tree)
                {
                    GenerateXML(node, writer, true);
                }
            }
            
            writer.WriteEndElement(); // Trees
            writer.WriteEndElement(); // Bt
            writer.WriteEndElement(); // BT
            writer.Close();
        }

        private void GenerateXML(BTNode node, XmlWriter writer, bool treeRoot = false)
        {
            switch (node.NodeType)
            {
                case BTNodeType.Tree:
                    if (treeRoot)
                    {
                        writer.WriteStartElement("Tree");
                        writer.WriteAttributeString("Root", node.IsRoot ? "YES" : "NO");
                        writer.WriteElementString("Name", node.NodeName);
                        writer.WriteStartElement("Child_Nodes");

                        if (node.GetChildren().Count() == 1)
                        {
                            GenerateXML(node.GetChildren().First(), writer);
                        }
                    
                        writer.WriteEndElement(); // Child_Nodes
                        writer.WriteEndElement(); // Tree
                    }
                    else
                    {
                        writer.WriteStartElement("CN");
                        writer.WriteElementString("CN_Type", "tree");
                        writer.WriteElementString("CN_Name", node.NodeName);
                        writer.WriteEndElement(); // CN
                    }
                    break;
                case BTNodeType.Sequence:
                    writer.WriteStartElement("CN");
                    writer.WriteElementString("CN_Type", "node");
                    writer.WriteElementString("CN_Name", "sequence");
                    writer.WriteStartElement("Child_Nodes");

                    foreach (BTNode child in node.GetOrderedChildren()) {
                        GenerateXML(child, writer);
                    }
                    
                    writer.WriteEndElement(); // Child_Nodes
                    writer.WriteEndElement(); // CN
                    break;
                case BTNodeType.Fallback:
                    writer.WriteStartElement("CN");
                    writer.WriteElementString("CN_Type", "node");
                    writer.WriteElementString("CN_Name", "fallback");
                    writer.WriteStartElement("Child_Nodes");

                    foreach (BTNode child in node.GetOrderedChildren()) {
                        GenerateXML(child, writer);
                    }
                    
                    writer.WriteEndElement(); // Child_Nodes
                    writer.WriteEndElement(); // CN
                    break;
                case BTNodeType.Action:
                    writer.WriteStartElement("CN");
                    writer.WriteElementString("CN_Type", "node");
                    writer.WriteElementString("CN_Name", node.NodeName);
                    writer.WriteEndElement(); // CN
                    break;
            }
        }

        private void SaveAs(string path)
        {
            graphSO = ScriptableObject.CreateInstance<BTGraphSO>();
            AssetDatabase.CreateAsset(graphSO, path);
            AssetDatabase.SaveAssets();

            bool rootFinded = false;
            foreach (BTNode node in nodes)
            {
                if (node.IsRoot)
                {
                    if (rootFinded)
                    {
                        Debug.LogError("Solo el arbol principal debe ser marcado como root.");
                    }

                    rootFinded = true;
                }
                BTNodeSO nodeSo = ScriptableObject.CreateInstance<BTNodeSO>();
                nodeSo.Initialize(node.Id, node.GetPosition().position, node.NodeName, node.IsRoot, node.NodeType);
                foreach (BTNode child in node.GetChildren())
                {
                    nodeSo.ConnectedTo.Add(child.Id);
                }

                graphSO.Nodes.Add(nodeSo);
                AssetDatabase.AddObjectToAsset(nodeSo, graphSO);
                AssetDatabase.SaveAssets();
            }

            if (!rootFinded)
            {
                Debug.LogError("Al menos un arbol debe ser marcado como root.");
            }
        }

        private void SaveAs()
        {
            string path = EditorUtility.SaveFilePanelInProject("Save Behavior Tree", "BehaviorTree", "asset", "Please enter a file name to save the behavior tree to");
            if (path.Length == 0)
            {
                return;
            }
            SaveAs(path);
            Window.titleContent.text = graphSO.GetName();
        }

        private void Save()
        {
            string path = AssetDatabase.GetAssetPath(graphSO);
            SaveAs(path);
        }
        
        public void LoadSO(BTGraphSO graph)
        {
            graphSO = graph;
            Dictionary<string, BTNode> instantiatedNodes = new Dictionary<string, BTNode>();

            foreach (BTNodeSO nodeSo in graphSO.Nodes)
            {
                BTNode node = CreateNode(nodeSo.NodeType, nodeSo.Position, nodeSo.Name, nodeSo.Root);
                AddElement(node);
                instantiatedNodes[nodeSo.Id] = node;
            }
            
            foreach (BTNodeSO nodeSo in graphSO.Nodes)
            {
                foreach (string id in nodeSo.ConnectedTo)
                {
                    Edge edge = instantiatedNodes[nodeSo.Id].outputPort.ConnectTo(instantiatedNodes[id].inputPort);
                    AddElement(edge);
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

        private BTNode CreateNode(BTNodeType nodeType, Vector2 position, string nodeName = null, bool isRoot = false)
        {
            BTNode node = new BTNode();

            node.Initialize(nodeType, position, nodeName, isRoot);
            node.Draw();

            return node;
        }

        private void AddManipulators()
        {
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
                    menuEvent.menu.AppendAction("Add Tree", actionEvent => AddElement(CreateNode(BTNodeType.Tree, actionEvent.eventInfo.localMousePosition)));
                    menuEvent.menu.AppendAction("Add Sequence", actionEvent => AddElement(CreateNode(BTNodeType.Sequence, actionEvent.eventInfo.localMousePosition)));
                    menuEvent.menu.AppendAction("Add Fallback", actionEvent => AddElement(CreateNode(BTNodeType.Fallback, actionEvent.eventInfo.localMousePosition)));
                    menuEvent.menu.AppendAction("Add Action", actionEvent => AddElement(CreateNode(BTNodeType.Action, actionEvent.eventInfo.localMousePosition)));
                    menuEvent.menu.AppendAction("Export to XML", actionEvent => ExportToXML());
                    menuEvent.menu.AppendAction("Save", actionEvent => Save());
                    menuEvent.menu.AppendAction("Save As", actionEvent => SaveAs());
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

