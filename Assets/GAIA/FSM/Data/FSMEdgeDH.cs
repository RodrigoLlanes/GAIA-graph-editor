using System.Collections.Generic;
using GAIA.Utils;

namespace GAIA.FSM.Data
{
    public class FSMEdgeDH
    {
        public SerializableCallback Event;
        public FSMNodeDH Target;
    }
}