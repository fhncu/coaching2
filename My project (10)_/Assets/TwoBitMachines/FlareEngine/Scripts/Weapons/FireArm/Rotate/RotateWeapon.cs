﻿using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [System.Serializable]
        public class RotateWeapon
        {
                [SerializeField] public AutoSeek autoSeek = new AutoSeek ( );
                [SerializeField] public Transform transformDirection;
                [SerializeField] public WeaponOrientation orientation;
                [SerializeField] public RotateType rotate;
                [SerializeField] public float angleOffset;
                [SerializeField] public float maxLimit;
                [SerializeField] public float minLimit;
                [SerializeField] public float fixedAngle;
                [SerializeField] public Vector2 fixedDirection;
                [SerializeField] public bool diagonal = false;
                [SerializeField] public InputButtonSO up;
                [SerializeField] public InputButtonSO down;
                [SerializeField] public InputButtonSO left;
                [SerializeField] public InputButtonSO right;

                [System.NonSerialized] private int weaponDirection;
                [System.NonSerialized] private Vector3 mousePosition;
                [System.NonSerialized] private Vector3 smoothDirection;
                [System.NonSerialized] private Vector3 previousWeaponPosition;
                [System.NonSerialized] private Vector3 previousCharacterPosition;

                private bool clampAngle => minLimit != 0 || maxLimit != 0;
                private bool mouseRotation => orientation == WeaponOrientation.MouseDirection;

                public void Reset ( )
                {
                        autoSeek.Reset ( );
                        previousWeaponPosition = Vector3.zero; // avoid jitter in rotation
                        previousCharacterPosition = Vector3.zero;
                }

                public void Execute (Firearm firearm, Character equipment, ref bool fire)
                {
                        if (rotate == RotateType.NoRotation)
                        {
                                return;
                        }
                        if (rotate == RotateType.FixedDirection)
                        {
                                FixedDirection (firearm);
                                return;
                        }
                        Transform transform = firearm.transform;
                        // Get weapon direction
                        mousePosition = Util.MousePosition ( );
                        SetWeaponDirection (firearm, equipment);
                        // Change weapon x position
                        transform.localPosition = Util.FlipXSign (transform.localPosition, weaponDirection);
                        // Rotate weapon
                        if (rotate == RotateType.Mouse)
                        {
                                RotateToMouse (transform, equipment.transform.right);
                        }
                        else if (rotate == RotateType.EightDirection)
                        {
                                RotateToEightDirection (transform);
                        }
                        else if (rotate == RotateType.AutoSeek)
                        {
                                RotateToAutoSeek (transform, equipment, ref fire);
                        }
                        else if (rotate == RotateType.FixedAngle)
                        {
                                Rotate (transform, fixedAngle * weaponDirection);
                        }
                        previousWeaponPosition = transform.position;
                }

                private void FixedDirection (Firearm firearm)
                {
                        Vector2 v = new Vector2 (Mathf.Abs (fixedDirection.x), fixedDirection.y);
                        float angle = Vector2.Angle (Vector2.right, v) * Mathf.Sign (v.y);
                        weaponDirection = fixedDirection.x > 0 ? 1 : -1;
                        FixedRotate (firearm.transform, angle);
                }

                private void SetWeaponDirection (Firearm firearm, Character equipment)
                {
                        if (firearm.IsRecoiling ( ))
                        {
                                equipment.signals.characterDirection = weaponDirection; // if player is recoiling, retain same weapon direction or weapon may flip based on player input Override.
                        }
                        if (orientation != WeaponOrientation.CharacterDirection)
                        {
                                bool useTarget = orientation == WeaponOrientation.TransformDirection && transformDirection != null;
                                Vector3 targetPosition = autoSeek.Rotate ( ) ? autoSeek.Position (mousePosition) : useTarget? transformDirection.position: (Vector3) mousePosition;
                                Vector3 targetDirection = (targetPosition - previousCharacterPosition).normalized;
                                int direction = Compute.CrossSign (equipment.transform.up, targetDirection) >= 0 ? -1 : 1;
                                equipment.signals.Set ("mouseDirectionLeft", direction == -1);
                                equipment.signals.Set ("mouseDirectionRight", direction == 1);
                                equipment.signals.characterDirection = direction;
                        }
                        previousCharacterPosition = equipment.transform.position; // avoid jitter
                        weaponDirection = equipment.signals.characterDirection;
                }

                private void RotateToAutoSeek (Transform transform, Character equipment, ref bool fire)
                {
                        autoSeek.Seek (equipment.transform.position, ref fire);
                        Vector3 direction = autoSeek.Rotate ( ) ? (autoSeek.Position (mousePosition) - previousWeaponPosition) : Vector3.right * weaponDirection;
                        smoothDirection = Compute.LerpConditional (smoothDirection, direction.normalized, ref autoSeek.newTarget);
                        float angle = Compute.AngleDirection (equipment.transform.right * weaponDirection, smoothDirection); //
                        Rotate (transform, angle);
                }

                private void RotateToMouse (Transform transform, Vector2 characterRight)
                {
                        Vector2 rightDirection = characterRight * weaponDirection;
                        Vector2 mouseDirection = (mousePosition - previousWeaponPosition).normalized; // use previous position of weapon to avoid jitter
                        float rotate = Compute.AngleDirection (rightDirection, mouseDirection);

                        if (clampAngle)
                        {
                                float minAngle = weaponDirection == 1 ? minLimit : -maxLimit;
                                float maxAngle = weaponDirection == 1 ? maxLimit : -minLimit;

                                if (rotate < minAngle || rotate > maxAngle) // clamp mouse to range,if not in range, make gun still rotate to mouse by checking which limit it's closest to
                                {
                                        Vector2 v1 = Compute.RotateVector (rightDirection, minAngle);
                                        Vector2 v2 = Compute.RotateVector (rightDirection, maxAngle);
                                        rotate = Vector3.Angle (v1, mouseDirection) < Vector3.Angle (v2, mouseDirection) ? minAngle : maxAngle;
                                }
                        }
                        Rotate (transform, rotate, angleOffset);
                }

                private void RotateToEightDirection (Transform transform)
                {
                        int inputX = right != null && right.Holding ( ) ? 1 : left != null && left.Holding ( ) ? -1 : 0;
                        int inputY = up != null && up.Holding ( ) ? 1 : down != null && down.Holding ( ) ? -1 : 0;
                        float angle = ((diagonal && inputX != 0) ? 45f : 90f) * weaponDirection * inputY;
                        Rotate (transform, angle);
                }

                private void Rotate (Transform transform, float angle, float angleOffset = 0)
                {
                        float rotateAngle = weaponDirection < 0 ? -angle : angle;
                        transform.localRotation = Quaternion.Euler (0, weaponDirection < 0 ? 180f : 0f, rotateAngle + angleOffset);
                }

                private void FixedRotate (Transform transform, float angle)
                {
                        transform.localRotation = Quaternion.Euler (0, weaponDirection < 0 ? 180f : 0f, angle);
                }
        }

        public enum RotateType
        {
                Mouse,
                EightDirection,
                AutoSeek,
                FixedAngle,
                FixedDirection,
                NoRotation
        }

        public enum WeaponOrientation
        {
                CharacterDirection,
                MouseDirection,
                TransformDirection
        }
}