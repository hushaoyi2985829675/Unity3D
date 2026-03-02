using System.Collections;
using System.Collections.Generic;
using RoleCampNs;
using RoleNs;
using UnityEngine;

public class RoleManager : Singleton<RoleManager>
{
    private string path = "RolePrefab/";
    // public GameObject RoleObj(int heroId)
    // {
    //     
    // }
    
    public GameObject GetRoleRef(int id)
    {
        RoleInfo roleInfo = ConfigManager.Instance.GetRoleInfoById(id);
        string type = "1HS/";
        switch ((RoleCamp)roleInfo.campId)
        {
            case RoleCamp.DoubleSwords:
                type = "2HS/";
                break;
            case RoleCamp.Bow:
                type = "Bow/";
                break;
            case RoleCamp.Magic:
                type = "Magic/";
                break;
            case RoleCamp.SwordShield:
                type = "1HSS/";
                break;
        }

        GameObject obj = Resources.Load<GameObject>(path + type + roleInfo.path);
        return obj;
    }

    public GameObject GetBaseRoleInstance(int id,Transform parent,Vector3 position = default(Vector3),Quaternion rotation = default(Quaternion))
    {
        GameObject roleObj =  GetRoleRef(id);
        GameObject roleIns = Instantiate(roleObj, position, rotation);
        roleIns.transform.SetParent(parent,false);
        CapsuleCollider capsuleCollider = roleIns.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 1.78f;
        capsuleCollider.center = new Vector3(0, 0.88f, 0f);
        capsuleCollider.radius = 0.62f;
        return roleIns;
    }
}
