using System.Collections.Generic;
using UnityEngine;

namespace TwoBitMachines.Safire2DCamera
{
	[CreateAssetMenu (menuName = "FlareEngine/ShakesSO")]
	public class ShakeStateSaved : ScriptableObject
	{
		[SerializeField, HideInInspector] public List<ShakeInfo> shakes = new List<ShakeInfo> ( );

		public void Shake (string shakeName)
		{
			if (Safire2DCamera.mainCamera == null) return;
			Safire2DCamera.mainCamera.Shake (shakeName);
		}

		#region ▀▄▀▄▀▄ Editor Variables ▄▀▄▀▄▀
		#if UNITY_EDITOR
		#pragma warning disable 0414
		[SerializeField, HideInInspector] public int signalIndex;
		[SerializeField, HideInInspector] public bool active;
		#pragma warning restore 0414
		#endif
		#endregion
	}
}