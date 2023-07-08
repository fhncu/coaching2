#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class NullifyGravity : Action
        {
                public override NodeState RunNodeLogic (Root root)
                {
                        root.velocity.y -= root.gravity.gravityEffect;
                        return NodeState.Running;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool HasNextState ( ) { return false; }
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "Undo the effect of gravity" +
                                        "\n \n Returns Running");
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}