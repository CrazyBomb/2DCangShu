using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCControl : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private bool canDrag;   
    //开始画轨迹,记录倒数第二个点
    private bool startDraw;
    public bool StartDraw { get => startDraw; set => startDraw = value; }   
    //检测角色是否移动中，控制Brush的生成
    private bool npcMove;
    public bool NpcMove { get => npcMove; set => npcMove = value; }
   
    public GameObject SeedPrefab;
    public GameObject OldImg;
    public Transform NPCTrans;
    private GameObject Seed;
    private Text seedNumTxt;

    private Vector3 dir = Vector3.zero;
    private Vector3 offsetPos;
    [HideInInspector]public float time;
    private NPCMove nm;
   
    void Start()
    {
        canDrag = false;        
        startDraw = false;
        npcMove = false;
        offsetPos = new Vector2(20, 100);
        time = 0;
        seedNumTxt = transform.Find("Num").GetComponent<Text>();
        nm = NPCTrans.GetComponent<NPCMove>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && canDrag)
        {
            if (!Seed)
            {
                Seed = Instantiate(SeedPrefab);
                Seed.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos);
                Seed.name = "Seed";               
            }
            else
            {
                float x = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos).x;
                float y = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos).y;
                Seed.transform.position = new Vector3(x, y, -5);
            }
        }
        
        if (GameManager.Instance.ShowSeed != null)
        {
            time = GameManager.Instance.ShowSeed.Invoke();    //将显示时间固定     
        }
        ShowSeed(time);
    }

    //栗子显示在场景中
    void ShowSeed(float timeTemp)
    {
        if (GameManager.Instance.CanShow)
        {
            if (timeTemp > 0f)
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    time = 0f;
                }                   
                if (Seed && !canDrag)
                {
                    if (GameManager.Instance.InitialHole.gameObject.activeSelf)
                    {
                        GameManager.Instance.InitialHole.gameObject.SetActive(false);
                    }
                    NpcMove = true;
                    NPCTrans.GetComponent<Animator>().SetBool("Run", true);
                    dir = Seed.transform.position - NPCTrans.position;
                    if (dir.magnitude > 0.1f)
                        nm.NPCMoveAutomatic(dir.normalized);
                }
            }
            else
            {
                GameManager.Instance.CanShow = false;
                Destroy(Seed);
                NpcMove = false;
                NPCTrans.GetComponent<Animator>().SetBool("Run", false);
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //如果没实例化种子，那么可以进行拖拽
        if (!Seed)
        {
            canDrag = true;
            StartDraw = true;
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        int count = PlayerPrefs.GetInt("seedNum") - 1;
        if (count <= 0)
        {
            count = 0;
            return;
        }
        else
        {
            canDrag = false;
            OldImg.SetActive(true);
            PlayerPrefs.SetInt("seedNum", count);
            seedNumTxt.text = PlayerPrefs.GetInt("seedNum") + "";
        }
    }
}
