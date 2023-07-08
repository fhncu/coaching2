using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        public class PropJump : MonoBehaviour
        {
                [SerializeField] public float jumpForce = 5f;
                [SerializeField] public float moveForce = 2f;
                [SerializeField] public float torqueAngle = 90f;
                [SerializeField] public Rigidbody2D rigidBody;

                public void Activate (ImpactPacket impact)
                {
                        float variance = Random.Range (0.75f, 1.25f);
                        float signX = Mathf.Sign (impact.direction.x);
                        rigidBody.AddForce (Vector3.right * signX * moveForce * variance, ForceMode2D.Impulse);
                        rigidBody.AddForce (Vector3.up * jumpForce * variance, ForceMode2D.Impulse);
                        rigidBody.AddTorque (torqueAngle * Mathf.Deg2Rad * -signX, ForceMode2D.Impulse);
                }
        }
}