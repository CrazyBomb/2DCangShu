﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                         

public class DrawMask : MonoBehaviour
{
    private NPCControl nc;
    private Transform npcTrans;

    [Tooltip("半径")][Range(0,1f)]
    public float radius = 0.5f;
    [Tooltip("笔刷预制体")]
    public GameObject brush;
    [Header("笔刷父对象")]
    public Transform brushParent;

    bool twoPoints = false;
    Vector2 lastPos;//最后一个点
    Vector2 penultPos;//倒数第二个点
    List<GameObject> brushesPool = new List<GameObject>(),activeBrushes = new List<GameObject>();//笔刷对象池

    private void Start()
    {
        nc = GameObject.Find("Canvas/SeedNum").GetComponent<NPCControl>();
        npcTrans = GameObject.Find("NPC").transform;
    }

    void Update()
    {
        GetInput();
    }
   
    void GetInput()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(npcTrans.position);
        if (nc.StartDraw)
        {
            penultPos = screenPos;
            nc.StartDraw = false;
        }
        //拖拽中
        else if (nc.NpcMove)
        {
            if (twoPoints && Vector2.Distance(screenPos, lastPos) > 0.5f)//如果两次记录的鼠标坐标距离大于一定的距离，开始记录鼠标的点
            {
                Vector2 pos = screenPos;
                float dis = Vector2.Distance(lastPos, pos);
                int segments = (int)(dis / radius);//计算出平滑的段数
                segments = segments < 1 ? 1 : segments;
                Vector2[] points = Beizier(penultPos, lastPos, pos, segments);//进行贝塞尔平滑
                //生成预制体
                for (int i = 0; i < points.Length; i++)
                {
                    InstanceBrush(VectorTransfer(points[i]));
                }
                lastPos = pos;
                penultPos = points[points.Length - 2];
            }
            else
            {
                twoPoints = true;
                lastPos = screenPos;
            }
        }
        else if (!nc.NpcMove)
        {
            twoPoints = false;
        }
    }

    //该方法体中的方法在每一帧渲染之后执行,只能挂载在摄像机上才能被调用
    private void OnPostRender()
    {
        InitBrushes();
    }

    void InitBrushes()
    {
        for (int i = 0; i < activeBrushes.Count; i++)
        {
            activeBrushes[i].SetActive(false);
            brushesPool.Add(activeBrushes[i]);
        }
        activeBrushes.Clear();
    }

    void InstanceBrush(Vector2 pos)
    {
        GameObject brushClone;
        if (brushesPool.Count > 0)
        {
            brushClone = brushesPool[brushesPool.Count - 1];
            brushesPool.RemoveAt(brushesPool.Count - 1);
        }
        else
        {
            brushClone = Instantiate(brush, pos, Quaternion.identity);
        }
        brushClone.transform.SetParent(brushParent);
        brushClone.transform.position = pos;

        brushClone.transform.localScale = Vector3.one * radius;
        brushClone.SetActive(true);
        activeBrushes.Add(brushClone);
    }

    /// <summary>
    /// 贝塞尔平滑
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="mid">中点</param>
    /// <param name="end">终点</param>
    /// <param name="segments">段数</param>
    /// <returns></returns>
    public Vector2[] Beizier(Vector2 start,Vector2 mid, Vector2 end,int segments)
    {
        float d = 1f / segments;
        Vector2[] points = new Vector2[segments - 1];
        for (int i = 0; i < points.Length; i++)
        {
            float t = d * (i + 1);
            points[i] = (1 - t) * (1 - t) * mid + 2 * t * (1 - t) * start + t * t * end;
        }
        List<Vector2> rps = new List<Vector2>();
        rps.Add(mid);
        rps.AddRange(points);
        rps.Add(end);
        return rps.ToArray(); 
    }

    Vector2 VectorTransfer(Vector2 point)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(point.x, point.y, 0));
    }
}
