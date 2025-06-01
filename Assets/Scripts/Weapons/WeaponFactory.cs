using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 武器工厂，负责创建和管理Weapon实例
/// </summary>
public class WeaponFactory : MonoBehaviour
{
    [Header("武器预制体列表")]
    public Weapon[] weaponPrefabs;

    // 武器池（可选）
    private Dictionary<string, Queue<Weapon>> weaponPool = new();

    /// <summary>
    /// 根据武器名创建武器实例
    /// </summary>
    /// <param name="weaponName">武器名</param>
    /// <param name="parent">武器挂载的父物体</param>
    /// <returns>武器实例</returns>
    public Weapon CreateWeapon(string weaponName, Transform parent = null)
    {
        // 尝试从池里取
        if (weaponPool.TryGetValue(weaponName, out Queue<Weapon> pool) && pool.Count > 0)
        {
            Weapon weapon = pool.Dequeue();
            weapon.gameObject.SetActive(true);
            if (parent != null)
                weapon.transform.SetParent(parent, false);
            return weapon;
        }

        // 没有池或者空，实例化新武器
        Weapon prefab = FindWeaponPrefabByName(weaponName);
        if (prefab == null)
        {
            Debug.LogWarning($"WeaponFactory: 没有找到名为 {weaponName} 的武器预制体！");
            return null;
        }

        Weapon newWeapon = Instantiate(prefab, parent);
        newWeapon.name = prefab.name; // 去掉 "(Clone)"

        // **初始化武器数据，确保渲染和属性生效**
        if (prefab.data != null)
        {
            newWeapon.Initialize(prefab.data);
        }
        else
        {
            Debug.LogWarning($"WeaponFactory: 预制体 {weaponName} 没有关联 WeaponData！");
        }

        return newWeapon;
    }

    /// <summary>
    /// 回收武器到池中（隐藏，不销毁）
    /// </summary>
    /// <param name="weapon">武器实例</param>
    public void RecycleWeapon(Weapon weapon)
    {
        weapon.gameObject.SetActive(false);
        weapon.transform.SetParent(this.transform); // 回收到工厂下，方便管理

        if (!weaponPool.ContainsKey(weapon.name))
        {
            weaponPool[weapon.name] = new Queue<Weapon>();
        }
        weaponPool[weapon.name].Enqueue(weapon);
    }

    /// <summary>
    /// 通过名字查找武器预制体
    /// </summary>
    /// <param name="weaponName"></param>
    /// <returns></returns>
    private Weapon FindWeaponPrefabByName(string weaponName)
    {
        foreach (var prefab in weaponPrefabs)
        {
            if (prefab.name == weaponName)
                return prefab;
        }
        return null;
    }
}
