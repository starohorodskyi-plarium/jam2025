using UnityEngine;

namespace Platform
{
	public class IosSettings : MonoBehaviour
	{
		private const int DefaultFrameRate = 60;
			
#if UNITY_IOS
		private void Start()
		{
			var refreshRate = (int) Screen.currentResolution.refreshRateRatio.value;
			Application.targetFrameRate = refreshRate <= 31 ? DefaultFrameRate : refreshRate;
		}
 #else
 		private void Start() { }
 #endif
		
	}
}


