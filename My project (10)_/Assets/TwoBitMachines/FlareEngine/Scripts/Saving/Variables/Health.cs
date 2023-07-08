using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("Flare Engine/Health")]
        public class Health : WorldFloat
        {
                [SerializeField] public float recoveryTime = 0;
                [SerializeField] public float counter = 0;
                [SerializeField] public bool hasShield = false;
                [SerializeField] public int shieldDirection = 1;

                [SerializeField] private Character character;
                [SerializeField] private UnityEventEffect onShield = new UnityEventEffect ( );

                [System.NonSerialized] public int originLayer;
                [System.NonSerialized] public int currentLayer;
                [System.NonSerialized] public static Dictionary<Transform, Health> health = new Dictionary<Transform, Health> ( );

                private void Awake ( )
                {
                        originLayer = transform.gameObject.layer;
                        currentLayer = originLayer;
                }

                public override void Register ( )
                {
                        if (register)
                        {
                                soReference.Register (this);
                        }
                        if (!health.ContainsKey (transform))
                        {
                                health.Add (transform, this);
                        }
                }

                public override void Reset ( )
                {
                        counter = 0;
                        transform.gameObject.layer = originLayer;
                        if (isSaved)
                        {
                                SetRefreshValue ( );
                        }
                }

                public void StartRecovery ( )
                {
                        if (recoveryTime > 0)
                        {
                                counter = 0;
                                CanTakeDamage (false);
                                WorldManager.get.update -= Recover;
                                WorldManager.get.update += Recover;
                        }
                }

                public void Recover (bool gameReset = false)
                {
                        if (gameReset || Clock.Timer (ref counter, recoveryTime))
                        {
                                counter = 0;
                                CanTakeDamage (true);
                                WorldManager.get.update -= Recover;
                        }
                }

                public bool Recovering ( )
                {
                        return recoveryTime > 0 && transform.gameObject.layer == 2;
                }

                public void HideLayer (bool isHiding)
                {
                        int layer = transform.gameObject.layer;
                        if (isHiding && layer != WorldManager.hideLayer)
                        {
                                currentLayer = WorldManager.hideLayer;
                        }
                        else if (!isHiding && layer != originLayer)
                        {
                                currentLayer = originLayer;
                        }
                        if (layer != 2 && layer != currentLayer)
                        {
                                transform.gameObject.layer = currentLayer;
                        }
                }

                public void CanTakeDamage (bool value)
                {
                        transform.gameObject.layer = value ? currentLayer : 2; // 2 = ignore raycast layer
                }

                public bool ShieldActive (Transform from, Vector2 direction)
                {
                        if (character == null)
                        {
                                character = gameObject.GetComponent<Character> ( );
                        }
                        if (character == null)
                        {
                                return false;
                        }
                        if (shieldDirection > 0)
                        {
                                if ((character.signals.characterDirection > 0 && direction.x < 0) || (character.signals.characterDirection < 0 && direction.x > 0))
                                {
                                        onShield.Invoke (BasicImpact (from, direction));
                                        return true;
                                }
                        }
                        if (shieldDirection < 0)
                        {
                                if ((character.signals.characterDirection > 0 && direction.x > 0) || (character.signals.characterDirection < 0 && direction.x < 0))
                                {
                                        onShield.Invoke (BasicImpact (from, direction));
                                        return true;
                                }
                        }
                        return false;
                }

                public static bool HitContactResults (Transform aggressor, List<Collider2D> contactResults, int hits, float damage, float damageForce, Vector2 origin)
                {
                        bool hit = false;
                        for (int i = 0; i < hits; i++)
                        {
                                if (contactResults[i] != null)
                                {
                                        Vector2 targetPosition = contactResults[i].transform.position;
                                        if (IncrementHealth (aggressor, contactResults[i].transform, damage, (targetPosition - origin).normalized * damageForce))
                                        {
                                                hit = true;
                                        }
                                }
                        }
                        return hit;
                }

                public static bool IncrementHealth (Transform from, Transform to, float amount, Vector2 direction)
                {
                        if (to != null && health.TryGetValue (to, out Health healthKey))
                        {
                                if (healthKey.Recovering ( ))
                                {
                                        return false;
                                }
                                if (healthKey.hasShield && healthKey.ShieldActive (from, direction))
                                {
                                        return false;
                                }
                                if (amount < 0)
                                {
                                        healthKey.StartRecovery ( ); // damage is negative
                                }
                                return healthKey.IncrementValue (from, amount, direction);
                        }
                        return false;
                }

                public static bool IsDamageable (Transform transform)
                {
                        return transform != null && health.ContainsKey (transform);
                }
        }
}