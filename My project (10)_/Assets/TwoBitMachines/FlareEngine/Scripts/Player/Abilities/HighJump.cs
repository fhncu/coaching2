#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu ("")]
        public class HighJump : Ability
        {
                [System.NonSerialized] private Vector2 force;
                [System.NonSerialized] private bool initialized;
                [System.NonSerialized] private int highJump; // 1 = jump, 2 = wind

                public override void Reset (AbilityManager player)
                {
                        highJump = 0;
                        initialized = false;
                        force = Vector2.zero;
                }

                public override bool TurnOffAbility (AbilityManager player)
                {
                        Reset (player);
                        return true;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        if (pause) return false;

                        if (highJump == 1 && player.world.onGround)
                        {
                                highJump = 0;
                                Reset (player);
                        }
                        if (highJump == 2 && !Interactables.HighJump.Find (player.world, velocity.y, ref highJump, ref force))
                        {
                                highJump = 0;
                                Reset (player);
                        }
                        if (Interactables.HighJump.Find (player.world, velocity.y, ref highJump, ref force))
                        {
                                initialized = false;
                        }
                        return highJump != 0;
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        if (highJump == 1)
                        {
                                if (!initialized)
                                {
                                        initialized = true;
                                        velocity.y = force.y;
                                        player.CheckForAirJumps ( );
                                }
                                if (force.x != 0)
                                {
                                        if (Compute.SameSign (velocity.x, force.x)) // dont increase force 
                                        {
                                                velocity.x = force.x;
                                        }
                                        else
                                        {
                                                force.x += velocity.x * Time.deltaTime;
                                                velocity.x = force.x;
                                        }
                                }
                                player.signals.Set ("highJump", velocity.y > 0);
                        }
                        else if (highJump == 2)
                        {
                                if (Interactables.HighJump.Find (player.world, velocity.y, ref highJump, ref force))
                                {
                                        velocity.y += force.y;
                                        velocity.x += force.x;
                                        player.signals.Set ("windJump");
                                        player.signals.Set ("windLeft", force.x < 0);
                                        player.signals.Set ("windRight", force.x > 0);
                                        force.y = 0;
                                }
                        }
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [System.NonSerialized] public bool foldOutEvent;
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open (parent, "High Jump", barColor, labelColor))
                        {
                                // FoldOut.Box (1, FoldOut.boxColorLight, yOffset: -2);
                                // {
                                //         parent.FieldToggle ("Can Air Jump", "canAirJump");
                                // }
                                Layout.VerticalSpacing (3);
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion

        }

}