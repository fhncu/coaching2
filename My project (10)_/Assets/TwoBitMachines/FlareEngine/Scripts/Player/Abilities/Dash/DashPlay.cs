using UnityEngine;

namespace TwoBitMachines.FlareEngine.ThePlayer
{
        public class DashPlay
        {
                [System.NonSerialized] public int directionX;
                [System.NonSerialized] public Vector2 direction;
                [System.NonSerialized] private Vector2 distance;
                [System.NonSerialized] public bool clearYNextFrame;
                [System.NonSerialized] private float dashRateCounterCounter;
                [System.NonSerialized] private float incCounter;
                [System.NonSerialized] private Health health;
                [System.NonSerialized] private Dash dash;

                public void Initialize (Dash dashRef, Transform transform)
                {
                        dash = dashRef;
                        health = transform.GetComponent<Health> ( );
                }

                public void Reset ( )
                {
                        clearYNextFrame = false;
                        incCounter = 0;
                        dashRateCounterCounter = 10000f;
                }

                public void ResetDamage ( )
                {
                        if (!dash.canTakeDamage && health != null)
                        {
                                health.CanTakeDamage (true);
                        }
                }

                public void GetPlayerDirectionX (AbilityManager player, float velX)
                {
                        directionX = player.playerDirection;
                        if (player.world.onWall)
                        {
                                directionX = player.world.leftWall ? 1 : -1;
                        }
                        else if (velX != 0)
                        {
                                directionX = (int) Mathf.Sign (velX);
                        }
                }

                public void SetDashDirection (AbilityManager player)
                {
                        if (dash.directionType == DashDirection.HorizontalAxis)
                        {
                                direction.x = directionX;
                                distance.x = dash.dashDistance;
                                distance.y = direction.y = 0;
                        }
                        else
                        {
                                int x = (player.inputs.Holding (dash.left) ? -1 : 0) + (player.inputs.Holding (dash.right) ? 1 : 0);
                                int y = (player.inputs.Holding (dash.up) ? 1 : 0) + (player.inputs.Holding (dash.down) ? -1 : 0);
                                x = x != 0 && player.world.onWall ? directionX : x;
                                x = x == 0 && y == 0 ? directionX : x; // Force x to have a value if none
                                bool bothActive = x != 0 && y != 0;

                                direction.x = player.world.box.right.x * x;
                                direction.y = player.world.box.up.y * y;
                                distance.x = bothActive ? Mathf.Cos (45f * Mathf.Deg2Rad) * dash.dashDistance : x != 0 ? dash.dashDistance : 0;
                                distance.y = bothActive ? Mathf.Cos (45f * Mathf.Deg2Rad) * dash.dashDistance : y != 0 ? dash.dashDistance : 0;
                        }

                        if (dash.crouch)
                        {
                                player.world.box.ChangeColliderHeight (dash.crouchHeight);
                        }
                        if (!dash.canTakeDamage && health != null)
                        {
                                health.CanTakeDamage (false);
                        }
                }

                public void Execute (AbilityManager player, ref Vector2 velocity, ref bool isDashing)
                {
                        isDashing = false;
                        if (dash.type == DashType.Instant && !dash.dontDash)
                        {
                                Dash (player, ref velocity, Time.deltaTime, dash.directionType == DashDirection.HorizontalAxis);
                        }
                        else if (dash.time != 0 && !dash.dontDash && Clock.TimerInverse (ref incCounter, dash.time))
                        {
                                isDashing = true;
                                Dash (player, ref velocity, dash.time, true);
                        }
                        if (isDashing && player.world.onWall && direction.y == 0)
                        {
                                if (direction.x < 0 && player.world.leftWall) isDashing = false;
                                if (direction.x > 0 && player.world.rightWall) isDashing = false;
                        }
                        if (isDashing && player.world.onCeiling && direction.x == 0 && direction.y > 0)
                        {
                                isDashing = false;
                        }
                        if (!isDashing && dash.crouch && player.world.boxCollider.size.y != player.world.box.boxSize.y)
                        {
                                if (SafelyStandUp (player.world.box))
                                {
                                        player.world.box.ColliderReset ( );
                                }
                                else
                                {
                                        dash.dontDash = true;
                                        isDashing = true;
                                        player.signals.Set ("crouch");
                                }
                        }
                        if (!isDashing)
                        {
                                ResetDamage ( );
                                dash.OnEndEvent (player);
                        }
                }

                private void Dash (AbilityManager player, ref Vector2 velocity, float time, bool nullGravity)
                {
                        velocity.x = distance.x != 0 ? (distance.x / time) * direction.x : velocity.x;
                        velocity.y = distance.y != 0 ? (distance.y / time) * direction.y : velocity.y;

                        player.signals.Set ("dashing");
                        player.signals.Set ("dashX", distance.x != 0);
                        player.signals.Set ("dashY", distance.y != 0);
                        player.signals.Set ("dashDiagonal", distance.x != 0 && distance.y != 0);

                        if (direction.x != 0)
                        {
                                player.signals.SetDirection ((int) direction.x);
                                player.playerDirection = (int) direction.x;
                        }
                        if (nullGravity && dash.nullifyGravity && distance.y == 0 && !player.world.onGround && velocity.y < 0)
                        {
                                velocity.y = 0;
                        }
                        if (distance.y != 0)
                        {
                                clearYNextFrame = true;
                        }
                        if (dash.onDashRate > 0 && Clock.Timer (ref dashRateCounterCounter, dash.onDashRate))
                        {
                                ImpactPacket impact = ImpactPacket.impact.Set (dash.worldEffect, dash.transform, player.world.boxCollider, dash.transform.position, null, direction, 0);
                                dash.onDash.Invoke (impact);
                        }
                }

                public bool SafelyStandUp (BoxInfo ray)
                {
                        float length = Mathf.Abs (ray.boxSize.y - ray.collider.size.y) * ray.collider.transform.localScale.y;
                        for (int i = 0; i < ray.rays.y; i++)
                        {
                                Vector2 origin = ray.cornerTopLeft + ray.right * (ray.spacing.x * i);
                                #region Debug
                                #if UNITY_EDITOR
                                if (WorldManager.viewDebugger)
                                {
                                        Debug.DrawRay (origin, ray.up * length, Color.white);
                                }
                                #endif
                                #endregion
                                if (Physics2D.Raycast (origin, ray.up, length, WorldManager.collisionMask))
                                {
                                        return false;
                                }
                        }
                        return true;
                }
        }
}