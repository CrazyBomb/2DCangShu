using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleTrigger : MonoBehaviour
{
    RectTransform myRectTrans;
    RectTransform newImgTrans;
    RectTransform imgTrans;
    bool stop;
    Vector2 oldSizeDelta;

    void Awake()
    {
        myRectTrans = GetComponent<RectTransform>();
        newImgTrans = transform.Find("NewImg") as RectTransform;
        imgTrans = transform.Find("image") as RectTransform;
        oldSizeDelta = newImgTrans.sizeDelta;
    }

    private void OnEnable()
    {
        newImgTrans.sizeDelta = oldSizeDelta;
        stop = false;
    }

    void Update()
    {
        if (gameObject.activeSelf && !stop)
            newImgTrans.sizeDelta = Vector3.Lerp(newImgTrans.sizeDelta, Vector3.zero, Mathf.Pow(Time.deltaTime, 2) * 100);
        TouchOnce();
    }

    void TouchOnce()
    {
        //if (Input.touchCount == 0)
        //    return;
        //if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    StartCoroutine(Wait());
        //}
        if (Input.GetMouseButtonUp(0) && gameObject.activeSelf)
            StartCoroutine(Wait());
        //如果没有点击。那么半径小于10时直接消失，并只显示最短时间
        if (newImgTrans.sizeDelta.x < 10)
            StartCoroutine(Wait_2());
    }

    IEnumerator Wait_2()
    {
        //TODO：Normal图片的显示
        stop = true;
        GameManager.Instance.ShowSeed += ReturnTime_2;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ShowSeed -= ReturnTime_2;
        GameManager.Instance.CanShow = true;
        gameObject.SetActive(false);
    }

    IEnumerator Wait()
    {
        //TODO：Normal , Perfect , Excellent 三种图片的显示
        stop = true;
        GameManager.Instance.ShowSeed += ReturnTime;
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ShowSeed -= ReturnTime;
        GameManager.Instance.CanShow = true;
        gameObject.SetActive(false);        
    }

    /// <summary>
    /// 都是圆形，即无需考虑Width or Height，任选其一都为半径
    /// </summary>
    float ReturnTime()
    {
        if ((newImgTrans.sizeDelta.x - myRectTrans.sizeDelta.x) > 10 || (imgTrans.sizeDelta.x - newImgTrans.sizeDelta.x) > 20)
            return 2f;
        if (myRectTrans.sizeDelta.x > newImgTrans.sizeDelta.x && (newImgTrans.sizeDelta.x - imgTrans.sizeDelta.x) > 20)
            return 5f;
        if (Mathf.Abs(newImgTrans.sizeDelta.x - imgTrans.sizeDelta.x) <= 20 && newImgTrans.sizeDelta.x > 10)
            return 7f;        
        return 2f;
    }

    float ReturnTime_2()
    {
        return 2f;
    }
}
