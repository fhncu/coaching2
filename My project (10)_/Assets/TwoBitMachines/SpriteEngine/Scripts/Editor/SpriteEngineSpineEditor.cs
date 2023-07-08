using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

#if SPINE_UNITY
using Spine.Unity;
#endif

namespace TwoBitMachines.TwoBitSprite.Editors
{
        [CustomEditor (typeof (SpriteEngineSpine), true)]
        [CanEditMultipleObjects]
        public class SpriteEngineSpineEditor : Editor
        {
                #if SPINE_UNITY
                #pragma warning disable CS1061
                public SpriteEngineSpine main;
                public SerializedObject parent;
                public static List<string> animations = new List<string> ( );
                public string[] names = new string[] { "Empty" };
                public int index = -1;

                private void OnEnable ( )
                {
                        main = target as SpriteEngineSpine;
                        parent = serializedObject;
                        Layout.Initialize ( );
                        index = -1;
                }

                private void GetAnimationList ( )
                {
                        if (main == null || main.animator == null || main.animator.skeleton == null) return;

                        if (main.animator.skeleton.Data == null || main.animator.skeleton.Data.Animations == null) return;

                        if (main.animator.Skeleton.Data.Animations.Count > 0 && names.Length != index)
                        {
                                Spine.Animation[] anim = main.animator.Skeleton.Data.Animations.ToArray ( );
                                names = new string[anim.Length];
                                for (int i = 0; i < anim.Length; i++)
                                {
                                        names[i] = anim[i].Name;
                                }
                                index = anim.Length;
                        }
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);

                        GetAnimationList ( );

                        serializedObject.Update ( );
                        {
                                if (FoldOut.Bar (parent, Tint.SoftDark).Label ("Animator", Tint.White).BR ("add", execute : parent.Bool ("foldOut")).FoldOut ( ))
                                {
                                        FoldOut.Box (1, Tint.SoftDark, yOffset: -2);
                                        {
                                                parent.Field ("Skeleton Anim", "animator");
                                        }
                                        Layout.VerticalSpacing (3);

                                        SerializedProperty animations = parent.Get ("animations");

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
                                                        element.ConstructList (names, "name", S.FW - 45);

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
                                                                FoldOut.BoxSingle (2, Tint.SoftDark, extraHeight : 2, yOffset: -2);
                                                                element.FieldAndEnable ("Synchronize", "syncID", "canSync");
                                                                Labels.FieldText ("Sync ID", rightSpacing : 18);
                                                                element.FieldToggle ("Loop Once", "loopOnce");
                                                                Fields.EventFoldOut (element.Get ("onLoopOnce"), element.Get ("loopFoldOut"), "On Loop Once", color : Tint.SoftDark);
                                                        }
                                                }
                                                ListReorder.Grip (parent, animations, Fields.fieldRect, i, Tint.WarmWhite);
                                        }

                                }
                                SpriteTreeEditor.TreeInspector (main.tree, parent.Get ("tree"), AnimationList ( ), main.tree.signals.ToArray ( ));
                                TransitionEditor.animationMenu = AddTransitionToAnimation;
                                TransitionEditor.Transition (parent, parent.Get ("animations"), AnimationList ( ).ToArray ( ), main.tree.signals.ToArray ( ));
                                SpriteEngineEditor.ShowCurrentState (main.currentAnimation);
                        }
                        serializedObject.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (10);

                        if (GUI.changed && !EditorApplication.isPlaying)
                        {
                                Repaint ( );
                        }
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

                public List<string> AnimationList ( )
                {
                        animations.Clear ( );
                        for (int i = 0; i < main.animations.Count; i++)
                        {
                                animations.Add (main.animations[i].name);
                        }
                        return animations;
                }
                #pragma warning restore CS1061
                #else
                public override void OnInspectorGUI ( )
                {
                        Labels.InfoBox (60,
                                "First install Spine-Unity. Then add the SPINE_UNITY symbol into Project" +
                                " Settings > Player > Script Compilation > Scripting Define Symbols and click Apply."
                        );
                }
                #endif
        }
}