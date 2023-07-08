using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (ManageScenes))]
        public class ManageScenesEditor : UnityEditor.Editor
        {
                private ManageScenes main;
                private SerializedObject parent;
                private string[] sceneNames;

                private void OnEnable ( )
                {
                        main = target as ManageScenes;
                        parent = serializedObject;

                        Layout.Initialize ( );

                        int sceneCount = Util.SceneCount ( );
                        sceneNames = new string[sceneCount];
                        for (int i = 0; i < sceneCount; i++)
                        {
                                sceneNames[i] = Util.GetSceneName (i);
                        }
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        Layout.VerticalSpacing (10);
                        parent.Update ( );
                        {
                                SceneManagement ( );
                        }
                        parent.ApplyModifiedProperties ( );
                }

                public void SceneManagement ( )
                {
                        if (FoldOut.Bar (parent).Label ("Manage Scenes").BR (execute: parent.Bool ("foldOut")).FoldOut ( ))
                        {
                                int size = sceneNames == null || sceneNames.Length == 0 ? 2 : 4;
                                FoldOut.Box (size, FoldOut.boxColor);

                                if (sceneNames != null)
                                {
                                        parent.DropDownList (sceneNames, "Next Scene", "nextSceneName");
                                        parent.DropDownList (sceneNames, "Menu Scene", "menuName");
                                }
                                parent.Field ("Load Scene", "loadSceneOn");
                                parent.Field ("Pause Game", "pause");
                                Layout.VerticalSpacing (5);

                                SerializedProperty text = parent.Get ("text");
                                if (text.arraySize == 0) text.arraySize++;
                                FoldOut.Box (text.arraySize, FoldOut.boxColor);
                                text.FieldProperty ("Random Text");
                                Layout.VerticalSpacing (5);

                                SerializedProperty steps = parent.Get ("step");
                                if (parent.ReadBool ("add"))
                                {
                                        steps.arraySize++;
                                        steps.LastElement ( ).Get ("time").floatValue = 1f;
                                        steps.LastElement ( ).Get ("loadSpeed").floatValue = 1f;
                                }
                                for (int i = 0; i < steps.arraySize; i++)
                                {
                                        SerializedProperty step = steps.Element (i);
                                        LoadStepType type = (LoadStepType) step.Enum ("type");

                                        if (FoldOut.Bar (FoldOut.boxColor, height : 20).SL (17).Label (type.ToString ( ), FoldOut.titleColor, false).BBR ("Delete"))
                                        {
                                                steps.DeleteArrayElement (i);
                                                break;
                                        }
                                        ListReorder.Grip (parent, steps, Bar.barStart.CenterRectHeight ( ), i, Tint.WarmWhite);

                                        if (type == LoadStepType.TransitionIn || type == LoadStepType.TransitionOut)
                                        {
                                                FoldOut.Box (2, FoldOut.boxColor, extraHeight : 5, yOffset: -2);
                                                step.FieldAndDoubleEnum ("GameObject", "gameObject", "deactivate", "type");
                                                step.Field ("Transition Time", "time");

                                                bool eventOpen = FoldOut.FoldOutButton (step.Get ("eventFoldOut"));
                                                Fields.EventFoldOut (step.Get ("onStart"), step.Get ("startFoldOut"), "On Start", execute : eventOpen);
                                                Fields.EventFoldOut (step.Get ("onComplete"), step.Get ("completeFoldOut"), "On Complete", execute : eventOpen);

                                        }
                                        else
                                        {
                                                FoldOut.Box (2, FoldOut.boxColor, extraHeight : 5, yOffset: -2);
                                                step.FieldAndDoubleEnum ("GameObject", "gameObject", "deactivate", "type");
                                                step.Slider ("Load Speed", "loadSpeed");

                                                bool eventOpen = FoldOut.FoldOutButton (step.Get ("eventFoldOut"));
                                                Fields.EventFoldOut (step.Get ("onStart"), step.Get ("startFoldOut"), "On Start", execute : eventOpen);
                                                Fields.EventFoldOut (step.Get ("onComplete"), step.Get ("completeFoldOut"), "On Complete", execute : eventOpen);
                                                Fields.EventFoldOut (step.Get ("loadingProgress"), step.Get ("loadingFoldOut"), "Loading Progress Float", execute : eventOpen);
                                                Fields.EventFoldOut (step.Get ("loadingProgressString"), step.Get ("stringFoldOut"), "Loading Progress String", execute : eventOpen);
                                        }

                                }
                        }
                }

        }
}