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
        public class FindCollider : Action
        {
                [SerializeField] public LayerMask colliderLayer;
                [SerializeField] private bool found;
                [SerializeField] private bool beginSearch;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                found = false;
                                beginSearch = true;
                        }
                        return found ? NodeState.Success : NodeState.Running;
                }

                public override void OnReset (bool skip = false)
                {
                        found = false;
                        beginSearch = false;
                }

                private void OnTriggerEnter2D (Collider2D collider)
                {
                        if (!beginSearch) return;

                        if (Compute.ContainsLayer (colliderLayer, collider.gameObject.layer))
                        {
                                found = true;
                        }
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;
                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (60, "Find a target collider on the specified layer. Set the collider" +
                                        " on this AI to IsTrigger. Returns Success if found, otherwise returns Running."
                                );
                        }

                        FoldOut.Box (1, color, yOffset: -2);
                        parent.Field ("Collider Layer", "colliderLayer");
                        Layout.VerticalSpacing (3);

                        return true;
                }

                #pragma warning restore 0414
                #endif
                #endregion

        }

}