using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.BulletType
{
        [AddComponentMenu ("")]
        public class Bounce : BulletBase
        {
                [SerializeField] private Vector2 bounceFriction = Vector2.one * 0.05f;
                [SerializeField] private float bounceRadius = 0.5f;
                [SerializeField] private float bounceSpin;
                [SerializeField] private bool noSpin;
                [SerializeField] private bool activeAfterHit;
                [SerializeField] private bool addMomentum = false;

                [System.NonSerialized] private float dissipateX = 1f;
                [System.NonSerialized] private float dissipateY = 1f;
                [System.NonSerialized] private float angleCount = 0;

                public override void OnReset (Vector2 characterVelocity)
                {
                        dissipateX = 1f;
                        dissipateY = 1f;
                        angleCount = 0;
                        AddMomentum (addMomentum, characterVelocity);
                }

                public override void Execute ( )
                {
                        if (SetToSleep ( )) return;

                        LifeSpanTimer ( );
                        ApplyGravity (0.5f);
                        if (!noSpin && bounceSpin == 0) ApplyRotation (this.transform);

                        Vector3 actualVel = velocity * Time.deltaTime;
                        float velocityMagnitude = actualVel.magnitude;
                        Vector3 velocityNormal = actualVel / velocityMagnitude;
                        CollisionBounce (velocityNormal, velocityMagnitude);
                        if (gravity > 0) CollisionGround (velocityNormal);
                        transform.position = position;
                        transform.rotation = rotation;

                }

                private void CollisionBounce (Vector2 velocityNormal, float magnitude)
                {
                        RaycastHit2D ray = Physics2D.Raycast (position, velocityNormal, magnitude + bounceRadius, layer);
                        #region Debug
                        #if UNITY_EDITOR
                        if (WorldManager.viewDebugger)
                        {
                                Debug.DrawRay (position, velocityNormal * (magnitude + bounceRadius), Color.red); // only for testing
                        }
                        #endif
                        #endregion
                        Vector2 tempVel = velocity;
                        if (ray && !IgnoreEdges (ray.collider))
                        {
                                if (Health.IsDamageable (ray.transform))
                                {
                                        DealDamage (ray.transform, ray.distance > 0 ? ray.point : position, velocityNormal, !activeAfterHit);
                                }
                                else if (ray.distance > 0)
                                {
                                        dissipateX = dissipateX < 0.1 ? 0f : dissipateX * (1 - bounceFriction.x);
                                        dissipateY = dissipateY < 0.1 ? 0f : dissipateY * (1 - bounceFriction.y);
                                        velocity = Vector2.Reflect (velocity, ray.normal);
                                        velocity.x *= dissipateX;
                                        velocity.y *= dissipateY;
                                        tempVel = Time.deltaTime <= 0 ? Vector2.zero : (velocityNormal * (ray.distance - bounceRadius)) / Time.deltaTime;
                                }
                        }

                        ApplyGravity (0.5f); // Half of gravity must be applied after incase it bounces, so that reflection doesn't get extra energy
                        if (!noSpin && bounceSpin != 0 && velocity.x != 0)
                        {
                                rotation = Quaternion.Euler (new Vector3 (0, 0, (angleCount += bounceSpin * 10f * Time.deltaTime) * -velocity.x));
                        }
                        position += tempVel * Time.deltaTime;
                }

                private void CollisionGround (Vector2 velocityNormal)
                {
                        if (velocity.y > 0) return;

                        RaycastHit2D ray = Physics2D.Raycast (position, Vector2.down, bounceRadius, WorldManager.collisionMask);
                        if (ray && !IgnoreEdges (ray.collider))
                        {
                                position.y = ray.point.y + bounceRadius;
                                dissipateY = dissipateY < 0.1f ? 0f : dissipateY * (1f - bounceFriction.y);
                                velocity.y = Mathf.Abs (velocity.y) * Mathf.Sign (ray.normal.y) * dissipateY;
                        }
                }

        }
}