using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/// <summary>
/// 该脚本不能被销毁
/// 进行黑屏渐显渐消，动态创建一个Canvas和一个Image
/// </summary>
public class SceneTransition : MonoBehaviour
{
    private Image fadeImg;
    private Canvas fadeCanvas;

    #region 实例新的对象并加载不摧毁
    private static SceneTransition instance;
    public static SceneTransition Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<SceneTransition>();
                if (!instance)
                {
                    GameObject ST = new GameObject("SceneTransition");
                    instance = ST.AddComponent<SceneTransition>();
                    DontDestroyOnLoad(ST);
                }
            }
            return instance;
        }
    }
    #endregion

    public void TransitionNextScene(string sceneName, float loadTime_Out, float loadTime_in)
    {
        GC.Collect();
        StartCoroutine(CreateImgAndCanvas(sceneName, loadTime_Out, loadTime_in));
    }

    IEnumerator CreateImgAndCanvas(string sceneName, float loadTime_Out, float loadTime_in)
    {
        DestroyImgAndCanvas();

        //新建Canvas
        GameObject fadeCanvasObj = new GameObject("FadeCanvas");
        fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
        fadeCanvas.transform.SetParent(this.transform);
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 20;
        //新建Image：尺寸、颜色、位置
        GameObject fadeImgObj = new GameObject("FadeImage");
        fadeImg = fadeImgObj.AddComponent<Image>();
        fadeImg.transform.SetParent(fadeCanvas.transform);
        fadeImg.color = Color.clear;
        fadeImg.rectTransform.anchoredPosition = Vector2.zero;
        fadeImg.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        //此处有问题
        yield return StartCoroutine(AlphaChange(loadTime_Out));

        if (sceneName != "")
        {
            SceneManager.LoadScene(sceneName);
            //等待加载完毕，内有循环
            yield return StartCoroutine(WaitLoadEnd(sceneName)); 
        }

        yield return StartCoroutine(AlphaChange(loadTime_in, false));

        //摧毁加载过渡对象
        DestroyImgAndCanvas();
    }

    void DestroyImgAndCanvas()
    {
        if (fadeImg != null)
            Destroy(fadeImg.gameObject);
        if (fadeCanvas != null)
            Destroy(fadeCanvas.gameObject);
    }

    /// <summary>
    /// 场景转黑淡入淡出，核心
    /// </summary>
    /// <param name="duration"> 经过时间 </param>
    /// <param name="isOut"> 是否是载入 </param>
    IEnumerator AlphaChange(float duration, bool isOut = true)
    {
        Color startColor = isOut ? Color.clear : Color.black;
        Color endColor = isOut ? Color.black : Color.clear;
        float tempTime = 0f;
        if (fadeImg != null)
        {
            while (tempTime < duration)
            {
                fadeImg.color = Color.Lerp(startColor, endColor, Mathf.Pow(tempTime / duration, 2));
                tempTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    /// <summary>
    /// 等待场景加载完毕
    /// </summary>
    IEnumerator WaitLoadEnd(string sceneName)
    {        
        while (SceneManager.GetActiveScene().name != sceneName && SceneManager.GetActiveScene().name != null)
        {
            yield return null;
        }
    }
}
