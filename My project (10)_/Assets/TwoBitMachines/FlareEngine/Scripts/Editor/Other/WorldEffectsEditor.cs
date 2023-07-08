using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        [CustomEditor (typeof (WorldEffects))]
        public class WorldEffectsEditor : UnityEditor.Editor
        {
                public WorldEffects main;
                public SerializedObject parent;
                public List<string> bulletTypes = new List<string> ( );

                private void OnEnable ( )
                {
                        main = target as WorldEffects;
                        parent = serializedObject;
                        Layout.Initialize ( );
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );

                        if (FoldOut.LargeButton ("Grab Effects", Tint.Blue, Tint.WarmWhite, Icon.Get ("BackgroundLight")))
                        {
                                main.effect.Clear ( );
                                for (int i = 0; i < main.transform.childCount; i++)
                                {
                                        WorldEffectPool pool = new WorldEffectPool ( );
                                        pool.gameObject = main.transform.GetChild (i).gameObject;
                                        main.effect.Add (pool);
                                }
                                Debug.Log ("Found: " + main.effect.Count + " effects");
                        }
                }
        }
}