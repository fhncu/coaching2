using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [System.Serializable]
        public class MeleeBlock
        {
                [SerializeField] public bool canBlock;
                [SerializeField] public InputButtonSO input;
                [SerializeField] public InputButtonSO inputTwo;
                [SerializeField] public LayerMask blockLayer;
                [SerializeField] public string blockSignal;
                [SerializeField] public BlockHoldType mustHold;
                [SerializeField] public bool stopVelocityX;
                [SerializeField] public bool cancelCombo;

                [System.NonSerialized] public Collider2D collider2DRef;
                [System.NonSerialized] public bool needToRelease;

                #region 
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOut = false;
                #pragma warning restore 0414
                #endif
                #endregion

                public void Initialize (Collider2D collider2DRef)
                {
                        this.collider2DRef = collider2DRef;
                        WorldManager.RegisterInput (input);
                        WorldManager.RegisterInput (inputTwo);

                        if (canBlock)
                        {
                                this.collider2DRef.gameObject.AddComponent<Health> ( );
                        }
                }

                public bool IsBlocking ( )
                {
                        if (input == null) return false;

                        if (input.Holding ( ) && (mustHold == BlockHoldType.None || (inputTwo != null && inputTwo.Holding ( ))))
                        {
                                return true;
                        }
                        return false;
                }

                public void Block (AnimationSignals signals, ref Vector2 velocity)
                {
                        if (collider2DRef != null) collider2DRef.enabled = true;
                        signals.Set ("meleeCombo", true);
                        signals.Set (blockSignal);
                        if (stopVelocityX) velocity.x = 0;
                        needToRelease = true;
                }

        }

        public enum BlockHoldType
        {
                None,
                Include
        }
}