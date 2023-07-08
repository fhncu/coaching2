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
	public class FollowSurfacePoints : Action
	{
		[SerializeField] public TargetPoints target;

		[System.NonSerialized] private Vector3 targetP;
		[System.NonSerialized] private Vector3 previousTargetP;

		public override NodeState RunNodeLogic (Root root)
		{

			if (target == null)
			{
				return NodeState.Failure;
			}
			if (nodeSetup == NodeSetup.NeedToInitialize)
			{
				targetP = target.GetTarget ( );
			}

			return NodeState.Success;
		}

		#region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
		#if UNITY_EDITOR
		#pragma warning disable 0414
		public override bool HasNextState ( ) { return false; }
		public override bool OnInspector (AIBase ai, SerializedObject parent, Color color, bool onEnable)
		{
			// if (parent.Bool ("showInfo"))
			// {
			// 	Labels.InfoBox (35, "Sets an AI animation signal true.");
			// 	Labels.InfoBox (35, "Returns Success");
			// }
			// FoldOut.Box (1, color, yOffset: -2);
			// parent.Field ("Signal", "signalName");
			// Layout.VerticalSpacing (3);
			return true;
		}
		#pragma warning restore 0414
		#endif
		#endregion 
	}
}