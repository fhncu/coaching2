using TwoBitMachines.FlareEngine.ThePlayer;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("")]
        public class ProjectileGrapplingHook : ProjectileBase
        {
                [SerializeField] public LayerMask layer;
                [SerializeField] public IgnoreEdge ignoreEdges;

                [SerializeField] public UnityEvent onMissed;
                [SerializeField] public UnityEventEffect onHook;
                [SerializeField] public string hookWE = "";

                [SerializeField] public float swingForce = 0.01f;
                [SerializeField] public float maxLength = 25f;
                [SerializeField] public float gravity = 0.05f;
                [SerializeField] public float jumpAway = 15f;

                [SerializeField] public bool retract = true;
                [SerializeField] public float rate = 5f;
                [SerializeField] public float minLength = 4f;
                [SerializeField] public RetractType retractType;
                [SerializeField] public InputButtonSO retractButton;
                [SerializeField] public Vector2 retractFriction = new Vector2 (0.2f, 0.55f);

                [SerializeField] public LineRenderer lineRenderer;
                [SerializeField] public AnimationCurve shootCurve;
                [SerializeField] public int pointsInLine = 30;
                [SerializeField] public float amplitude = 1.5f;
                [SerializeField] public float speed = 1f;

                [System.NonSerialized] private Particle handle = new Particle ( );
                [System.NonSerialized] private Particle anchor = new Particle ( );
                [System.NonSerialized] private Stick stick = new Stick ( );
                [System.NonSerialized] private RaycastHit2D hit;
                [System.NonSerialized] private float returnTime;
                [System.NonSerialized] private float moveTime;
                [System.NonSerialized] private bool hookSet;
                [System.NonSerialized] private bool retreat;
                [System.NonSerialized] private int inZone = 0;
                [System.NonSerialized] private float oldHandleVel;

                // editor
                [SerializeField] public bool hookFoldOut;
                [SerializeField] public bool missedFoldOut;

        public RaycastHit2D test;

                public override void ResetAll ( )
                {
                        TurnOff ( );
                }

                private void TurnOff ( )
                {
                        hookSet = false;
                        gameObject.SetActive (false);
                }

                public override bool FireProjectile (Vector2 position, Quaternion rotation, Vector2 playerVelocity)
                {
                        if (!ammunition.Consume (1, inventory))
                        {
                                return false;
                        }
                        if (ignoreEdges == IgnoreEdge.NeverIgnore)
                        {
                                SingleRay (rotation * Vector3.right);
                        }
                        else
                        {
                                MultipleRays (rotation * Vector3.right);
                        }
                        return true;
                }

                #region Effects

                private void ExtendRope (float sign = 1f, float speedBoost = 1f, float scale = 1f)
                {
                        Vector2 lineDirection = anchor.position - handle.position;
                        Vector2 waveDirection = lineDirection.normalized.Rotate (90f);
                        moveTime = Mathf.Clamp01 (moveTime + Time.deltaTime * speed * speedBoost * sign);
                        returnTime = Mathf.Clamp01 (moveTime >= 1f ? returnTime + Time.deltaTime * speed : 0);

                        for (int i = 0; i < pointsInLine; i++)
                        {
                                float percentPosition = (float) i / (pointsInLine - 1); // Calculate the lerp amount for each point based on its position in the line
                                Vector2 oscillate = waveDirection * shootCurve.Evaluate (percentPosition) * amplitude * scale * (1f - returnTime);
                                Vector2 pointPosition = Vector2.Lerp (handle.position, anchor.position, percentPosition * moveTime) + oscillate;
                                lineRenderer.SetPosition (i, pointPosition);
                        }

                        if (sign < 0 && moveTime <= 0)
                        {
                                TurnOff ( );
                        }
                }
                #endregion

                #region ShootHook,Rays
                public void SingleRay (Vector2 direction)
                {
                        RaycastHit2D ray = Physics2D.Raycast (transform.position, direction, maxLength, layer);

                        if (ray && ray.distance > 0)
                        {
                                SetHook (ray, ray.distance);
                        }
                        else
                        {
                                onMissed.Invoke ( );
                        }
                }

                public void MultipleRays (Vector2 direction)
                {
                        Vector2 origin = transform.position;
                        float actualDistance = 0;
                        for (int i = 0; i < 25; i++) // Will ignore up to twentyfive (arbitrary number), most of the time it will only execute once
                        {
                                RaycastHit2D ray = Physics2D.Raycast (origin, direction, maxLength, layer);
                                bool hitTarget = ray && ray.distance > 0;

                                if (ray)
                                {
                                        if (ray.collider is EdgeCollider2D && (ignoreEdges == IgnoreEdge.IgnoreAlways || direction.y > 0))
                                        {
                                                origin = ray.point + direction * 0.001f;
                                                actualDistance += ray.distance > 0 ? ray.distance : 0.001f;
                                                continue;
                                        }
                                        else if (ray.distance <= 0)
                                        {
                                                onMissed.Invoke ( );
                                                return;
                                        }
                                }
                                if (!ray)
                                {
                                        onMissed.Invoke ( );
                                        return;
                                }
                                if (hitTarget)
                                {
                                        actualDistance += ray.distance;
                                        SetHook (ray, actualDistance);
                                        return;
                                }
                        }
                }

                private void SetHook (RaycastHit2D hit, float distance)
                {
                        hookSet = true;
                        retreat = false;
                        moveTime = 0;
                        this.hit = hit;

            test = hit;
                        Debug.Log("a" + test.collider.gameObject.name);


                        stick.SetLength (distance);
                        anchor.Set (hit.point, 0, true);
                        handle.Set (transform.position, -gravity, false);
                        oldHandleVel = inZone = 0;

                        lineRenderer.positionCount = pointsInLine;
                        for (int i = 0; i < pointsInLine; i++)
                        {
                                lineRenderer.SetPosition (i, hit.transform.position);
                        }
                }
                #endregion

                public void FixedUpdate ( )
                {


                        if (!hookSet) return;

                        float hasFriction = retract && stick.length > minLength ? 1f : 0f;
                        handle.FixedUpdate (1f - retractFriction.x * hasFriction, 1f - retractFriction.y * hasFriction);
                        for (int i = 0; i < 10; i++)
                        {
                                stick.FixedUpdate (anchor, handle);
                        }
                }

                public override void LateExecute (AbilityManager player, ref Vector2 velocity)
                {

                        if (retreat)
                        {
                                handle.SetPosition (transform.position);
                                ExtendRope (-1f, 1.5f, 0.4f);
                        }
                        if (!hookSet)
                        {
                                return;
                        }

                        if (retract && (retractType == RetractType.Automatic || (retractButton != null && retractButton.Holding ( ))))
                        {
                                float rateBoost = player.world.box.center.y >= anchor.position.y ? 1.5f : 1f;
                                stick.length = Mathf.MoveTowards (stick.length, minLength, Time.deltaTime * rate * rateBoost);
                        }
                        if (player.world.onWall || player.world.onCeiling)
                        {
                                handle.SetPosition (transform.position);
                        }

                        handle.ApplyAcceleration (Vector2.right * player.inputX * swingForce); //     swing
                        velocity = (handle.position - (Vector2) transform.position) / Time.deltaTime;

                        ExtendRope ( );
                        Exit (player, ref velocity);
                        if (hookSet && !gameObject.activeInHierarchy) // enable here incase hook is being shot while player is on ground
                        {
                                onHook.Invoke (ImpactPacket.impact.Set (hookWE, hit.point, hit.normal));
                                gameObject.SetActive (true);
                        }
                        // Set signals
                        if (hookSet)
                        {
                                player.signals.Set ("grapplingHook", true);
                                player.signals.Set ("retractingHook", retract && stick.length > minLength);
                                player.signals.Set ("swingingHook", (player.inputX * swingForce) != 0);

                                float limit = 1.25f;
                                float handleVel = handle.velocity.x;
                                if (Mathf.Abs (handle.x - anchor.x) > limit)
                                {
                                        inZone = 0;
                                }
                                if (Mathf.Sign (handleVel) != Mathf.Sign (oldHandleVel))
                                {
                                        inZone++;
                                }
                                oldHandleVel = handleVel;

                                if (Mathf.Abs (handle.x - anchor.x) < limit && inZone >= 3) //Mathf.Abs (previousLeft - previousRight) < limit)
                                {
                                        player.signals.ForceDirection (1);
                                        player.signals.SetDirection (1);
                                }
                                if ((player.inputX * swingForce) != 0)
                                {
                                        player.signals.ForceDirection ((int) player.inputX);
                                        player.signals.SetDirection ((int) player.inputX);
                                }
                        }
                }

                private void Exit (AbilityManager player, ref Vector2 velocity)
                {
                        if (player.world.onGround && player.world.box.center.y >= anchor.position.y)
                        {
                                hookSet = false;
                                retreat = true;
                        }
                        if (player.jumpButtonPressed)
                        {
                                hookSet = false;
                                retreat = true;
                                velocity = new Vector2 (velocity.x, jumpAway);
                                player.CheckForAirJumps ( );
                        }
                }

        }

        public enum RetractType
        {
                Automatic,
                Button
        }
}