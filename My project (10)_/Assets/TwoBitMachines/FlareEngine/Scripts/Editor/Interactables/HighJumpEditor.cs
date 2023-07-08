using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.Interactables;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (HighJump))]
        public class HighJumpEditor : UnityEditor.Editor
        {
                private HighJump main;
                private SerializedObject parent;

                private void OnEnable ( )
                {
                        main = target as HighJump;
                        parent = serializedObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        parent.Update ( );
                        if (FoldOut.Bar (parent, Tint.Blue).Label ("High Jump", Color.white).FoldOut ( ))
                        {
                                int type = parent.Enum ("type");
                                int height = type == 1 ? 1 : 0;
                                FoldOut.Box (2 + height, FoldOut.boxColor, extraHeight : 5, yOffset: -2);
                                parent.Field ("Type", "type");
                                parent.Field ("Force", "force");
                                parent.Field ("Direction", "windDirection", execute : type == 1);

                                if (type == 0 && FoldOut.FoldOutButton (parent.Get ("eventFoldOut")))
                                {
                                        Fields.EventFoldOutEffect (parent.Get ("onTrampoline"), parent.Get ("trampolineWE"), parent.Get ("trampolineFoldOut"), "On Trampoline");
                                }
                        }
                        parent.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);
                }

                public void OnSceneGUI ( )
                {
                        if (Application.isPlaying)
                        {
                                Draw.Square (main.bounds.rawPosition, main.bounds.size, Tint.Blue);
                                return;
                        }
                        else
                        {
                                main.transform.position = Compute.Round (main.transform.position, 0.25f);
                        }
                        parent.Update ( );
                        {
                                Vector3 newPosition = main.transform.position;

                                if (main.bounds.position == Vector2.zero)
                                {
                                        main.bounds.position = main.transform.position - (Vector3) main.bounds.size * 0.5f;
                                }

                                if ((main.oldPosition.x != newPosition.x || main.oldPosition.y != newPosition.y) && !Application.isPlaying) // && TwoBitMachines.Editors.Mouse.ctrl)
                                {
                                        main.bounds.position += (Vector2) (newPosition - main.oldPosition);
                                        main.oldPosition = newPosition;
                                }
                                else
                                {
                                        if (!Application.isPlaying) main.bounds.MoveRaw (main.transform.position);
                                        SceneTools.DrawAndModifyBounds (ref main.bounds.position, ref main.bounds.size, Tint.Blue);
                                }
                                parent.FindProperty ("bounds").Get ("size").vector2Value = main.bounds.size;
                                parent.FindProperty ("bounds").Get ("position").vector2Value = main.bounds.position;
                                parent.FindProperty ("oldPosition").vector3Value = newPosition;
                        }
                        parent.ApplyModifiedProperties ( );
                }
        }
}