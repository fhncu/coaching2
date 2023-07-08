using System;
using System.Collections.Generic;
using TwoBitMachines.Editors;
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        public delegate void Display (SerializedProperty parent, Player main, int ID);

        [CustomEditor (typeof (Player))]
        public class PlayerEditor : UnityEditor.Editor
        {
                private Player main;
                private SerializedObject parent;
                private GameObject objReference;
                private List<string> availableList = new List<string> ( );
                private Dictionary<string, SerializedObject> foundList = new Dictionary<string, SerializedObject> ( );

                public static int abilitiesListSize = 0;
                public static string[] abilityNamesList;
                public static string[] inputList = new string[] { "None" };

                private void OnEnable ( )
                {
                        main = target as ThePlayer.Player;
                        parent = serializedObject;
                        objReference = main.gameObject;
                        Layout.Initialize ( );
                        if (abilityNamesList == null || abilityNamesList.Length == 0 || abilitiesListSize != abilityNamesList.Length)
                        {
                                abilityNamesList = Util.GetFileNames ("TwoBitMachines", "/FlareEngine/Scripts/Player/Abilities");
                                abilitiesListSize = abilityNamesList.Length;
                        }
                        if (inputList == null || inputList.Length <= 1 || inputList.Length != (main.inputs.inputSO.Count + 1))
                        {
                                InputList (main);
                        }
                        for (int i = 0; i < main.ability.Count; i++)
                        {
                                if (main.ability[i] != null) main.ability[i].hideFlags = HideFlags.HideInInspector;
                        }
                }

                public override void OnInspectorGUI ( )
                {
                        Layout.Update ( );
                        if (inputList.Length <= 1 || inputList.Length != (main.inputs.inputSO.Count + 1))
                        {
                                InputList (main);
                        }

                        parent.Update ( );
                        {
                                InputInspectorEditor.Display (parent, parent.Get ("inputs").Get ("inputSO"), main, this);
                                SerializedProperty abilities = parent.Get ("ability");
                                List<SerializedObject> serialObjList = GetAbilities (abilities);
                                GetActiveAbilitiesList (serialObjList, availableList, foundList);
                                PriorityInspectorEditor.Display (parent, abilities, abilityNamesList);
                                DisplayAbilities (abilities, foundList, Tint.SoftDark, Tint.WarmWhite);

                                CreateAbility (availableList);
                                DeleteAbility (abilities, serialObjList);
                        }
                        parent.ApplyModifiedProperties ( );

                }

                private void GetActiveAbilitiesList (List<SerializedObject> array, List<string> availableList, Dictionary<string, SerializedObject> foundList)
                {
                        availableList.Clear ( );
                        for (int i = 0; i < abilityNamesList.Length; i++)
                                if (abilityNamesList[i] != "None" && abilityNamesList[i] != "All") //                set available list
                                        availableList.Add (abilityNamesList[i]);

                        foundList.Clear ( );
                        for (int i = 0; i < array.Count; i++)
                        {
                                SerializedObject element = array[i]; //.Element (i);
                                availableList.Remove (element.String ("abilityName"));
                                foundList.Add (element.String ("abilityName"), element);
                        }
                }

                private void DisplayAbilities (SerializedProperty abilities, Dictionary<string, SerializedObject> foundList, Color barColor, Color labelColor)
                {
                        if (!FoldOut.Bar (parent, Tint.SoftDark * Tint.LightGrey).Label ("Abilities", labelColor).FoldOut ("abilityFoldOut")) return;
                        Settings (parent.Get ("world"), parent.Get ("abilities").Get ("gravity"), barColor, labelColor);
                        Walk.OnInspector (parent.Get ("abilities").Get ("walk"), inputList, barColor, labelColor); //  walk ability always displayed

                        for (int i = abilities.arraySize - 1; i >= 0; i--)
                        {
                                UnityEngine.Object obj = abilities.Element (i).objectReferenceValue;
                                if (obj == null) continue;
                                Ability ability = (Ability) obj;
                                SerializedObject abilityObj = new SerializedObject (abilities.Element (i).objectReferenceValue);

                                if (ability != null) ability.hideFlags = HideFlags.HideInInspector;
                                abilityObj.Update ( );
                                if (!ability.OnInspector (parent, abilityObj, inputList, barColor, labelColor))
                                {
                                        string name = abilityObj.String ("abilityName");
                                        if (Ability.Open (abilityObj, Util.ToProperCase (name), barColor, labelColor))
                                        {
                                                int fields = EditorTools.CountObjectFields (abilityObj);
                                                if (fields != 0) FoldOut.Box (fields, FoldOut.boxColorLight, yOffset: -2);
                                                EditorTools.IterateObject (abilityObj, fields);
                                                if (fields == 0) Layout.VerticalSpacing (3);
                                        }
                                }
                                abilityObj.ApplyModifiedProperties ( );
                        }
                }

                private void Settings (SerializedProperty world, SerializedProperty gravity, Color barColor, Color labelColor)
                {
                        if (FoldOut.Bar (world, barColor).Label ("Collision", labelColor).FoldOut ( ))
                        {
                                FoldOut.Box (3, FoldOut.boxColor, yOffset: -2);
                                {
                                        gravity.FieldDouble ("Gravity, Jump", "jumpTime", "jumpHeight");
                                        Labels.FieldDoubleText ("Time", "Height");
                                        gravity.Field ("Gravity Multiplier", "multiplier");
                                        gravity.Field ("Terminal Velocity", "terminalVelocity");
                                }
                                Layout.VerticalSpacing (3);

                                FoldOut.Box (1, FoldOut.boxColor);
                                {
                                        world.Get ("box").Field ("Rays", "rays");
                                        world.Get ("box").ClampV2Int ("rays", min : 2, max : 100);
                                }
                                Layout.VerticalSpacing (5);

                                int extraHeight = world.Bool ("climbSlopes") ? 3 : 0;
                                FoldOut.Box (1 + extraHeight, FoldOut.boxColor);
                                {
                                        world.FieldAndEnableHalf ("Climb Slopes", "maxSlopeAngle", "climbSlopes");
                                        world.Clamp ("maxSlopeAngle", 0, 88f);
                                        Labels.FieldText ("Max Slope", rightSpacing : Layout.boolWidth + 4);
                                        if (world.Bool ("climbSlopes"))
                                        {
                                                world.FieldAndEnable ("Rotate To Slope", "rotateRate", "rotateToSlope");
                                                Labels.FieldText ("Rotate Rate", rightSpacing : Layout.boolWidth + 4);
                                                world.Clamp ("rotateRate", max : 2f);
                                                GUI.enabled = world.Bool ("rotateToSlope");
                                                {
                                                        world.FieldAndEnable ("Rectify In Air", "rotateTo", "rectifyInAir");
                                                        world.FieldToggleAndEnable ("Rotate To Wall", "rotateToWall");
                                                }
                                                GUI.enabled = true;
                                        }
                                }
                                Layout.VerticalSpacing (5);

                                FoldOut.Box (6, FoldOut.boxColor, extraHeight : 3);
                                {
                                        world.Field ("Skip Edge 2D", "skipDownEdge");
                                        world.FieldToggleAndEnable ("Auto Jump Edge 2D", "jumpThroughEdge");
                                        world.FieldToggleAndEnable ("Check Corners", "horizontalCorners");
                                        world.FieldToggleAndEnable ("Use Bridges", "useBridges");
                                        world.FieldToggleAndEnable ("Use Moving Platforms", "useMovingPlatform");
                                        world.FieldToggleAndEnable ("Collide With World Only", "collideWorldOnly");
                                        bool eventOpen = FoldOut.FoldOutButton (world.Get ("eventsFoldOut"));
                                        Fields.EventFoldOutEffect (world.Get ("onCrushed"), world.Get ("crushedWE"), world.Get ("crushedFoldOut"), "Crushed By Platform", execute : eventOpen);
                                }
                        }
                }

                public void InputList (ThePlayer.Player main)
                {
                        int size = main.inputs.inputSO.Count;
                        inputList = new string[size + 1];
                        inputList[0] = "None";

                        for (int i = 0; i < main.inputs.inputSO.Count; i++)
                        {
                                inputList[i + 1] = main.inputs.inputSO[i].buttonName;
                        }
                }

                private void CreateAbility (List<string> availableList)
                {
                        if (FoldOut.CornerButton (Tint.Delete))
                        {
                                GenericMenu menu = new GenericMenu ( );
                                for (int i = 0; i < availableList.Count; i++)
                                {
                                        menu.AddItem (new GUIContent (availableList[i]), false, CreateAbilityInstance, availableList[i]);
                                }
                                menu.ShowAsContext ( );
                        }
                }

                private void CreateAbilityInstance (object obj)
                {
                        string typeName = (string) obj;
                        string fullTypeName = "TwoBitMachines.FlareEngine.ThePlayer." + typeName;
                        Type type = EditorTools.RetrieveType (fullTypeName);
                        if (type == null)
                        {
                                Debug.LogWarning ("FlareEngine: Ability type '" + typeName + "' not found.");
                                return;
                        }
                        Component comp = main.transform.gameObject.AddComponent (type);
                        comp.hideFlags = HideFlags.HideInInspector;
                        Ability newAbility = (Ability) comp;
                        newAbility.abilityName = typeName;

                        if (typeName == "Firearms") newAbility.exception.Add ("Jump");
                        if (typeName == "PushBack") newAbility.exception.Add ("Crouch");

                        parent.Update ( );
                        SerializedProperty abilities = parent.Get ("ability");
                        abilities.arraySize++;
                        abilities.LastElement ( ).objectReferenceValue = newAbility;
                        parent.ApplyModifiedProperties ( );
                }

                private void DeleteAbility (SerializedProperty abilities, List<SerializedObject> array)
                {
                        for (int i = 0; i < array.Count; i++)
                                if (array[i].ReadBool ("delete"))
                                {
                                        for (int j = abilities.arraySize - 1; j >= 0; j--)
                                        {
                                                if (abilities.Element (j).objectReferenceValue == array[i].targetObject)
                                                {
                                                        DestroyImmediate (array[i].targetObject);
                                                        abilities.DeleteArrayElement (j);
                                                        return;
                                                }
                                        }
                                }
                }

                private List<SerializedObject> GetAbilities (SerializedProperty abilities)
                {
                        List<SerializedObject> serialObjList = new List<SerializedObject> ( );
                        for (int i = abilities.arraySize - 1; i >= 0; i--)
                        {
                                if (abilities.Element (i).objectReferenceValue == null)
                                {
                                        abilities.DeleteArrayElement (i);
                                        continue;
                                }
                                serialObjList.Add (new SerializedObject (abilities.Element (i).objectReferenceValue));

                        }
                        return serialObjList;
                }

                private void OnDisable ( )
                {
                        if (main == null && objReference != null && !EditorApplication.isPlayingOrWillChangePlaymode)
                        {
                                objReference.AddComponent<AbilityClean> ( );
                        }
                }

        }
}