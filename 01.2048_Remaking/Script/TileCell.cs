using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    //使用Vector2Int类型来表示二维空间中的位置。
    public Tile tile { get; set; }
    //定义一个Tile类型的属性tile，用于存储单元格中的Tile对象。

    //使用布尔值来表示单元格的状态，即是否为空或被占用。
    //如果tile为null，则表示单元格为空。
    //如果tile不为null，则表示单元格被占用。
    public bool empty => tile == null;
    public bool occupied => tile != null;
}
    