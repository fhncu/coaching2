using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("")]
        public class CameraShake : ReactionBehaviour
        {
                [SerializeField] public string shakeName;

                public override void Activate (ImpactPacket packet)
                {
                        if (Safire2DCamera.Safire2DCamera.mainCamera != null)
                        {
                                Safire2DCamera.Safire2DCamera.mainCamera.Shake (shakeName);
                        }
                }
        }
}