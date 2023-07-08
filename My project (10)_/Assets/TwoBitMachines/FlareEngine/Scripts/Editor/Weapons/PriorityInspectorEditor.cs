using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Editors
{
        public class PriorityInspectorEditor
        {
                public static void Display (SerializedObject parent, SerializedProperty abilities, string[] names)
                {
                        SortAbilities (abilities);
                        if (FoldOut.Bar (parent, Tint.Orange).Label ("Priority", Color.white).FoldOut ("priorityFoldOut"))
                        {
                                for (int i = 0; i < abilities.arraySize; i++)
                                {
                                        SerializedObject ability = new SerializedObject (abilities.Element (i).objectReferenceValue);
                                        Label (parent, abilities, ability, ability.String ("abilityName"), names, (i + 1).ToString ( ) + ".", i, space : 5);
                                }
                                for (int i = 0; i < abilities.arraySize; i++)
                                {
                                        SerializedObject ability = new SerializedObject (abilities.Element (i).objectReferenceValue);
                                        ability.Update ( );
                                        ability.Get ("ID").intValue = i;
                                        ability.ApplyModifiedProperties ( );
                                }
                        }
                }

                public static void SortAbilities (SerializedProperty array)
                {
                        int size = array.arraySize;
                        for (int i = 0; i < size; i++)
                        {
                                for (int j = 0; j < size - 1; j++)
                                {
                                        SerializedObject a = new SerializedObject (array.Element (j).objectReferenceValue);
                                        SerializedObject b = new SerializedObject (array.Element (j + 1).objectReferenceValue);

                                        if (b.Int ("ID") < a.Int ("ID"))
                                        {
                                                array.MoveArrayElement (j + 1, j);
                                        }
                                }
                        }
                }

                public static void Label (SerializedObject parent, SerializedProperty array, SerializedObject ability, string name, string[] names, string index, int i, int space = 0)
                {
                        ability.Update ( );

                        bool open = FoldOut.Bar (ability, Tint.Box, height : 20).SL (17).Label (index + "  " + name, Color.black, false).FoldOut ("editMask");
                        ListReorder.Grip (parent, array, Bar.barStart.CenterRectHeight ( ), i, Tint.WarmWhite);
                        if (open)
                        {
                                SerializedProperty exceptions = ability.Get ("exception");

                                FoldOut.BoxSingle (1, Tint.Box * Tint.LightGrey);
                                if (ability.DropDownListAndButton (names, "Add Exception", "tempName", "Add"))
                                {
                                        exceptions.arraySize++;
                                        exceptions.LastElement ( ).stringValue = ability.String ("tempName");
                                }
                                Layout.VerticalSpacing (2);

                                for (int j = 0; j < exceptions.arraySize; j++)
                                {
                                        FoldOut.BoxSingle (1, Tint.Box * Tint.LightGrey, yOffset: -2);
                                        if (Labels.LabelAndButton (exceptions.Element (j).stringValue, "Delete", 3))
                                        {
                                                exceptions.DeleteArrayElement (j);
                                        }
                                }
                        }
                        ability.ApplyModifiedProperties ( );
                }
        }
}