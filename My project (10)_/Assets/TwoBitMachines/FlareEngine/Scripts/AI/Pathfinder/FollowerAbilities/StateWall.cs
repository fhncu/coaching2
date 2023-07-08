using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateWall : FollowerState
        {
                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        ClimbWall (ai, ref velocity);

                        if (ai.currentNode.Same (ai.nextNode) && ai.futureNode != null)
                        {
                                if (StateMoving.WaitForMovingPlatform (ai, this, ref velocity))
                                {
                                        if (ai.state == PathState.Wall && ai.HitWall ( ))
                                        {
                                                ai.SetAnimation ("wallClimb", true);
                                        }
                                        return;
                                }
                                else if (!ai.nextNode.SameX (ai.futureNode))
                                {
                                        if (StateCornerGrab.GrabFromWall (ai))
                                        {
                                                return;
                                        }
                                        Jump (ai, 1f, ref velocity);
                                        return;
                                }
                                else if (ai.futureNode.Below (ai.nextNode) && !ai.futureNode.wall)
                                {
                                        Jump (ai, 0.01f, ref velocity);
                                        return;
                                }
                        }
                        if (ai.HitWall ( ))
                        {
                                ai.SetAnimation ("wallClimb", true);
                        }
                }

                private void ClimbWall (TargetPathfinding ai, ref Vector2 velocity)
                {
                        velocity.y = 0;
                        RemoveMoveSafelyY (ai);
                        int direction = ai.nextNode.ShiftX (ai.map, -1).block ? -1 : 1;
                        MoveToTarget (ai.position.x, ai.nextNode.position.x + direction, ai.followSpeed * 0.5f, ref velocity.x);
                        if (ai.HitWall ( )) MoveToTarget (ai.position.y, ai.nextNode.position.y, ai.wallSpeed, ref velocity.y);
                }

                public static bool FindWallToClimb (TargetPathfinding ai, ref Vector2 velocity)
                {
                        if (ai.nextNode.wall && ai.futureNode.wall)
                        {
                                ai.state = PathState.Wall;
                                return true;
                        }
                        return false;
                }

                public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity)
                {
                        if (!ai.stateChanged && !ai.currentNode.wall)
                        {
                                ai.state = PathState.Follow;
                                ai.SetAnimation ("wallClimb", false);
                        }
                }

        }
}