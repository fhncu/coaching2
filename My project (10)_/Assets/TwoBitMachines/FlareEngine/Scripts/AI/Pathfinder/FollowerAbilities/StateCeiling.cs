using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateCeiling : FollowerState
        {
                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        ClimbCeiling (ai, ref velocity);
                        if (ai.currentNode.SameX (ai.nextNode) && ai.futureNode != null && (!ai.futureNode.ceiling || ai.futureNode.onGround))
                        {
                                ai.state = PathState.Jump;
                                ai.jumpType = StateJumpType.Fall;
                                return;
                        }
                        ai.SetAnimation ("ceilingClimb", true);
                }

                private void ClimbCeiling (TargetPathfinding ai, ref Vector2 velocity)
                {
                        velocity.y = 0;
                        if (ai.currentNode.SameX (ai.nextNode))
                        {
                                RemoveNode (ai);
                        }
                        MoveToTarget (ai.position.x, ai.nextNode.position.x, ai.ceilingSpeed, ref velocity.x);
                        MoveToTarget (ai.position.y, ai.nextNode.position.y, ai.map.cellSize, ref velocity.y);
                }

                public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity) //
                {
                        if (ai.OnGround ( ) || !ai.OnCeiling ( ))
                        {
                                ai.state = PathState.Jump;
                                ai.jumpType = StateJumpType.Fall;
                                ai.SetAnimation ("ceilingClimb", false);
                        }
                }

        }

}