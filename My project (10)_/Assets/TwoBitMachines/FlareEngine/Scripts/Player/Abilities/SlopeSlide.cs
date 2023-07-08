#region
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        [AddComponentMenu ("")]
        public class SlopeSlide : Ability
        {
                [SerializeField] public string button;
                [SerializeField] public SlideType type;
                [SerializeField] public float scale = 1f;
                [SerializeField] public float exitTime = 1f;
                [SerializeField] public float rangeStart = 5f;
                [SerializeField] public float rangeEnd = 88f;
                [SerializeField] public bool autoSlide = false;

                [SerializeField] public LayerMask damageLayer;
                [SerializeField] public bool dealDamage;
                [SerializeField] public float damageAmount = 5f;
                [SerializeField] public float damageForce = 1f;

                [System.NonSerialized] private Health health;
                [System.NonSerialized] private bool sliding;
                [System.NonSerialized] private float counter;
                [System.NonSerialized] private float direction;

                private bool automatic => type == SlideType.Automatic;

                private void Awake ( )
                {
                        health = this.gameObject.GetComponent<Health> ( );
                        autoSlide = false;
                }

                public override void Reset (AbilityManager player)
                {
                        if (sliding && dealDamage && health != null) health.CanTakeDamage (true);
                        sliding = false;
                        direction = 0;
                        counter = 0;
                }

                public override bool IsAbilityRequired (AbilityManager player, ref Vector2 velocity)
                {
                        if (pause) return false;

                        if (sliding && (!player.world.onGround || player.jumpButtonActive || player.world.onWall))
                        {
                                Reset (player);
                                return false;
                        }
                        if (sliding && (direction != 0 && velocity.x != 0 && !Compute.SameSign (direction, velocity.x)))
                        {
                                Reset (player); // player changed direction while sliding, exit out
                                return false;
                        }
                        if (sliding && (!player.world.climbingSlopeDown || ValidAngle (player)) && (exitTime <= 0 || counter >= exitTime))
                        {
                                Reset (player); // no ease time or ease time complete
                                return false;
                        }
                        if (sliding)
                        {
                                return true;
                        }
                        // if (autoSlide && player.world.onGround && ValidAngle (player))
                        // {
                        //         return sliding = true;
                        // }
                        if ((automatic || player.inputs.Holding (button)) && player.world.climbingSlopeDown && ValidAngle (player))
                        {
                                return sliding = true;
                        }
                        return false;
                }

                public bool ValidAngle (AbilityManager player)
                {
                        float angle = Vector2.Angle (player.world.box.up, player.world.groundNormal);
                        return angle >= rangeStart && angle <= rangeEnd;
                }

                public override void ExecuteAbility (AbilityManager player, ref Vector2 velocity, bool isRunningAsException = false)
                {
                        player.signals.Set ("slopeSlide");

                        if (player.world.climbingSlopeDown) //|| autoSlide)
                        {
                                counter = 0;
                                direction = direction == 0 ? velocity.x : direction; //(autoSlide ? player.playerDirection : velocity.x) : direction;
                                velocity.x = direction * scale;
                                if (dealDamage && direction != 0)
                                {
                                        Attack (player, Mathf.Sign (direction), direction * Time.deltaTime);
                                }
                                return;
                        }
                        if (exitTime > 0)
                        {
                                counter += Time.deltaTime;
                                float percent = 1f - (counter / exitTime);
                                velocity.x = direction * scale * percent;
                                player.signals.Set ("slopeSlideAuto");
                        }
                }

                public void Attack (AbilityManager player, float signX, float velX)
                {
                        if (health != null) health.CanTakeDamage (false);
                        Vector2 normal = player.world.groundNormal;
                        Vector2 direction = normal.Rotate (90f * signX);
                        Vector2 corner = player.world.box.BottomCorner (signX);
                        RaycastHit2D enemy = Physics2D.Raycast (corner, direction, velX * 1.75f, damageLayer);
                        // Debug.DrawRay (corner, direction * velX * 1.25f, Color.red);
                        if (enemy)
                        {
                                Health.IncrementHealth (transform, enemy.transform, -damageAmount, direction * damageForce);
                        }
                }

                public enum SlideType
                {
                        Button,
                        Automatic
                }

                #region ▀▄▀▄▀▄ Custom Inspector ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool OnInspector (SerializedObject controller, SerializedObject parent, string[] inputList, Color barColor, Color labelColor)
                {
                        if (Open (parent, "Slope Slide", barColor, labelColor))
                        {
                                int type = parent.Enum ("type");
                                int height = parent.Bool ("dealDamage") ? 2 : 0;
                                FoldOut.Box (5 + height, FoldOut.boxColorLight, yOffset: -2);
                                parent.DropDownListAndField (inputList, "Button", "button", "type", execute : type == 0);
                                parent.Field ("Button", "type", execute : type == 1);
                                parent.FieldDouble ("Angle Range", "rangeStart", "rangeEnd");
                                Labels.FieldDoubleText ("Start", "End");
                                parent.Field ("Speed Boost", "scale");
                                parent.Field ("Exit Time", "exitTime");
                                // parent.FieldToggle ("Auto Slide", "autoSlide");
                                parent.FieldAndEnable ("Deal Damage", "damageLayer", "dealDamage");

                                bool dealDamage = parent.Bool ("dealDamage");
                                parent.Field ("Damage Amount", "damageAmount", execute : dealDamage);
                                parent.Field ("Damage Force", "damageForce", execute : dealDamage);
                                Layout.VerticalSpacing (3);
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}