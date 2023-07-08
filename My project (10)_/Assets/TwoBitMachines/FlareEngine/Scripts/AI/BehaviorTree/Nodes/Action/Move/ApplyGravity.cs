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
        public class ApplyGravity : Action
        {
                [SerializeField] public Vector2 direction;
                [SerializeField] public float force;
                [System.NonSerialized] private float gravity;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                gravity = 0;
                        }

                        gravity += force;
                        root.velocity = direction * gravity * Time.deltaTime;
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
                                Labels.InfoBoxTop (55, "This will simulate gravity. Mostly needed by moving platforms." +
                                        "\n \n Returns Running");
                        }
                        FoldOut.Box (2, color, yOffset: -2);
                        parent.Field ("Force", "force");
                        parent.Field ("Direction", "direction");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }

}