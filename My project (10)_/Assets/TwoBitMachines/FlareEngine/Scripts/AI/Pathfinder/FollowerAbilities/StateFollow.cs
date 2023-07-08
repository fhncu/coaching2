using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
        [System.Serializable]
        public class StateFollow : FollowerState
        {
                [SerializeField] private bool enable;
                [SerializeField] public float speed = 1;

                public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        if (ai.pauseAfterJumpActive && ai.pauseAfterJump > 0 && TwoBitMachines.Clock.TimerInverse (ref ai.pauseCounter, ai.pauseAfterJump))
                        {
                                return; // Maybe pause can be based on jump distance and jump direction, maybe no pause when jumping up or down, etc.
                        }
                        ai.pauseAfterJumpActive = false;

                        if (ai.nextNode.bridge && ai.world != null)
                        {
                                if (ai.currentNode.SameX (ai.nextNode)) RemoveNode (ai);
                                MoveToCenterXNoBool (ai, ai.nextNode, ref velocity.x);
                        }
                        else if (onGround)
                        {
                                RemoveMoveSafelyX (ai);
                                MoveToCenterXNoBool (ai, ai.nextNode, ref velocity.x); // follow target on ground
                        }
                        if (ai.futureNode != null && SearchForNewState (ai, onGround, ref velocity))
                        {
                                return;
                        }
                        if (ai.futureNode != null && onGround)
                        {
                                StateJump.Search (this, ai, ai.currentNode.Same (ai.nextNode), ref velocity);
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

                public bool SearchForNewState (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
                {
                        // if (StateMoving.OnMovingPlatform (ai)) //* To be added soon
                        //       return true;
                        if (!onGround) //|| !ai.currentNode.Same (ai.nextNode))
                                return false;
                        // if (StateCornerGrab.FindCornerToGrab (ai, this, ref velocity))
                        //        return true;
                        //if (StateMoving.WaitForMovingPlatform (ai, this, ref velocity))
                        //   return true;
                        if (StateLadder.FindLadderClimb (ai, ref velocity))
                                return true;
                        if (StateWall.FindWallToClimb (ai, ref velocity))
                                return true;
                        return false;
                }

                public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity)
                {
                        if (ai.currentNode.onGround && ai.currentNode.DistanceY (ai.nextNode) > 0)
                        {
                                ai.CalculatePath (ai.targetRef);
                        }
                }

        }
}