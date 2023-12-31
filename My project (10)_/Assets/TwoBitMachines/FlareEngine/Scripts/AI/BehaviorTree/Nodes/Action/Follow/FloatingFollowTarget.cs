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
        public class FloatingFollowTarget : Action
        {
                [SerializeField] public Blackboard target;
                [SerializeField] public Vector2 offset;
                [SerializeField] public float speed = 5f;
                [SerializeField] public float wobbleSpeed = 1f;
                [SerializeField] public float wobbleHeight = 0.5f;

                [System.NonSerialized] private Character character;

                public override NodeState RunNodeLogic (Root root)
                {
                        if (target == null)
                        {
                                return NodeState.Failure;
                        }
                        if (nodeSetup == NodeSetup.NeedToInitialize)
                        {
                                Transform transform = target.GetTransform ( );
                                character = null;
                                if (transform != null)
                                {
                                        character = transform.GetComponent<Character> ( );
                                }
                        }
                        if (Time.deltaTime != 0)
                        {
                                Vector2 followPosition = target.GetTarget ( );
                                Vector2 followOffset = offset;
                                if (character != null)
                                {
                                        followOffset.x = character.signals.characterDirection > 0 ? -Mathf.Abs (followOffset.x) : Mathf.Abs (followOffset.x);
                                }
                                followPosition += followOffset;
                                followPosition.y += Compute.SineWave (0, wobbleSpeed, wobbleHeight);

                                Vector2 newPosition = Vector2.Lerp (root.position, followPosition, speed * Time.deltaTime);
                                root.velocity = (newPosition - root.position) / Time.deltaTime; // 
                        }
                        return NodeState.Running;
                }

                #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                #if UNITY_EDITOR
                #pragma warning disable 0414
                [SerializeField, HideInInspector] public bool foldOutEvent;

                public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                {
                        if (parent.Bool ("showInfo"))
                        {
                                Labels.InfoBoxTop (55, "Follow a target while floating in the air." +
                                        "\n \n Returns Running, Success, Failure");
                        }

                        FoldOut.Box (4, color, yOffset: -2);
                        AIBase.SetRef (ai.data, parent.Get ("target"), 0);
                        parent.Field ("Speed", "speed");
                        parent.Field ("Offset", "offset");
                        parent.FieldDouble ("Wobble", "wobbleSpeed", "wobbleHeight");
                        Labels.FieldDoubleText ("Speed", "Height");
                        Layout.VerticalSpacing (3);
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }

}