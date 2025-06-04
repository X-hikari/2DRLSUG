using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("���˵����")]
    public GameObject mainMenuPanel;     // ���� ������Ϸ�������������������á������˳��� ��ť
    public Button newGameButton;
    public Button continueButton;
    public Button settingsButton;
    public Button exitButton;

    [Header("����Ϸ�����")]
    public GameObject newGamePanel;      // ����Ϸ��壨������ѡ�������壩
    public Button backToMenuButton;      // ����Ϸ����� ���������˵��� ��ť

    [Header("��ѡ����� (MainSelect)")]
    public GameObject mainSelectPanel;   // ���� ��ѡ���ɫ������ѡ��������������ʼ��Ϸ��
    public Button selectCharacterButton;
    public Button selectWeaponButton;
    public Button startGameButton;

    // ��ѡ�������չʾ��ѡ��ɫ�������� Image + Text
    public Image mainSelectCharacterImage;
    public TextMeshProUGUI mainSelectCharacterName;
    public Image mainSelectWeaponImage;
    public TextMeshProUGUI mainSelectWeaponName;

    [Header("����ѡ����� (WeaponSelect)")]
    public GameObject weaponSelectPanel; // ���� ���� �л���ť �� ��ȷ�ϡ� ��ť
    public Button weaponLeftButton;
    public Button weaponRightButton;
    public Button weaponConfirmButton;

    // ����ѡ���������ʾ����Ԥ��
    public Image weaponPreviewImage;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;

    [Header("��ɫѡ����� (CharacterSelect)")]
    public GameObject charaSelectPanel;  // ���� ���� �л���ť �� ��ȷ�ϡ� ��ť
    public Button charaLeftButton;
    public Button charaRightButton;
    public Button charaConfirmButton;

    // ��ɫѡ���������ʾ��ɫԤ��
    public Image charaPreviewImage;
    public TextMeshProUGUI charaNameText;
    public TextMeshProUGUI charaStatsText;

    // �Ѽ��ص����� WeaponData
    private List<WeaponData> weaponList = new List<WeaponData>();
    private int currentWeaponIndex = 0;
    private WeaponData selectedWeapon;

    // �Ѽ��ص����� PlayerStatsData
    private List<PlayerStatsData> playerList = new List<PlayerStatsData>();
    private int currentPlayerIndex = 0;
    private PlayerStatsData selectedPlayer;

    void Start()
    {
        // ���� ���˵���ť�� ���� 
        newGameButton.onClick.AddListener(OnNewGame);
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);

        // ���� ����Ϸ����� ��ť�� ���� 
        backToMenuButton.onClick.AddListener(OnBackToMainMenu);
        selectCharacterButton.onClick.AddListener(OnEnterCharacterSelect);
        selectWeaponButton.onClick.AddListener(OnEnterWeaponSelect);
        startGameButton.onClick.AddListener(OnStartGame);

        // ���� ����ѡ����� ��ť�� ���� 
        weaponLeftButton.onClick.AddListener(() => ChangeWeaponSelection(-1));
        weaponRightButton.onClick.AddListener(() => ChangeWeaponSelection(1));
        weaponConfirmButton.onClick.AddListener(OnConfirmWeaponSelect);

        // ���� ��ɫѡ����� ��ť�� ���� 
        charaLeftButton.onClick.AddListener(() => ChangePlayerSelection(-1));
        charaRightButton.onClick.AddListener(() => ChangePlayerSelection(1));
        charaConfirmButton.onClick.AddListener(OnConfirmCharacterSelect);

        // Ĭ��ֻ��ʾ���˵�
        ShowMainMenu();

        // ���� Resources/Data/Weapons �µ���������
        LoadAllWeapons();

        // ���� Resources/Data/Players �µĽ�ɫ����
        LoadAllPlayers();
    }

    /// <summary>
    /// ��ʾ���˵��������������������
    /// </summary>
    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        newGamePanel.SetActive(false);

        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(false);
        charaSelectPanel.SetActive(false);
    }

    /// <summary>
    /// ��� ������Ϸ�����������˵�����ʾ ����Ϸ + ��ѡ�����
    /// </summary>
    private void OnNewGame()
    {
        mainMenuPanel.SetActive(false);
        newGamePanel.SetActive(true);

        mainSelectPanel.SetActive(true);
        weaponSelectPanel.SetActive(false);
        charaSelectPanel.SetActive(false);

        // ˢ����ѡ�������� ��ɫ & ���� Ԥ��
        UpdateMainSelectCharacterPreview();
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// ��� �������������˴�ʾ��Ϊ��־�����貹��浵�߼�
    /// </summary>
    private void OnContinue()
    {
        GameManager.Instance.LoadGame();
        SceneManager.LoadScene("Gameplay");
    }

    /// <summary>
    /// ��� �����á�����ʾ��Ϊ��־�����赯�����ý���
    /// </summary>
    private void OnSettings()
    {
        Debug.Log("Settings: ��δʵ���������");
    }

    /// <summary>
    /// ��� ���˳��������༭��ģʽֹͣ��������˳�
    /// </summary>
    private void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// ��� ����Ϸ������ ���������˵���
    /// </summary>
    private void OnBackToMainMenu()
    {
        ShowMainMenu();
    }

    /// <summary>
    /// ��� ��ѡ����� �� ��ѡ���ɫ��
    /// </summary>
    private void OnEnterCharacterSelect()
    {
        mainSelectPanel.SetActive(false);
        charaSelectPanel.SetActive(true);

        // �����ɫѡ��ʱˢ����ʾ
        UpdateCharacterSelectUI();
    }

    /// <summary>
    /// ��� ��ѡ����� �� ��ѡ��������
    /// </summary>
    private void OnEnterWeaponSelect()
    {
        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(true);

        // ��������ѡ��ʱˢ����ʾ
        UpdateWeaponSelectUI();
    }

    /// <summary>
    /// ��� ����ѡ����� �� ��ȷ�ϡ� ��ť
    /// </summary>
    private void OnConfirmWeaponSelect()
    {
        if (weaponList.Count > 0)
        {
            selectedWeapon = weaponList[currentWeaponIndex];
        }
        GameManager.Instance.SelectedWeapon = selectedWeapon;
        weaponSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // ������ѡ������������Ԥ��
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// ��� ��ɫѡ����� �� ��ȷ�ϡ� ��ť
    /// </summary>
    private void OnConfirmCharacterSelect()
    {
        if (playerList.Count > 0)
        {
            selectedPlayer = playerList[currentPlayerIndex];
        }
        GameManager.Instance.SelectedPlayer = selectedPlayer;
        charaSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // ������ѡ�������Ľ�ɫԤ��
        UpdateMainSelectCharacterPreview();
    }

    /// <summary>
    /// ��� ��ѡ����� �� ����ʼ��Ϸ��
    /// </summary>
    private void OnStartGame()
    {
        if (selectedPlayer == null)
        {
            Debug.LogWarning("����ѡ��һ����ɫ��");
            return;
        }
        if (selectedWeapon == null)
        {
            Debug.LogWarning("����ѡ��һ��������");
            return;
        }
        Debug.Log($"��ʼ��Ϸ����ɫ��{selectedPlayer.playerName}��������{selectedWeapon.weaponName}");
         SceneManager.LoadScene("Gameplay"); // ������Ŀ��Ϊʵ�ʳ�����
    }

    // ============================
    // === WeaponData ��������Ⱦ ===
    // ============================

    /// <summary>
    /// �� Resources/Data/Weapons �м������� WeaponData
    /// </summary>
    private void LoadAllWeapons()
    {
        weaponList.Clear();
        WeaponData[] loaded = Resources.LoadAll<WeaponData>("Data/Weapons");
        if (loaded != null && loaded.Length > 0)
        {
            weaponList.AddRange(loaded);
            currentWeaponIndex = 0;
            selectedWeapon = weaponList[0];
        }
        else
        {
            Debug.LogWarning("δ�� Resources/Data/Weapons ���ҵ��κ� WeaponData��");
        }
    }

    /// <summary>
    /// �л���ǰ��ȷ��������������ˢ��ѡ����� UI
    /// </summary>
    /// <param name="delta">-1 ��ʾ��һ�ѣ�+1 ��ʾ��һ��</param>
    private void ChangeWeaponSelection(int delta)
    {
        if (weaponList.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex + delta + weaponList.Count) % weaponList.Count;
        UpdateWeaponSelectUI();
    }

    /// <summary>
    /// ˢ�� ����ѡ����� �ϵ�Ԥ��ͼ�����ơ�����
    /// </summary>
    private void UpdateWeaponSelectUI()
    {
        if (weaponList.Count == 0) return;
        RenderWeaponPreview(
            weaponList[currentWeaponIndex],
            weaponPreviewImage,
            weaponNameText,
            weaponStatsText
        );
    }

    /// <summary>
    /// ˢ�� ��ѡ����� �� ��ȷ�� ���� ��Ԥ�� (ͼ��+����)
    /// </summary>
    private void UpdateMainSelectWeaponPreview()
    {
        if (selectedWeapon == null) return;
        RenderWeaponPreview(
            selectedWeapon,
            mainSelectWeaponImage,
            mainSelectWeaponName,
            null    // ��ѡ����� ������ʾ�����ı�
        );
    }

    /// <summary>
    /// ���÷������� WeaponData ��Ⱦ��ָ���� Image+Text �����
    ///   ����˳����ҳ���� UI ����˳��һ�£�Ԥ��ͼ �� ���� �� ����
    /// </summary>
    private void RenderWeaponPreview(
        WeaponData weaponData,
        Image previewImage,
        TextMeshProUGUI nameText,
        TextMeshProUGUI statsText
    )
    {
        if (weaponData == null) return;

        // 1. Ԥ��ͼ������ʹ�� idleSprite�������� icon
        if (previewImage != null)
            previewImage.sprite = weaponData.idleSprite != null ? weaponData.idleSprite : weaponData.icon;

        // 2. ����
        if (nameText != null)
            nameText.text = weaponData.weaponName;

        // 3. ���ԣ���ѡ��
        if (statsText != null)
        {
            statsText.text =
                $"�˺�: {weaponData.damage}\n" +
                $"����: {weaponData.attackInterval:F2}��\n" +
                $"���: {weaponData.attackRange:F1}";
        }
    }

    // ===============================
    // === PlayerStatsData ��������Ⱦ ===
    // ===============================

    /// <summary>
    /// �� Resources/Data/Players �м������� PlayerStatsData
    /// </summary>
    private void LoadAllPlayers()
    {
        playerList.Clear();
        PlayerStatsData[] loaded = Resources.LoadAll<PlayerStatsData>("Data/Players");
        if (loaded != null && loaded.Length > 0)
        {
            playerList.AddRange(loaded);
            currentPlayerIndex = 0;
            selectedPlayer = playerList[0];
        }
        else
        {
            Debug.LogWarning("δ�� Resources/Data/Players ���ҵ��κ� PlayerStatsData��");
        }
    }

    /// <summary>
    /// �л���ǰ��ȷ�Ͻ�ɫ��������ˢ�½�ɫѡ����� UI
    /// </summary>
    /// <param name="delta">-1 ��ʾ��һλ��+1 ��ʾ��һλ</param>
    private void ChangePlayerSelection(int delta)
    {
        if (playerList.Count == 0) return;
        currentPlayerIndex = (currentPlayerIndex + delta + playerList.Count) % playerList.Count;
        UpdateCharacterSelectUI();
    }

    /// <summary>
    /// ˢ�� ��ɫѡ����� �ϵ�Ԥ��ͼ�����ơ�����
    /// </summary>
    private void UpdateCharacterSelectUI()
    {
        if (playerList.Count == 0) return;
        RenderCharacterPreview(
            playerList[currentPlayerIndex],
            charaPreviewImage,
            charaNameText,
            charaStatsText
        );
    }

    /// <summary>
    /// ˢ�� ��ѡ����� �� ��ȷ�� ��ɫ ��Ԥ�� (ͼ��+����)
    /// </summary>
    private void UpdateMainSelectCharacterPreview()
    {
        if (selectedPlayer == null) return;
        RenderCharacterPreview(
            selectedPlayer,
            mainSelectCharacterImage,
            mainSelectCharacterName,
            null    // ��ѡ����� ����ʾͼ��+����
        );
    }

    /// <summary>
    /// ���÷������� PlayerStatsData ��Ⱦ��ָ���� Image+Text �����
    ///   ����˳����ҳ���� UI ����˳��һ�£�Ԥ��ͼ �� ���� �� ����
    /// </summary>
    private void RenderCharacterPreview(
        PlayerStatsData playerData,
        Image previewImage,
        TextMeshProUGUI nameText,
        TextMeshProUGUI statsText
    )
    {
        if (playerData == null) return;

        // 1. Ԥ��ͼ������ʹ�� idleSprite�������� icon
        if (previewImage != null)
            previewImage.sprite = playerData.idleSprite != null ? playerData.idleSprite : playerData.icon;

        // 2. ����
        if (nameText != null)
            nameText.text = playerData.playerName;

        // 3. ���ԣ���ѡ��
        if (statsText != null)
        {
            statsText.text =
                $"HP: {playerData.baseMaxHp}\n" +
                $"����: {playerData.baseAttack}\n" +
                $"�ƶ��ٶ�: {playerData.baseMoveSpeed:F1}";
        }
    }
}
