using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
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
        // 初始化全局数据、UI、加载主场景等
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameOver()
    {
        // 游戏结束逻辑
    }
}
