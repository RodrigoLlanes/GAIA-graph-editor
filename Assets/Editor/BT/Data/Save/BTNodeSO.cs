using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.TreeGrouper;
using UnityEngine;

namespace GAIA.AI.Data.Save
{
    using Enumerations;
    
    public class BTNodeSO : ScriptableObject 
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public BTNodeType NodeType { get; set; }

        public void Initialize(string name, BTNodeType type)
        {
            Name = name;
            NodeType = type;
        }
    }
}
