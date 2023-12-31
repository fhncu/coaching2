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
        public class FollowBezierPath : Action
        {
                [SerializeField] public TargetBezierPath bezierPath;
                [SerializeField] public float time = 2f;

                [System.NonSerialized] public float counter;
                [System.NonSerialized] public int index;
                [System.NonSerialized] public float shift;
                [System.NonSerialized] public bool inReverse;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (bezierPath == null)
                        {
                                return NodeState.Failure;
                        }
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                inReverse = false;
                                counter = shift = index = 0;
                        }

                        Vector2 newPosition = bezierPath.FollowPath (root.position, this, out NodeState complete);
                        root.velocity = Time.deltaTime <= 0 ? Vector2.zero : (newPosition - root.position) / Time.deltaTime;
                        return complete;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;

                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (60, "Follow the bezier path. Time is the duration it takes to complete a segment in the path." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        FoldOut.Box (2, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("bezierPath"), 0);
                        parent.Field ("Time", "time");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

}