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
            public class RotatePosition : Action
            {
                        [SerializeField] public Vector2 center;
                        [SerializeField] public float radius = 1f;
                        [SerializeField] public float offset;
                        [SerializeField] public float speed;

                        void Start ( )
                        {
                                    Vector2 newDirection = Compute.RotateVector (Vector2.right, offset);
                                    transform.position = center + newDirection * radius;
                        }

                        public override NodeState RunNodeLogic (Root root)
                        {
                                    if (nodeSetup == NodeSetup.NeedToInitialize)
                                    {
                                                if (radius <= 0) radius = 1f;
                                    }
                                    if (Time.deltaTime != 0)
                                    {
                                                Vector2 startPoint = transform.position;
                                                Vector2 direction = (startPoint - center) / radius;
                                                Vector2 newDirection = Compute.RotateVector (direction, speed * Time.deltaTime);
                                                Vector2 endPoint = center + newDirection * radius;
                                                root.velocity = (endPoint - startPoint) / Time.deltaTime;
                                    }
                                    return NodeState.Running;
                        }

                        #region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
                        #if UNITY_EDITOR
                        #pragma warning disable 0414
                        public override bool HasNextState ( ) { return false; }
                        public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
                        {
                                    if (parent.Bool ("showInfo"))
                                    {
                                                Labels.InfoBoxTop (55, "Rotate the transform's position around the center point at the specified radius." +
                                                            "\n \n Returns Running");
                                    }

                                    FoldOut.Box (4, color, yOffset: -2);
                                    parent.Field ("Center", "center");
                                    parent.Field ("Radius", "radius");
                                    parent.Field ("Speed", "speed");
                                    parent.Field ("Offset Angle", "offset");
                                    Layout.VerticalSpacing (3);
                                    return true;
                        }

                        public override void OnSceneGUI (UnityEditor.Editor editor)
                        {
                                    this.hideFlags = HideFlags.HideInInspector;
                                    if (center == Vector2.zero) center = this.transform.position;
                                    center = SceneTools.MovePositionCircleHandle (center, Vector2.zero, Tint.Delete, out bool changed);
                        }
                        #pragma warning restore 0414
                        #endif
                        #endregion 
            }
}