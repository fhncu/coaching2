#region 
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class SpriteColor : Action
        {
                [SerializeField] public SpriteRenderer sprite;
                [SerializeField] public Color from;
                [SerializeField] public Color to;
                [SerializeField] public float time;
                [SerializeField] public bool revertOnReset = true;

                [System.NonSerialized] private float counter;
                [System.NonSerialized] private Color origin;

                void Awake ( )
                {
                        if (sprite != null) origin = sprite.color;
                }

                public override NodeState RunNodeLogic (Root root)
                {
                        if (sprite == null) return NodeState.Failure;

                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                counter = 0;
                        }

                        if (TwoBitMachines.Clock.TimerInverse (ref counter, time))
                        {
                                sprite.color = Color.Lerp (from, to, counter / time);
                                return NodeState.Running;
                        }
                        sprite.color = to;
                        return NodeState.Success;
                }

                public override void OnReset (bool skip = false)
                {
                        if (sprite != null && revertOnReset)
                        {
                                sprite.color = origin;
                        }
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "This will modify the sprite's color by lerping to a new one." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        FoldOut.Box (5, color, yOffset: -2);
                        parent.Field ("Sprite Renderer", "sprite");
                        parent.Field ("From", "from");
                        parent.Field ("To", "to");
                        parent.Field ("Time", "time");
                        parent.Field ("Revert On Reset", "revertOnReset");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion 
        }

}