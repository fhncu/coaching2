using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.Interactables
{
        [AddComponentMenu ("Flare Engine/一Interactables/Teleport")]
        public class Teleport : MonoBehaviour
        {
                [SerializeField] public Transform destination;
                [SerializeField] public LayerMask layerMask;
                [SerializeField] public UnityEvent onDelayStart;
                [SerializeField] public UnityEventEffect onTeleport;
                [SerializeField] public string teleportWE;
                [SerializeField] public float delay = 0;
                [SerializeField] public TeleportType type;
                [SerializeField] public InputButtonSO input;
                [System.NonSerialized] public bool pause = false;
                [System.NonSerialized] private WaitForSecondsRealtime waitForSeconds;
                public static List<Teleport> teleports = new List<Teleport> ( );

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] private bool foldOut;
                [SerializeField, HideInInspector] private bool eventFoldOut;
                [SerializeField, HideInInspector] private bool delayFoldOut;
                #pragma warning restore 0414
                #endif
                #endregion

                private void Awake ( )
                {
                        WorldManager.RegisterInput (input);
                        waitForSeconds = new WaitForSecondsRealtime (delay);
                }

                private void OnEnable ( )
                {
                        if (!teleports.Contains (this))
                        {
                                teleports.Add (this);
                        }
                }

                private void OnDisable ( )
                {
                        if (teleports.Contains (this))
                        {
                                teleports.Remove (this);
                        }
                }

                public static void ResetAll ( )
                {
                        for (int i = 0; i < teleports.Count; i++)
                        {
                                if (teleports[i] != null)
                                {
                                        teleports[i].StopAllCoroutines ( );
                                }
                        }

                }

                public void OnTriggerEnter2D (Collider2D other)
                {
                        if (pause)
                        {
                                pause = false;
                                return;
                        }
                        if (type == TeleportType.Automatic)
                        {
                                ValidateTransform (other);
                        }
                }

                public void OnTriggerStay2D (Collider2D other)
                {
                        if (type == TeleportType.Button && input != null && input.Holding ( ))
                        {
                                ValidateTransform (other);
                        }
                }

                private void ValidateTransform (Collider2D other)
                {
                        if (Compute.ContainsLayer (layerMask, other.gameObject.layer))
                        {
                                if (delay <= 0)
                                {
                                        TeleportNow (other.transform);
                                        return;
                                }
                                onDelayStart.Invoke ( );
                                StartCoroutine (TeleportDelay (other.transform));
                        }
                }

                private void TeleportNow (Transform target)
                {
                        if (destination == null)
                        {
                                return;
                        }

                        target.position = destination.position;
                        Teleport teleport = destination.GetComponent<Teleport> ( );

                        if (teleport != null)
                        {
                                teleport.pause = true;
                        }

                        onTeleport.Invoke (ImpactPacket.impact.Set (teleportWE, transform.position, Vector2.zero));
                }

                IEnumerator TeleportDelay (Transform target)
                {
                        yield return waitForSeconds;
                        TeleportNow (target);
                }

                public enum TeleportType
                {
                        Automatic,
                        Button
                }
        }
}