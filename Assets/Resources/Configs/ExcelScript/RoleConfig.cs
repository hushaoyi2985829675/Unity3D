using System.Collections.Generic;
using UnityEngine;

namespace RoleNs
{

	[System.Serializable]
	public class RoleInfo
	{
		public int roleId;
		public string name;
		public int quality;
		public int campId;
		public string path;
		public string desc;
		public int hp;
		public int moveSpeed;
		public int runSpeed;
		public int jumpSpeed;
		public float idleTime;
		public float attackDic;
	}

	public class RoleConfig: ScriptableObject
	{
		public List<RoleInfo> roleInfoList = new List<RoleInfo>();
	}
}
