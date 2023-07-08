using System.Collections.Generic;
using System.Linq;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
        public static class TransitionEditor
        {
                public delegate void AnimationMenu (object obj);

                public static AnimationMenu animationMenu;

                public static void CallBack (object obj)
                {
                        animationMenu.Invoke (obj);
                }

                public static void Transition (SerializedObject parent, SerializedProperty animation, string[] spriteNamesArray, string[] signals)
                {
                        if (FoldOut.Bar (parent, Tint.SoftDark).Label ("Transition", Tint.White).BR ("createTransition", execute : parent.Bool ("transitionFoldOut")).FoldOut ("transitionFoldOut"))
                        {

                                if (parent.ReadBool ("createTransition"))
                                {
                                        GenericMenu menu = new GenericMenu ( );
                                        for (int i = 0; i < spriteNamesArray.Length; i++)
                                        {
                                                menu.AddItem (new GUIContent (spriteNamesArray[i]), false, CallBack, spriteNamesArray[i]);
                                        }
                                        menu.ShowAsContext ( );
                                }

                                for (int i = 0; i < animation.arraySize; i++)
                                {
                                        SerializedProperty element = animation.Element (i);
                                        if (element.Bool ("hasTransition"))
                                        {
                                                FoldOut.Bar (element, Tint.Green, 25)
                                                        .Label (element.String ("name"), Tint.White)
                                                        .BR ("addTransition")
                                                        .BR ("deleteTransition", "Delete")
                                                        .FoldOut ("transitionFoldOut");

                                                ListReorder.Grip (parent, animation, Bar.barStart.CenterRectHeight ( ), i, Tint.WarmWhite);

                                                if (element.ReadBool ("deleteTransition"))
                                                {
                                                        element.SetFalse ("hasTransition");
                                                        continue;
                                                }

                                                SerializedProperty array = element.Get ("transition");
                                                if (array.arraySize == 0 || element.ReadBool ("addTransition"))
                                                {
                                                        element.SetTrue ("transitionFoldOut");
                                                        array.arraySize++;
                                                }

                                                if (element.Bool ("transitionFoldOut"))
                                                {
                                                        element.SetFalse ("hasChangedDirection");

                                                        for (int j = 0; j < array.arraySize; j++)
                                                        {
                                                                FoldOut.Box (2, Tint.Box);

                                                                SerializedProperty transition = array.Element (j);
                                                                if (transition.DropDownListAndButton (signals, "Condition", "condition", "Delete"))
                                                                {
                                                                        array.DeleteArrayElement (j);
                                                                        break;
                                                                }

                                                                transition.DropDownDoubleList (spriteNamesArray, "From, To", "from", "to");
                                                                if (transition.String ("condition") == "changedDirection")
                                                                {
                                                                        element.SetTrue ("hasChangedDirection");
                                                                }

                                                                Layout.VerticalSpacing (5);
                                                        }
                                                }
                                        }
                                }
                        }
                }

        }

}