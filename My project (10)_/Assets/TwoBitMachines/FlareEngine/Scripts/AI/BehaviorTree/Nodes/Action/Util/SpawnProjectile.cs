#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
using TwoBitMachines.FlareEngine.AI.BlackboardData;
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class SpawnProjectile : Action
        {
                [SerializeField] public ProjectileBase projectile;
                [SerializeField] public Vector2 direction;
                [SerializeField] public PositionType type;
                [SerializeField] public Blackboard target;
                [SerializeField] public Vector2 position;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (projectile == null || (type == PositionType.Target && target == null)) return NodeState.Failure;

                        if (type == PositionType.Point)
                        {
                                projectile.FireProjectile (position, direction);
                        }
                        else
                        {
                                projectile.FireProjectile (target.GetTarget ( ), direction);
                        }
                        return NodeState.Success;
                }

                public enum PositionType
                {
                        Point,
                        Target
                }

                #region ▀▄▀▄▀▄ Custom Inspector▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public bool foldOutEvent;
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (85, "Spawn a projectile at the specified position." +
                                        "\n \n Returns Success, Failure");
                        }

                        int type = parent.Enum ("type");

                        FoldOut.Box (4, color, yOffset: -2);
                        parent.Field ("Projectile", "projectile");
                        parent.Field ("Direction", "direction");
                        parent.Field ("Type", "type");
                        parent.Field ("Point", "position", execute : type == 0);
                        if (type == 1) AIBase.SetRef (ai.data, parent.Get ("target"), 0);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

}