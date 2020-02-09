using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 脚本中的方法都已经手动绑定场景
/// </summary>
public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// 分享好友
    /// </summary>
    public void ShareFriends()
    {
        //PlayerPrefs.SetInt("diamondNum", PlayerPrefs.GetInt("diamondNum"));
    }

    /// <summary>
    /// 排行榜，根据种子数量
    /// </summary>
    public void Rank()
    {
        //PlayerPrefs.GetInt("seedNum");
    }

    /// <summary>
    /// 返回按钮
    /// </summary>
    public void OnClickBackButton()
    {
        SceneTransition.Instance.TransitionNextScene("Start", 1f, 1f);
    }

    /// <summary>
    /// 退出按钮
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void ReloadGame()
    {
        int count = PlayerPrefs.GetInt("seedNum");
        PlayerPrefs.SetInt("seedNum", count - 5);
        SceneTransition.Instance.TransitionNextScene("Main", 1f, 1f);
    }

    /// <summary>
    /// 观看广告
    /// </summary>    
    public void SeeADS()
    {

    }

    /// <summary>
    /// 下一关
    /// </summary>
    public void NextGame()
    { 
        SceneTransition.Instance.TransitionNextScene("Main", 1f, 1f);
    }
}
