using System.Data;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows { get; private set; }
    public TileCell[] cells { get; private set; }
    //声明两个数组类型的变量，分别表示网格中的行和单元格

    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size / height;
    //分别获取组件的方格数，行数，列数

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
        //获取两个组件的所有子对象，前面创建数组也是为了这个目的
    }

    private void Start()
    {
        for (int y = 0; y < rows.Length; y++)
        {
            for (int x = 0; x < rows[y].cells.Length; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }
    //在 Start 阶段，遍历整个网格，设置每个单元格的坐标，确保网格中所有单元格都知道自己的位置，也就是给每个单元格定位

    #region 定义函数方法

    /// <summary>
    /// 在 TileGrid 中找到一个随机且未被占用的单元格，用于生成新数字块。如果所有单元格都已被占用，则返回 null，表示无法再生成新的块。
    /// </summary>
    /// <returns></returns>
    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, cells.Length);
        int startingindex = index;

        while (cells[index].occupied)
        {
            index++;

            if (index >= cells.Length)
            {
                index = 0;
            }

            // all cells are occupied
            if (index == startingindex)
            { return null; }
        }

        return cells[index];
    //方法首先随机选取一个单元格，并检查其是否被占用,如果被占用，则继续检查下一个单元格，循环遍历整个网格，如果整个网格都被占用，返回 null，如果找到未被占用的单元格，返回该单元格。
    }


    /// <summary>
    /// GetCell 方法的主要目的是根据给定的坐标 (x, y) 返回对应的 TileCell 对象
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TileCell GetCell(int x, int y) 
    //一定要注意，这个方法返回的是 TileCell 对象，而不是 Tile 对象，也不是TileCell对象的占用（occupied）属性
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return (rows[y].cells[x]);
            //返回了位于第 y 行、第 x 列的 TileCell 对象
        }
        else
        { return null; }
    }

    /// <summary>
    /// 重载了GetCell，允许传入Vector2Int参数，该参数表示单元格的坐标，然后调用之前的GetCell方法
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }


    /// <summary>
    /// 根据给定的 TileCell 对象和一个方向向量，返回该单元格在指定方向上的相邻单元格
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    #endregion

}
