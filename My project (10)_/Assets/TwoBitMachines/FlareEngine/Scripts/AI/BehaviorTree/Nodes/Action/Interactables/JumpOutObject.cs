#region 
#if UNITY_EDITOR
using System.Resources;
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class JumpOutObject : Conditional
        {
                [SerializeField] public InputButtonSO inputButtonSO;
                [SerializeField] public Player player;
                [SerializeField] public string signal;
                [SerializeField] public float top = 1.5f;
                [SerializeField] public int playerOrder;
                [SerializeField, SortingLayer] public string playerLayer;
                [SerializeField] public UnityEvent onJumpedOut;

                [SerializeField] private State state;
                [SerializeField] private Health playerHealth;
                [SerializeField] private SpriteRenderer playerRenderer;
                [SerializeField] private float holdTimer = 0;
                [SerializeField] private Vector3 oldPosition;

                [System.NonSerialized] private Vector2 velocity;

                private float gravity => player.abilities.gravity.gravity;
                private Vector2 topPoint => transform.position + Vector3.up * top;

                public enum State
                {
                        Waiting,
                        JumpOverTop,
                        HoldTimer,
                        Complete,
                        HardReset
                }

                private void Awake ( )
                {
                        WorldManager.RegisterInput (inputButtonSO);
                }

                public override NodeState RunNodeLogic (Root root)
                {
                        if (player == null || inputButtonSO == null)
                        {
                                return NodeState.Failure;
                        }

                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                state = State.Waiting;
                                if (playerRenderer == null)
                                {
                                        playerRenderer = player.transform.GetComponent<SpriteRenderer> ( );
                                }
                                if (playerHealth == null)
                                {
                                        playerHealth = player.transform.GetComponent<Health> ( );
                                }
                                if (playerRenderer != null)
                                {
                                        playerRenderer.enabled = false;
                                }
                        }

                        if (state == State.Waiting)
                        {
                                if (inputButtonSO.Pressed ( ))
                                {
                                        state = State.JumpOverTop;
                                        oldPosition = player.transform.position;
                                        velocity = Compute.ArchObject (oldPosition, topPoint, 0.5f, gravity);
                                        player.signals.SetDirection (1);
                                        if (playerRenderer != null)
                                        {
                                                playerRenderer.enabled = true;
                                        }
                                }
                        }
                        if (state != State.Waiting)
                        {
                                root.signals.Set (signal);
                        }
                        if (state == State.JumpOverTop)
                        {
                                velocity.y += gravity * Time.deltaTime;
                                player.transform.position = oldPosition + (Vector3) velocity * Time.deltaTime;
                                oldPosition = player.transform.position; //     must remember old position because player controller will also apply gravity
                                player.abilities.ClearVelocity ( ); //          clear gravity so it doesn't build up

                                if (velocity.y <= 0)
                                {
                                        state = State.HoldTimer;
                                        if (playerRenderer != null)
                                        {
                                                playerRenderer.sortingLayerName = playerLayer;
                                                playerRenderer.sortingOrder = playerOrder;
                                        }
                                        onJumpedOut.Invoke ( );
                                        player.world.isHidingExternal = false;
                                        player?.BlockInput (false);
                                        playerHealth?.CanTakeDamage (true);
                                }
                        }
                        if (state == State.HoldTimer)
                        {
                                if (TwoBitMachines.Clock.Timer (ref holdTimer, 0.50f))
                                {
                                        state = State.Complete;
                                        return NodeState.Success;
                                }
                        }
                        return NodeState.Running;
                }

                public override bool HardReset ( )
                {
                        state = State.Waiting;
                        player.world.isHidingExternal = false;
                        if (playerRenderer != null)
                        {
                                playerRenderer.enabled = true;
                                playerRenderer.sortingLayerName = playerLayer;
                                playerRenderer.sortingOrder = playerOrder;
                        }
                        return true;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (75,
                                        "The player will jump out the object's top point on button press and set its rendering layer to the specified one." +
                                        " While jumping out, the AI's jump out signal will be set true." +
                                        "\n \n Returns Running, Failure, Success");
                        }

                        FoldOut.Box (5, color, yOffset: -2);
                        parent.Field ("Exit Button", "inputButtonSO");
                        parent.Field ("Player", "player");
                        parent.Field ("Top", "top");
                        parent.FieldDouble ("Player Layer", "playerLayer", "playerOrder");
                        Labels.FieldText ("Order");
                        parent.Field ("Jump Out Signal", "signal");
                        Layout.VerticalSpacing (1);
                        Fields.EventFoldOut (parent.Get ("onJumpedOut"), parent.Get ("foldOutEvent"), "On Jumped Out", color : color);
                        return true;
                }

                public override void OnSceneGUI (Editor editor)
                {
                        Vector3 p = this.transform.position;
                        Draw.Circle (p + Vector3.up * top, 0.15f, Color.red);
                }

                #pragma warning restore 0414
                #endif
                #endregion

        }
}