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
        public class Jump : Action
        {
                [SerializeField] public float height;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                if (!root.world.onGround)
                                {
                                        return NodeState.Failure;
                                }
                                Vector2 velocity = Compute.ArchObject (root.position, root.position, height, root.gravity.gravity); //  this method will find the exact velocity to jump the necessary height.
                                velocity.y += root.gravity.gravity * Time.deltaTime * 0.5f; //                                          adjust jump
                                root.velocity.y = velocity.y; //                                                                        finally, apply the velocity to the AI.
                                root.hasJumped = true; //                                                                               let the system know we are jumping.
                        }
                        else
                        {
                                if (root.world.onGround)
                                {
                                        return NodeState.Success; //                                                                     once the AI hits the ground, the jump action is complete.
                                }
                        }
                        return NodeState.Running;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "If on the ground, the AI will jump." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        FoldOut.Box (1, color, yOffset: -2);
                        parent.Field ("Height", "height");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 

        }
}