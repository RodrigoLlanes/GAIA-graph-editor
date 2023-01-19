using System.Collections.Generic;
using System.Numerics;
using GAIA.Utils;
using Vector2 = UnityEngine.Vector2;

namespace GAIA.FSM.Data
{
    public class FSMNodeDH
    {
        public List<SerializableCallback> OnEnter = new List<SerializableCallback>();
        public List<SerializableCallback> OnExit = new List<SerializableCallback>();
        public List<SerializableCallback> Action = new List<SerializableCallback>();
        public List<FSMEdgeDH> Edges = new List<FSMEdgeDH>();

        public string name;
        public string Id;
        public Vector2 position;
    }
}