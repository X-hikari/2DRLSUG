using UnityEngine;

public class GameplayInitializer : MonoBehaviour
{
    public Transform playerSpawnPoint; // ��ҳ�����
    public WeaponFactory weaponFactory; // �ο������нű�
    public GameObject playerPrefab; // ��ѡ����Ԥ����ʵ�������

    private void Start()
    {
        // 1. ��ȡ GameManager ������
        var playerData = GameManager.Instance.SelectedPlayer;
        var weaponData = GameManager.Instance.SelectedWeapon;

        // 2. �ҵ������е� Player ��ʵ����
        GameObject playerObj = GameObject.FindWithTag("Player"); // ����ǳ�����ֱ�ӷŵ�
        // GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity); // �����Ԥ����

        if (playerObj == null)
        {
            Debug.LogError("GameplayInitializer: �Ҳ��� Player ����");
            return;
        }

        // 3. ��ʼ�� Player ����
 //       var playerController = playerObj.GetComponent<PlayerController>();
 //       var playerStats = playerObj.GetComponent<PlayerStats>(); // ��ʵ�ֵ����Կ��ƽű�

  //      if (playerStats != null && playerData != null)
   //     {
    //        playerStats.Initialize(playerData);
   //     }

        // 4. ��������������
        if (weaponFactory != null && weaponData != null)
        {
            Transform holdPoint = playerObj.transform.Find("WeaponHoldPoint");
            if (holdPoint == null)
            {
                Debug.LogWarning("�Ҳ��� WeaponHoldPoint��");
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
