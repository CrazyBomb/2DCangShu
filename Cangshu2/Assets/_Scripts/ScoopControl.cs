using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScoopControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool canDrag;
    private Text seedNumTxt;
    public GameObject scoopPrefab;
    GameObject scoop;
    Vector3 offsetPos;

    private void Awake()
    {
        seedNumTxt = GameObject.Find("SeedNum/Num").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => InstantiateScoop());
        offsetPos = new Vector2(20, 20);
    }

    void InstantiateScoop()
    {
        if (!scoop)
        {
            scoop = Instantiate(scoopPrefab);
            scoop.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos);
            scoop.name = "Scoop";
        }
        else
        {
            scoop.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos);
            scoop.SetActive(true);
        }           
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && canDrag)
        {
            if (!scoop)
            {
                scoop = Instantiate(scoopPrefab);
                scoop.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos);
                scoop.name = "Scoop";
            }
            else
            {
                float x = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos).x;
                float y = Camera.main.ScreenToWorldPoint(Input.mousePosition + offsetPos).y;
                scoop.transform.position = new Vector3(x, y, -5);
            }
        }

    }
   
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //如果没实例化种子，那么可以进行拖拽
        if (!scoop && !GameObject.Find("Seed"))
        {
            canDrag = true;            
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        int count = PlayerPrefs.GetInt("seedNum") - 3;
        if (count <= 0)
        {
            count = 0;
            return;
        }
        else
        {
            canDrag = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                StartCoroutine(Wait(hitInfo.collider.gameObject));
                PlayerPrefs.SetInt("seedNum", count);
                seedNumTxt.text = PlayerPrefs.GetInt("seedNum") + "";
            }
            else
                Destroy(scoop);
        }
    }

    IEnumerator Wait(GameObject go)
    {
        //TODO：显示挖掘动画
        yield return new WaitForSeconds(1f);
        go.SetActive(false);
        scoop.SetActive(false);
    }
}
