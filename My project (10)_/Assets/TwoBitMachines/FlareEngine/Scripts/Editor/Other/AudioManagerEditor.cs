using System;
using System.Reflection;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (AudioManager))]
        public class AudioManagerEditor : UnityEditor.Editor
        {
                private AudioManager main;
                private SerializedObject parent;
                private GameObject objReference;
                public static string inputName = "Name";

                private void OnEnable ( )
                {
                        main = target as AudioManager;
                        parent = serializedObject;
                        objReference = main.gameObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );

                        Layout.VerticalSpacing (10);

                        parent.Update ( );
                        {
                                FoldOut.Box (2, FoldOut.boxColor);
                                {
                                        parent.Field ("Music", "music");
                                        parent.Slider ("Master Volume", "musicMasterVolume");
                                        Layout.VerticalSpacing (5);
                                        FoldOut.Box (2, FoldOut.boxColor);
                                        parent.Field ("SFX", "sfx");
                                        parent.Slider ("Master Volume", "sfxMasterVolume");
                                }
                                Layout.VerticalSpacing (5);

                                FoldOut.Box (5, FoldOut.boxColor);
                                {
                                        parent.Field ("AudioManagerSO", "audioManagerSO");
                                        parent.Field ("Fade In Time", "fadeInTime");
                                        parent.Field ("Fade Out Time", "fadeOutTime");
                                        parent.FieldToggle ("Main Manager", "isMainManager");
                                        parent.FieldAndEnable ("Play On Start", "startMusic", "playOnStart");
                                }
                                Layout.VerticalSpacing (5);

                                SerializedProperty categories = parent.Get ("categories");
                                if (Fields.InputAndButtonBox ("Audio Category", "Add", Tint.Blue, ref inputName))
                                {
                                        categories.arraySize++;
                                        categories.LastElement ( ).Get ("name").stringValue = inputName;
                                        parent.Get ("categoryName").stringValue = "Name";
                                }

                                Layout.VerticalSpacing (1);

                                for (int i = 0; i < categories.arraySize; i++)
                                {
                                        SerializedProperty category = categories.Element (i);
                                        SerializedProperty audio = category.Get ("audio");

                                        FoldOut.Bar (category, Tint.Orange, 0)
                                                .Grip (parent, categories, i)
                                                .Label (category.String ("name"))
                                                .BR ( )
                                                .BR ("deleteAsk", "Delete")
                                                .BR ("delete", "Close", execute : category.Bool ("deleteAsk"))
                                                .FoldOut ( );

                                        if (Bar.FoldOpen (category.Get ("foldOut")))
                                        {
                                                for (int j = 0; j < audio.arraySize; j++)
                                                {
                                                        SerializedProperty element = audio.Element (j);
                                                        float width = Layout.labelWidth + Layout.contentWidth - 67;

                                                        FoldOut.Bar (element, Tint.BoxTwo, 0)
                                                                .G (category, audio, j, color : Tint.Grey)
                                                                .LF ("clip", (int) (width * 0.69f), -4, 2)
                                                                .LF ("volume", (int) (width * 0.125f), -4, 2)
                                                                .LF ("type", (int) (width * 0.15f), -4, 2);
                                                        element.Clamp ("volume");

                                                        if (Bar.ButtonRight ("Delete", Tint.White))
                                                        {
                                                                audio.MoveArrayElement (j, audio.arraySize - 1);
                                                                audio.arraySize--;
                                                                break;
                                                        }
                                                        if (Bar.ButtonRight ("Play", Tint.White))
                                                        {
                                                                StopAllClips ( );
                                                                PlayClip (element.Get ("clip").objectReferenceValue as AudioClip);
                                                        }
                                                        if (Bar.ButtonRight ("Red", Tint.White))
                                                        {
                                                                StopAllClips ( );
                                                        }
                                                }
                                                if (audio.arraySize == 0) Layout.VerticalSpacing (5);
                                        }

                                        if (category.ReadBool ("add"))
                                        {
                                                audio.arraySize++;
                                                audio.LastElement ( ).Get ("volume").floatValue = 1f;
                                                category.SetTrue ("foldOut");
                                        }

                                        if (category.ReadBool ("delete"))
                                        {
                                                categories.DeleteArrayElement (i);
                                                break;
                                        }
                                }
                        }
                        parent.ApplyModifiedProperties ( );
                        Layout.VerticalSpacing (20);

                }
                //https: //forum.unity.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/
                public static void PlayClip (AudioClip clip, int startSample = 0, bool loop = false)
                {
                        if (clip == null) return;
                        Assembly unityEditorAssembly = typeof (AudioImporter).Assembly;

                        Type audioUtilClass = unityEditorAssembly.GetType ("UnityEditor.AudioUtil");
                        MethodInfo method = audioUtilClass.GetMethod (
                                "PlayPreviewClip",
                                BindingFlags.Static | BindingFlags.Public,
                                null,
                                new Type[] { typeof (AudioClip), typeof (int), typeof (bool) },
                                null
                        );

                        method.Invoke (null, new object[] { clip, startSample, loop });
                }

                public static void StopAllClips ( )
                {
                        Assembly unityEditorAssembly = typeof (AudioImporter).Assembly;

                        Type audioUtilClass = unityEditorAssembly.GetType ("UnityEditor.AudioUtil");
                        MethodInfo method = audioUtilClass.GetMethod (
                                "StopAllPreviewClips",
                                BindingFlags.Static | BindingFlags.Public,
                                null,
                                new Type[] { },
                                null
                        );

                        method.Invoke (null, new object[] { });
                }

        }
}