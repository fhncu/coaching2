using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.Interactables
{
        [AddComponentMenu ("Flare Engine/一Interactables/HighJump")]
        public class HighJump : MonoBehaviour
        {
                [SerializeField] public HighJumpType type;
                [SerializeField] public float force = 30f;
                [SerializeField] public string trampolineWE;
                [SerializeField] public bool moveWithParent = false;
                [SerializeField] public Vector2 windDirection = new Vector2 (0, 1f);
                [SerializeField] public UnityEventEffect onTrampoline;
                [SerializeField] public SimpleBounds bounds = new SimpleBounds ( );

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool foldOut;
                [SerializeField, HideInInspector] private bool eventFoldOut;
                [SerializeField, HideInInspector] private bool trampolineFoldOut;
                [SerializeField, HideInInspector] public Vector3 oldPosition;
                #pragma warning restore 0414
                #endif
                #endregion

                public static List<HighJump> highJumps = new List<HighJump> ( );

                private void Start ( )
                {
                        bounds.Initialize (transform);
                }
                private void OnEnable ( )
                {
                        if (!highJumps.Contains (this))
                        {
                                highJumps.Add (this);
                        }
                }

                private void OnDisable ( )
                {
                        if (highJumps.Contains (this))
                        {
                                highJumps.Remove (this);
                        }
                }

                public static bool Find (WorldCollision character, float characterVelocityY, ref int highJump, ref Vector2 force)
                {
                        for (int i = 0; i < HighJump.highJumps.Count; i++)
                        {
                                if (highJumps[i] != null && highJumps[i].bounds.ContainsRaw (character.oldPosition))
                                {
                                        if (highJumps[i].type == HighJumpType.Trampoline)
                                        {
                                                if (characterVelocityY > 0)
                                                {
                                                        return false;
                                                }
                                                character.hitInteractable = true;
                                                force = highJumps[i].force * highJumps[i].transform.up;
                                                highJumps[i].onTrampoline.Invoke (ImpactPacket.impact.Set (highJumps[i].trampolineWE, character.position, Vector2.zero));
                                                highJump = 1;
                                                return true;
                                        }
                                        else
                                        {
                                                character.hitInteractable = true;
                                                Vector2 windDirection = highJumps[i].windDirection;
                                                float percentToTop = Mathf.Clamp01 (character.transform.position.y / (highJumps[i].bounds.top - 0.5f));
                                                float windForce = Mathf.Lerp (0f, highJumps[i].force, 1f - percentToTop);
                                                force = windDirection * windForce * Time.deltaTime * 10f;
                                                highJump = 2;
                                                return true;
                                        }
                                }
                        }
                        return false;
                }
        }

        public enum HighJumpType
        {
                Trampoline,
                Wind
        }
}