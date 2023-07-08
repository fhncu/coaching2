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
        public class Move : Action
        {
                [SerializeField] public Vector2 velocity;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (velocity.x != 0) root.velocity.x = velocity.x;
                        if (velocity.y != 0) root.velocity.y = velocity.y;
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
                                Labels.InfoBoxTop (55, "This will move the AI with the specified velocity." +
                                        "\n \n Returns Running");
                        }

                        FoldOut.Box (1, color, yOffset: -2);
                        parent.Field ("Velocity", "velocity");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }
}