using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//单例模式的入口
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    //实例化控制
    public int point = 0;
    public static GameManager Instance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<GameManager>();
            if (_instance == null)
            {
                GameObject obj = new GameObject("GameManager");
                _instance = obj.AddComponent<GameManager>();//挂载
            }
        }
        return _instance;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 加分方法
    public void AddScore(int add)
    {
        point += add;
        Debug.Log($"当前分数：{point}");
    }

    // 退出时保存分数
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Point", point);
        PlayerPrefs.Save();
    }
}
