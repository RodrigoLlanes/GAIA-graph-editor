using GAIA.Utils;
using UnityEditor.Experimental.GraphView;

namespace GAIA.FSM.Elements
{
    public class FSMEdge: Edge
    {
        public SerializableCallback Event;
    }
}