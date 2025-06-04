using UnityEngine;

/// <summary>
/// ȫ����Ϸ��������洢����ڲ˵���ѡ���Ľ�ɫ�����������糡�����档
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // �˵���ѡ�õĽ�ɫ/����
    [HideInInspector] public PlayerStatsData SelectedPlayer;
    [HideInInspector] public WeaponData SelectedWeapon;

    private void Awake()
    {
        // ��������
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
