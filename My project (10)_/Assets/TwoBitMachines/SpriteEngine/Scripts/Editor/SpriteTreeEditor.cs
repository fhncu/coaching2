using System.Collections.Generic;
using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
        public static class SpriteTreeEditor
        {
                private static string[] sprites;
                private static string[] signals;
                private static int nodeLimit = 4;
                public static string inputName = "Name";

                public static void TreeInspector (SpriteTree main, SerializedProperty property, List<string> spriteNames, string[] newSignals)
                {
                        signals = newSignals;
                        sprites = spriteNames.ToArray ( );
                        Signals (property.Get ("signals"), property);
                        TreeState (property.Get ("branch"), property.Get ("spriteFlip"), property);
                }

                private static void Signals (SerializedProperty signal, SerializedProperty property)
                {
                        if (FoldOut.Bar (property, Tint.SoftDark).Label ("Signals", Tint.White).FoldOut ("signalFoldOut"))
                        {
                                SerializedProperty signalRef = property.Get ("signalRef");
                                SerializedProperty userCreated = signalRef.Get ("extra");

                                if (Fields.InputAndButtonBox ("Create Signal", "Add", Tint.Blue, ref inputName))
                                {
                                        userCreated.arraySize++;
                                        userCreated.LastElement ( ).Get ("name").stringValue = inputName;
                                        AddSignal (signal, inputName);
                                        inputName = "Name";
                                }
                                Display (signalRef.Get ("all"), signal);
                                Display (signalRef.Get ("extra"), signal, true);
                        }
                }

                private static void Display (SerializedProperty array, SerializedProperty signal, bool canDelete = false)
                {
                        for (int i = 0; i < array.arraySize; i++)
                        {
                                SerializedProperty element = array.Element (i);
                                GUI.enabled = element.Bool ("use");
                                string name = element.String ("name");
                                FoldOut.Bar (FoldOut.boxColor, height : 20).Label (name, FoldOut.titleColor, false);
                                GUI.enabled = true;

                                if (FoldOut.bar.C (true))
                                {
                                        element.Toggle ("use");
                                        if (element.Bool ("use"))
                                        {
                                                AddSignal (signal, name);
                                        }
                                        else
                                        {
                                                RemoveSignal (signal, name);
                                        }
                                }
                                if (canDelete && TwoBitMachines.Editors.Bar.ButtonRight ("Delete", Color.white))
                                {
                                        RemoveSignal (signal, name);
                                        array.DeleteArrayElement (i);
                                        return;
                                }
                        }
                }

                private static void AddSignal (SerializedProperty signal, string name)
                {
                        for (int i = 0; i < signal.arraySize; i++)
                        {
                                if (signal.Element (i).stringValue == name)
                                        return;
                        }
                        signal.arraySize++;
                        signal.LastElement ( ).stringValue = name;
                }

                private static void RemoveSignal (SerializedProperty signal, string name)
                {
                        for (int i = 0; i < signal.arraySize; i++)
                        {
                                if (signal.Element (i).stringValue == name)
                                {
                                        signal.MoveArrayElement (i, signal.arraySize - 1);
                                        signal.arraySize--;
                                        return;
                                }
                        }
                }

                private static void TreeState (SerializedProperty branchArray, SerializedProperty spriteFlip, SerializedProperty property)
                {
                        if (FoldOut.Bar (property, Tint.SoftDark).Label ("State", Tint.White).BR ("createState", execute : property.Bool ("stateFoldOut")).FoldOut ("stateFoldOut"))
                        {
                                if (property.ReadBool ("createState"))
                                {
                                        branchArray.arraySize++;
                                        branchArray.LastElement ( ).Get ("sprite").stringValue = "";
                                        branchArray.LastElement ( ).Get ("signal").stringValue = "";
                                        branchArray.LastElement ( ).Get ("nodes").arraySize = 0;
                                        branchArray.MoveArrayElement (branchArray.arraySize - 1, 0);
                                }
                                if (signals.Length == 0)
                                {
                                        if (spriteFlip.arraySize == 1 && spriteFlip.LastElement ( ).Get ("nodes").arraySize <= 1) // if no signals, remove sprite flip if size of one to prevent a sprite flip!
                                        {
                                                spriteFlip.arraySize = 0;
                                        }
                                        GUILayout.Label ("No signals available.");
                                        return;
                                }
                                for (int i = 0; i < branchArray.arraySize; i++)
                                {
                                        Tree (branchArray.Element (i), branchArray, property.Get ("active"), property.Get ("signalIndex"), i, 0);
                                }
                                if (spriteFlip.arraySize == 0) spriteFlip.arraySize++;
                                Tree (spriteFlip.LastElement ( ), null, null, null, 0, 0, true);

                        }
                }

                private static void Tree (SerializedProperty branch, SerializedProperty parentArray, SerializedProperty active, SerializedProperty signalIndex, int index, float depth = 0, bool spriteFlip = false)
                {
                        float offset = depth * 20f;
                        Bar (branch, spriteFlip ? Tint.Blue : Tint.Orange, offset, 20, depth + 1 < nodeLimit, !spriteFlip || (spriteFlip && depth > 0));
                        ListReorder.Grip (signalIndex, active, parentArray, TwoBitMachines.Editors.Bar.barStart.CenterRectHeight ( ), index, Tint.White, offset);
                        branch.DropList (new Rect (TwoBitMachines.Editors.Bar.barStart) { width = Layout.infoWidth - offset - 60, y = TwoBitMachines.Editors.Bar.barStart.y - 1 }, signals, "signal");

                        SerializedProperty nodes = branch.Get ("nodes");
                        if (nodes != null && (depth + 1) < nodeLimit && branch.ReadBool ("add"))
                        {
                                nodes.arraySize++;
                                return;
                        }
                        if (parentArray != null && branch.ReadBool ("delete"))
                        {
                                parentArray.DeleteArrayElement (index);
                                return;
                        }
                        if (!branch.Bool ("foldOut"))
                        {
                                return;
                        }
                        if (nodes == null || nodes.arraySize == 0) // display chosen sprite
                        {
                                Color previous = GUI.color;
                                GUI.color = Tint.PastelGreen;
                                Rect spriteRect = Layout.CreateRect (Layout.longInfoWidth - 110, 20f, xOffset : 100, yOffset : 1);
                                branch.DropList (spriteRect, spriteFlip ? TwoBitSprite.SpriteTree.spriteDirection : sprites, "sprite");
                                GUI.color = previous;
                        }
                        if (++depth >= nodeLimit || nodes == null)
                        {
                                return;
                        }
                        for (int i = 0; i < nodes.arraySize; i++) // iterate children
                        {
                                Tree (nodes.Element (i), nodes, branch.Get ("active"), branch.Get ("signalIndex"), i, depth, spriteFlip);
                        }
                }

                public static void Bar (SerializedProperty property, Color barColor, float xAdjust, int xOffset = 8, bool showAdd = true, bool showDelete = true)
                {
                        TwoBitMachines.Editors.Bar.Setup (Icon.Get ("BackgroundLight128x128"), barColor, xAdjust : xAdjust, height : 22);
                        TwoBitMachines.Editors.Bar.SpaceLeft (xOffset);
                        TwoBitMachines.Editors.Bar.ButtonRight (property.Get ("add"), "Add", Tint.White, Tint.White, execute : showAdd);
                        TwoBitMachines.Editors.Bar.ButtonRight (property.Get ("delete"), "Delete", Tint.White, Tint.White, execute : showDelete);
                        TwoBitMachines.Editors.Bar.ButtonLeft (property.Get ("foldOut"), property.Bool ("foldOut") ? "ArrowDown" : "ArrowRight", Tint.White, Tint.White);
                }

        }
}