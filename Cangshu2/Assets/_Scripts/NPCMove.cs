using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCMove : MonoBehaviour
{
    private Transform NPCTrans;
    public Image BackButton;
    private float speed = 0.8f;
    private Vector2 newPos;

    float npcWidth;
    float npcHeight;

    //左边界
    private float x_Min;
    //右边界
    private float x_Max;
    //下边界
    private float y_Min;
    //上边界
    private float y_Max;

    private void Awake()
    {
        NPCTrans = this.transform;
    }

    /// <summary>
    /// 根据得到的实际宽度是一种限制范围的方法，
    /// 也可以：先得到UI的世界坐标，再将世界坐标转为屏幕坐标去求匹配的范围。但前提在同一渲染摄像机下，不同摄像机可能导致错误
    /// 因此：还是这种直接得到尺寸的比较保险
    /// </summary>
    private void Start()
    {
        //sprite的宽度，注意要乘上pixel Per Unit,最后乘上世界坐标下的缩放,最终得到在屏幕上的宽度大小
        npcWidth = NPCTrans.GetComponent<SpriteRenderer>().size.x * 100 * NPCTrans.lossyScale.x;
        x_Min = 0 + npcWidth;
        x_Max = Screen.width - npcWidth;
        //sprite的高度，原理同宽度
        npcHeight = NPCTrans.GetComponent<SpriteRenderer>().size.y * 100 * NPCTrans.lossyScale.y;
        //得到UI在屏幕中的实际高度，注意是本地坐标下的缩放
        y_Min = 0 + npcHeight;
        y_Max = Screen.height - npcHeight;
    }

    //角色移动模块：动画状态，方向，移动范围
    public void NPCMoveAutomatic(Vector2 dir)
    {
        //向右
        if (dir.x > 0)
        {
            if (NPCTrans.localScale.x > 0)
                NPCTrans.localScale = new Vector3(-1 * NPCTrans.localScale.x, NPCTrans.localScale.y);
        }
        //向左
        else
        {
            if (NPCTrans.localScale.x < 0)
                NPCTrans.localScale = new Vector3(-1 * NPCTrans.localScale.x, NPCTrans.localScale.y);
        }
        NPCTrans.Translate(dir * speed * Time.deltaTime);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(NPCTrans.position);
        newPos = new Vector2(Mathf.Clamp(screenPos.x, x_Min, x_Max), Mathf.Clamp(screenPos.y, y_Min, y_Max));
        Vector3 pos = Camera.main.ScreenToWorldPoint(newPos);
        NPCTrans.position = new Vector3(pos.x, pos.y, NPCTrans.position.z);

        AudioManager.Instance.CutAudio();
    }
}
