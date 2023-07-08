#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu ("")]
        public class Firearms : Ability
        {
                [SerializeField] public UnityEvent onCancel;
                private float ammunition = 0;

                public override bool TurnOffAbility (AbilityManager player)
                {
                        onCancel.Invoke ( );
                        player.character.canUseTool = false;
                        return true;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        Character equipment = player.character;
                        equipment.canUseTool = false;
                        player.character.canUseTool = false;
                        if (pause || equipment.tools == null || player.inputs.block) return false;

                        ammunition = 0;
                        for (int i = equipment.tools.Count - 1; i >= 0; i--)
                        {
                                if (equipment.tools[i] == null || equipment.tools[i].gameObject == null)
                                {
                                        equipment.tools.RemoveAt (i);
                                        continue;
                                }
                                if (equipment.tools[i].gameObject.activeInHierarchy)
                                {
                                        ammunition = equipment.tools[i].ToolValue ( );
                                        if (equipment.tools[i].ToolActive ( )) return true;
                                }
                        }
                        return false;
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        player.character.canUseTool = true; //tools
                }

                public override void LateExecute (AbilityManager player, ref Vector2 velocity)
                {
                        Character equipment = player.character;
                        if (equipment.tools == null) return;

                        for (int i = equipment.tools.Count - 1; i >= 0; i--)
                        {
                                if (equipment.tools[i] == null || equipment.tools[i].gameObject == null)
                                {
                                        equipment.tools.RemoveAt (i);
                                        continue;
                                }
                                if (equipment.tools[i].gameObject.activeInHierarchy)
                                {
                                        equipment.tools[i].LateExecute (player, ref velocity);
                                }
                                if (equipment.tools[i].gameObject.activeInHierarchy && equipment.tools[i].IsRecoiling ( ))
                                {

                                        equipment.tools[i].Recoil (ref velocity, player.signals);
                                }
                        }
                }

                public float Ammunition ( )
                {
                        return ammunition;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public bool foldOutCancel;
                public bool foldOutActive;
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open (parent, "Firearms", barColor, labelColor))
                        {
                                Fields.EventFoldOut (parent.Get ("onCancel"), parent.Get ("foldOutCancel"), "On Cancel");
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion

        }

}