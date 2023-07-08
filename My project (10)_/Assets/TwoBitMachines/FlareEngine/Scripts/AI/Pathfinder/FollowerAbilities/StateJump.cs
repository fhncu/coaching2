using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateJump : FollowerState
        {
                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        if (ai.jumpTo == null)
                        {
                                Complete (ai, PathState.Follow);
                        }
                        else if (ai.jumpType == StateJumpType.Jump)
                        {
                                bool dontCurve = ai.adjacentJump && ai.jumpTo.gridY > ai.currentNode.gridY;
                                if (!dontCurve)
                                {
                                        velocity.x = ai.velRef.x;
                                }
                                if (onGround)
                                {
                                        Complete (ai, PathState.Follow);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.FreeFall)
                        {
                                if (ai.followToCenter)
                                        MoveToCenterXNoBool (ai, ai.jumpTo, ref velocity.x);
                                else
                                        velocity.x = ai.velRef.x;
                                if (onGround && (Mathf.Abs (ai.position.y - ai.jumpTo.position.y) >= 0.1f || Mathf.Abs (ai.position.x - ai.jumpTo.position.x) >= ai.map.cellSize * 0.25f))
                                {
                                        RemoveLandingNodes (ai, ai.currentNode);
                                        Complete (ai, PathState.Follow);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.CornerGrab)
                        {
                                velocity.x = ai.velRef.x;
                                if (ai.HitWall ( ))
                                {
                                        Complete (ai, PathState.CornerGrab);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.Ladder)
                        {
                                velocity.x = ai.velRef.x;
                                if (ai.currentNode.Same (ai.jumpTo))
                                {
                                        Complete (ai, PathState.Ladder);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.Wall)
                        {
                                velocity.x = ai.velRef.x;
                                if (ai.currentNode.Same (ai.jumpTo))
                                {
                                        Complete (ai, PathState.Wall);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.Moving)
                        {
                                velocity.x = ai.velRef.x;
                                if (onGround && ai.currentNode.Same (ai.jumpTo))
                                {
                                        Complete (ai, PathState.Moving);
                                }
                        }
                        else if (ai.jumpType == StateJumpType.Fall)
                        {
                                if (onGround)
                                {
                                        Complete (ai, PathState.Follow);
                                }
                        }
                        else
                        {
                                if (ai.OnCeiling ( )) // ceiling
                                {
                                        Complete (ai, PathState.Ceiling);
                                        velocity.y = ai.map.cellSize;
                                }
                                else if (onGround)
                                {
                                        Complete (ai, PathState.Follow);
                                }
                        }
                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                ai.DrawTargets (null, ai.jumpTo);
                        }
                        #endif
                        #endregion
                }

                private void Complete (TargetPathfinding ai, PathState nextState)
                {
                        ai.state = nextState;
                        ai.jumpType = StateJumpType.Fall;
                        ai.adjacentJump = false;
                        ai.jumpTo = null;
                        ai.pauseAfterJumpActive = true;
                        ai.pauseCounter = 0;

                }

                public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity) //
                {
                        if (!ai.stateChanged && ai.OnGround ( ) && ai.jumpType != StateJumpType.FreeFall) // do not test on frame of state change
                        {
                                ai.state = PathState.Follow;
                        }
                }

                public static void Search (StateFollow state, TargetPathfinding ai, bool sameAsNextNode, ref Vector2 velocity)
                {
                        if (ai.futureNode.gridY == ai.nextNode.gridY) //                                 on same level, look for gap jump
                        {
                                if (sameAsNextNode && !ai.nextNode.NextToGridX (ai.futureNode))
                                {
                                        state.Jump (ai, ai.minJumpHeight + 0.1f, ref velocity);
                                }
                        }
                        else if (ai.futureNode.gridY > ai.nextNode.gridY) //                             platform is above
                        {
                                if (sameAsNextNode && (!ai.nextNode.exact || state.MoveToCenterX (ai, ai.nextNode, ref velocity.x)))
                                {
                                        state.JumpShorten (ai, ai.futureNode, 0.35f, ref velocity, JumpType (ai.futureNode));
                                }
                        }
                        else //                                                                          platform is below
                        {
                                float target = ai.nextNode.position.x + (ai.map.cellSize * 0.5f + ai.size.x * 0.47f) * ai.futureNode.DirectionX (ai.nextNode); // follow to edge node before jumping down
                                if (FollowerState.MoveToTargetBool (ai.position.x, target, ai.followSpeed, ref velocity.x))
                                {
                                        float distance = ai.nextNode.DistanceX (ai.futureNode);
                                        if (distance <= 1)
                                        {
                                                state.SetupFall (ai, ref velocity, true);
                                        }
                                        else if (ai.futureNode.isFall && ai.futureNode.DistanceX (ai.targetNode) > 2f)
                                        {
                                                state.SetupFall (ai, ref velocity);
                                        }
                                        else
                                        {
                                                float arch = distance <= 2 ? 0.05f : distance <= 3 ? 0.15f : 0.5f;
                                                state.JumpTo (ai, ai.futureNode, arch, ref velocity);
                                        }
                                }

                        }
                }

                public static StateJumpType JumpType (PathNode node)
                {
                        if (node.ladder)
                                return StateJumpType.Ladder;
                        if (node.wall)
                                return StateJumpType.Wall;
                        if (node.ceiling)
                                return StateJumpType.Ceiling;
                        return StateJumpType.Jump;
                }

        }

        public enum StateJumpType
        {
                Jump,
                Ladder,
                Wall,
                Ceiling,
                Moving,
                CornerGrab,
                Fall,
                FreeFall
        }
}