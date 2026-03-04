using System.Collections.Generic;
using UnityEngine;

namespace MonsterNs
{

	[System.Serializable]
	public class MonsterInfo
	{
		public int monsterId;
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
		public float attackSpeed;
	}

	public class MonsterConfig: ScriptableObject
	{
		public List<MonsterInfo> monsterInfoList = new List<MonsterInfo>();
	}
}
