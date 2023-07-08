using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
        [System.Serializable]
        public class ScreenZone
        {
                [SerializeField] public Vector2 size;
                [System.NonSerialized] public Vector2 origin;

                public Vector3 Velocity (Vector2 target, Camera camera, bool isUser)
                {
                        if (this.size == Vector2.zero || isUser) return Vector3.zero;

                        Vector2 velocityClamp = Vector3.zero;
                        Vector2 cameraPosition = camera.transform.position;
                        Vector2 zone = new Vector2 (camera.Width ( ) * size.x, camera.Height ( ) * size.y);

                        if (zone.x > 0 && target.x < (cameraPosition.x - zone.x)) velocityClamp.x = target.x - (cameraPosition.x - zone.x);
                        if (zone.x > 0 && target.x > (cameraPosition.x + zone.x)) velocityClamp.x = target.x - (cameraPosition.x + zone.x);
                        if (zone.y > 0 && target.y < (cameraPosition.y - zone.y)) velocityClamp.y = target.y - (cameraPosition.y - zone.y);
                        if (zone.y > 0 && target.y > (cameraPosition.y + zone.y)) velocityClamp.y = target.y - (cameraPosition.y + zone.y);
                        return velocityClamp;
                }

                public void Clamp (ref Vector2 read, ref Vector2 set, Camera camera)
                {
                        Vector2 cameraPosition = camera.transform.position;
                        Vector2 zone = new Vector2 (camera.Width ( ) * size.x, camera.Height ( ) * size.y);
                        if (zone.x > 0 && read.x < (cameraPosition.x - zone.x)) set.x = (cameraPosition.x - zone.x);
                        if (zone.x > 0 && read.x > (cameraPosition.x + zone.x)) set.x = (cameraPosition.x + zone.x);
                        if (zone.y > 0 && read.y < (cameraPosition.y - zone.y)) set.y = (cameraPosition.y - zone.y);
                        if (zone.y > 0 && read.y > (cameraPosition.y + zone.y)) set.y = (cameraPosition.y + zone.y);
                }
        }
}