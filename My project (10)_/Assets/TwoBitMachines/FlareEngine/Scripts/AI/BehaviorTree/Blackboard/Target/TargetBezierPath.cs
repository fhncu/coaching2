#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]

        public class TargetBezierPath : Blackboard
        {
                [SerializeField] public BezierPathType onCompletePath;
                [SerializeField] public int endlessLimit = 5;
                [SerializeField] public List<CubicBezier> path = new List<CubicBezier> ( );

                public bool endless => onCompletePath == BezierPathType.Endless || onCompletePath == BezierPathType.EndlessReverse;

                public Vector2 FollowPath (Vector2 position, FollowBezierPath follower, out NodeState complete)
                {
                        complete = NodeState.Running;

                        if (index >= 0 && index < path.Count)
                        {
                                if (follower.counter >= follower.time)
                                {
                                        if (onCompletePath == BezierPathType.Stop)
                                        {
                                                follower.counter = 0;
                                                if (index == path.Count - 1)
                                                {
                                                        follower.counter = follower.time;
                                                        complete = NodeState.Success;
                                                }
                                                index = Mathf.Min (index + 1, path.Count - 1);
                                        }
                                        else if (onCompletePath == BezierPathType.Restart)
                                        {
                                                follower.counter = 0;
                                                index = index + 1 >= path.Count ? 0 : index + 1;
                                        }
                                        else if (onCompletePath == BezierPathType.Reverse)
                                        {
                                                follower.counter = 0;
                                                if (!follower.inReverse)
                                                {
                                                        if (index == path.Count - 1)
                                                                follower.inReverse = true;
                                                        else
                                                                index = Mathf.Clamp (index + 1, 0, path.Count - 1);
                                                }
                                                else
                                                {
                                                        if (index == 0)
                                                                follower.inReverse = false;
                                                        else
                                                                index = Mathf.Clamp (index - 1, 0, path.Count - 1);
                                                }
                                        }
                                        else if (onCompletePath == BezierPathType.ReverseAndStop)
                                        {
                                                follower.counter = 0;
                                                if (!follower.inReverse)
                                                {
                                                        if (index == path.Count - 1)
                                                                follower.inReverse = true;
                                                        else
                                                                index = Mathf.Clamp (index + 1, 0, path.Count - 1);
                                                }
                                                else
                                                {
                                                        if (index == 0)
                                                        {
                                                                follower.inReverse = false;
                                                                complete = NodeState.Success;
                                                        }
                                                        else
                                                                index = Mathf.Clamp (index - 1, 0, path.Count - 1);
                                                }
                                        }
                                        else if (onCompletePath == BezierPathType.Endless)
                                        {
                                                follower.counter = 0;
                                                index = index + 1 >= path.Count ? 0 : index + 1;
                                                if (index == 0)
                                                {
                                                        follower.shift++;
                                                }
                                        }
                                        else if (onCompletePath == BezierPathType.EndlessReverse)
                                        {
                                                follower.counter = 0;
                                                if (!follower.inReverse)
                                                {
                                                        if (follower.shift == endlessLimit && index == path.Count - 1)
                                                        {
                                                                follower.inReverse = true;
                                                        }
                                                        else
                                                        {
                                                                index = index + 1 >= path.Count ? 0 : index + 1;
                                                                if (index == 0) follower.shift++;
                                                        }
                                                }
                                                else
                                                {
                                                        if (follower.shift == 0 && index == 0)
                                                        {
                                                                follower.inReverse = false;
                                                        }
                                                        else
                                                        {
                                                                index = index - 1 < 0 ? path.Count - 1 : index - 1;
                                                                if (index == path.Count - 1) follower.shift--;
                                                        }
                                                }
                                        }
                                }

                                float t = follower.counter / follower.time;
                                Vector2 target = follower.inReverse? path[index].FollowReverse (t) : path[index].Follow (t);
                                if (endless) target += Shift ( ) * follower.shift;
                                follower.counter = Mathf.Clamp (follower.counter + Time.deltaTime, 0, follower.time);

                                return target;
                        }
                        return position;
                }

                public Vector2 Shift ( )
                {
                        return path[path.Count - 1].end - path[0].start;
                }

                public enum BezierPathType
                {
                        Restart,
                        Stop,
                        Reverse,
                        ReverseAndStop,
                        Endless,
                        EndlessReverse
                }

                #region ▀▄▀▄▀▄ Editor ▄▀▄▀▄▀
                #if UNITY_EDITOR
                [SerializeField, HideInInspector] public Vector3 oldPosition;

                public override void OnSceneGUI (UnityEditor.Editor editor)
                {
                        if (!Application.isPlaying) this.transform.position = Compute.Round (this.transform.position, 0.25f);
                        SerializedObject parent = new SerializedObject (this);
                        parent.Update ( );
                        {
                                SerializedProperty path = parent.FindProperty ("path");
                                Vector3 newPosition = this.transform.position;
                                if ((oldPosition.x != newPosition.x || oldPosition.y != newPosition.y) && !Application.isPlaying) // && TwoBitMachines.Editors.Mouse.ctrl)
                                {
                                        Vector2 move = (Vector2) (newPosition - oldPosition);

                                        for (int i = 0; i < path.arraySize; i++)
                                        {
                                                SerializedProperty p = path.Element (i);
                                                p.Get ("end").vector2Value += move;
                                                p.Get ("start").vector2Value += move;
                                                p.Get ("control1").vector2Value += move;
                                                p.Get ("control2").vector2Value += move;

                                                SceneTools.Line (p.Get ("start").vector2Value, p.Get ("control1").vector2Value, Color.yellow);
                                                SceneTools.Line (p.Get ("end").vector2Value, p.Get ("control2").vector2Value, Color.yellow);
                                                Handles.DrawBezier (p.Get ("start").vector2Value, p.Get ("end").vector2Value, p.Get ("control1").vector2Value, p.Get ("control2").vector2Value, Color.green, Texture2D.whiteTexture, 2.5f);
                                        }
                                }
                                else
                                {

                                        for (int i = 0; i < path.arraySize; i++)
                                        {
                                                SerializedProperty p = path.Element (i);
                                                if (!p.Bool ("init"))
                                                {
                                                        p.SetTrue ("init");
                                                        p.Get ("start").vector2Value = i > 0 ? path.Element (i - 1).Get ("end").vector2Value : SceneTools.SceneCenter (p.Get ("start").vector2Value);
                                                        p.Get ("start").vector2Value = Compute.Round (p.Get ("start").vector2Value, 0.25f);
                                                        p.Get ("end").vector2Value = p.Get ("start").vector2Value + Vector2.right * 5f;
                                                        p.Get ("control1").vector2Value = p.Get ("start").vector2Value + Vector2.up * 5f;
                                                        p.Get ("control2").vector2Value = p.Get ("end").vector2Value + Vector2.up * 5f;
                                                }
                                                if (i > 0)
                                                {
                                                        path.Element (i - 1).Get ("end").vector2Value = p.Get ("start").vector2Value;
                                                }

                                                p.Get ("end").vector2Value = SceneTools.MovePositionCircleHandle (p.Get ("end").vector2Value, Color.red, snap : 0.25f, handleSize : 0.5f);
                                                p.Get ("start").vector2Value = SceneTools.MovePositionCircleHandle (p.Get ("start").vector2Value, Color.green, snap : 0.25f, handleSize : 0.5f);
                                                p.Get ("control1").vector2Value = SceneTools.MovePositionCircleHandle (p.Get ("control1").vector2Value, Color.blue, snap : 0.25f, handleSize : 0.35f);
                                                p.Get ("control2").vector2Value = SceneTools.MovePositionCircleHandle (p.Get ("control2").vector2Value, Color.blue, snap : 0.25f, handleSize : 0.35f);

                                                SceneTools.Line (p.Get ("start").vector2Value, p.Get ("control1").vector2Value, Color.yellow);
                                                SceneTools.Line (p.Get ("end").vector2Value, p.Get ("control2").vector2Value, Color.yellow);
                                                Handles.DrawBezier (p.Get ("start").vector2Value, p.Get ("end").vector2Value, p.Get ("control1").vector2Value, p.Get ("control2").vector2Value, Color.green, Texture2D.whiteTexture, 2.5f);
                                        }
                                }
                                parent.FindProperty ("oldPosition").vector3Value = newPosition;
                        }
                        parent.ApplyModifiedProperties ( );
                }
                #endif
                #endregion
        }

        [System.Serializable]
        public class CubicBezier
        {
                [SerializeField] public Vector2 start;
                [SerializeField] public Vector2 end;
                [SerializeField] public Vector2 control1;
                [SerializeField] public Vector2 control2;
                [SerializeField] public bool init = false;

                public Vector3 Follow (float t)
                {
                        float u = 1f - t;
                        float tt = t * t;
                        float uu = u * u;

                        Vector2 p = uu * u * start;
                        p += 3f * uu * t * control1;
                        p += 3f * u * tt * control2;
                        p += tt * t * end;
                        return p;
                }

                public Vector3 FollowReverse (float t)
                {
                        float u = 1f - t;
                        float tt = t * t;
                        float uu = u * u;

                        Vector2 p = uu * u * end;
                        p += 3f * uu * t * control2;
                        p += 3f * u * tt * control1;
                        p += tt * t * start;
                        return p;
                }
        }
}