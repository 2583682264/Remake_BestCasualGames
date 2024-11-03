using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRow : MonoBehaviour
{
    // 声明一个名为cells的公共数组，用于存储子对象TileCell的引用
    public TileCell[] cells { get; private set; }


    // 声明一个名为Awake的私有方法，用于在游戏开始时初始化
    private void Awake()
    {
        // 从子对象中获取TileCell类型的数组
        cells = GetComponentsInChildren<TileCell>();
    }
}
