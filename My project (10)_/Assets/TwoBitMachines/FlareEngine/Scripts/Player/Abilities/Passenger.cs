#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu ("")]
        public class Passenger : Ability
        {
                [SerializeField] public LayerMask vehicleLayer;
                [SerializeField] public string exit;
                [SerializeField] public bool exitOnDeath;
                [SerializeField] public UnityEvent onExit;

                [System.NonSerialized] private Health health;
                [System.NonSerialized] private Vehicle vehicle;
                [System.NonSerialized] private Player player;

                private bool isExiting = false;
                public bool isDead => health != null && health.GetValue ( ) <= 0;

                public override void Initialize (Player playerRef)
                {
                        player = playerRef;
                        health = gameObject.GetComponent<Health> ( );
                }

                public override void Reset (AbilityManager player)
                {
                        ExitVehicle ( );
                        player.onVehicle = false;
                }

                public override bool TurnOffAbility (AbilityManager player)
                {
                        Reset (player);
                        return true;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        if (pause) return false;

                        if (isExiting && player.ground)
                        {
                                isExiting = false;
                        }

                        if ((vehicle != null && player.inputs.PressedUnblocked (exit)) || (exitOnDeath && isDead))
                        {
                                Reset (player);
                                onExit.Invoke ( );
                                isExiting = true;
                                return false;
                        }

                        if (vehicle != null)
                        {
                                return true;
                        }

                        if (Search (player, velocity))
                        {
                                return true;
                        }
                        return false;
                }

                public void ExitVehicle ( )
                {
                        if (vehicle != null)
                        {
                                vehicle.passengerRef = null;
                                vehicle.passengerAbility = null;
                                vehicle = null;
                        }
                        player.BlockInput (false);
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        player.onVehicle = true;
                        this.player.BlockInput (true);
                        velocity = Vector2.zero;
                }

                public bool Search (AbilityManager player, Vector2 velocity) // since using EarlyExecute, this will ALWAYS execute. No need for priority.
                {
                        if (isExiting || vehicle != null || player.ground || velocity.y > 0) return false;

                        Vector2 v = velocity * Time.deltaTime;
                        BoxInfo box = player.world.box;
                        float magnitude = Mathf.Abs (v.y) + box.skin.y;

                        Vector2 origin = box.bottomCenter + box.right * v.x;
                        RaycastHit2D hit = Physics2D.Raycast (origin, -box.up, magnitude, vehicleLayer);

                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                Debug.DrawRay (origin, -box.up * magnitude, Color.blue);
                        }
                        #endif
                        #endregion

                        if (hit)
                        {
                                Vehicle vehicle = hit.transform.GetComponent<Vehicle> ( );
                                if (vehicle != null)
                                {
                                        this.vehicle = vehicle;
                                        this.vehicle.passengerRef = this;
                                        this.vehicle.passengerAbility = this.player;
                                        this.vehicle.onMounted.Invoke ( );
                                        this.vehicle.counter = 0;

                                        player.signals.characterDirection = this.vehicle.playerRef.signals.characterDirection;
                                        return true;
                                }
                        }
                        return false;
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open (parent, "Passenger", barColor, labelColor))
                        {
                                FoldOut.Box (3, FoldOut.boxColorLight, yOffset: -2);
                                {
                                        parent.Field ("Layer", "vehicleLayer");
                                        parent.DropDownList (inputList, "Exit", "exit");
                                        parent.FieldToggleAndEnable ("Exit On Death", "exitOnDeath");
                                }
                                Layout.VerticalSpacing (3);
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}