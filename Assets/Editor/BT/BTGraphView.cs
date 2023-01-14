using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using GAIA.AI.Enumerations;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;


namespace GAIA.AI.Windows
{
    using Elements;
    
    public class BTGraphView : GraphView
    {
        public BTGraphView()
        {
            AddManipulators();
            AddGridBackground();

            AddStyles();
        }

        public void GenerateXML(string name)
        {
            StreamWriter stream = new StreamWriter(Path.Combine(Application.dataPath, name + ".xml"));
            XmlWriterSettings sts = new XmlWriterSettings() { Indent = true };
            XmlWriter writer = XmlWriter.Create(stream, sts);
            
            writer.WriteStartDocument();
            writer.WriteStartElement("BT");
            writer.WriteElementString("BTid", name);
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

        private BTNode CreateNode(BTNodeType nodeType, Vector2 position)
        {
            BTNode node = new BTNode();

            node.Initialize(nodeType, position);
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
                    menuEvent.menu.AppendAction("Generate", actionEvent => GenerateXML("Test"));
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

