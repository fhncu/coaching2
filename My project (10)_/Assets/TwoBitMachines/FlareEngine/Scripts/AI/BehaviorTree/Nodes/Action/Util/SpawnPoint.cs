#region 
#if UNITY_EDITOR
using System.Threading;
using TwoBitMachines.Editors;
using UnityEditor;
#endif
#endregion
using UnityEngine;
using UnityEngine.Events;

namespace TwoBitMachines.FlareEngine.AI
{
        [AddComponentMenu ("")]
        public class SpawnPoint : Action
        {
                [SerializeField] private Vector3 spawnPoint;
                [SerializeField] private Vector3 eulerAngle;

                private void Awake ( )
                {
                        spawnPoint = this.transform.position;
                        eulerAngle = this.transform.localEulerAngles;
                }

                public override NodeState RunNodeLogic (Root root)
                {
                        this.transform.position = spawnPoint;
                        this.transform.localEulerAngles = eulerAngle;
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
                                Labels.InfoBoxTop (55, "Reset the AI to the position it had at the beginning of the scene." +
                                        "\n \n Returns Success");
                        }
                        return true;
                }
                #pragma warning restore 0414
                #endif
                #endregion
        }
}