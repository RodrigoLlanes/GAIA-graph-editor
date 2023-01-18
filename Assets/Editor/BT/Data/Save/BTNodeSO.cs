using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GAIA.BT.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class BTNodeSO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public List<string> ConnectedTo { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public bool Root { get; set; }
        [field: SerializeField] public BTNodeType NodeType { get; set; }

        public void Initialize(string id, Vector2 position, string name, bool root, BTNodeType nodeType)
        {
            Id = id;
            Position = position;
            Name = name;
            NodeType = nodeType;
            ConnectedTo = new List<string>();
            Root = root;
        }
    }
}
