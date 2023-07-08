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
        public class FollowTarget : Action
        {
                [SerializeField] public Blackboard target;
                [SerializeField] public Axis axis;
                [SerializeField] public float speed = 5f;
                [SerializeField] public float findDistance;
                [SerializeField] public bool hasGravity;
                [SerializeField] public float pauseTimer;
                [SerializeField] public bool pauseOnChange;
                [SerializeField] private float xVelRef;
                [SerializeField] private bool crossedThreshold;
                [SerializeField] private bool refreshTarget = true;
                [System.NonSerialized] private Vector3 targetP;

                private bool pauseX;
                private float counter;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (target == null)
                        {
                                return NodeState.Failure;
                        }
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                xVelRef = 0;
                                counter = 0;
                                pauseX = false;
                                crossedThreshold = false;
                                target.ResetIndex ( );
                                targetP = target.GetTarget ( );
                        }

                        if (refreshTarget)
                        {
                                targetP = target.GetTarget ( );
                        }

                        if (Time.deltaTime == 0)
                        {
                                return NodeState.Running;
                        }

                        if (axis == Axis.X)
                        {
                                float newP = Mathf.MoveTowards (root.position.x, targetP.x, speed * Time.deltaTime);
                                root.velocity.x += (newP - root.position.x) / Time.deltaTime;
                                if (pauseOnChange && !pauseX && !Compute.SameSign (xVelRef, root.velocity.x))
                                {
                                        pauseX = true;
                                }
                                if (pauseX && TwoBitMachines.Clock.TimerInverse (ref counter, pauseTimer))
                                {
                                        root.velocity.x = 0;
                                        root.signals.Set ("directionChangedPause");
                                }
                                else
                                {
                                        pauseX = false;
                                }

                                if (hasGravity && !root.onGround)
                                {
                                        root.velocity.x = xVelRef; // If jumping, retain old direction.
                                        if (Mathf.Abs (targetP.x - root.position.x) <= findDistance)
                                        {
                                                crossedThreshold = true;
                                        }
                                }
                                else if ((Mathf.Abs (targetP.x - root.position.x) <= findDistance) || crossedThreshold)
                                {
                                        root.velocity.x = 0;
                                        crossedThreshold = false;
                                        return target.NextTarget (ref targetP);
                                }
                                xVelRef = root.velocity.x;
                        }
                        else if (axis == Axis.Y)
                        {
                                float newP = Mathf.MoveTowards (root.position.y, targetP.y, speed * Time.deltaTime);
                                root.velocity.y = (newP - root.position.y) / Time.deltaTime;
                                if (Mathf.Abs (targetP.y - root.position.y) <= findDistance)
                                {
                                        root.velocity.y = 0;
                                        return target.NextTarget (ref targetP);
                                }
                        }
                        else
                        {
                                Vector2 newPosition = Vector2.MoveTowards (root.position, targetP, speed * Time.deltaTime);
                                root.velocity = (newPosition - root.position) / Time.deltaTime;
                                if ((targetP - (Vector3) root.position).sqrMagnitude <= findDistance * findDistance)
                                {
                                        root.velocity = Vector2.zero;
                                        return target.NextTarget (ref targetP);
                                }
                        }
                        return NodeState.Running;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;

                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (128, "Follow the target on the chosen axis with the specified speed. The AI will stop near the target if it's within the stop distance. Refresh Target will refresh the target's position every frame. If has gravity is enabled, the AI will retain its direction in the x-axis if it's jumping. The AI will pause if target changes direction." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        FoldOut.Box (7, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("target"), 0);
                        parent.Field ("Axis", "axis");
                        parent.Field ("Speed", "speed");
                        parent.Field ("Stop Distance", "findDistance");
                        parent.FieldAndEnable ("Pause On Change", "pauseTimer", "pauseOnChange");
                        parent.FieldToggleAndEnable ("Refresh Target", "refreshTarget");
                        parent.FieldToggleAndEnable ("Has Gravity", "hasGravity");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

        public enum Axis
        {
                XY,
                X,
                Y
        }

}