using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
        [System.Serializable]
        public class DetectWalls
        {
                [SerializeField] public DetectWallsType direction;
                [SerializeField] public LayerMask layerMask;

                [System.NonSerialized] private BoxCollider2D boxCollider;
                [System.NonSerialized] private Vector2 holdPosition;
                [System.NonSerialized] private Vector2 holdPositionTarget;
                [System.NonSerialized] private Follow follow;
                [System.NonSerialized] private bool found = false;
                public bool ignoreGravity => direction == DetectWallsType.IgnoreGravity;

                public void Initialize (Follow follow)
                {
                        this.follow = follow;
                        if (follow.targetTransform != null)
                        {
                                boxCollider = follow.targetTransform.GetComponent<BoxCollider2D> ( );
                        }
                }

                public void Reset ( )
                {
                        found = false;
                        holdPosition = follow.TargetPosition ( );
                        holdPositionTarget = holdPosition;
                }

                public Vector3 Position (Vector3 target, ScreenZone screenZone, Camera camera)
                {
                        //*  Ignore Gravity ignores target jumping. Camera can still follow in x.
                        //*  Detect Walls will only follow if target makes contact with surface.
                        if (direction == DetectWallsType.None || boxCollider == null)
                        {
                                return holdPosition = target;
                        }
                        FindWall (target, screenZone, camera);
                        return new Vector3 (ignoreGravity ? target.x : holdPosition.x, holdPosition.y);
                }

                public void FindWall (Vector2 target, ScreenZone screenZone, Camera camera)
                {
                        Physics2D.SyncTransforms ( );
                        UnityEngine.Bounds bounds = boxCollider.bounds;
                        screenZone.Clamp (ref target, ref holdPosition, camera);

                        if (ignoreGravity) // only detect bottom collision
                        {
                                bounds.center = bounds.center - bounds.extents.y * Vector3.up;
                                bounds.size = new Vector3 (bounds.size.x - 0.1f, 0.1f);
                        }
                        else //               detect all surfaces
                        {
                                bounds.Expand (0.025f);
                        }
                        bool previouslyNotFound = !found;
                        found = Physics2D.OverlapBox (bounds.center, bounds.size, boxCollider.transform.eulerAngles.z, layerMask);

                        if (previouslyNotFound && found && ignoreGravity)
                        {
                                follow.ForceTargetSmooth (x: false);
                        }
                        if (previouslyNotFound && found && !ignoreGravity)
                        {
                                follow.ForceTargetSmooth ( );
                        }
                        if (found)
                        {
                                holdPosition = target;
                        }

                        // if (found)
                        // {
                        //         holdPositionTarget = target;
                        // }
                        // holdPosition = Compute.Lerp (holdPosition, holdPositionTarget, 0.99f);
                }

                public void Set (int key)
                {
                        direction = (DetectWallsType) key;
                }

                public enum DetectWallsType
                {
                        None = 0,
                        IgnoreGravity = 1,
                        DetectWalls = 2
                }
        }

}