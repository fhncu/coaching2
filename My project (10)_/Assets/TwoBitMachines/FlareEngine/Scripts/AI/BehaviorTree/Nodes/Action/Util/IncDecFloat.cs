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
        public class IncDecFloat : Action
        {
                [SerializeField] public Blackboard data;
                [SerializeField] public IncDecType type;
                [SerializeField] public float by = 1f;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (data == null) return NodeState.Failure;

                        if (type == IncDecType.Increase)
                        {
                                data.Set (data.GetValue ( ) + by);
                        }
                        else
                        {
                                data.Set (data.GetValue ( ) - by);
                        }
                        return NodeState.Success;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool HasNextState ( ) { return false; }
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "Increase or decrease a float variable by the specified amount." +
                                        "\n \n Returns Success");
                        }
                        int index = parent.Enum ("type");
                        FoldOut.Box (3, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("data"), 0);
                        parent.Field ("Type", "type");
                        parent.Field ("By", "by");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }

        public enum IncDecType
        {
                Increase,
                Decrease
        }

}