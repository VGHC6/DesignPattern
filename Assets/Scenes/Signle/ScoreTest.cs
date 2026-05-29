using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//加分测试
public class ScoreTest : MonoBehaviour
{
    private void Start()
    {
        //读取分数
        GameManager.Instance().point= PlayerPrefs.GetInt("Point",0);
        Debug.Log($"上次保存的分数：{GameManager.Instance().point}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance().AddScore(1);
        }
    }
}
