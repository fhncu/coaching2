using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI.BlackboardData
{
      //* moving platforms that work with pathfinding must be one cellsize in height
      [System.Serializable]
      public class StateMoving : FollowerState
      {
            public override void Execute (TargetPathfinding ai, bool onGround, ref Vector2 velocity)
            {
                  if (!ai.OnMovingPlatform ( )) return;

                  if (ai.nextNode.moving && ai.futureNode != null && ai.futureNode.moving)
                  {
                        RemoveNode (ai);
                  }

                  if (ai.MovingPlatVel ( ).sqrMagnitude < 0.001f) // platform not moving
                  {
                        if (ai.canFollow && ai.futureNode != null)
                        {
                              if (onGround && ai.currentNode.Same (ai.nextNode) && !ai.futureNode.moving)
                              {
                                    Jump (ai, 1f, ref velocity);
                              }
                              else if (ai.nextNode.moving && !ai.futureNode.moving && ai.InPlatformXrange (ai.nextNode.position.x)) // only move towards an exit node
                              {
                                    MoveToCenterX (ai, ai.nextNode, ref velocity.x);
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
                  }
                  else
                  {
                        ai.canFollow = true;
                  }
            }

            public static bool WaitForMovingPlatform (TargetPathfinding ai, FollowerState state, ref Vector2 velocity)
            {
                  // if (ai.futureNode.moving && TwoBitMachines.Clock.Timer (ref ai.waitTimer, 0.5f)) // don't check every frame, moving platform should have wait times
                  // {
                  //       Collider2D platform = Physics2D.OverlapPoint (ai.futureNode.position - ai.map.cellYOffset * 2.95f, ); //Need to rethink layermask
                  //       if (platform && MovingPlatform.Exists (platform.transform) && MovingPlatform.foundPlatform.velocity == Vector2.zero) // wait until moving platform has zero velocity to jump on
                  //       {
                  //             ai.canFollow = false;
                  //             state.JumpTo (ai, ai.futureNode, 1f, ref velocity, StateJumpType.Moving);
                  //             return true;
                  //       }
                  // }
                  return ai.futureNode != null && ai.futureNode.moving;
            }

            public static bool OnMovingPlatform (TargetPathfinding ai)
            {
                  if (ai.currentNode.moving)
                  {
                        ai.state = PathState.Moving;
                        ai.canFollow = false;
                        return true;
                  }
                  return false;
            }

            public override void IsPathReachable (TargetPathfinding ai, Vector2 velocity) //
            {
                  if (!ai.stateChanged && !ai.currentNode.moving)
                  {
                        ai.state = PathState.Follow;
                  }
            }
      }
}