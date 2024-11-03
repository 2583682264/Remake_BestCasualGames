using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public Tile tilePrefab;
    public TileState[] tileStates;
    //用于存储 Tile 对象的预制体和 Tile 状态数组

    public GameManager gameManager;
    private TileGrid grid;
    private List<Tile> tiles;

    private bool waiting;

    //以下属性用于生成 Tile，即CreateTile()函数的参数设置
    private List<TileCell> lastGeneratedCells = new List<TileCell>();
    private const int maxLastGeneratedCells = 3;
    private const float probabilityOfTwo = 0.9f;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
        //用于获取 TileGrid 组件，并创建一个空的 Tile 对象列表，这里的 16 是 TileGrid 中单元格的数量
    }


    private void Update()
    {
        if (!waiting)
        //目的是防止玩家在 Tile 移动动画完成之前再次按下移动键
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
        }
    }


    #region 定义函数方法

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.occupied)
                {
                changed |= MoveTile(cell.tile, direction);
                //MoveTile先被调用，如果 Tile 对象成功移动，则返回 true，否则返回 false
                //意思是但凡有一个子类对象移动了，changed 就会被设置为 true，就会调用后续的协程
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    //返回布尔值，表示 Tile 对象是否成功移动
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        //仅仅为了安全性的考虑，如果 adjacent 为 null，则直接跳出循环
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }
                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        //这个应在循环结束后执行，如果 newCell 不为 null，则表示找到了一个空单元格，可以移动 Tile 对象
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 用于随机生成 Tile 对象
    /// </summary>
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);

        // 使用随机数决定生成2还是4
        int value = UnityEngine.Random.value < probabilityOfTwo ? 2 : 4;
        tile.SetState(tileStates[value == 2 ? 0 : 1], value);

        // 获取随机空单元格
        TileCell emptyCell = GetRandomEmptyCellExcludingLast();

        // 如果没有空单元格，直接返回
        if (emptyCell == null)
        {
            Destroy(tile.gameObject);
            return;
        }

        // 生成tile
        tile.Spawn(emptyCell);

        // 更新最近生成的位置
        lastGeneratedCells.Add(emptyCell);
        if (lastGeneratedCells.Count > maxLastGeneratedCells)
        {
            lastGeneratedCells.RemoveAt(0);
        }

        tiles.Add(tile);
    }
    private TileCell GetRandomEmptyCellExcludingLast()
    {
        TileCell emptyCell;
        int attempts = 0;
        int maxAttempts = grid.size; // 假设 grid.size 是总的单元格数量

        do
        {
            emptyCell = grid.GetRandomEmptyCell();
            attempts++;

            // 如果尝试次数超过最大值，清空最近生成的单元格列表
            if (attempts > maxAttempts)
            {
                lastGeneratedCells.Clear();
                return emptyCell;
            }
        }
        while (emptyCell != null && lastGeneratedCells.Contains(emptyCell));

        return emptyCell;
    }
    ///// <summary>
    ///// 在 TileGrid 中随机的一个空单元格中创建一个新的 Tile 对象
    ///// </summary>
    //public void CreateTile()
    //{
    //    Tile tile = Instantiate(tilePrefab, grid.transform);
    //    tile.SetState(tileStates[0], 2);
    //    tile.Spawn(grid.GetRandomEmptyCell());
    //    //用于实例化一个 Tile 对象，并将其添加到 TileGrid 中随机的一个空单元格中, 并设置其状态为 tileStates 数组中的第一个元素(包含背景颜色和文本颜色), 以及数字2

    //    tiles.Add(tile);
    //    //目的是将新创建的 Tile 对象添加到 tiles 列表中，以便能够方便地管理、跟踪和操作这些 Tile 对象
    //}


    /// <summary>
    /// 判断两个 Tile 对象是否可以合并，并且阻止一下合并多个Tile对象的情况
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    private bool CanMerge(Tile tile1, Tile tile2)
    {
        return tile1.state == tile2.state && !tile2.locked;
        //两个 Tile 对象的状态相同且 tile2 对象没有被锁定时才可以合并,不然会出现一下子合并多个 Tile 对象的情况

        //return tile1.state == tile2.state && tile1.number < 2048 && !tile2.locked;
        //这段用于硬编码，即小于2048的 Tile 对象才可以合并
    }


    /// <summary>
    /// 合并两个 Tile 对象，并且要调用Tile对象的 Merge 方法
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    private void Merge(Tile tile1, Tile tile2)
    {
        tiles.Remove(tile1);
        tile1.Merge(tile2.cell);

        int index = Mathf.Clamp(IndexOf(tile2.state) + 1, 0, tileStates.Length - 1);
        int number = tile2.number * 2;

        tile2.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);//用于增加游戏得分
    }

    /// <summary>
    /// 根据 TileState 枚举类型返回对应的索引值，用于查询与设置 Tile 对象的状态
    /// </summary>
    /// <param name="state"></param>
    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (tileStates[i] == state)
            {
                return i;
            }
        }

        return -1;
        //返回-1，表示没有找到对应的 TileState 枚举类型，仅仅是为了防止出现未处理的异常；一般用不到
    }


    /// <summary>
    /// 这个协程用于等待一段时间，防止在 Tile 移动动画完成之前按下移动键导致斜向移动，同时用于判断游戏是否结束和是否需要创建新的 Tile
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        if(tiles.Count != grid.size)
        {
            CreateTile();
        }

        foreach (var tile in tiles)
        {
            tile.locked = false;
            //用于解锁 Tile 对象，使其可以合并
        }

        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
    }

    /// <summary>
    /// 检查游戏是否结束，如果所有单元格都被填满，并且没有两个相邻的单元格具有相同的状态，则游戏结束
    /// </summary>
    /// <returns></returns>
    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)){
                return false;
            }


            if (down != null && CanMerge(tile, down.tile)){
                return false;
            }


            if (left != null && CanMerge(tile, left.tile)){
                return false;
            }


            if (right != null && CanMerge(tile, right.tile)){
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// 公开，销毁所有 tiles 对象，并清空 tiles 列表 ，并且重置所有Cell对象
    /// </summary>
    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
        //清空 Tile 对象的集合
    }


    #endregion
}