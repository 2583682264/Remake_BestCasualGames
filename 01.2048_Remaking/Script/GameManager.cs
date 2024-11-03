using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScore;

    private int score = 0;

    private void Start()
    {
        NewGame();
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void NewGame()
    {
        SetScore(0);
        highScore.text = LoadHighScore().ToString();

        gameOver.alpha = 0;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1, 1f));
    }   

    /// <summary>
    /// 游戏结束动画，利用线性插值（lerping）函数逐渐改变CanvasGroup的alpha值，实现淡入淡出效果
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="to"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        // 在开始执行淡入淡出效果之前等待特定时间
        yield return new WaitForSeconds(delay);

        // 初始化变量
        float elapsed = 0f;          // 记录已经过去的时间
        float duration = 0.5f;       // 动画的总持续时间，为0.5秒
        float from = canvasGroup.alpha; // 动画开始时CanvasGroup的初始alpha值

        // 使用while循环逐渐改变alpha值
        while (elapsed < duration)
        {
            // 使用线性插值（lerping）函数计算当前的alpha值
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);

            // 增加已经过去的时间（这里会根据帧率逐帧增加）
            elapsed += Time.deltaTime;

            // 暂停协程的执行，等待下一帧继续
            yield return null;
        }

        // 确保在循环结束后，alpha值准确设定为目标值
        canvasGroup.alpha = to;
    }


    /// <summary>
    /// 通过传入分数参数来增加分数
    /// </summary>
    /// <param name="points"></param>
    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }


    /// <summary>
    /// 通过传入分数参数来设置显示分数，并保存最高分
    /// </summary>
    /// <param name="score"></param>
    private void SetScore(int score)
    {
        this.score = score;

        scoreText.text = score.ToString();

        SaveHighScore();
    }


    /// <summary>
    /// 保存最高分
    /// </summary>
    private void SaveHighScore()
    {
        int highScore = LoadHighScore();

        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
        }
    }


    /// <summary>
    /// 加载最高分
    /// </summary>
    /// <returns></returns>
    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("highScore", 0);
    }

}
