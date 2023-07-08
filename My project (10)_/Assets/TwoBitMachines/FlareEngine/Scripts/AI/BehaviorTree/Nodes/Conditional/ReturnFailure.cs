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
        public class ReturnFailure : Conditional
        {
                public override NodeState RunNodeLogic (Root root)
                {
                        return NodeState.Failure;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public bool foldOutEvent;
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (35, "This will always return Failure.");
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}