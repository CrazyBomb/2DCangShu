using Random = System.Random;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
            {
                instance = GameObject.FindObjectOfType<GameManager>();
                if (instance != null)
                    return instance;
                else
                {
                    GameObject newGO = new GameObject("GameManager");
                    instance = newGO.AddComponent<GameManager>();
                    return instance;
                }
            }
        }
    }

    private Text seedNum;
    public Camera mycamera;
    public Text seedNumTxt;

    //能够显示进度框标志位
    private bool canShow;
    public bool CanShow { get => canShow; set => canShow = value; }
    //只能选择单一物品标志位
    private bool onlyOne;
    public bool OnlyOne { get => onlyOne; set => onlyOne = value; }

    private Transform NPC;
    private Transform Snake;
    public GameObject WaterPrefab;
    private Transform Portral;
    [HideInInspector]public Transform InitialHole;

    private float oldTime;
    private float newTime;

    public Func<float> ShowSeed { get; set; }   

    private void Awake()
    {
        mycamera.gameObject.SetActive(false);        
        seedNum = GameObject.Find("SeedNum/Num").GetComponent<Text>();
        NPC = GameObject.Find("NPC").transform;
        Snake = GameObject.Find("Snake").transform;
        Portral = GameObject.Find("Portal").transform;
        InitialHole = GameObject.Find("InitialHole").transform;
    }

    Vector3 oldPos;
    void Start()
    {
        CanShow = false;
        OnlyOne = false;

        oldTime = 0;
        newTime = 0;

        InitNPCPos();
        InitSnakePos();
        for (int i = 0; i < 5; i++)
        {                        
            Transform waterTrans = Instantiate(WaterPrefab).transform;
            InitWaterPos(waterTrans);
            if (i == 0)
                oldPos = waterTrans.position;
            else
            {
                while (Mathf.Abs(waterTrans.position.x - oldPos.x) < 0.5f || Mathf.Abs(Snake.position.y - oldPos.y) < 1f)
                {
                    InitWaterPos(waterTrans);
                }
                oldPos = waterTrans.position;
            } 
        }       

        //InitPortalPos();
        InitialHole.position = NPC.position;

        if (!PlayerPrefs.HasKey("seedNum"))
        {
            PlayerPrefs.SetInt("seedNum", 500); //后续改为0
            seedNum.text = "100";
        }
        else
            seedNum.text = PlayerPrefs.GetInt("seedNum") + "";  

        StartCoroutine(WaitNull());
    }

    /// <summary>
    /// 每隔120s增加一个果子
    /// </summary>
    private void Update()
    {
        newTime = Time.time;
        if (newTime - oldTime >= 120f)
        {
            PlayerPrefs.SetInt("seedNum", PlayerPrefs.GetInt("seedNum") + 1);
            seedNumTxt.text = PlayerPrefs.GetInt("seedNum") + "";
            oldTime = newTime;
        }
    }

    IEnumerator WaitNull()
    {
        yield return new WaitForSeconds(0.02f);
        mycamera.gameObject.SetActive(true);
    }
     
    /// <summary>
    /// WorldSpace X Range： -2.61 ~ 2.61
    /// WorldSpace Y Range:  -1.45 ~ 3.95
    /// 此中方法可能造成死循环，后续变更全新随机
    /// </summary>
    #region 重随蛇、仓鼠、 传送门、水的位置,保证四者不重叠
    void InitNPCPos()
    {
        do
        {
            Random random = new Random(GetRandomSeed());
            float x = random.Next((int)-2.61f * 100, (int)2.61f * 100) / 100f;
            float y = random.Next((int)-1.45f * 100, (int)3.95f * 100) / 100f;
            NPC.position = new Vector3(x, y, -5);
        }
        while (Mathf.Abs(Portral.position.x - NPC.position.x) < 0.5f || Mathf.Abs(Portral.position.y - NPC.position.y) < 1f);
    }
    void InitSnakePos()
    {
        do
        {
            Random random = new Random(GetRandomSeed());
            float x = random.Next((int)-2.61f * 100, (int)2.61f * 100) / 100f;
            float y = random.Next((int)-1.45f * 100, (int)3.95f * 100) / 100f;
            Snake.position = new Vector3(x, y, -5);
        } while (Mathf.Abs(Snake.position.x - NPC.position.x) < 0.5f || Mathf.Abs(Snake.position.y - NPC.position.y) < 1f);
    }
    void InitWaterPos(Transform Water)
    {
        do
        {
            Random random = new Random(GetRandomSeed());
            float x = random.Next((int)-2.61f * 100, (int)2.61f * 100) / 100f;
            float y = random.Next((int)-1.45f * 100, (int)3.95f * 100) / 100f;
            Water.position = new Vector3(x, y, -5);
        } while (Mathf.Abs(Water.position.x - NPC.position.x) < 0.5f || Mathf.Abs(Water.position.y - NPC.position.y) < 1f
        || Mathf.Abs(Water.position.x - Snake.position.x) < 0.5f || Mathf.Abs(Water.position.y - Snake.position.y) < 1f);
    }
    //void InitPortalPos()
    //{
    //    do
    //    {
    //        Random random = new Random(GetRandomSeed());
    //        float x = random.Next((int)-2.61f * 100, (int)2.61f * 100) / 100f;
    //        float y = random.Next((int)-1.45f * 100, (int)3.95f * 100) / 100f;
    //        Portral.position = new Vector3(x, y, -5);
    //    } while (Mathf.Abs(Portral.position.x - NPC.position.x) < 0.5f || Mathf.Abs(Portral.position.y - NPC.position.y) < 1f
    //    || Mathf.Abs(Portral.position.x - Snake.position.x) < 0.5f || Mathf.Abs(Portral.position.y - Snake.position.y) < 1f
    //    || Mathf.Abs(Portral.position.x - Water.position.x) < 0.5f || Mathf.Abs(Portral.position.y - Water.position.y) < 1f);
    //}   
    int GetRandomSeed()
    {
        return (int)DateTime.Now.Ticks;
    }
    #endregion
}
