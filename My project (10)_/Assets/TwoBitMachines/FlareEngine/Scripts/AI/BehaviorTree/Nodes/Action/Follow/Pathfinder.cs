#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class Pathfinder : Action
        {
                [SerializeField] public Blackboard pathfinding;
                [SerializeField] public Blackboard target;
                [SerializeField] public float resetDistance = 1f;
                [SerializeField] public Vector2 findDistance = Vector2.one;
                [SerializeField] public bool useFindDistance = true;
                [SerializeField] public PathTargetFind findType;

                private Vector2 previousPosition;
                private bool refreshed = false;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (pathfinding == null || target == null)
                        {
                                return NodeState.Failure;
                        }

                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                pathfinding.ResetState (root.world, root.gravity, root.signals, target.GetTarget ( ));
                        }
                        if (useFindDistance) // exit out if found to prevent the path from recalculating
                        {
                                if (findType == PathTargetFind.TargetWithinDistance && Found (root.position, target.GetTarget ( )) && pathfinding.CanExit ( ))
                                {
                                        return NodeState.Success;
                                }
                                else if (findType == PathTargetFind.ReachedPathEnd && pathfinding.AtFinalTarget ( ) && pathfinding.CanExit ( ))
                                {
                                        return NodeState.Success;
                                }
                        }

                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                float cellSize = pathfinding.CellSize ( );
                                findDistance.x = Mathf.Clamp (findDistance.x, cellSize, float.MaxValue);
                                findDistance.y = Mathf.Clamp (findDistance.y, cellSize, float.MaxValue);
                                previousPosition = target.GetTarget ( );
                                refreshed = true;
                        }
                        else
                        {
                                pathfinding.RunPathFollower (ref root.velocity);
                                root.onSurface = root.onSurface || pathfinding.OnASurface ( );
                        }
                        if (pathfinding.PathSafeToChange ( ))
                        {
                                Vector2 targetPosition = target.GetTarget ( );
                                if (refreshed || pathfinding.TargetPlaneChanged (targetPosition) || ((previousPosition - targetPosition).sqrMagnitude > resetDistance * resetDistance))
                                {
                                        previousPosition = targetPosition;
                                        pathfinding.CalculatePath (target);
                                        refreshed = false;
                                }
                        }
                        return NodeState.Running;
                }

                public bool Found (Vector2 position, Vector2 target)
                {
                        if (findDistance.x != 0 && (target.x > (position.x + findDistance.x * 0.52f) || target.x < (position.x - findDistance.x * 0.52f)))
                                return false;
                        if (findDistance.y != 0 && (target.y > (position.y + findDistance.y * 0.52f) || target.y < (position.y - findDistance.y * 0.52f)))
                                return false;
                        return true;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (125, "Follow a path to a target using the pathfinding algorithm. This algorithm takes gravity into account, making it ideal for platformers. When the target has changed its position by the reset distance amount, the path will recalculate. If Success On is enabled, success is returned when the specified setting is met." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        int index = (int) findType;
                        int height = index == 1 ? 1 : 0;
                        FoldOut.Box (4 + height, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("pathfinding"), 0);
                        AIBase.SetRef (ai.data, parent.Get ("target"), 1);
                        parent.Field ("Reset Distance", "resetDistance");
                        parent.FieldAndEnable ("Success On", "findType", "useFindDistance");
                        if (parent.Bool ("useFindDistance")) parent.Field ("Find Distance", "findDistance", execute : index == 1);
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }

        public enum PathTargetFind
        {
                ReachedPathEnd,
                TargetWithinDistance
        }
}