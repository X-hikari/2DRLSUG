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
    public GameObject newGamePanel;      // ����Ϸ��壨������ѡ�������ѡ������壩
    public Button backToMenuButton;      // ����Ϸ����� ���� ���˵� ��ť

    [Header("��ѡ����� (MainSelect)")]
    public GameObject mainSelectPanel;   // ������ѡ���ɫ������ѡ��������������ʼ��Ϸ�������������˵���
    public Button selectCharacterButton;
    public Button selectWeaponButton;
    public Button startGameButton;

    // ��ѡ�������չʾ��ѡ������ Image + Text
    public Image mainSelectWeaponImage;
    public TextMeshProUGUI mainSelectWeaponName;

    [Header("����ѡ����� (WeaponSelect)")]
    public GameObject weaponSelectPanel; // ���� ���� �л���ť �� ȷ�ϰ�ť
    public Button weaponLeftButton;
    public Button weaponRightButton;
    public Button weaponConfirmButton;

    // ����ѡ�������չʾ��������� UI
    public Image weaponPreviewImage;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;

    // ���д� Resources/Data/Weapons ���ص��� WeaponData
    private List<WeaponData> weaponList = new List<WeaponData>();
    private int currentWeaponIndex = 0;
    private WeaponData selectedWeapon;   // ȷ�Ϻ������

    void Start()
    {
        // ���� ���˵���ť�� ���� 
        newGameButton.onClick.AddListener(OnNewGame);
        continueButton.onClick.AddListener(OnContinue);
        settingsButton.onClick.AddListener(OnSettings);
        exitButton.onClick.AddListener(OnExit);

        // ���� ����Ϸ����� ��ť�� ���� 
        backToMenuButton.onClick.AddListener(OnBackToMainMenu);
        selectCharacterButton.onClick.AddListener(OnEnterCharacterSelect); // ���޽�ɫѡ��������
        selectWeaponButton.onClick.AddListener(OnEnterWeaponSelect);
        startGameButton.onClick.AddListener(OnStartGame);

        // ���� ����ѡ����� ��ť�� ���� 
        weaponLeftButton.onClick.AddListener(() => ChangeWeaponSelection(-1));
        weaponRightButton.onClick.AddListener(() => ChangeWeaponSelection(1));
        weaponConfirmButton.onClick.AddListener(OnConfirmWeaponSelect);

        // Ĭ��ֻ��ʾ���˵�
        ShowMainMenu();

        // ���� Resources/Data/Weapons �µ�������������
        LoadAllWeapons();
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
       // charaSelectPanel?.SetActive(false); // ���ʵ�ֽ�ɫѡ�񣬿ɱ���
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
       // charaSelectPanel?.SetActive(false);

        // ˢ����ѡ������������Ԥ���������� selectedWeapon��
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// ��� �������������˴�ʾ��Ϊ��־�����貹��浵�߼�
    /// </summary>
    private void OnContinue()
    {
        Debug.Log("Continue: ��δʵ�ִ浵����");
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
    /// ��� ��ѡ����� �� ��ѡ���ɫ������ʾ����ʵ�ֽ�ɫѡ�񣬽���ռλ��
    /// </summary>
    private void OnEnterCharacterSelect()
    {
        Debug.Log("Enter Character Select: ��δʵ��");
        // mainSelectPanel.SetActive(false);
        // charaSelectPanel.SetActive(true);
    }

    /// <summary>
    /// ��� ��ѡ����� �� ��ѡ��������
    /// </summary>
    private void OnEnterWeaponSelect()
    {
        mainSelectPanel.SetActive(false);
        weaponSelectPanel.SetActive(true);

        // ˢ������ѡ�������ʾ��ǰ��ȷ�ϵ�����
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
        weaponSelectPanel.SetActive(false);
        mainSelectPanel.SetActive(true);

        // ������ѡ������������Ԥ��
        UpdateMainSelectWeaponPreview();
    }

    /// <summary>
    /// ��� ��ѡ����� �� ����ʼ��Ϸ��
    /// </summary>
    private void OnStartGame()
    {
        if (selectedWeapon == null)
        {
            Debug.LogWarning("����ѡ��һ��������");
            return;
        }
        Debug.Log($"��ʼ��Ϸ��ʹ��������{selectedWeapon.weaponName}");
        // SceneManager.LoadScene("Gameplay"); // ������Ŀ��Ϊʵ�ʳ�����
    }

    // ����ʵ�ֽ�ɫѡ������ɲ��� OnConfirmCharacterSelect �ȣ�

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
    /// ��������ѡ������ϵ�Ԥ��ͼ�����ơ�����
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
    /// ������ѡ���������ȷ��������Ԥ��ͼ+����
    /// </summary>
    private void UpdateMainSelectWeaponPreview()
    {
        if (selectedWeapon == null) return;
        RenderWeaponPreview(
            selectedWeapon,
            mainSelectWeaponImage,
            mainSelectWeaponName,
            null    // ��ѡ��������ʾͼ��+���ƣ����������ı�
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
        {
            previewImage.sprite = (weaponData.idleSprite != null) ? weaponData.idleSprite : weaponData.icon;
        }

        // 2. ����
        if (nameText != null)
        {
            nameText.text = weaponData.weaponName;
        }

        // 3. ���ԣ���ѡ��
        if (statsText != null)
        {
            statsText.text =
                $"�˺�: {weaponData.damage}\n" +
                $"����: {weaponData.attackInterval:F2}��\n" +
                $"���: {weaponData.attackRange:F1}";
        }
    }
}
