#region
#if UNITY_EDITOR
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;

namespace TwoBitMachines.FlareEngine
{
        [AddComponentMenu ("")]

        public class ReactionEvent : ReactionBehaviour
        {
                [SerializeField] public UnityEventEffect onReaction = new UnityEventEffect ( );

                public override void Activate (ImpactPacket impact)
                {
                        onReaction.Invoke (impact);
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField] public bool foldOutEvent;
                public override bool OnInspector (SerializedObject parent, Color barColor, Color labelColor)
                {
                        if (Open (parent, "On Reaction", barColor, labelColor))
                        {
                                Fields.EventFoldOut (parent.Get ("onReaction"), parent.Get ("foldOutEvent"), "On Reaction");

                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}