#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]

        public class TeleportNextToTarget : Action
        {
                [SerializeField] public Blackboard target;
                [SerializeField] public float distance;
                [SerializeField] public bool exitOnWall;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (target == null)
                        {
                                return NodeState.Failure;
                        }

                        BoxInfo box = root.world.box;
                        float tempDistance = target.GetTarget ( ).x - root.position.x;
                        float sign = Mathf.Sign (tempDistance);
                        float magnitude = distance + Mathf.Abs (tempDistance) + box.skin.x * 2f;
                        Vector2 corner = sign > 0 ? box.bottomRight - box.skinX : box.bottomLeft + box.skinX;

                        for (int i = 0; i < box.rays.x; i++)
                        {
                                Vector2 origin = corner + box.up * box.spacing.y * i;
                                RaycastHit2D hit = Physics2D.Raycast (origin, box.right * sign, magnitude, WorldManager.collisionMask);
                                if (hit)
                                {
                                        if (exitOnWall) return NodeState.Failure;
                                        if (hit.distance > 0) magnitude = hit.distance - box.skin.x * 2f;
                                }
                        }
                        this.transform.position += Vector3.right * sign * magnitude; // teleport
                        return NodeState.Success;

                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;

                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "Teleport next to the target by the distance specified. Exit teleportation if there is a wall in the way." +
                                        "\n \n Returns Success, Failure");
                        }

                        FoldOut.Box (3, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("target"), 0);
                        parent.Field ("Distance", "distance");
                        parent.Field ("Exit on Wall", "exitOnWall");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

}