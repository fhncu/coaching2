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
        public class GetRandomTarget : Action
        {
                [SerializeField] public Blackboard list;
                [SerializeField] public Blackboard setTarget;
                [SerializeField] public SetRandomResultAs setTargetAs;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (setTarget == null || list == null || list.ListCount ( ) == 0) return NodeState.Failure;

                        if (setTargetAs == SetRandomResultAs.Transform)
                        {
                                setTarget.Set (list.GetRandomTransformTarget ( ));
                        }
                        else
                        {
                                setTarget.Set (list.GetRandomTarget ( ));
                        }

                        return NodeState.Success;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "Get a random target from a list and set the reference to this target." +
                                        "\n \n Returns Success, Failure");
                        }

                        FoldOut.Box (3, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("list"), 0);
                        AIBase.SetRef (ai.data, parent.Get ("setTarget"), 1);
                        parent.Field ("Set Target As", "setTargetAs");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }

        public enum SetRandomResultAs
        {
                Transform,
                Vector2
        }
}