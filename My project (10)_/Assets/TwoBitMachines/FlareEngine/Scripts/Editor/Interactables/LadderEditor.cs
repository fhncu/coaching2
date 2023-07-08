using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (Ladder))]
        [CanEditMultipleObjects]
        public class LadderEditor : UnityEditor.Editor
        {
                private Ladder main;
                private SerializedObject parent;

                private void OnEnable ( )
                {
                        main = target as Ladder;
                        parent = serializedObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        parent.Update ( );

                        if (FoldOut.Bar (parent, Tint.Blue).Label ("Ladder", Color.white).FoldOut ( ))
                        {
                                SerializedProperty ladder = this.parent.Get ("ladder");

                                FoldOut.Box (1, FoldOut.boxColor, yOffset: -2);
                                {
                                        ladder.Field ("Size", "size");
                                }
                                Layout.VerticalSpacing (3);

                                FoldOut.Box (4, FoldOut.boxColor, yOffset: -2);
                                {
                                        ladder.FieldToggleAndEnable ("Stand On Top", "standOnLadder");
                                        ladder.FieldToggleAndEnable ("Align To Center", "alignToCenter");
                                        ladder.FieldToggleAndEnable ("Can Jump Up", "canJumpUp");
                                        ladder.FieldToggleAndEnable ("Stop Side Jump", "stopSideJump");
                                }
                                Layout.VerticalSpacing (3);

                                SerializedProperty fence = this.parent.Get ("fenceFlip");

                                if (FoldOut.Bar (fence, FoldOut.boxColorLight).Label ("Fence Flip", FoldOut.titleColor, false).BRE ("canFlip").FoldOut ("fenceFoldOut"))
                                {
                                        FoldOut.Box (2, FoldOut.boxColor, yOffset: -2);
                                        {
                                                GUI.enabled = fence.Bool ("canFlip");
                                                fence.Field ("Flip time", "flipTime");
                                                fence.Field ("SpriteEngine", "spriteEngine");
                                                GUI.enabled = true;
                                        }
                                        Layout.VerticalSpacing (3);
                                }

                                if (FoldOut.Bar (parent, FoldOut.boxColorLight).Label ("Fence Reverse", FoldOut.titleColor, false).BRE ("canFlip").FoldOut ("reverseFoldOut"))
                                {
                                        FoldOut.Box (3, FoldOut.boxColor, yOffset: -2);
                                        {
                                                GUI.enabled = parent.Bool ("canFlip");
                                                parent.Field ("Reverse Time", "reverseTime");
                                                parent.FieldToggleAndEnable ("Flip X", "canFlipX");
                                                parent.FieldToggleAndEnable ("Flip Y", "canFlipY");
                                                GUI.enabled = true;
                                        }
                                        Layout.VerticalSpacing (3);
                                }

                        }
                        parent.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);
                }

        }
}