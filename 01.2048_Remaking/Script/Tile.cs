using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }
    public bool locked { get; set; }
    //这个布尔属性locked，用于表示 Tile 对象是否被锁定，防止在游戏过程中被连续合并

    private Image background;
    private TextMeshProUGUI text;


    private void Awake()
    { 
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    #region 定义函数

    /// <summary>
    /// 目的是为了设置或者转换 Tile 对象的背景颜色、文本颜色和显示的数字
    /// </summary>
    /// <param name="state"></param>
    /// <param name="number"></param>
    public void SetState(TileState state, int number)
    //接受了两个参数，TileState 包含了单元格的背景颜色和文本颜色，而 number 则是显示在单元格上的数字
    {
        this.state = state;
        this.number = number;
        //将传入的参数赋值给Tile对象(也就是this）的state和number属性

        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
        //注意这个ToString()方法，要将数字转换为字符串，以便在UI上显示
    }


    public void Spawn(TileCell cell)
    //Spawn 方法的主要作用是处理 Tile 对象与 TileCell 对象之间的关联和位置同步。它解除旧的关联，建立新的关联，并将 Tile 的位置更新为与新的 TileCell 相同
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        //检查当前 Tile 是否已经关联到了某个 TileCell;如果已存在关联（即 cell 不为 null）,解除该关联
        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }


    /// <summary>
    /// 将Tile对象移动到指定的TileCell对象上
    /// </summary>
    /// <param name="cell"></param>
    public void MoveTo(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        //transform.position = cell.transform.position;这段仅用于调试，实际游戏中需要动画效果，但这也可以实现移动逻辑

        StartCoroutine(Animate(cell.transform.position));
    //这个移动的逻辑跟Spawn方法一模一样，但是会多一个动画效果，原理都是根据cell的位置来定位
    }


    /// <summary>
    /// 平滑动画移动，用于MoveTo函数和Merge函数调用
    /// </summary>
    /// <param name="to"></param>
    /// <returns></returns>
    private IEnumerator Animate(Vector3 to, bool merging = false)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;
        //记录 Tile 的起始位置

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
            //yield return null 让协程等待到下一帧再继续执行
            //使动画能够分布在多个帧上，创造平滑的视觉效果，不然动画会在一帧内完成
        }

        transform.position = to;
    //通过在短时间内（0.1秒内）多次小幅度移动 Tile，创造出平滑移动的视觉效果
    //使用 Vector3.Lerp 确保移动速度先快后慢，给人一种自然的感觉

        if (merging)
        {
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// 用于 Tile 对象的合并操作并且会触发动画效果
    /// </summary>
    /// <param name="cell"></param>
    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;
        //将 Tile 对象的 cell 属性设置为 null
        cell.tile.locked = true;
        //并将目标 TileCell 对象的 tile 属性设置为 locked，以防止它被再次合并

        StartCoroutine(Animate(cell.transform.position, true));
    }

    #endregion
}
