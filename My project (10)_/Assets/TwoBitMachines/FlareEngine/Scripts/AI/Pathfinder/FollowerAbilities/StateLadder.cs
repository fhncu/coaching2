using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateLadder : FollowerState
        {
                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        ClimbLadder (ai, ref velocity);

                        if (ai.currentNode.Same (ai.nextNode) && ai.futureNode != null)
                        {
                                if (StateMoving.WaitForMovingPlatform (ai, this, ref velocity))
                                {
                                        ai.SetAnimation ("ladderClimb", ai.state == PathState.Ladder);
                                        return;
                                }
                                else if (!ai.nextNode.SameX (ai.futureNode))
                                {
                                        Jump (ai, 1f, ref velocity);
                                        ai.SetAnimation ("ladderClimb", false);
                                        return;
                                }
                                else if (ai.futureNode.Below (ai.nextNode) && !ai.futureNode.ladder)
                                {
                                        Jump (ai, 0.1f, ref velocity);
                                        ai.SetAnimation ("ladderClimb", false);
                                        return;
                                }
                        }
                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                ai.DrawTargets (ai.nextNode, ai.futureNode);
                        }
                        #endif
                        #endregion
                }

                private void ClimbLadder (TargetPathfinding ai, ref Vector2 velocity)
                {
                        velocity.y = 0;
                        RemoveMoveSafelyY (ai);
                        MoveToTarget (ai.position.x, ai.nextNode.position.x, ai.followSpeed * 0.5f, ref velocity.x);
                        if ((ai.currentNode.ladder && ai.nextNode.gridY > ai.currentNode.gridY) || !ai.OnGround ( ))
                        {
                                ai.SetAnimation ("ladderClimb", true);
                                MoveToTarget (ai.position.y, ai.nextNode.position.y, ai.ladderSpeed, ref velocity.y);
                        }
                }

                public static bool FindLadderClimb (TargetPathfinding ai, ref Vector2 velocity)
                {
                        if (ai.nextNode.ladder && ai.futureNode.ladder)
                        {
                                ai.state = PathState.Ladder;
                                return true;
                        }
                        return false;
                }

                public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity)
                {
                        if (!ai.stateChanged && !ai.currentNode.ladder && (ai.nextNode == null || !ai.nextNode.ladder))
                        {
                                ai.state = PathState.Follow;
                                ai.SetAnimation ("ladderClimb", false);
                        }
                }
        }
}