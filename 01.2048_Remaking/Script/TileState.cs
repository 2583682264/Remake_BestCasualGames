using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]
public class TileState : ScriptableObject
{
    public Color backgroundColor;
    public Color textColor;
}

//允许在编辑器中通过脚本或界面较为轻松地配置单元格的样式，比如背景色和文本色。