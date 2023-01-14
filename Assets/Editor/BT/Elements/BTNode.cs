using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Linq;


namespace GAIA.AI.Elements
{
    using Enumerations;
    
    public class BTNode : Node
    {
        public string Id { get; set; }
        public string NodeName { get; set; }
        public bool IsRoot { get; set; }
        public BTNodeType NodeType { get; set; }

        public Port inputPort;
        public Port outputPort;
        
        public void Initialize(BTNodeType nodeType, Vector2 position)
        {
            Id = Guid.NewGuid().ToString();
            NodeType = nodeType;
            switch (NodeType)
            {
                case BTNodeType.Tree:
                    NodeName = "Tree";
                    break;
                case BTNodeType.Sequence:
                    NodeName = "Sequence";
                    break;
                case BTNodeType.Fallback:
                    NodeName = "Fallback";
                    break;
                case BTNodeType.Action:
                    NodeName = "Action";
                    break;
            }

            SetPosition(new Rect(position, Vector2.zero));
        }
        
        public void Draw()
        {
            title = NodeName;

            if (NodeType == BTNodeType.Action || NodeType == BTNodeType.Tree)
            {
                TextField title = new TextField() { value = NodeName + " name" };
                title.RegisterCallback((ChangeEvent<string> evt) =>
                {
                    NodeName = evt.newValue;
                });
                mainContainer.Insert(1, title);
            }
            
            if (NodeType == BTNodeType.Tree)
            {
                Toggle root = new Toggle() { value = IsRoot, label = "root"};
                root.RegisterCallback((ChangeEvent<bool> evt) =>
                {
                    IsRoot = evt.newValue;
                });
                mainContainer.Insert(2, root);
            }

            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input,
                NodeType == BTNodeType.Tree ? Port.Capacity.Multi : Port.Capacity.Single, typeof(bool));
            inputPort.portName = "In";
            inputContainer.Add(inputPort);
            
            if (NodeType != BTNodeType.Action)
            {
                outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output,
                    NodeType == BTNodeType.Tree ? Port.Capacity.Single : Port.Capacity.Multi, typeof(bool));
                outputPort.portName = "Out";
                outputContainer.Add(outputPort);
            }
        }

        public IEnumerable<BTNode> GetChildren()
        {
            return outputPort.connections.Select(edge => (BTNode) edge.input.node);
        }
        
        public IEnumerable<BTNode> GetOrderedChildren() // TODO: Modificar si se va a hacer vertical
        {
            return GetChildren().OrderBy(n => n.GetPosition().y);
        }
    }
}
