using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ȫ����Ϸ����������¼ѡ�н�ɫ�����������״̬���ݣ�������Ϸ�й�����ȡ�
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ���� ��ɫ & �����������˵����ã� ����
    [HideInInspector] public PlayerStatsData SelectedPlayer;
    [HideInInspector] public WeaponData SelectedWeapon;

    // ���� ��Ϸ��״̬���ݣ�UI�ɶ�ȡ�� ����
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentLevel { get; private set; }
    public int CurrentExp { get; private set; }

    // ���� ��Ϸ����״̬ ����
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        // ����ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // ����������ʼ�����ݣ������Gameplay��
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Gameplay") // �滻Ϊʵ������
        {
            InitGameData();
        }
    }

    /// <summary>
    /// ��ʼ����Ϸ��״̬���ӽ�ɫ���ݶ�ȡ��ʼֵ��
    /// </summary>
    private void InitGameData()
    {
        if (SelectedPlayer != null)
        {
            MaxHP = SelectedPlayer.baseMaxHp;
            CurrentHP = MaxHP;
            CurrentLevel = SelectedPlayer.baseLevel;
            CurrentExp = SelectedPlayer.baseExp;
        }
        else
        {
            Debug.LogWarning("GameManager: δ���ý�ɫ���ݣ�");
        }

        IsPaused = false;
    }

    /// <summary>
    /// �˺�����
    /// </summary>
    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);
        if (CurrentHP == 0)
        {
            Debug.Log("Player ����");
            // ��������չ Game Over
        }
    }

    /// <summary>
    /// ���Ӿ���
    /// </summary>
    public void GainExp(int amount)
    {
        CurrentExp += amount;
        // ʾ���������ƣ��ɻ��ɸ����ӹ�ʽ��
        int expToNext = 100 + 20 * (CurrentLevel - 1);
        while (CurrentExp >= expToNext)
        {
            CurrentExp -= expToNext;
            CurrentLevel++;
            Debug.Log($"�������ȼ� {CurrentLevel}��");
            expToNext = 100 + 20 * (CurrentLevel - 1);
        }
    }

    /// <summary>
    /// ��ͣ��ָ���Ϸ��UI��ť���ã�
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
