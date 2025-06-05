using System.IO;
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
    public int CurrentHP { get;  set; }
    public int MaxHP { get;  set; }
    public int CurrentLevel { get;  set; }
    public int CurrentExp { get;  set; }
    public float MaxMana { get; set; }
    public float CurrentMana { get; set; }

    // ���� ��Ϸ����״̬ ����
    public bool IsPaused { get; private set; }
    private string savePath => Application.persistentDataPath + "/save.json";

    public void SaveGame()
    {

        
        SaveData data = new SaveData
        {
            playerId = SelectedPlayer.name,   // ��ΨһID
            weaponId = SelectedWeapon.name,   // ��ΨһID
            hp = CurrentHP,
            maxHp = MaxHP,
            level = CurrentLevel
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        Debug.Log($"��Ϸ�ѱ��浽 {savePath}");
    }

    public bool HasSave() => File.Exists(savePath);

    public void LoadGame()
    {
        if (!HasSave()) return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // �������ݵ� GameManager
        SelectedPlayer = LoadPlayerById(data.playerId);
        SelectedWeapon = LoadWeaponById(data.weaponId);
        CurrentHP = data.hp;
        MaxHP = data.maxHp;
        CurrentLevel = data.level;
    }

    private PlayerStatsData LoadPlayerById(string id)
    {
        // �ɴ� Resources��ScriptableObject �������ȼ���
        return Resources.Load<PlayerStatsData>("Data/Players/" + id);
    }

    private WeaponData LoadWeaponById(string id)
    {
        return Resources.Load<WeaponData>("Data/Weapons/" + id);
    }

    public void ClearSave()
    {
        if (HasSave()) File.Delete(savePath);
    }
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
        BuffConditionRegister.RegisterAll();
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
            // InitGameData();
        }
    }

    // /// <summary>
    // /// ��ʼ����Ϸ��״̬���ӽ�ɫ���ݶ�ȡ��ʼֵ��
    // /// </summary>
    // private void InitGameData()
    // {
    //     if (SelectedPlayer != null)
    //     {
    //         MaxHP = SelectedPlayer.baseMaxHp;
    //         CurrentHP = MaxHP;
    //         CurrentLevel = SelectedPlayer.baseLevel;
    //         CurrentExp = SelectedPlayer.baseExp;
    //     }
    //     else
    //     {
    //         Debug.LogWarning("GameManager: δ���ý�ɫ���ݣ�");
    //     }

    //     IsPaused = false;
    // }

    /// <summary>
    /// ��ͣ��ָ���Ϸ��UI��ť���ã�
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
