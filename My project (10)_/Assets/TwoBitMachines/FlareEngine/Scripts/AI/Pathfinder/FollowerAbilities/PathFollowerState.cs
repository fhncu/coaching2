using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData

{
        [System.Serializable]
        public class FollowerState
        {
                #region 
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField] private bool foldOut = false;
                #pragma warning restore 0414
                #endif
                #endregion

                public virtual void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {

                }
                public virtual void IsPathReachable (TargetPathfinding ai, Vector2 velocity)
                {

                }

                public void JumpTo (TargetPathfinding ai, PathNode jumpTo, float archHeight, ref Vector2 velocity, StateJumpType type = StateJumpType.Jump, bool xOffset = false, float offsetX = 1f, float offsetY = 1f)
                {
                        Vector2 cellOffsetX = xOffset ? Vector2.right * offsetX * ai.currentNode.DirectionX (jumpTo) : Vector2.zero;
                        velocity = Compute.ArchObject (ai.bottomCenter, jumpTo.position - ai.map.cellYOffset * offsetY + cellOffsetX, archHeight * ai.map.cellSize, ai.GravityValue ( ));

                        SetupJump (ai, type, jumpTo, ref velocity);
                }

                public void JumpShorten (TargetPathfinding ai, PathNode jumpTo, float archHeight, ref Vector2 velocity, StateJumpType type = StateJumpType.Jump)
                {
                        bool adjacentJump = ai.currentNode.DistanceXOne (jumpTo);
                        Vector2 cellOffsetX = ai.currentNode.SameX (jumpTo) ? Vector2.zero : Vector2.right * ai.map.cellSize * 0.25f * ai.currentNode.DirectionX (jumpTo); //if going right, will return negative
                        velocity = Compute.ArchObject (ai.bottomCenter, jumpTo.position - ai.map.cellYOffset + cellOffsetX, archHeight * ai.map.cellSize, ai.GravityValue ( ));
                        SetupJump (ai, type, jumpTo, ref velocity);

                        if (adjacentJump)
                        {
                                ai.adjacentJump = true;
                                velocity.x *= 0.00001f; // we want vel to be zero, but we make it really small so that ai points in that direction
                        }
                }

                public void Jump (TargetPathfinding ai, float archHeight, ref Vector2 velocity)
                {
                        velocity = Compute.ArchObject (ai.bottomCenter, ai.futureNode.position - ai.map.cellYOffset, archHeight * ai.map.cellSize, ai.GravityValue ( ));
                        SetupJump (ai, StateJump.JumpType (ai.futureNode), ai.futureNode, ref velocity);
                }

                private void SetupJump (TargetPathfinding ai, StateJumpType type, PathNode jumpTo, ref Vector2 velocity)
                {
                        velocity.y += ai.GravityValue ( ) * Time.deltaTime * 0.5f; // added for jump precision, will more or less jump the correct archHeight
                        ai.velRef = velocity;
                        ai.jumpType = type;
                        ai.jumpTo = jumpTo;
                        ai.state = PathState.Jump;
                        FollowerState.RemoveNode (ai);
                        ai.adjacentJump = false;
                }

                public void SetupFall (TargetPathfinding ai, ref Vector2 velocity, bool followToCenter = false)
                {
                        FollowerState.MoveToTarget (ai.position.x, ai.futureNode.position.x, ai.followSpeed, ref velocity.x);
                        ai.jumpType = StateJumpType.FreeFall;
                        ai.state = PathState.Jump;
                        ai.jumpTo = ai.currentNode;
                        ai.velRef = velocity;
                        FollowerState.RemoveNode (ai);
                        ai.adjacentJump = false;
                        ai.followToCenter = followToCenter;
                }

                public static void MoveToTarget (float position, float target, float speed, ref float velocity) //
                {
                        if (Time.deltaTime == 0) return;
                        float newPosition = Mathf.MoveTowards (position, target, speed * Time.deltaTime);
                        velocity = (newPosition - position) / Time.deltaTime;
                }

                public static bool MoveToTargetBool (float position, float target, float speed, ref float velocity) //
                {
                        if (Time.deltaTime == 0) return false;
                        float newPosition = Mathf.MoveTowards (position, target, speed * Time.deltaTime);
                        velocity = (newPosition - position) / Time.deltaTime;
                        return Mathf.Abs (position - newPosition) <= 0.01f;
                }

                public bool MoveToCenterX (TargetPathfinding ai, PathNode target, ref float velocity)
                {
                        if (Time.deltaTime == 0) return false;
                        float position = Mathf.MoveTowards (ai.position.x, target.position.x, ai.followSpeed * Time.deltaTime);
                        velocity = (position - ai.position.x) / Time.deltaTime;
                        return Mathf.Abs (ai.position.x - position) <= 0.01f;
                }

                public static void MoveToCenterXNoBool (TargetPathfinding ai, PathNode target, ref float velocity)
                {
                        if (Time.deltaTime == 0) return;
                        float newPosition = Mathf.MoveTowards (ai.position.x, target.position.x, ai.followSpeed * Time.deltaTime);
                        velocity = (newPosition - ai.position.x) / Time.deltaTime;
                }

                public static bool RemoveMoveSafelyX (TargetPathfinding ai)
                {
                        if (ai.nextNode != null && ai.futureNode != null)
                        {
                                if (ai.currentNode.Same (ai.nextNode) && ai.nextNode.NextToX (ai.futureNode))
                                {
                                        RemoveNode (ai);
                                        return true;
                                }
                        }
                        return false;
                }

                public static bool RemoveMoveSafelyY (TargetPathfinding ai)
                {
                        if (ai.nextNode != null && ai.futureNode != null)
                        {
                                if (ai.currentNode.Same (ai.nextNode) && ai.nextNode.NextToY (ai.futureNode))
                                {
                                        RemoveNode (ai);
                                        return true;
                                }
                        }
                        return false;
                }

                public static void RemoveNextNode (TargetPathfinding ai)
                {
                        if (ai.currentNode.Same (ai.nextNode))
                        {
                                RemoveNode (ai);
                        }
                }

                public static void RemoveNode (TargetPathfinding ai)
                {
                        if (ai.path.Count == 0)
                        {
                                ai.nextNode = ai.currentNode;
                                ai.futureNode = null;
                                return;
                        }
                        else if (ai.path.Count == 1)
                        {
                                ai.nextNode = ai.path.Peek ( );
                                ai.futureNode = null;
                                return;
                        }
                        else // at least two nodes
                        {
                                ai.nextNode = ai.path.Pop ( );
                                ai.futureNode = ai.path.Peek ( );
                        }
                }

                public static void RemoveLandingNodes (TargetPathfinding ai, PathNode node)
                {
                        int max = 0;
                        while (max++ < 100) //max node check
                        {
                                if (ai.path.Count > 1 && ai.path.Peek ( ).gridY == node.gridY)
                                {
                                        bool sameX = ai.path.Peek ( ).gridX == node.gridX;
                                        RemoveNode (ai);
                                        if (sameX) break;
                                }
                                else
                                        break;
                        }
                }

                private void RemoveNode (Stack<PathNode> path)
                {
                        if (path.Count > 0)
                        {
                                path.Pop ( );
                        }
                }

        }
}