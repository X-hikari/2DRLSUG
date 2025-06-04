using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    public Transform playerSpawnPoint; // 玩家出生点
    public WeaponFactory weaponFactory; // 参考你已有脚本
    public GameObject playerPrefab; // 可选：用预制体实例化玩家

    private void Start()
    {
        // 1. 获取 GameManager 的数据
        var playerData = GameManager.Instance.SelectedPlayer;
        var weaponData = GameManager.Instance.SelectedWeapon;

        // 2. 找到场景中的 Player 或实例化
        GameObject playerObj = GameObject.FindWithTag("Player"); // 如果是场景中直接放的
        // GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity); // 如果用预制体

        if (playerObj == null)
        {
            Debug.LogError("GameplayInitializer: 找不到 Player 对象！");
            return;
        }

        // 3. 初始化 Player 属性
 //       var playerController = playerObj.GetComponent<PlayerController>();
 //       var playerStats = playerObj.GetComponent<PlayerStats>(); // 你实现的属性控制脚本

  //      if (playerStats != null && playerData != null)
   //     {
    //        playerStats.Initialize(playerData);
   //     }

        // 4. 创建并挂载武器
        if (weaponFactory != null && weaponData != null)
        {
            Transform holdPoint = playerObj.transform.Find("WeaponHoldPoint");
            if (holdPoint == null)
            {
                Debug.LogWarning("找不到 WeaponHoldPoint！");
                return;
            }

            Weapon weapon = weaponFactory.CreateWeapon(weaponData.weaponName, holdPoint);
            if (weapon != null)
            {
                weapon.Initialize(weaponData);
            }
        }
    }
}
