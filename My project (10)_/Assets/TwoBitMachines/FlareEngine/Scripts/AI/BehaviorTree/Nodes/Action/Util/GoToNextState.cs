#region
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
    [AddComponentMenu ("")]
    public class GoToNextState : Action
    {
        public override NodeState RunNodeLogic (Root root)
        {
            return NodeState.Success;
        }

        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
        #if UNITY_EDITOR
        #pragma warning disable 0414

        public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
        {
            if (parent.Bool ("showInfo"))
            {
                Labels.InfoBoxTop (60, "Jump to another state. Useful if a node you are working with can't jump to a state." +
                    "\n \n Returns Success");
            }
            return true;
        }

        #pragma warning restore 0414
        #endif
        #endregion 
    }

}