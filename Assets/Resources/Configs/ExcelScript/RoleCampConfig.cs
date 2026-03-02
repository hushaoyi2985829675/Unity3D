using System.Collections.Generic;
using UnityEngine;

namespace RoleCampNs
{

	[System.Serializable]
	public class RoleCampInfo
	{
		public int campId;
		public string campImg;
		public string desc;
	}

	public class RoleCampConfig: ScriptableObject
	{
		public List<RoleCampInfo> roleCampInfoList = new List<RoleCampInfo>();
	}
}
