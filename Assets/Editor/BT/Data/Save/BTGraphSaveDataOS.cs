using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GAIA.AI.Data.Save
{
    public class BTGraphSaveDataOS : ScriptableObject
    {
        public string FileName { get; set; }
        public List<BTNodeSaveData> Nodes { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;
            Nodes = new List<BTNodeSaveData>();
        }
    }
}
