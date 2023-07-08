  #if UNITY_EDITOR
  using UnityEditor;
  using UnityEngine;

  namespace TwoBitMachines.Editors
  {
          public static class Layout
          {
                  public static int shorten = 0;
                  // public static int maxWidth => Screen.width - (25 + shorten);
                  public static int maxWidth => (int) EditorGUIUtility.currentViewWidth - (25 + shorten);
                  public static int infoWidth => maxWidth - 12;
                  public static int longInfoWidth => infoWidth + 16;
                  public static float labelWidth;
                  public static float contentWidth;
                  public static float buttonWidth = 15f;
                  public static float boolWidth = 15f;
                  public static int rectFieldHeight = 19; //8;
                  public static int rectVertSpace = 2;
                  public static bool previousGUIEnabled = false;
                  public static float rectTempWidth = 0;
                  public static float totalWidth => labelWidth + contentWidth;
                  public static float half => contentWidth * 0.5f;
                  public static float thrice => contentWidth * 0.333f;
                  public static float quarter => contentWidth * 0.25f;
                  public static float quint => contentWidth * 0.2f;
                  private static bool initiated = false;

                  public static void Initialize ( )
                  {
                          if (initiated)
                          {
                                  return; // save load time
                          }
                          EditorTools.LoadGUI ("TwoBitMachines", "/Tools/Icons", Icon.icon);
                          Skin.Set ( );
                          Labels.InitializeStyle ( );
                          FoldOut.Initialize ( );
                          initiated = true;
                  }

                  public static void Update (float labelWidthPercent = 0.45f)
                  {
                          labelWidthPercent = Mathf.Clamp01 (labelWidthPercent);
                          labelWidth = infoWidth * labelWidthPercent;
                          contentWidth = infoWidth * (1 - labelWidthPercent);
                          Labels.useWhite = EditorGUIUtility.isProSkin;
                          Mouse.Update ( );
                  }

                  public static void BeginGUIEnable (bool toggleState)
                  {
                          previousGUIEnabled = GUI.enabled;
                          GUI.enabled = toggleState && previousGUIEnabled;
                  }
                  public static void EndGUIEnable (Rect rect = default (Rect), SerializedProperty toggle = null)
                  {
                          GUI.enabled = previousGUIEnabled;
                          if (toggle != null) EditorGUI.PropertyField (rect, toggle, GUIContent.none);
                  }
                  public static void UseEvent ( )
                  {
                          GUI.changed = true;
                          Event.current.Use ( );
                  }

                  #region Get Rects
                  public static Rect CreateRect (float width, float height, float xOffset = 0, float yOffset = 0)
                  {
                          return Set (GUILayoutUtility.GetRect (0, height), width, height, xOffset, yOffset);
                  }

                  public static Rect CreateRectAndDraw (float width, float height, float xOffset = 0, float yOffset = 0, Texture2D texture = default (Texture2D), Color color = default (Color))
                  {
                          // creating a gui rect that is near the same size as layout maxwidth will trigger a horizontal bar :( but this only happens if it's inside a guilayout area
                          // to avoid all issues in any scenario, create gui with an initial witdh of zero, then reset to desired size. this seems to fix the problem.
                          Rect rect = Set (GUILayoutUtility.GetRect (0, height), width, height, xOffset, yOffset);
                          Skin.Draw (rect, color == Color.clear ? Color.white : color, texture);
                          return rect;
                  }
                  public static Rect CreateRectField (float xOffset = 0, float yOffset = 0)
                  {
                          Rect rect = Set (GUILayoutUtility.GetRect (0, rectFieldHeight), infoWidth, rectFieldHeight, xOffset - 3, yOffset);
                          rect.height -= 2;
                          rect.y += 1;
                          return rect;
                  }

                  public static Rect GetLastRect (float width, float height, float xOffset = 0, float yOffset = 0)
                  {
                          return Set (GUILayoutUtility.GetLastRect ( ), width, height, xOffset, yOffset);
                  }

                  public static Rect GetLastRectDraw (float width, float height, float xOffset = 0, float yOffset = 0, Texture2D texture = default (Texture2D), Color color = default (Color))
                  {
                          Rect rect = Set (GUILayoutUtility.GetLastRect ( ), width, height, xOffset, yOffset);
                          Skin.Draw (rect, color == Color.clear ? Color.white : color, texture);
                          return rect;
                  }

                  public static Rect Box (Color color, int height = FoldOut.h, int yOffset = 0)
                  {
                          VerticalSpacing (1);
                          Rect rect = CreateRectAndDraw (longInfoWidth, height, -11, yOffset, Icon.Get ("BackgroundLight128x128"), color);
                          return rect.CenterRectHeight (rectFieldHeight);
                  }

                  public static Rect CenterRectContent (this ref Rect rect, Vector2 contentSize)
                  {
                          rect.x += (rect.width / 2f) - (contentSize.x / 2f);
                          rect.y += (rect.height / 2f) - (contentSize.y / 2f);
                          rect.width = contentSize.x;
                          rect.height = contentSize.y;
                          return rect;
                  }

                  public static Rect CenterRectHeight (this ref Rect rect, float height = 17)
                  {
                          rect.y += (rect.height / 2f) - (height / 2f);
                          rect.height = height;
                          return rect;
                  }

                  public static Rect Set (Rect rect, float width, float height, float xOffset, float yOffset)
                  {
                          rect.width = width;
                          rect.height = height;
                          rect.x += xOffset;
                          rect.y += yOffset;
                          return rect;
                  }

                  public static Rect Offset (this ref Rect rect, float xOffset, float yOffset, float addWidth, float addHeight)
                  {
                          return rect = Set (rect, rect.width + addWidth, rect.height + addHeight, xOffset, yOffset);
                  }

                  public static Rect Adjust (this ref Rect rect, float xOffset, float width)
                  {
                          return rect = Set (rect, width, rect.height, xOffset, 0);
                  }

                  public static void Pushclear ( )
                  {
                          rectTempWidth = 0;
                  }

                  public static Rect Push (this ref Rect rect, float width = 15f)
                  {
                          float xOffset = rectTempWidth;
                          return rect = Set (rect, rectTempWidth = width, rect.height, xOffset, 0);
                  }

                  public static Rect OffsetX (this ref Rect rect, float xOffset)
                  {
                          return rect = Set (rect, rect.width, rect.height, xOffset, 0);
                  }

                  public static void VerticalSpacing (int height = 1)
                  {
                          GUILayoutUtility.GetRect (0, height);
                  }

                  #endregion
          }

  }
  #endif