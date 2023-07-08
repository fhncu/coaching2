using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class TargetPathfindingBasic : Blackboard
        {
                [SerializeField] public PathfindingBasic map;
                [SerializeField] public float followSpeed;

                [System.NonSerialized] public BasicNode currentNode;
                [System.NonSerialized] public BasicNode targetNode;
                [System.NonSerialized] public JobHandle jobHandle;
                [System.NonSerialized] public PathfindingBasicJob calculateJob;
                [System.NonSerialized] public NativeList<Vector2> pathReference;
                [System.NonSerialized] public Stack<Vector2> path = new Stack<Vector2> ( );
                [System.NonSerialized] public bool waitForPath;
                [System.NonSerialized] public bool activeUnit;
                [System.NonSerialized] public bool wait;
                [System.NonSerialized] public float counter;
                [System.NonSerialized] public Vector2 shift;

                public void Awake ( )
                {
                        waitForPath = false;
                        activeUnit = true;
                        pathReference = new NativeList<Vector2> (Allocator.Persistent);
                        if (map != null) map.RegisterFollower (this);
                        float offset = 1f;
                        shift = new Vector2 (Random.Range (-offset, offset), Random.Range (-offset, offset)) * map.cellSize;
                        followSpeed = Random.Range (0.65f, 1f) * followSpeed;
                }

                private void OnEnable ( )
                {
                        activeUnit = true;
                }

                private void OnDisable ( )
                {
                        activeUnit = false;
                }

                private void OnDestroy ( )
                {
                        DisposeFollower ( );
                }

                public void DisposeFollower ( )
                {
                        jobHandle.Complete ( );
                        if (pathReference.IsCreated) pathReference.Dispose ( );
                }

                public void GetPath ( )
                {
                        if (map == null) return;

                        if (waitForPath && jobHandle.IsCompleted)
                        {
                                waitForPath = false;
                                jobHandle.Complete ( );
                                path.Clear ( );
                                for (int i = 0; i < pathReference.Length; i++)
                                        path.Push (pathReference[i] + shift);
                                if (path.Count > 1) path.Pop ( ); // get rid of first only if path is bigger than one. if path changes every frame, ai might get stuck following the first node since path gets refilled on next frame. thus the first node gets repopulated
                        }
                }

                public override void CalculatePath (Blackboard target)
                {
                        if (map == null || target == null || map.jobGrid.Length == 0) return;

                        BasicNode startNode = map.PositionToNode ((Vector2) transform.position + map.cellYOffset);
                        targetNode = map.PositionToNode (target.GetTarget ( ) + map.cellYOffset);

                        if (targetNode == null || startNode == null) return;

                        jobHandle.Complete ( );
                        calculateJob = new PathfindingBasicJob
                        {
                                linesX = map.linesX,
                                linesY = map.linesY,
                                jobGrid = map.jobGrid,
                                cellSize = map.cellSize,
                                targets = pathReference,
                                position = map.bounds.position,
                                start = map.jobGrid[startNode.gridX + startNode.gridY * map.linesX],
                                target = map.jobGrid[targetNode.gridX + targetNode.gridY * map.linesX]
                        };
                        jobHandle = calculateJob.Schedule ( );
                        waitForPath = true;
                }

                public override void RunPathFollower (ref Vector2 velocity)
                {
                        GetPath ( );
                        if (waitForPath || map == null)
                        {
                                return;
                        }

                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                for (int i = 0; i < path.Count - 1; i++)
                                {
                                        Debug.DrawLine (pathReference[i], pathReference[i + 1], Color.red);
                                }
                        }
                        #endif
                        #endregion

                        Vector2 position = transform.position;
                        if (path.Count > 0)
                        {
                                float checkSize = path.Count == 1 ? 0.01f : map.cellSize * 0.5f;
                                if ((path.Peek ( ) - position).sqrMagnitude <= checkSize * checkSize) path.Pop ( ); // both peek and player position should be at bottom of cell
                        }
                        if (path.Count > 0 && Time.deltaTime != 0)
                        {
                                Vector2 newPosition = Vector2.MoveTowards (position, path.Peek ( ), followSpeed * Time.deltaTime);
                                velocity = (newPosition - position) / Time.deltaTime;
                        }
                        return;
                }

                public override bool AtFinalTarget ( )
                {
                        return path.Count == 0;
                }

                public override Vector2 GetTarget ( )
                {
                        return transform.position;
                }
        }
}