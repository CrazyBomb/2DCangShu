using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : MonoBehaviour
{
    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGame()
    {
        AudioManager.Instance.GetResources();
        SceneTransition.Instance.TransitionNextScene("Main", 1f, 1f);       
    }
}
