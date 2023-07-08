using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateCornerGrab : FollowerState
        {
                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        // Vector2 jumpPoint = ai.nextNode.position - ai.map.cellYOffset * (ai.size.y * 2f + 1f + ai.cornerYOffset);
                        // velocity = Time.deltaTime == 0 ? Vector2.zero : ((jumpPoint - ai.bottomCenter) * 0.75f) / Time.deltaTime;

                        // if (ai.grabComplete)
                        // {
                        //         velocity = new Vector2 (0, -1f);
                        //         ai.transform.position = ai.nextNode.position - ai.map.cellYOffset * 0.99f;
                        //         ai.state = PathState.Follow;
                        //         ai.grabComplete = false;
                        //         ai.BoxUpdate ( );
                        //         return;
                        // }
                        ai.SetAnimation ("ledgeClimb", true);
                }

                public static bool FindCornerToGrab (TargetPathfinding ai, FollowerState state, ref Vector2 velocity)
                {
                        // if (ai.cornerGrab && ai.futureNode.gridY > ai.nextNode.gridY && ai.futureNode.onGround && ai.nextNode.DistanceY (ai.futureNode) >= ai.map.maxJumpHeight) //                             platform is above
                        // {
                        //         if (state.MoveToCenterX (ai, ai.nextNode, ref velocity.x))
                        //         {
                        //                 ai.grabComplete = false;
                        //                 state.JumpTo (ai, ai.futureNode, 0.25f, ref velocity, StateJumpType.CornerGrab, true, (0.49f + ai.size.x * 0.5f) * ai.map.cellSize, ai.size.y * 2f + 1f + ai.cornerYOffset);
                        //         }
                        //         return true;
                        // }
                        return false;
                }

                public static bool GrabFromWall (TargetPathfinding ai)
                {
                        // if (ai.cornerGrab && ai.futureNode.ground)
                        // {
                        //         RemoveNode (ai);
                        //         ai.grabComplete = false;
                        //         ai.state = PathState.CornerGrab;
                        //         if (ai.HitWall ( )) ai.SetAnimation ("wallClimb", true);
                        //         return true;
                        // }
                        return false;
                }
        }

}