using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GAIA.AI.Data.Save
{
    using Enumerations;
    
    [Serializable]
    public class BTNodeSaveData 
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public List<string> ConnectedTo { get; set; }
        [field: SerializeField] public Vector2 Position { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public BTNodeType NodeType { get; set; } 
    }
}
