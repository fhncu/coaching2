using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class TargetMouse : Blackboard
        {
                public override Vector2 GetTarget ( )
                {
                        return Util.MousePosition ( );
                }
        }
}