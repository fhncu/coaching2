  #if UNITY_EDITOR
  using UnityEditor;
  using UnityEngine;

  namespace TwoBitMachines.Editors
  {
          public static class Buttons
          {
                  public static bool Toggle (this Rect rect, SerializedProperty property, Texture2D icon, Color on, Color off, bool center = true)
                  {
                          if (ButtonRect.Get (rect, icon, property.boolValue ? on : off, isSelected : false, center : center))
                          {
                                  property.Toggle ( );
                          }
                          return property.boolValue;
                  }

                  public static bool Button (this Rect rect, Texture2D icon, Texture2D background, Color color, bool isSelected = false, bool center = false)
                  {
                          bool button = ButtonRect.Get (rect, background, color, isSelected, center);
                          rect.CenterRectContent (new Vector2 (icon.width, icon.height));
                          Skin.DrawTexture (rect, icon, Tint.White);
                          return button;
                  }

                  public static bool Button (this Rect rect, string name, Texture2D background, Color color, LabelType type, bool isSelected, bool center = false)
                  {
                          bool button = ButtonRect.Get (rect, background, color, isSelected, center);
                          Labels.LabelCenterWidth (name, rect, type);
                          return button;
                  }

                  public static bool Button (this Rect rect, Texture2D icon, Color color, bool isSelected = false, bool center = false, string toolTip = "")
                  {
                          return ButtonRect.Get (rect, icon, color, isSelected, center, toolTip);
                  }
          }

  }
  #endif