   #if UNITY_EDITOR//
   using TwoBitMachines.Editors;
   using UnityEditor.SceneManagement;
   using UnityEditor;
   using UnityEngine.SceneManagement;
   using UnityEngine;

   namespace TwoBitMachines
   {
           public static class SceneTools
           {
                   //source: https://forum.unity.com/threads/mimic-mecanims-grid-in-background.236205/

                   public static void ArrowDown (Vector2 position, float length, Color color)
                   {
                           Vector2 tail = position + Vector2.up * length;
                           Vector2 point = position + Vector2.down * length;
                           Vector2 left = position + Vector2.left * length;
                           Vector2 right = position + Vector2.right * length;

                           Line (point, tail, color);
                           Line (point, left, color);
                           Line (point, right, color);
                   }

                   public static void ArrowLeft (Vector2 position, float length, Color color)
                   {
                           Vector2 tail = position + Vector2.right * length;
                           Vector2 point = position + Vector2.left * length;
                           Vector2 up = position + Vector2.up * length;
                           Vector2 down = position + Vector2.down * length;

                           Line (point, tail, color);
                           Line (point, up, color);
                           Line (point, down, color);
                   }

                   public static void ArrowRight (Vector2 position, float length, Color color)
                   {
                           Vector2 tail = position + Vector2.left * length;
                           Vector2 point = position + Vector2.right * length;
                           Vector2 up = position + Vector2.up * length;
                           Vector2 down = position + Vector2.down * length;

                           Line (point, tail, color);
                           Line (point, up, color);
                           Line (point, down, color);
                   }

                   public static void ArrowUp (Vector2 position, float length, Color color, float shift = 0)
                   {
                           Vector2 tail = position + Vector2.down * length;
                           Vector2 point = position + Vector2.up * length;
                           Vector2 left = position + Vector2.left * length;
                           Vector2 right = position + Vector2.right * length;

                           Line (point, tail, color);
                           Line (point, left, color);
                           Line (point, right, color);
                   }

                   public static void Label (Vector2 position, string label, Color color)
                   {
                           Color previous = Handles.color;
                           Handles.color = color;
                           Handles.Label (position, label);
                           Handles.color = previous;
                   }

                   public static Vector2 SceneCenter (Vector2 defaultValue)
                   {
                           if (SceneView.lastActiveSceneView.camera != null) return (Vector2) SceneView.lastActiveSceneView.camera.transform.position;
                           return defaultValue;
                   }

                   public static void Circle (Vector3 center, float radius)
                   {
                           Handles.SphereHandleCap (0, center, Quaternion.identity, radius * 2, EventType.Repaint);
                   }

                   public static void DottedLine (Vector3 from, Vector3 to, Color color, float size = 8)
                   {
                           Color normal = Handles.color;
                           Handles.color = color;
                           Handles.DrawDottedLine (from, to, size);
                           Handles.color = normal;
                   }

                   public static void DottedLine (Vector3 from, Vector3 to)
                   {
                           Handles.DrawDottedLine (from, to, 5);
                   }

                   public static void Grid (Vector3 direction, Vector3 start, Vector3 end, float length, float step)
                   {
                           if (step == 0 || length == Mathf.Infinity || length == -Mathf.Infinity) return;
                           for (float i = 0; i < length; i++)
                           {
                                   Vector3 shift = direction * step * i;
                                   Handles.DrawLine (start + shift, end + shift);
                           }
                   }

                   public static void Line (Vector3 from, Vector3 to, Color color)
                   {
                           Color normal = Handles.color;
                           Handles.color = color;
                           Handles.DrawLine (from, to);
                           Handles.color = normal;
                   }

                   public static void Line (Vector3 from, Vector3 to)
                   {
                           Handles.DrawLine (from, to);
                   }

                   public static void Square (Vector2 position, Vector2 size, Color color)
                   {
                           Vector2 tL = position + Vector2.up * size.y;
                           Vector2 tR = tL + Vector2.right * size.x;
                           Vector2 bR = tR + Vector2.down * size.y;

                           Color previousColor = Handles.color;
                           Handles.color = color;
                           Handles.DrawLine (position, tL);
                           Handles.DrawLine (tL, tR);
                           Handles.DrawLine (tR, bR);
                           Handles.DrawLine (position, bR);
                           Handles.color = previousColor;
                   }

                   public static void SquareCenter (Vector2 position, Vector2 size, Color color)
                   {
                           Vector2 bL = position - size * 0.5f;
                           Vector2 tL = bL + Vector2.up * size.y;
                           Vector2 tR = tL + Vector2.right * size.x;
                           Vector2 bR = tR + Vector2.down * size.y;

                           Color previousColor = Handles.color;
                           Handles.color = color;
                           Handles.DrawLine (bL, tL);
                           Handles.DrawLine (tL, tR);
                           Handles.DrawLine (tR, bR);
                           Handles.DrawLine (bR, bL);
                           Handles.color = previousColor;
                   }

                   public static void SquareCenterBroken (Vector2 position, Vector2 size, Color color, float gap = 1)
                   {
                           Vector2 bL = position - size * 0.5f;
                           Vector2 tL = bL + Vector2.up * size.y;
                           Vector2 tR = tL + Vector2.right * size.x;
                           Vector2 bR = tR + Vector2.down * size.y;

                           Color previousColor = Handles.color;
                           Handles.color = color;
                           Handles.DrawDottedLine (bL, tL, gap);
                           Handles.DrawDottedLine (tL, tR, gap);
                           Handles.DrawDottedLine (tR, bR, gap);
                           Handles.DrawDottedLine (bR, bL, gap);
                           Handles.color = previousColor;
                   }

                   public static void SolidDisc (Vector3 center, Vector3 normal, float radius, Color color)
                   {
                           Color original = Handles.color;
                           Handles.color = color;
                           Handles.DrawSolidDisc (center, normal, radius);
                           Handles.color = original;
                   }

                   public static void TwoDGrid (Vector2 position, Vector2 gridSize, Vector2 cellSize, Color color)
                   {
                           if (cellSize.x == 0 || cellSize.y == 0) return;

                           float linesX = gridSize.x / cellSize.x + 1f;
                           float linesY = gridSize.y / cellSize.y + 1f;

                           Color previousColor = Handles.color;
                           Handles.color = color;
                           for (int i = 0; i < linesX; i++)
                           {
                                   Vector2 p = position + Vector2.right * cellSize.x * i;
                                   Handles.DrawLine (p, p + Vector2.up * gridSize.y);
                           }

                           for (int i = 0; i < linesY; i++)
                           {
                                   Vector2 p = position + Vector2.up * cellSize.y * i;
                                   Handles.DrawLine (p, p + Vector2.right * gridSize.x);
                           }
                           Handles.color = previousColor;
                   }

                   public static void WireDisc (Vector3 center, Vector3 normal, float radius, Color color)
                   {
                           Color original = Handles.color;
                           Handles.color = color;
                           Handles.DrawWireDisc (center, normal, radius);
                           Handles.color = original;
                   }

                   public static void Circle (Vector2 center, float radius, Color color, float precision = 5)
                   {
                           Vector2 centerBottom = center + Vector2.down * radius;
                           float angle = 45f / precision;
                           float c = 2f * Mathf.PI * radius;
                           float quadrant = c / 4f;
                           float segment = quadrant / precision;
                           Vector2 startPoint = centerBottom;

                           Color previousColor = Handles.color;
                           Handles.color = color;

                           for (int i = 0; i < precision * 4f; i++)
                           {
                                   float curve = 1f + (i * 2f);
                                   Vector2 direction = new Vector2 (Mathf.Cos (Mathf.Deg2Rad * angle * curve) * segment, Mathf.Sin (Mathf.Deg2Rad * angle * curve) * segment);
                                   Handles.DrawLine (startPoint, startPoint + direction);
                                   startPoint += direction;
                           }

                           Handles.color = previousColor;

                   }

                   public static bool NearMouse (Vector2 position, float distance = 1f)
                   {
                           return (position - (Vector2) Mouse.position).sqrMagnitude < distance * distance;
                   }

                   public static Vector2 MousePosition ( )
                   {
                           if (SceneView.currentDrawingSceneView != null && SceneView.currentDrawingSceneView.camera != null)
                           {

                                   Vector2 position = Event.current.mousePosition;
                                   position.y = SceneView.currentDrawingSceneView.camera.pixelHeight - position.y;
                                   return SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint (position);
                           }
                           return Vector2.zero;
                   }

                   public static void DrawAndModifyBounds (ref Vector2 position, ref Vector2 size, Color color, float handleSize = 0.25f, bool draw = true, bool move = true)
                   {
                           Vector2 rightHandle = position + Vector2.right * size.x + Vector2.up * size.y * 0.5f;
                           Vector2 topHandle = position + Vector2.right * size.x * 0.5f + Vector2.up * size.y;

                           if (move) position = MovePositionDotHandle (position, Vector2.one, handleSize : handleSize, snap : 0.25f);
                           size = SlideHandle (size, rightHandle, true, new Vector2 (-1, 0), handleSize : handleSize);
                           size = SlideHandle (size, topHandle, false, new Vector2 (0, -1), handleSize : handleSize);

                           size = Compute.Clamp (size, 0.5f);
                           if (!move) position = Compute.Round (position, 0.125f);
                           if (draw) Square (position, size, color);
                   }

                   public static bool UpperRightButton (Vector2 position, Vector2 size, Color color, float handleSize = 0.25f)
                   {
                           Vector2 corner = position + Vector2.right * size.x + Vector2.up * size.y + Vector2.one * -handleSize;
                           Color previous = Handles.color;
                           Handles.color = color;
                           bool pressed = Handles.Button (corner, Quaternion.identity, handleSize, handleSize * 2f, Handles.DotHandleCap);
                           Handles.color = previous;
                           return pressed;
                   }

                   public static void ModifyPercentH (ref float percent, Vector2 position, Vector2 size, Color color, float direction = 1f, float handleSize = 0.25f)
                   {
                           Vector2 origin = position;
                           Vector2 handle = origin + Vector2.right * direction * (size.x * 0.5f * percent);

                           Color previous = Handles.color;
                           Handles.color = color;
                           Vector2 value = handle + new Vector2 (direction, 0) * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.RectangleHandleCap);
                           if (percent > 0) Handles.DrawLine (handle + Vector2.down * size.y * 0.5f, handle + Vector2.up * size.y * 0.5f);
                           if (moved != value)
                           {
                                   if (direction > 0) percent = moved.x <= origin.x ? 0 : Mathf.Abs (moved.x - origin.x) / (size.x * 0.5f);
                                   if (direction < 0) percent = moved.x > origin.x ? 0 : Mathf.Abs (moved.x - origin.x) / (size.x * 0.5f);
                           }
                           percent = Compute.Round (percent, 0.001f);
                           if (percent < 0) percent = 0;
                           Handles.color = previous;
                   }

                   public static void ModifyPercentV (ref float percent, Vector2 position, Vector2 size, Color color, float direction = 1f, float handleSize = 0.25f)
                   {
                           Vector2 origin = position;
                           Vector2 handle = origin + Vector2.up * direction * (size.y * 0.5f * percent);

                           Color previous = Handles.color;
                           Handles.color = color;
                           Vector2 value = handle + new Vector2 (0, direction) * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.RectangleHandleCap);
                           if (percent > 0) Handles.DrawLine (handle + Vector2.right * size.x * 0.5f, handle - Vector2.right * size.x * 0.5f);
                           if (moved != value)
                           {
                                   if (direction > 0) percent = moved.y <= origin.y ? 0 : Mathf.Abs (moved.y - origin.y) / (size.y * 0.5f);
                                   if (direction < 0) percent = moved.y > origin.y ? 0 : Mathf.Abs (moved.y - origin.y) / (size.y * 0.5f);
                           }
                           percent = Compute.Round (percent, 0.001f);
                           if (percent < 0) percent = 0;
                           Handles.color = previous;
                   }

                   public static bool MovePositionHandle (SerializedProperty position, Handles.CapFunction handle, Vector2 offset, float snap = 0.125f, float handleSize = 0.25f, bool repaint = true, UnityEditor.Editor editor = null)
                   {
                           Vector2 value = position.vector2Value + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, handle);
                           if (value != moved)
                           {
                                   if (repaint && editor != null)
                                           editor.Repaint ( );
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   position.vector2Value = moved;
                                   return true;
                           }
                           return false;
                   }

                   public static Vector2 MovePositionHandle (Vector2 position, Vector2 offset, float snap = 0.125f, float handleSize = 0.25f, bool repaint = true, UnityEditor.Editor editor = null)
                   {
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.RectangleHandleCap);
                           if (value != moved)
                           {
                                   if (repaint && editor != null)
                                           editor.Repaint ( );
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   return position = moved;
                           }
                           return position;
                   }

                   public static Vector2 MovePositionDotHandle (Vector2 position, Vector2 offset = default (Vector2), float snap = 0.5f, float handleSize = 0.5f, bool repaint = true, UnityEditor.Editor editor = null)
                   {
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.DotHandleCap);

                           if (value != moved)
                           {
                                   if (repaint && editor != null)
                                   {
                                           editor.Repaint ( );
                                   }
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   return position = moved;
                           }
                           return position;
                   }

                   public static Vector2 MovePositionCircleHandle (Vector2 position, Vector2 offset = default (Vector2), float snap = 0.5f, float handleSize = 0.5f, bool repaint = true, UnityEditor.Editor editor = null)
                   {
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.CircleHandleCap);

                           if (value != moved)
                           {
                                   if (repaint && editor != null)
                                   {
                                           editor.Repaint ( );
                                   }
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   return position = moved;
                           }
                           return position;
                   }

                   public static Vector2 MovePositionCircleHandle (Vector2 position, Color color, float snap = 0.25f, float handleSize = 0.75f)
                   {
                           Color previousColor = Handles.color;
                           Handles.color = color;
                           Vector2 value = position;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.CircleHandleCap);
                           Handles.color = previousColor;
                           SceneTools.Circle (value, handleSize * 0.4f, Color.white);
                           if (value != moved)
                           {
                                   moved = Compute.Round (moved, snap);
                                   return position = moved;
                           }
                           return position;
                   }

                   public static Vector2 MovePositionCircleHandle (Vector2 position, Vector2 offset, Color color, out bool changed, float snap = 0.125f, float handleSize = 0.75f)
                   {
                           changed = false;
                           Color previousColor = Handles.color;
                           Handles.color = color;
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.CircleHandleCap);
                           Handles.color = previousColor;
                           SceneTools.Circle (value, handleSize * 0.4f, Color.white);
                           if (value != moved)
                           {
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   changed = true;
                                   return position = moved;
                           }
                           return position;
                   }

                   public static void MoveSceneCamera (Vector3 targetPosition)
                   {
                           if (SceneView.lastActiveSceneView == null)
                                   return;
                           SceneView.lastActiveSceneView.pivot = targetPosition;
                           SceneView.lastActiveSceneView.Repaint ( );
                   }

                   public static void MoveSceneCamera (SerializedProperty element)
                   {
                           if (SceneView.lastActiveSceneView == null)
                                   return;
                           Vector2 position = element.Get ("bounds").Get ("position").vector2Value;
                           Vector2 size = element.Get ("bounds").Get ("size").vector2Value;
                           Vector2 targetPosition = position + size * 0.5f;
                           SceneView.lastActiveSceneView.pivot = targetPosition;
                           SceneView.lastActiveSceneView.Repaint ( );
                   }

                   public static void SlideHandle (Vector2 position, SerializedProperty size, Handles.CapFunction handle, bool xAxis, Vector2 offset = default (Vector2), float snap = 0.125f, float handleSize = 0.25f)
                   {
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, handle);
                           if (value != moved)
                           {
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   if (xAxis)
                                           size.vector2Value = new Vector2 (size.vector2Value.x + (moved.x - value.x), size.vector2Value.y);
                                   else
                                           size.vector2Value = new Vector2 (size.vector2Value.x, size.vector2Value.y + (moved.y - value.y));
                           }
                   }

                   public static Vector2 SlideHandle (Vector2 size, Vector2 position, bool xAxis, Vector2 offset = default (Vector2), float snap = 0.125f, float handleSize = 0.25f)
                   {
                           Vector2 value = position + offset * handleSize;
                           Vector2 moved = Handles.FreeMoveHandle (value, Quaternion.identity, handleSize, Vector3.one, Handles.RectangleHandleCap);
                           if (value != moved)
                           {
                                   moved = Compute.Round (moved, snap) - offset * snap;
                                   if (xAxis)
                                           size = new Vector2 (size.x + (moved.x - value.x), size.y);
                                   else
                                           size = new Vector2 (size.x, size.y + (moved.y - value.y));
                           }
                           return size;
                   }

                   public static void Repaint<T> ( )
                   {
                           UnityEngine.Object[] objects = GameObject.FindObjectsOfType (typeof (T));
                           for (int i = 0; i < objects.Length; i++)
                           {
                                   Repaint (objects[i]);
                           }
                   }

                   public static void Repaint (UnityEngine.Object target)
                   {
                           if (target != null) EditorUtility.SetDirty (target);
                           if (!EditorApplication.isPlaying) EditorSceneManager.MarkSceneDirty (SceneManager.GetActiveScene ( ));
                           SceneView.RepaintAll ( );
                   }

           }
   }
   #endif