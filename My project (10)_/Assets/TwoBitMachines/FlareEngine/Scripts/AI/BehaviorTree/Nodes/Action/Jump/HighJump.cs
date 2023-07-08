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
        public class HighJump : Action
        {
                [System.NonSerialized] private Vector2 force;
                [System.NonSerialized] private int highJump;
                [System.NonSerialized] private bool initialized;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                highJump = 0;
                                initialized = false;
                        }
                        if (highJump == 1 && root.world.onGround)
                        {
                                highJump = 0;
                                return NodeState.Success;
                        }
                        if (highJump == 2 && !Interactables.HighJump.Find (root.world, root.velocity.y, ref highJump, ref force))
                        {
                                highJump = 0;
                                return NodeState.Success;
                        }
                        if (highJump == 0)
                        {
                                if (Interactables.HighJump.Find (root.world, root.velocity.y, ref highJump, ref force))
                                {
                                        initialized = true;
                                }
                        }
                        if (highJump == 1)
                        {
                                if (initialized)
                                {
                                        initialized = false;
                                        root.velocity.y = force.y;
                                }
                                if (force.x != 0)
                                {
                                        root.velocity.x = force.x;
                                }
                                root.signals.Set ("highJump", root.velocity.y > 0);
                        }
                        if (highJump == 2)
                        {
                                if (Interactables.HighJump.Find (root.world, root.velocity.y, ref highJump, ref force))
                                {
                                        root.velocity.y += force.y;
                                        root.velocity.x += force.x;
                                        root.signals.Set ("windJump");
                                        root.signals.Set ("windLeft", force.x < 0);
                                        root.signals.Set ("windRight", force.x > 0);
                                        force.y = 0;
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
                                Labels.InfoBoxTop (55, "Detect the High Jump interactable." +
                                        "\n \n Returns Running, Success");
                        }

                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 

        }
}