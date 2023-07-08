using TwoBitMachines.Editors;
using UnityEditor;
using UnityEngine;

namespace TwoBitMachines.TwoBitSprite.Editors
{
        public static class InspectSprite
        {
                public static void Display (SerializedObject parent, SerializedProperty frames, SerializedProperty frameIndex, SerializedProperty sprite, bool usingExtra = false)
                {
                        if (frames.arraySize == 0) return;

                        SliderGlobalRate (frames, sprite.Get ("globalRate"));
                        ScrollThroughSprites (frames, frameIndex);
                        DisplaySprites (frames, frameIndex, sprite, usingExtra);
                }

                private static void SliderGlobalRate (SerializedProperty frames, SerializedProperty globalRate)
                {
                        if (!Slider.Set (Tint.White, Tint.SoftDark, globalRate, min : 0.25f, max : 60f)) return;

                        for (int i = 0; i < frames.arraySize; i++)
                        {
                                frames.Element (i).Get ("rate").floatValue = 1f / globalRate.floatValue;
                        }
                }

                private static void ScrollThroughSprites (SerializedProperty frames, SerializedProperty frameIndex)
                {
                        Layout.VerticalSpacing (0);
                        Rect scroll = Layout.GetLastRect (Layout.longInfoWidth, 22 * frames.arraySize, xOffset: -11);
                        if (scroll.ContainsScrollWheel (true))
                        {
                                int inc = Event.current.delta.y > 0 ? 1 : -1;
                                int newIndex = frameIndex.intValue + inc;
                                frameIndex.intValue = Compute.WrapArrayIndex (newIndex, frames.arraySize);
                        }
                        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow)
                        {
                                int newIndex = frameIndex.intValue + 1;
                                frameIndex.intValue = Compute.WrapArrayIndex (newIndex, frames.arraySize);
                        }
                        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow)
                        {
                                int newIndex = frameIndex.intValue - 1;
                                frameIndex.intValue = Compute.WrapArrayIndex (newIndex, frames.arraySize);
                        }
                }

                private static void DisplaySprites (SerializedProperty frames, SerializedProperty frameIndex, SerializedProperty sprite, bool usingExtra)
                {
                        for (int i = 0; i < frames.arraySize; i++)
                        {
                                SerializedProperty frame = frames.Element (i);
                                Bar (frame, frame.Get ("rate"), frameIndex, i, frames.arraySize);
                                if (ListReorder.Grip (sprite, frames, TwoBitMachines.Editors.Bar.barStart.CenterRectHeight ( ), i, Tint.White))
                                {
                                        frameIndex.intValue = i;
                                        SpriteProperty.ReorderNestedArrays (sprite.Get ("property"), ListReorder.source, ListReorder.destination);
                                }
                                if (frame.Bool ("eventFoldOut"))
                                {
                                        if (usingExtra)
                                                FrameEvents (frame);
                                        else
                                                Fields.EventField (frame.Get ("onEnterFrame"), adjustX : 10, color : Tint.SoftDark);
                                }
                                if (frame.ReadBool ("delete"))
                                {
                                        frames.DeleteArrayElement (frameIndex.intValue);
                                        SpriteProperty.DeleteNestedArrayElement (sprite.Get ("property"), frameIndex.intValue);
                                        frameIndex.intValue = Mathf.Clamp (frameIndex.intValue, 0, frames.arraySize - 1);
                                        return;
                                }
                                if (frame.ReadBool ("add"))
                                {
                                        frames.arraySize++;
                                        SpriteProperty.MatchArraySize (sprite.Get ("property"), frames.arraySize);
                                        frameIndex.intValue = Mathf.Clamp (frameIndex.intValue, 0, frames.arraySize - 1);
                                        return;
                                }
                        }
                }

                public static void FrameEvents (SerializedProperty frame)
                {
                        SerializedProperty array = frame.Get ("events");

                        for (int i = 0; i < array.arraySize; i++)
                        {
                                SerializedProperty element = array.Element (i);
                                if (Fields.EventFoldOutFloat (element.Get ("frameEvent"), element.Get ("atPercent"), element.Get ("eventFoldOut"), "       Frame Event", xOffset : 20, ySpace : false, color : Tint.SoftDark))
                                {
                                        array.DeleteArrayElement (i);
                                        break;
                                }
                                ListReorder.Grip (frame, array, Layout.GetLastRect (15, 15, yOffset : 2), i, Tint.Blue);

                        }

                        if (FoldOut.CornerButton (Tint.Blue, ySpace : false))
                        {
                                array.arraySize++;
                        }
                        Layout.VerticalSpacing ( );
                }

                public static void Bar (SerializedProperty frame, SerializedProperty rate, SerializedProperty frameIndex, int index, int size, int xoffset = 8)
                {
                        bool selected = frameIndex.intValue == index;
                        bool indexChanged = false;
                        TwoBitMachines.Editors.Bar.Setup (Texture2D.whiteTexture, frameIndex.intValue == index ? Tint.BoxTwo : Tint.SoftDark, space : false, height : 22);
                        TwoBitMachines.Editors.Bar.SpaceLeft (25);
                        if (TwoBitMachines.Editors.Bar.barStart.ContainsMouseDown (false)) indexChanged = true;
                        Rect spriteRect = new Rect (TwoBitMachines.Editors.Bar.barStart) { y = TwoBitMachines.Editors.Bar.barStart.y + 2, width = Layout.longInfoWidth - 118, height = 18 };
                        EditorGUI.ObjectField (spriteRect, frame.Get ("sprite"), GUIContent.none);
                        rate.floatValue = 1f / EditorGUI.FloatField (spriteRect.Adjust (spriteRect.width, 38), 1f / rate.floatValue);
                        if (selected) TwoBitMachines.Editors.Bar.ButtonRight (frame.Get ("eventFoldOut"), "Reopen", Tint.White, Tint.White);
                        if (selected) TwoBitMachines.Editors.Bar.ButtonRight (frame.Get ("delete"), "xsMinus", Tint.Delete, Tint.Delete);
                        if (selected) TwoBitMachines.Editors.Bar.ButtonRight (frame.Get ("add"), "xsAdd", Tint.On, Tint.On, execute : index == size - 1);
                        if (indexChanged) frameIndex.intValue = index;
                }
        }
}