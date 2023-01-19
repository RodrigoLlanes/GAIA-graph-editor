using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GAIA.BT.Elements;
using GAIA.BT.Enumerations;
using GAIA.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


namespace GAIA.FSM.Elements
{
    public class FSMNode: Node
    {
        // TODO: Customizar la clase para permitir genericidad
        //       y poder poner returntype void en estos
        public List<SerializableCallback> OnEnter = new List<SerializableCallback>();
        public List<SerializableCallback> OnExit = new List<SerializableCallback>();
        public List<SerializableCallback> Action = new List<SerializableCallback>();

        public bool IsRoot;
        public string NodeTitle;
        
        public string Id { get; set; }
        public Port InputPort;
        public Port OutputPort;
        
        
        public void Initialize(Vector2 position, string nodeTitle = null, bool isRoot = false)
        {
            Id = Guid.NewGuid().ToString();
            IsRoot = isRoot;
            NodeTitle = nodeTitle ?? "Node Name";
            SetPosition(new Rect(position, Vector2.zero));
        }
        
        public void Draw()
        {
            title = NodeTitle;
            InputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            InputPort.portName = "In";
            inputContainer.Add(InputPort);
            
            OutputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            OutputPort.portName = "Out";
            outputContainer.Add(OutputPort);
        }

        public IEnumerable<FSMNode> GetChildren()
        {
            return OutputPort.connections.Select(edge => (FSMNode) edge.input.node);
        }
        
        public IEnumerable<FSMNode> GetOrderedChildren() // TODO: Modificar si se va a hacer vertical
        {
            return GetChildren().OrderBy(n => n.GetPosition().y);
        }
    }
}
