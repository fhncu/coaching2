  #if UNITY_EDITOR
  using UnityEditor;
  using UnityEngine;

  namespace TwoBitMachines.Editors
  {
          public static class ScrollView
          {
                  public static int scrollWidth = 12;
                  public static Rect xScrollRect = new Rect ( );
                  public static Rect yScrollRect = new Rect ( );
                  public static float xAxis;
                  public static float yAxis;
                  public static int controlID;
                  public static bool mouseDownX = false;
                  public static bool mouseDownY = false;
                  public static Vector2 relativePoint;

                  public static Vector2 Begin (Rect viewRect, SerializedProperty scrollPosition, Rect targetTextureRect, Color backgroundColor)
                  {
                          controlID = GUIUtility.GetControlID (1005, FocusType.Passive);

                          Skin.DrawRect (viewRect, color : backgroundColor);
                          GUI.BeginGroup (viewRect);

                          Vector2 position = scrollPosition.vector2Value;
                          xAxis = viewRect.width / targetTextureRect.width;
                          yAxis = viewRect.height / targetTextureRect.height;
                          float deltaHeight = Mathf.Abs (viewRect.height - targetTextureRect.height);
                          float deltaWidth = Mathf.Abs (viewRect.width - targetTextureRect.width);

                          if (xAxis < 1)
                          {
                                  Rect xPositionRect = new Rect (viewRect) { y = viewRect.height - scrollWidth, height = scrollWidth };
                                  position.x = Mathf.Clamp (position.x, 0, deltaWidth);
                                  float x = position.x * xAxis;
                                  int adjustXWidth = yAxis < 1 ? scrollWidth : 0;
                                  xScrollRect = new Rect (xPositionRect) { x = x, width = xAxis * viewRect.width - adjustXWidth };
                          }

                          if (yAxis < 1)
                          {
                                  Rect yPositionRect = new Rect (viewRect) { x = viewRect.width - scrollWidth, width = scrollWidth };
                                  position.y = Mathf.Clamp (position.y, 0, deltaHeight);
                                  float y = position.y * yAxis;
                                  int adjustYHeight = xAxis < 1 ? scrollWidth : 0;
                                  yScrollRect = new Rect (yPositionRect) { y = y, height = yAxis * viewRect.height - adjustYHeight };
                          }

                          switch (Event.current.GetTypeForControl (controlID))
                          {
                                  case EventType.MouseDown:
                                          if (xAxis < 1 && xScrollRect.Contains (Event.current.mousePosition) && Event.current.button == 0)
                                          {
                                                  GUIUtility.hotControl = controlID;
                                                  relativePoint = Event.current.mousePosition;
                                                  mouseDownX = true;
                                                  GUI.changed = true;
                                                  Event.current.Use ( );
                                          }
                                          if (yAxis < 1 && yScrollRect.Contains (Event.current.mousePosition) && Event.current.button == 0)
                                          {
                                                  GUIUtility.hotControl = controlID;
                                                  relativePoint = Event.current.mousePosition;
                                                  mouseDownY = true;
                                                  GUI.changed = true;
                                                  Event.current.Use ( );
                                          }
                                          break;

                                  case EventType.MouseUp:
                                          if (GUIUtility.hotControl == controlID)
                                          {
                                                  GUIUtility.hotControl = 0;
                                                  mouseDownX = false;
                                                  mouseDownY = false;
                                                  GUI.changed = true;
                                                  Event.current.Use ( );
                                          }
                                          break;
                          }

                          if (Event.current.isMouse && GUIUtility.hotControl == controlID)
                          {
                                  if (xAxis < 1 && mouseDownX)
                                  {
                                          float deltaX = Event.current.mousePosition.x - relativePoint.x;
                                          relativePoint.x = Event.current.mousePosition.x;
                                          position.x += (deltaX / xAxis);
                                          position.x = Mathf.Clamp (position.x, 0, deltaWidth);
                                  }
                                  if (yAxis < 1 && mouseDownY)
                                  {
                                          float deltaY = Event.current.mousePosition.y - relativePoint.y;
                                          relativePoint.y = Event.current.mousePosition.y;
                                          position.y += (deltaY / yAxis);
                                          position.y = Mathf.Clamp (position.y, 0, deltaHeight);
                                  }
                          }

                          scrollPosition.vector2Value = position;
                          return position;
                  }

                  public static void End ( )
                  {

                          if (Event.current.type == EventType.Repaint)
                          {
                                  if (xAxis < 1)
                                  {
                                          Color color = mouseDownX && GUIUtility.hotControl == controlID ? Tint.SoftDark150 : Tint.SoftDarkA;
                                          Skin.DrawTexture (xScrollRect, Texture2D.whiteTexture, color);
                                  }
                                  if (yAxis < 1)
                                  {
                                          Color color = mouseDownY && GUIUtility.hotControl == controlID ? Tint.SoftDark150 : Tint.SoftDarkA;
                                          Skin.DrawTexture (yScrollRect, Texture2D.whiteTexture, color);
                                  }
                          }

                          if (Event.current.isMouse && GUIUtility.hotControl == controlID)
                          {
                                  GUI.changed = true;
                                  Event.current.Use ( );
                          }
                          GUI.EndGroup ( );
                  }
          }

          public static class VertScroll
          {
                  public static float previousM = 0;
                  public static int barWidth = 15;

                  public static void Scroll (Rect rect, int items, int rows, int rowHeight, ref int shift)
                  {
                          if (items <= 2 || items <= rows) return;
                          float barHeight = rowHeight * 2;
                          int excessItems = items - rows;
                          float scrollIncrement = (rows * rowHeight - barHeight) / excessItems;

                          Rect barArea = new Rect (rect) { x = rect.x + rect.width - barWidth, width = barWidth, height = rowHeight * rows };
                          float y = Mathf.Clamp (rect.y + scrollIncrement * shift, rect.y, rect.y + rows * rowHeight - barHeight);
                          Rect bar = new Rect (barArea) { y = y, height = barHeight }; // fiux bar height at two rows

                          if (rect.y != 0 && barArea.ContainsMouseDown ( ))
                          {
                                  previousM = Event.current.mousePosition.y;
                                  bar.y = Mathf.Clamp (Event.current.mousePosition.y - barHeight / 2f, rect.y, rect.y + rows * rowHeight - barHeight); // if clicked, place bar middle at mouse position
                                  float percent = Mathf.Abs (rect.y + barHeight - bar.y) / barHeight;
                                  int shifted = (int) (excessItems * (1f - percent));
                                  shift = Mathf.Clamp (shifted, 0, items - rows);
                          }
                          if (rect.y != 0 && bar.ContainsMouseDrag ( ))
                          {
                                  float mouseDiff = Event.current.mousePosition.y - previousM;
                                  if (Mathf.Abs (mouseDiff) >= scrollIncrement)
                                  {
                                          shift = Mathf.Clamp (shift + (int) Mathf.Sign (mouseDiff), 0, items - rows);
                                          bar.y = Mathf.Clamp (rect.y + scrollIncrement * shift, rect.y, rect.y + rows * rowHeight - barHeight);
                                          previousM = Event.current.mousePosition.y;
                                  }
                          }
                          Skin.Draw (bar, Tint.Grey50, Skin.square);
                  }
          }

  }
  #endif