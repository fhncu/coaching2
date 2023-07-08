﻿#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class TargetPoints : Blackboard
        {
                [SerializeField] public TargetFindType findType;
                [SerializeField] public Vector2 randomOffset;
                [SerializeField] public bool loopSequence = true;
                [SerializeField] public List<Vector2> point = new List<Vector2> ( );

                [System.NonSerialized] private bool reversing = false;
                [System.NonSerialized] private int previousRandom = -1;

                public override Vector2 GetTarget ( )
                {
                        if (findType == TargetFindType.FindNearest)
                        {
                                return GetNearestTarget (transform.position);
                        }
                        else if (findType == TargetFindType.GetRandom)
                        {
                                return GetRandomTarget ( );
                        }
                        else if (index >= 0 && index < point.Count)
                        {
                                return RandomOffset (index);
                        }
                        return transform.position;
                }

                public override NodeState NextTarget (ref Vector3 target)
                {
                        if (findType != TargetFindType.FollowSequence)
                        {
                                return NodeState.Success;
                        }
                        if (!loopSequence)
                        {
                                if (reversing)
                                {
                                        bool complete = index - 1 < 0;
                                        index = complete ? 0 : index - 1;
                                        target = RandomOffset (index);
                                        if (complete) reversing = false;
                                        return complete? NodeState.Success : NodeState.Running;
                                }
                                else
                                {
                                        bool complete = index + 1 >= point.Count;
                                        index = complete ? point.Count - 1 : index + 1;
                                        target = RandomOffset (index);
                                        if (complete) reversing = true;
                                        return complete? NodeState.Success : NodeState.Running;
                                }
                        }
                        else
                        {
                                bool complete = index + 1 >= point.Count;
                                index = complete ? 0 : index + 1;
                                target = RandomOffset (index);
                                return complete? NodeState.Success : NodeState.Running;
                        }
                }

                public override Vector2 GetNearestTarget (Vector2 position)
                {
                        Vector2 target = transform.position;
                        float sqrMagnitude = Mathf.Infinity;
                        for (int i = 0; i < point.Count; i++)
                        {
                                float squareDistance = (position - (Vector2) point[i]).sqrMagnitude;
                                if (squareDistance < sqrMagnitude)
                                {
                                        sqrMagnitude = squareDistance;
                                        target = RandomOffset (i);
                                }
                        }
                        return target;
                }

                public override Vector2 GetRandomTarget ( )
                {
                        return point.Count > 0 ? RandomOffset (Randomize ( )) : (Vector2) transform.position;
                }

                public override Vector3 GetVector ( )
                {
                        return point.Count > 0 ? (Vector3) point[point.Count - 1] : Vector3.zero;
                }

                public override bool AddToList (Vector3 newItem)
                {
                        if (newItem == null) return false;
                        point.Add (newItem);
                        return true;
                }

                public override bool RemoveFromList (Vector3 vector)
                {
                        for (int i = 0; i < point.Count; i++)
                        {
                                if (point[i] == (Vector2) vector)
                                {
                                        point.RemoveAt (i);
                                        return true;
                                }
                        }
                        return false;
                }

                private Vector2 RandomOffset (int index)
                {
                        Vector2 newPoint = point[index];
                        if (randomOffset.x != 0) newPoint.x += Random.Range (-randomOffset.x, randomOffset.x);
                        if (randomOffset.y != 0) newPoint.y += Random.Range (-randomOffset.y, randomOffset.y);
                        return newPoint;
                }

                private int Randomize ( )
                {
                        int newRand = Random.Range (0, point.Count);
                        if (newRand != previousRandom || point.Count == 1) return previousRandom = newRand;
                        newRand = newRand + 1 < point.Count ? newRand + 1 : 0;
                        return previousRandom = newRand;
                }

                public override int ListCount ( ) { return point.Count; }

                #region
                #if UNITY_EDITOR
                [SerializeField, HideInInspector] public Vector3 oldPosition;
                public override void OnSceneGUI (UnityEditor.Editor editor)
                {
                        if (!Application.isPlaying) this.transform.position = Compute.Round (this.transform.position, 0.25f);
                        SerializedObject parent = new SerializedObject (this);
                        parent.Update ( );
                        {
                                SerializedProperty point = parent.FindProperty ("point");

                                Vector3 newPosition = this.transform.position;
                                bool changed = false;
                                if ((oldPosition.x != newPosition.x || oldPosition.y != newPosition.y) && !Application.isPlaying) // && TwoBitMachines.Editors.Mouse.ctrl)
                                {
                                        MovePoints (point, -10, newPosition - oldPosition);
                                        changed = true;
                                }
                                else
                                {
                                        for (int i = 0; i < point.arraySize; i++)
                                        {
                                                SerializedProperty p = point.Element (i);
                                                Vector2 origin = p.vector2Value;
                                                p.vector2Value = SceneTools.MovePositionCircleHandle (p.vector2Value, Vector2.zero, Color.green, out changed);
                                                if (origin != p.vector2Value && TwoBitMachines.Editors.Mouse.ctrl)
                                                {
                                                        MovePoints (point, i, p.vector2Value - origin);

                                                }
                                        }
                                        DrawLines ( );
                                }

                                parent.FindProperty ("oldPosition").vector3Value = newPosition;
                                if (changed) editor.Repaint ( );
                        }
                        parent.ApplyModifiedProperties ( );
                }

                private void DrawLines ( )
                {
                        if (findType == TargetFindType.FollowSequence && point.Count > 1)
                        {
                                for (int i = 0; i < point.Count - 1; i++)
                                {
                                        SceneTools.Line (point[i], point[i + 1], Color.green);
                                }
                        }
                }

                private void MovePoints (SerializedProperty point, int except, Vector2 velocity)
                {
                        for (int i = 0; i < point.arraySize; i++)
                        {
                                if (i != except) point.Element (i).vector2Value += velocity;
                        }
                }
                #endif
                #endregion
        }

        public enum TargetFindType
        {
                FindNearest,
                GetRandom,
                FollowSequence,
                None
        }

}