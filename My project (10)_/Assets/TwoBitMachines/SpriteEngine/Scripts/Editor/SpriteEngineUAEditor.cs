using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
        [CustomEditor (typeof (SpriteEngineUA), true)]
        [CanEditMultipleObjects]
        public class SpriteEngineUAEditor : Editor
        {
                public SpriteEngineUA main;
                public SerializedObject parent;
                public List<string> animationNames = new List<string> ( );

                private void OnEnable ( )
                {
                        main = target as SpriteEngineUA;
                        parent = serializedObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        serializedObject.Update ( );
                        {
                                SerializedProperty animations = parent.Get ("animations");
                                if (FoldOut.Bar (parent, Tint.SoftDark).Label ("Animator", Tint.White).BR ("add", execute : parent.Bool ("foldOut")).FoldOut ( ))
                                {
                                        FoldOut.Box (1, Tint.SoftDark, yOffset: -2);
                                        {
                                                parent.Field ("Animator", "animator");
                                        }
                                        Layout.VerticalSpacing (3);

                                        if (parent.ReadBool ("add"))
                                        {
                                                animations.arraySize++;
                                        }

                                        for (int i = 0; i < animations.arraySize; i++)
                                        {
                                                FoldOut.BoxSingle (1, Tint.SoftDark, yOffset: -2);
                                                {
                                                        SerializedProperty element = animations.Element (i);
                                                        Fields.ConstructField (-2);
                                                        Fields.ConstructSpace (20);
                                                        element.ConstructField ("name", S.FW - 45);

                                                        if (Fields.ConstructButton ("Delete"))
                                                        {
                                                                animations.DeleteArrayElement (i);
                                                                break;
                                                        }
                                                        if (Fields.ConstructButton ("Reopen"))
                                                        {
                                                                element.Toggle ("foldOut");
                                                        }
                                                        if (element.Bool ("foldOut"))
                                                        {
                                                                FoldOut.BoxSingle (1, Tint.SoftDark, extraHeight : 2, yOffset: -2);
                                                                element.FieldAndEnable ("Synchronize", "syncID", "canSync");
                                                                Labels.FieldText ("Sync ID", rightSpacing : 18);
                                                                // element.FieldToggle ("Loop Once", "loopOnce");
                                                                // Fields.EventFoldOut (element.Get ("onLoopOnce"), element.Get ("loopFoldOut"), "On Loop Once", color : Tint.SoftDark);
                                                        }
                                                }
                                                ListReorder.Grip (parent, animations, Fields.fieldRect, i, Tint.WarmWhite);
                                        }
                                }
                                animations.CreateNameList (animationNames);
                                SpriteTreeEditor.TreeInspector (main.tree, parent.Get ("tree"), animationNames, main.tree.signals.ToArray ( ));

                                TransitionEditor.animationMenu = AddTransitionToAnimation;
                                TransitionEditor.Transition (parent, parent.Get ("animations"), animationNames.ToArray ( ), main.tree.signals.ToArray ( ));

                                SpriteEngineEditor.ShowCurrentState (main.currentAnimation);
                        }
                        serializedObject.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);

                        if (GUI.changed && !EditorApplication.isPlaying) Repaint ( );
                }

                public void AddTransitionToAnimation (object obj)
                {
                        string animationName = (string) obj;
                        parent.Update ( );
                        SerializedProperty animations = parent.Get ("animations");
                        for (int i = 0; i < animations.arraySize; i++)
                        {
                                if (animations.Element (i).String ("name") == animationName)
                                {
                                        animations.Element (i).SetTrue ("hasTransition");
                                        break;
                                }
                        }
                        parent.ApplyModifiedProperties ( );
                }

        }
}