  #if UNITY_EDITOR
  using System.Collections.Generic;
  using UnityEditor;
  using UnityEngine;

  namespace TwoBitMachines.Editors
  {
          public class Bar
          {
                  public static Rect barStart;
                  public static Rect barEnd;
                  public static float startX;
                  public static float startXOrigin;
                  public static int padding = 3;
                  public static float tabButtonWidth = 1f;
                  public static SerializedProperty property;
                  public static SerializedObject objProperty;

                  public static Rect oldBarStart;
                  public static Rect oldBarEnd;
                  public static float oldStartX;
                  public static float oldStartXOrigin;

                  public static void Remember ( )
                  {
                          oldBarStart = barStart;
                          oldBarEnd = barEnd;
                          oldStartX = startX;
                          oldStartXOrigin = startXOrigin;
                  }

                  public static void ResetToOld ( )
                  {
                          barStart = oldBarStart;
                          barEnd = oldBarEnd;
                          startX = oldStartX;
                          startXOrigin = oldStartXOrigin;
                  }

                  public bool C (bool origin = false)
                  {
                          Rect openRect = new Rect (barStart) { x = origin ? startXOrigin : startX };
                          return Event.current.type == EventType.MouseDown && openRect.Contains (Event.current.mousePosition);
                  }

                  public Bar SU (Color color, int height = 23)
                  {
                          Setup (Editors.FoldOut.background, color, height : height);
                          return this;
                  }

                  public Bar Label (string label, Color? color = null, bool bold = true, int fontSize = 12, bool execute = true)
                  {
                          if (execute == false) return this;
                          Label (label, color ?? Color.white, bold : bold);
                          return this;
                  }

                  public Bar LabelRight (string label, Color? color = null, bool bold = true, int fontSize = 12, int xOffset = 0, bool execute = true)
                  {
                          if (execute == false) return this;
                          LabelRight (label, color ?? Color.white, xOffset : xOffset, bold : bold);
                          return this;
                  }

                  public Bar LabelAndEdit (string label, string edit, Color? color = null, bool bold = true, int fontSize = 12, int width = 150, bool execute = true)
                  {
                          if (execute == false) return this;

                          Rect area = barStart;
                          area.width = 25;

                          if (area.ContainsMouseDown ( ) || (property.Bool (edit) && Event.current.isKey && Event.current.keyCode == KeyCode.Return))
                          {
                                  property.Toggle (edit);
                          }
                          area.width = width;
                          if (Mouse.down && !area.ContainsMouse ( ))
                          {
                                  property.SetFalse (edit);
                          }
                          if (!property.Bool (edit))
                          {
                                  Label (property.String (label), color ?? Color.white, bold : bold);
                          }
                          else
                          {
                                  Rect fieldWidth = new Rect (barStart) { x = barStart.x - 1, y = barEnd.y + 3, width = width, height = Layout.rectFieldHeight - 1 };
                                  EditorGUI.PropertyField (fieldWidth, property.Get (label), GUIContent.none);
                          }
                          return this;
                  }

                  public Bar ToggleButton (string button, string iconOn, string iconOff, string toolTip = "", bool execute = true)
                  {
                          if (execute == false) return this;
                          bool value = property.Bool (button);
                          if (value && ButtonRight (iconOn, Tint.White, toolTip : toolTip)) property.Toggle (button);
                          if (!value && ButtonRight (iconOff, Tint.White, toolTip : toolTip)) property.Toggle (button);
                          return this;
                  }

                  public Bar SL (int space = 8)
                  {
                          SpaceLeft (space);
                          return this;
                  }

                  public Bar BlockRight (float width, Color color, float xOffset = 0)
                  {
                          BlockRightStatic (width, color, xOffset);
                          return this;
                  }

                  public Bar SR (int space = 8, bool execute = true)
                  {
                          if (execute == false) return this;
                          SpaceRight (space);
                          return this;
                  }

                  public bool BBR (string icon = "Add", Color? color = null)
                  {
                          return ButtonRight (icon, color ?? Color.white);
                  }

                  public Bar BR (string field = "add", string icon = "Add", Color? on = null, Color? off = null, bool execute = true)
                  {
                          if (execute == false) return this;
                          ButtonRight (Get (field), icon, on ?? Color.white, off ?? Color.white);
                          return this;
                  }

                  public Bar RightTab (string field, string icon, int index, Color on, Color off, string toolTip = "", bool execute = true)
                  {
                          if (execute == false) return this;

                          SerializedProperty tab = Get (field);

                          if (ButtonRight (icon, tab.intValue == index ? on : off, toolTip : toolTip))
                          {
                                  tab.intValue = tab.intValue == index ? -1 : index;
                          }
                          return this;
                  }

                  public Bar RightButton (string field = "add", string icon = "Add", string toolTip = "", Color? on = null, Color? off = null, bool execute = true)
                  {
                          if (execute == false) return this;
                          ButtonRight (Get (field), icon, on ?? Color.white, off ?? Color.white, toolTip : toolTip);
                          return this;
                  }

                  public Bar BRB (string field = "add", string icon = "Add", string background = "BackgroundLight", Color? on = null, Color? off = null, bool execute = true)
                  {
                          if (execute == false) return this;
                          ButtonRight (Get (field), icon, on ?? Color.white, off ?? Color.white);
                          return this;
                  }

                  public Bar BRE (string enable = "enable", string icon = "Open")
                  {
                          ButtonRight (Get (enable), icon, Tint.PastelGreen, Tint.WarmWhite);
                          return this;
                  }

                  public Bar BRE (string enable, string icon, Color on, Color off)
                  {
                          ButtonRight (Get (enable), icon, on, off);
                          return this;
                  }

                  public Bar BL (string field = "enable", string icon = "LeftButton", Color? on = null, Color? off = null)
                  {
                          ButtonLeft (Get (field), icon, on ?? Color.white, off ?? Color.white);
                          return this;
                  }

                  public Bar LF (string field, int width, int shortenHeight = 0, int yOffset = 3, int extraPad = 0, bool execute = true)
                  {
                          if (execute == false) return this;
                          LeftField (Get (field), width, shortenHeight, yOffset, extraPad);
                          return this;
                  }

                  public Bar LF (int width, int shortenHeight = 0, int yOffset = 3, int extraPad = 0, bool execute = true)
                  {
                          if (execute == false) return this;
                          LeftField (property, width, shortenHeight, yOffset, extraPad);
                          return this;
                  }

                  public Bar LDL (string field, int width, string[] list) // left drop down list
                  {
                          Rect rect = new Rect (barStart) { x = barStart.x, width = width, y = barStart.y + 3 };
                          property.DropList (rect, list, field);
                          SpaceLeft (width);
                          return this;
                  }

                  public Bar RF (string field, int width, int shortenHeight = 0, int yOffset = 0, int extraPad = 0, bool execute = true)
                  {
                          if (execute == false) return this;
                          RightField (Get (field), width, shortenHeight, yOffset, extraPad);
                          return this;
                  }

                  public bool FoldOut (string foldOut = "foldOut")
                  {
                          return FoldOpen (Get (foldOut));
                  }

                  public Bar Sprite (Sprite sprite, int size = 10, int yOffset = 0, bool execute = true)
                  {
                          if (execute == false) return this;
                          ShowSprite (sprite, size, yOffset);
                          return this;
                  }
                  public static void SpaceLeft (int space = 5)
                  {
                          barStart.x += space;
                          startX += space;
                  }

                  public static void SpaceRight (int space = 5)
                  {
                          barEnd.x -= space;
                  }

                  public Bar Grip (SerializedObject parent, SerializedProperty array, int index, int space = 25, Color? color = null)
                  {
                          ListReorder.Grip (parent, array, barStart.CenterRectHeight ( ), index, color ?? Color.white);
                          SL (space);
                          return this;
                  }

                  public Bar Grip (SerializedProperty parent, SerializedProperty array, int index, int space = 25, Color? color = null)
                  {
                          ListReorder.Grip (parent, array, barStart.CenterRectHeight ( ), index, color ?? Color.white);
                          SL (space);
                          return this;
                  }

                  public Bar G (SerializedProperty parent, SerializedProperty array, int index, int space = 25, Color? color = null)
                  {
                          ListReorder.Grip (parent, array, barStart.CenterRectHeight ( ), index, color ?? Color.white);
                          SL (space);
                          return this;
                  }

                  public static void RightField (SerializedProperty property, int width, int shortenHeight = 0, int yOffset = 0, int extraPad = 0, bool execute = true)
                  {
                          if (!execute) return;
                          Rect fieldWidth = new Rect (barEnd) { x = barEnd.x - width - padding - extraPad, y = barEnd.y + yOffset, width = width + padding, height = barEnd.height + shortenHeight };
                          EditorGUI.PropertyField (fieldWidth, property, GUIContent.none);
                          SpaceRight (width + (padding + extraPad) + 1);
                  }

                  public static void LeftField (SerializedProperty property, int width, int shortenHeight = 0, int yOffset = 0, int extraPad = 0, bool execute = true)
                  {
                          if (!execute) return;
                          Rect fieldWidth = new Rect (barStart) { x = barStart.x + extraPad, y = barEnd.y + yOffset, width = width + padding, height = barEnd.height + shortenHeight };
                          EditorGUI.PropertyField (fieldWidth, property, GUIContent.none);
                          SpaceLeft (width + (padding + extraPad) + 1);
                  }

                  public static void ShowSprite (Sprite sprite, int size = 10, int yOffset = 0, bool execute = true)
                  {
                          if (!execute || sprite == null) return;

                          // GUI.DrawTextureWithTexCoords (rect, sprite.texture, sprite.rect);

                          Rect spriteRect = sprite.rect;
                          Texture2D tex = sprite.texture;
                          Rect rect = new Rect (barStart) { x = barStart.x, y = barEnd.y + yOffset + 3, width = size, height = size };
                          GUI.DrawTextureWithTexCoords (rect, tex, new Rect (spriteRect.x / tex.width, spriteRect.y / tex.height, spriteRect.width / tex.width, spriteRect.height / tex.height));

                          SpaceLeft ((int) size + (padding) + 1);
                  }

                  public static bool FoldOpen (SerializedProperty property, bool selected = true)
                  {
                          Rect openRect = new Rect (barStart) { x = startX };
                          if (Event.current.type == EventType.MouseDown && openRect.Contains (Event.current.mousePosition))
                          {
                                  if (!selected && property.boolValue) property.boolValue = false; // keep open
                                  property.boolValue = !property.boolValue;
                                  GUI.changed = true;
                                  Event.current.Use ( );
                          }
                          return property.boolValue;
                  }

                  public static void Setup (Texture2D texture, Color barColor, bool space = true, int height = 23, float xAdjust = 0)
                  {
                          if (space) Layout.VerticalSpacing (1);
                          barStart = Layout.CreateRectAndDraw (width: Layout.longInfoWidth - xAdjust, height: height, xOffset: -11 + xAdjust, texture : texture, color : barColor);
                          barEnd = new Rect (barStart) { x = barStart.x + barStart.width - 6 };
                          startX = startXOrigin = barStart.x;
                  }

                  public static void SetupTabBar (int buttons, int height = 23)
                  {
                          Layout.VerticalSpacing (1);
                          barStart = Layout.CreateRect (width: Layout.longInfoWidth, height: height, xOffset: -11);
                          tabButtonWidth = ((float) Layout.longInfoWidth / (float) buttons);
                          startX = startXOrigin = barStart.x;
                          Layout.Pushclear ( );
                  }

                  public static bool TabButton (SerializedProperty index, int value, string icon, string background, Color colorOn, Color colorOff, string toolTip = "")
                  {
                          if (barStart.Push (tabButtonWidth).Button (Icon.Get (icon), Icon.Get (background), index.intValue == value ? colorOn : colorOff))
                          {
                                  index.intValue = index.intValue == value ? -1 : value;
                          }
                          if (toolTip != "") GUI.Label (barStart, new GUIContent ("", toolTip));
                          return index.intValue == value;
                  }

                  public static bool TabButton (string icon, string background, Color color, bool isSelected, string toolTip = "")
                  {
                          if (barStart.Push (tabButtonWidth).Button (Icon.Get (icon), Icon.Get (background), color, isSelected : isSelected))
                          {
                                  return true;
                          }
                          if (toolTip != "") GUI.Label (barStart, new GUIContent ("", toolTip));
                          return false;
                  }

                  public static void Label (string label, Color color, int fontSize = 12, bool bold = true)
                  {
                          Labels.textStyle.padding = Labels.rectZero;
                          Labels.textStyle.fontSize = (int) fontSize;
                          Labels.textStyle.normal.textColor = color;
                          Labels.textStyle.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;

                          Vector2 size = Labels.textStyle.CalcSize (new GUIContent (label));
                          float offsetY = Mathf.Abs (barStart.height - size.y) * 0.5f;
                          size.x = Mathf.Clamp (size.x, 0, Layout.infoWidth - 15);
                          Rect labelRect = new Rect (barStart) { y = barStart.y + offsetY, width = size.x };

                          Labels.textStyle.clipping = TextClipping.Clip;
                          GUI.Label (labelRect, label, Labels.textStyle);
                          barStart.x += size.x + 5;
                          Labels.ResetTextStyle ( );
                  }

                  public static void LabelRight (string label, Color color, int fontSize = 12, int xOffset = 0, bool bold = true)
                  {
                          Labels.textStyle.padding = Labels.rectZero;
                          Labels.textStyle.fontSize = (int) fontSize;
                          Labels.textStyle.normal.textColor = color;
                          Labels.textStyle.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;

                          Vector2 size = Labels.textStyle.CalcSize (new GUIContent (label));
                          float offsetY = Mathf.Abs (barEnd.height - size.y) * 0.5f;
                          size.x = Mathf.Clamp (size.x, 0, Layout.infoWidth - 15);
                          Rect labelRect = new Rect (barEnd) { x = barEnd.x - (size.x + 2) + xOffset, y = barEnd.y + offsetY, width = size.x };

                          Labels.textStyle.clipping = TextClipping.Clip;
                          GUI.Label (labelRect, label, Labels.textStyle);
                          Labels.ResetTextStyle ( );
                  }

                  public static void BarField (SerializedProperty property, string field, float percent)
                  {
                          float width = (barEnd.x - startX) * percent;
                          Rect rectField = new Rect (barStart) { width = width };
                          property.Get (field).Field (rectField);
                          SpaceLeft ((int) width);
                  }

                  public static bool ButtonRight (SerializedProperty property, string icon, Color on, Color off, int extraPad = 0, bool execute = true, string toolTip = "")
                  {
                          if (!execute) return false;

                          Texture2D texture = Icon.Get (icon);
                          Rect button = new Rect (barEnd) { x = barEnd.x - texture.width - (padding + extraPad), width = texture.width + (padding + extraPad) * 2 };
                          if (toolTip != "") GUI.Label (button, new GUIContent ("", toolTip));
                          button.Toggle (property, texture, on, off);
                          SpaceRight (texture.width + (padding + extraPad) * 2);
                          return property.boolValue;
                  }

                  public static void BlockRightStatic (float width, Color color, float xOffset = 0)
                  {
                          Rect block = new Rect (barEnd) { x = barEnd.x - width + xOffset, y = barEnd.y, width = width, height = barEnd.height - 2 };
                          block.DrawRect (color);
                  }

                  public static bool ButtonRight (SerializedProperty property, string icon, string background, Color on, Color off, int extraPad = 0, bool execute = true)
                  {
                          if (!execute) return false;
                          Texture2D texture = Icon.Get (icon);
                          Rect button = new Rect (barEnd) { x = barEnd.x - texture.width - (padding + extraPad), width = texture.width + (padding + extraPad) * 2 };

                          if (button.Button (texture, Icon.Get (background), on)) property.Toggle ( );
                          SpaceRight (texture.width + (padding + extraPad) * 2);
                          return property.boolValue;
                  }

                  public static bool ButtonRight (string icon, Color color, int extraPad = 0, bool execute = true, string toolTip = "")
                  {
                          if (!execute) return false;
                          Texture2D texture = Icon.Get (icon);
                          Rect button = new Rect (barEnd) { x = barEnd.x - texture.width - (padding + extraPad), width = texture.width + (padding + extraPad) * 2 };
                          if (toolTip != "") GUI.Label (button, new GUIContent ("", toolTip));
                          SpaceRight (texture.width + (padding + extraPad) * 2);
                          return button.Button (texture, color, center : true);
                  }

                  public static bool ButtonLeft (SerializedProperty property, string icon, Color on, Color off, int extraPad = 0, bool execute = true)
                  {
                          if (!execute) return false;
                          Texture2D texture = Icon.Get (icon);
                          Rect button = new Rect (barStart) { x = barStart.x, width = texture.width + (padding + extraPad) * 2 };

                          button.Toggle (property, texture, on, off);
                          SpaceLeft (texture.width + (padding + extraPad) * 2);
                          return property.boolValue;
                  }

                  public static SerializedProperty Get (string field)
                  {
                          return property != null ? property.Get (field) : objProperty != null ? objProperty.Get (field) : null;
                  }

          }

          public static class FoldOut
          {
                  public static GUIStyle boldStyle => EditorGUIUtility.isProSkin ? EditorStyles.whiteBoldLabel : EditorStyles.label;
                  public static Color backgroundColor => EditorGUIUtility.isProSkin ? Tint.Box : Color.white;
                  public static Color titleColor => EditorGUIUtility.isProSkin ? Color.white : Color.black;
                  public static Color boxColorLight => EditorGUIUtility.isProSkin ? Tint.BoxTwo : Tint.Box;
                  public static Color boxColor => EditorGUIUtility.isProSkin ? Tint.SoftDark : Tint.Box;
                  public static bool titleBold => EditorGUIUtility.isProSkin;
                  public static Bar bar = new Bar ( );
                  public static Texture2D background;
                  public const int h = 23; // default height for bars

                  public static void Initialize ( )
                  {
                          background = Icon.Get ("BackgroundLight128x128");

                  }

                  public static Bar Bar (SerializedProperty newProperty, Color? color = null, int space = 8, int height = h)
                  {
                          Editors.Bar.property = newProperty;
                          Editors.Bar.objProperty = null;
                          bar.SU (color ?? Tint.Blue, h);
                          bar.SL (space);
                          return bar;
                  }

                  public static Bar Bar (SerializedObject newProperty, Color? color = null, int space = 8, int height = h)
                  {
                          Editors.Bar.objProperty = newProperty;
                          Editors.Bar.property = null;
                          bar.SU (color ?? Tint.Blue, h);
                          bar.SL (space);
                          return bar;
                  }

                  public static Bar Bar (Color? color = null, int space = 8, int height = h)
                  {
                          bar.SU (color ?? Tint.Blue, h);
                          bar.SL (space);
                          return bar;
                  }

                  public static void TabBarString (Texture2D texture, Color barColor, Color selected, string[] names, SerializedProperty index, LabelType type, int height = h)
                  {
                          if (names.Length == 0) return;

                          Layout.VerticalSpacing (1);
                          Rect rect = Layout.CreateRect (width: Layout.longInfoWidth, height: height, xOffset: -11);
                          float width = (float) Layout.longInfoWidth / (float) names.Length;

                          Layout.Pushclear ( );
                          for (int i = 0; i < names.Length; i++)
                          {
                                  rect.Push (width);
                                  if (rect.Button (names[i], texture, index.intValue == i ? selected : barColor, type, false))
                                  {
                                          index.intValue = i;
                                  }
                          }
                  }

                  public static int TabBarString (Color barColor, Color selected, string[] names, SerializedProperty index, LabelType type, int height = h)
                  {
                          if (names.Length == 0) return 0;

                          Rect rect = Layout.CreateRect (width: Layout.longInfoWidth, height: height, xOffset: -11);
                          int width = (int) ((float) rect.width / (float) names.Length);
                          int extra = Layout.longInfoWidth - (width * names.Length);
                          Layout.Pushclear ( );
                          for (int i = 0; i < names.Length; i++)
                          {
                                  int realWidth = i == names.Length - 1 ? width + extra : width;
                                  rect.Push (realWidth);
                                  Texture2D texture = i == 0 ? Icon.Get ("LeftBar") : i == names.Length - 1 ? Icon.Get ("RightBar") : Icon.Get ("MiddleBar");
                                  if (rect.Button (names[i], texture, index.intValue == i ? selected : barColor, type, false))
                                  {
                                          index.intValue = i;
                                  }
                          }
                          return index.intValue;
                  }

                  public static bool LargeButton (string title, Color barColor, Color labelColor, Texture2D texture, int minusWidth = 0, int height = h)
                  {
                          Layout.VerticalSpacing (1);
                          Rect barStart = Layout.CreateRect (width: Layout.longInfoWidth - minusWidth, height: height, xOffset: -11);
                          Rect button = new Rect (barStart);

                          Labels.textStyle.padding = Labels.rectZero;
                          Labels.textStyle.fontSize = 12;
                          Labels.textStyle.normal.textColor = labelColor;
                          Labels.textStyle.fontStyle = FontStyle.Bold;
                          Vector2 labelSize = Labels.textStyle.CalcSize (new GUIContent (title));

                          bool open = button.Button (texture, barColor);
                          GUI.Label (barStart.CenterRectContent (labelSize), title, Labels.textStyle);
                          Labels.ResetTextStyle ( );
                          return open;
                  }

                  public static void Box (int members, Color color, int extraHeight = 0, int yOffset = 0, int shortenX = 0)
                  {
                          Layout.VerticalSpacing (1);
                          Layout.VerticalSpacing (0);
                          int height = (Layout.rectFieldHeight) * members + extraHeight + 10; // 10 is padding
                          Layout.GetLastRectDraw (Layout.longInfoWidth - shortenX, height, -11 + shortenX * 0.5f, yOffset, background, color);
                          Layout.VerticalSpacing (5);
                  }

                  public static void BoxSingle (int members, Color color, int extraHeight = 0, int yOffset = 0)
                  {
                          Layout.VerticalSpacing (1);
                          Layout.VerticalSpacing (0);
                          int height = (Layout.rectFieldHeight) * members + extraHeight + 4; // 4 is padding
                          Layout.GetLastRectDraw (Layout.longInfoWidth, height, -11, yOffset, background, color);
                          Layout.VerticalSpacing (2);
                  }

                  public static void Box (Texture2D texture, int members, Color color, int extraHeight = 0, int yOffset = 0)
                  {
                          Layout.VerticalSpacing (1);
                          Layout.VerticalSpacing (0);
                          int height = (Layout.rectFieldHeight) * members + extraHeight + 10; // 10 is padding
                          Layout.GetLastRectDraw (Layout.longInfoWidth, height, -11, yOffset, texture, color);
                          //Layout.VerticalSpacing (5);
                  }

                  public static bool FoldOutButton (SerializedProperty property, int yOffset = 0, string toolTip = "Events")
                  {
                          Rect eventButton = Layout.CreateRect (10, 0, 0, -5 + yOffset);
                          eventButton.height = 12;
                          GUI.Label (eventButton, new GUIContent ("", toolTip));
                          bool open = eventButton.Toggle (property, Icon.Get ("TriangleBottom"), Tint.WarmWhite, Tint.PastelGreen);
                          Layout.VerticalSpacing (8 + yOffset);
                          return open;
                  }

                  public static bool FoldOutLeftButton (SerializedProperty property, Color color, int xOffset = 0, int yOffset = 0, int width = 10, int height = 20, string toolTip = "Options")
                  {
                          Rect eventButton = Layout.GetLastRect (width, height, -width + xOffset, yOffset);
                          if (toolTip != "") GUI.Label (eventButton, new GUIContent ("", toolTip));
                          return eventButton.Toggle (property, Icon.Get ("LeftButton"), color, Tint.White, center : false);
                  }

                  public static bool FoldOutButton (SerializedProperty property, string title, string message = "Events")
                  {
                          Rect eventButton = Layout.CreateRect (10, 19, -6, 0);
                          //  eventButton.height = 11;
                          GUI.Label (eventButton, new GUIContent ("", message));
                          bool open = eventButton.Toggle (property, property.boolValue ? Icon.Get ("ArrowDown") : Icon.Get ("ArrowRight"), Tint.WarmWhite, Tint.WarmWhite);
                          Labels.Label (title, eventButton.Adjust (12, 100));
                          Layout.VerticalSpacing (5);
                          return open;
                  }

                  public static bool FoldOutBoxButton (SerializedProperty property, string title, Color color, string toolTip = "")
                  {
                          BoxSingle (1, color);
                          Rect eventButton = Layout.CreateRect (10, 19, -6, 0);
                          //  eventButton.height = 11;
                          if (toolTip != "") GUI.Label (eventButton, new GUIContent ("", toolTip));
                          bool open = eventButton.Toggle (property, property.boolValue ? Icon.Get ("ArrowDown") : Icon.Get ("ArrowRight"), Tint.WarmWhite, Tint.WarmWhite);
                          Labels.Label (title, eventButton.Adjust (12, 100));
                          Layout.VerticalSpacing (2);
                          return open;
                  }

                  public static bool CornerButton (Color color, int xOffset = 0, int yOffset = 0, string icon = "Add", bool ySpace = true)
                  {
                          if (ySpace) Layout.VerticalSpacing ( );
                          Rect rect = Layout.CreateRect (21, 21);
                          Rect button = new Rect (rect) { x = rect.x + Layout.longInfoWidth - 32 - xOffset, y = rect.y - yOffset };
                          return button.Button (Icon.Get (icon), Icon.Get ("BackgroundLight"), color);
                  }

                  public static void CornerButtonLR (this SerializedProperty toggle, int xOffset = 0, int yOffset = 0, string icon = "Add") // last rect
                  {
                          Layout.VerticalSpacing ( );
                          Rect rect = Layout.GetLastRect (23, 23);
                          Rect button = new Rect (rect) { x = rect.x + Layout.longInfoWidth - 34 - xOffset, y = rect.y - 23 - yOffset };
                          if (button.Button (Icon.Get (icon), Icon.Get ("BackgroundLight"), toggle.boolValue ? Tint.SoftDark : Tint.Box)) toggle.Toggle ( );
                  }

                  public static bool DropDownMenu (List<string> names, SerializedProperty shift, SerializedProperty index, Texture2D[] iconArray = null, int itemLimit = 5)
                  {
                          for (int i = 0; i < itemLimit; i++)
                          {
                                  int trueIndex = Mathf.Clamp (i + (int) shift.floatValue, 0, names.Count);
                                  Rect background = Layout.CreateRectAndDraw (Layout.longInfoWidth - 10, 22, xOffset: -11, texture : Texture2D.whiteTexture, color : Color.white);

                                  if (trueIndex < names.Count)
                                  {
                                          if (background.ContainsMouseUp ( )) { index.intValue = trueIndex; return true; }
                                          if (background.ContainsMouse ( )) background.DrawRect (color: Tint.SoftDark50);
                                          GUI.Label (new Rect (background) { x = background.x + 25 }, names[trueIndex], Editors.FoldOut.boldStyle); // display item name

                                          if (iconArray == null && index != null && trueIndex == index.intValue)
                                          {
                                                  Skin.TextureCentered (new Rect (background) { width = 20 }, Icon.Get ("CheckMark"), new Vector2 (11, 11), Tint.SoftDark);
                                          }
                                          if (iconArray != null && trueIndex < iconArray.Length)
                                          {
                                                  Skin.TextureCentered (new Rect (background) { width = 20 }, iconArray[trueIndex], new Vector2 (13, 13), Tint.White);
                                          }
                                  }
                                  if (background.ContainsScrollWheel ( ))
                                  {
                                          float scrollValue = Mathf.Abs (Event.current.delta.y) > 3f ? 2f : 1f;
                                          shift.floatValue += Event.current.delta.y > 0 ? scrollValue : -scrollValue;
                                          shift.floatValue = Mathf.Clamp (shift.floatValue, 0, names.Count - itemLimit);
                                  }
                          }
                          // reposition shift based on scrollbar position
                          float extraItems = names.Count - itemLimit;
                          float position = (float) shift.floatValue / extraItems;
                          shift.floatValue = Mathf.Clamp (extraItems * Editors.FoldOut.ScrollBar (itemLimit, 22f, names.Count, position), 0, extraItems);
                          if (names.Count <= itemLimit) shift.floatValue = 0;
                          return false;
                  }

                  public static float ScrollBar (float items, float itemHeight, float totalCount, float percentPosition)
                  {
                          float menuHeight = items * itemHeight;
                          float barHeight = (items / (totalCount == 0 ? 1f : totalCount)) * menuHeight;
                          barHeight = Mathf.Clamp (barHeight, itemHeight, menuHeight);
                          Rect vertBar = Layout.GetLastRect (10, menuHeight, xOffset : Layout.infoWidth - 5, yOffset: -menuHeight + itemHeight); // we are recreating menu from last rect
                          Rect scrollBar = new Rect (vertBar) { width = vertBar.width, height = barHeight, y = vertBar.y + (menuHeight - barHeight) * percentPosition };
                          vertBar.DrawTexture (Texture2D.whiteTexture, Tint.SoftDark50);
                          scrollBar.DrawTexture (Texture2D.whiteTexture, Tint.SoftDark50);
                          return Mouse.MouseDrag (false) ? ((Event.current.mousePosition.y) - vertBar.y) / menuHeight : percentPosition;
                  }

          }

  }
  #endif