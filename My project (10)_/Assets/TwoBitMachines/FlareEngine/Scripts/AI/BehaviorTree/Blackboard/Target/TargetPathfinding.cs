using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [AddComponentMenu ("")]
        public class TargetPathfinding : Blackboard
        {
                [SerializeField] public Pathfinding map;

                [SerializeField] public float minJumpHeight = 1.5f;
                [SerializeField] public float followSpeed = 5f;
                [SerializeField] public float ceilingSpeed = 5f;
                [SerializeField] public float ladderSpeed = 5f;
                [SerializeField] public float wallSpeed = 5f;
                //[SerializeField] public float cornerYOffset;
                //[SerializeField] public bool cornerGrab;
                [SerializeField] public bool ignoreUnits = true;
                [SerializeField] public float pauseAfterJump = 0;

                [System.NonSerialized] public Gravity gravity;
                [System.NonSerialized] public WorldCollision world;
                [System.NonSerialized] public AnimationSignals signals;

                [System.NonSerialized] public float pauseCounter = 0;
                [System.NonSerialized] public bool onSurface = false;
                [System.NonSerialized] public bool pauseAfterJumpActive = false;

                // state info
                [NonSerialized] public Blackboard targetRef;
                [NonSerialized] public PathNode targetNode; //   end of path 
                [NonSerialized] public PathNode nextNode; //     node ai is following
                [NonSerialized] public PathNode futureNode; //   node that comes after nextNode, useful too look for next state
                [NonSerialized] public PathNode currentNode; //  node ai currently inhabits

                [NonSerialized] public PathState state;
                [NonSerialized] public PathState previousState;
                [NonSerialized] public JobHandle jobHandle;
                [NonSerialized] public PathfindingJob calculateJob;
                [NonSerialized] public NativeList<int> pathReference;
                [NonSerialized] public Stack<PathNode> path = new Stack<PathNode> ( );

                [NonSerialized] public Vector2 position;
                [NonSerialized] public Vector2 bottomCenter;
                [NonSerialized] public Vector2 velRef;
                [NonSerialized] public Vector2 size; //           character Size
                [NonSerialized] public StateJumpType jumpType; // for jump state
                [NonSerialized] public PathNode jumpTo; //        for jump state

                [NonSerialized] public bool adjacentJump = false;
                [NonSerialized] public bool followToCenter = false;
                [NonSerialized] public float waitTimer; //        for moving platform state
                [NonSerialized] public float counter = 0;
                [NonSerialized] public int targetGridY = 0;
                [NonSerialized] public bool wait = false;
                [NonSerialized] public bool canFollow; //         for moving platform state
                [NonSerialized] public bool grabComplete; //      for corner grab state
                [NonSerialized] public bool waitForPath;
                [NonSerialized] public bool activeUnit = true;

                public bool pathX => state == PathState.Follow || state == PathState.Ceiling;
                public bool pathY => state == PathState.Ladder || state == PathState.Wall;
                public bool pathIgnore => !pathX && !pathY;
                public bool stateChanged => previousState != state;

                [SerializeField, HideInInspector] public FollowerState[] stateMachine = new FollowerState[]
                {
                        new StateFollow ( ),
                        new StateJump ( ),
                        new StateCeiling ( ),
                        new StateCornerGrab ( ),
                        new StateLadder ( ),
                        new StateWall ( ),
                        new StateMoving ( )
                };

                #if UNITY_EDITOR
                [NonSerialized] List<int> list = new List<int> ( );
                #endif

                public void Awake ( )
                {
                        waitForPath = false;
                        activeUnit = true;
                        pathReference = new NativeList<int> (Allocator.Persistent);
                        if (map != null) map.RegisterFollower (this);
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
                        DisposeFollower ( ); // follower should be disposed by pathfinding action. However, if this follower is destroyed by user, then onDestroy must be called to prevent memory leak
                }

                public void DisposeFollower ( )
                {
                        jobHandle.Complete ( );
                        if (pathReference.IsCreated) pathReference.Dispose ( );
                }

                public override void ResetState (WorldCollision world, Gravity gravity, AnimationSignals signals, Vector2 target)
                {
                        this.world = world;
                        this.gravity = gravity;
                        this.signals = signals;
                        size = CharacterSize ( );
                        state = PathState.Follow;
                        jumpType = StateJumpType.Fall;
                        CalculateStartNode ( );
                        nextNode = futureNode = currentNode;
                        targetNode = map.PositionToNode (target + map.cellYOffset);
                        targetGridY = targetNode.gridY;
                }

                public void GetPath ( )
                {
                        if (map == null) return;

                        CalculateStartNode ( );

                        if (waitForPath && jobHandle.IsCompleted)
                        {
                                waitForPath = false;
                                jobHandle.Complete ( );
                                path.Clear ( );

                                #if UNITY_EDITOR
                                list.Clear ( );
                                #endif

                                float length = pathReference.Length;
                                for (int i = 0; i < length; i++)
                                {
                                        path.Push (map.grid[pathReference[i]]);
                                        #if UNITY_EDITOR
                                        list.Add (pathReference[i]);
                                        #endif
                                        if (i == length - 2)
                                        {
                                                if (map.grid[pathReference[i]].Same (currentNode))
                                                {
                                                        break;
                                                }
                                        }
                                }
                                FollowerState.RemoveNode (this);
                        }
                }

                public override void CalculatePath (Blackboard target)
                {
                        if (map == null || target == null || map.jobGrid.Length == 0)
                        {
                                return;
                        }

                        CalculateStartNode ( );
                        targetRef = target;
                        targetNode = map.PositionToNode (target.GetTarget ( ) + map.cellYOffset);

                        if (targetNode == null || currentNode == null)
                        {
                                return;
                        }

                        jobHandle.Complete ( ); // ensure job is complete before doing a reset
                        calculateJob = new PathfindingJob
                        {
                                linesX = map.linesX,
                                linesY = map.linesY,
                                jobGrid = map.jobGrid,
                                result = pathReference,
                                characterSizeY = size.y,
                                jobNeighbors = map.jobNeighbors,
                                target = map.jobGrid[targetNode.gridX + targetNode.gridY * map.linesX],
                                start = map.jobGrid[currentNode.gridX + currentNode.gridY * map.linesX]
                        };
                        jobHandle = calculateJob.Schedule ( );
                        waitForPath = true;
                }

                private void CalculateStartNode ( )
                {
                        bottomCenter = BottomCenter (transform.position);
                        position = bottomCenter + map.cellYOffset;
                        currentNode = map.PositionToNode (position);
                }

                public override void RunPathFollower (ref Vector2 velocity)
                {
                        GetPath ( );
                        if (waitForPath || currentNode == null || targetNode == null || map == null)
                        {
                                return;
                        }

                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                for (int i = 0; i < list.Count; i++)
                                {
                                        Draw.Circle (map.grid[list[i]].position, 0.3f, Color.red);
                                }
                        }
                        #endif
                        #endregion

                        if (nextNode == null)
                        {
                                nextNode = currentNode;
                        }

                        onSurface = false;
                        bool onGround = OnGround ( );

                        previousState = state;
                        stateMachine[(int) state].Execute (this, onGround, ref velocity); // execute current state
                        stateMachine[(int) state].IsPathReachable (this, velocity); //       check If AI can reach current path in the state that it's in

                        Wait (nextNode, ref velocity);
                        if (wait && TwoBitMachines.Clock.TimerInverse (ref counter, 0.5f))
                        {
                                if (pathX) velocity.x = 0;
                                if (pathY) velocity.y = 0;
                        }
                        else wait = false;
                }

                public void Wait (PathNode node, ref Vector2 velocity)
                {
                        if (pathIgnore || node == null || !node.isOccupied || node.unit == null || node.unit == this || ignoreUnits) return;
                        wait = true;
                        counter = 0;
                }

                public void OccupyNode ( )
                {
                        if (pathIgnore || currentNode == null || currentNode.isOccupied || ignoreUnits) return;
                        currentNode.isOccupied = true;
                        currentNode.unit = this;
                }

                public override bool PathSafeToChange ( )
                {
                        return (pathX || pathY) && currentNode.path;
                }

                public override void IgnoreBlock (bool value)
                {
                        ignoreUnits = value;
                }

                public override bool TargetPlaneChanged (Vector2 position) // for recalculating path
                {
                        PathNode targetNode = map.PositionToNode (position + map.cellYOffset);
                        if (targetNode != null && targetNode.ground && targetNode.gridY != targetGridY)
                        {
                                targetGridY = targetNode.gridY;
                                return true;
                        }
                        return false;
                }

                public override bool AtFinalTarget ( )
                {
                        if (((path.Count <= 1 && futureNode == null) || currentNode.Same (targetNode)))
                        {
                                if (pathX && currentNode.DistanceX (transform.position) < 0.001f) return true;
                                if (pathY && currentNode.DistanceY (transform.position) < 0.001f) return true;
                        }
                        return false;;
                }

                public void DrawTargets (PathNode a, PathNode b)
                {
                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                if (a != null) Draw.Circle (a.position, 0.5f, Color.red);
                                if (b != null) Draw.Circle (b.position, 0.5f, Color.green);
                        }
                        #endif
                        #endregion
                }

                #region Path Methods
                public Vector2 BottomCenter (Vector2 position)
                {
                        return world != null ? world.box.bottomCenter : position;
                }

                public void CompleteCornerGrab ( )
                {
                        grabComplete = true;
                }

                public void BoxUpdate ( )
                {
                        if (world != null)
                                world.box.Update ( );
                }

                public float GravityValue ( )
                {
                        return gravity != null? gravity.gravity : 0;
                }

                public bool HitLeftWall ( )
                {
                        return world != null ? world.leftWall : false;
                }

                public bool HitRightWall ( )
                {
                        return world != null ? world.rightWall : false;
                }

                public bool HitWall ( )
                {
                        return HitLeftWall ( ) || HitRightWall ( );
                }

                public bool OnCeiling ( )
                {
                        return world != null ? world.onCeiling : false;
                }

                public bool OnGround ( )
                {
                        return world != null ? world.onGround : false;
                }

                public void SetAnimation (string name, bool value)
                {
                        signals.Set (name, value);
                }

                public bool OnMovingPlatform ( )
                {
                        return world != null ? world.mp.standing : false;
                }

                public Vector2 MovingPlatVel ( )
                {
                        return world != null ? world.mp.velocity : Vector2.zero;
                }

                public bool InPlatformXrange (float point)
                {
                        return false; // world != null && world.mp != null ? world.mp.InXRange (point) : false;
                }

                public Vector2 CharacterSize ( )
                {
                        if (world == null) return Vector2.zero;
                        Vector2 size = world.box.boxSize;
                        size.x *= this.transform.localScale.x;
                        size.y *= this.transform.localScale.y;
                        return size;
                }

                public void OnSurface ( )
                {
                        onSurface = true;
                }

                public override bool OnASurface ( )
                {
                        return onSurface;
                }

                public override bool CanExit ( )
                {
                        if (!pathIgnore)
                        {
                                wait = false;
                                counter = 0;
                        }
                        return !pathIgnore;
                }

                public override Vector2 GetTarget ( )
                {
                        return transform.position;
                }

                public override float CellSize ( )
                {
                        return map != null ? map.cellSize : 1f;
                }
                #endregion
        }

        public enum PathState
        {
                Follow = 0,
                Jump = 1,
                Ceiling = 2,
                CornerGrab = 3,
                Ladder = 4,
                Wall = 5,
                Moving = 6
        }

}