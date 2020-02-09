using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCColliderEvent : MonoBehaviour
{
    public NPCControl nc;

    [Tooltip("死亡弹窗")]public Transform Dead;
    [Tooltip("通关弹窗")]public Transform Pass;

    [Tooltip("被蛇咬伤图")] public Sprite biteImg;
    [Tooltip("被水淹图")] public Sprite downingImg;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            if (collision.collider.name == "SnakeBG" || collision.collider.name == "WaterBG")
            {
                GameObject parent = collision.transform.parent.gameObject;
                int index = parent.transform.childCount + 1;
                foreach (SpriteRenderer sr in parent.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    sr.sortingOrder = index;
                    index--;
                }

                if (collision.collider.name == "WaterBG")
                {
                    //碰到水停止，销毁松果，行进中途放传送门能够清除障碍物
                    StopMoveAndDestroy();
                }

                if (collision.collider.name == "SnakeBG")
                {
                    //碰到蛇死亡
                    StopMoveAndDestroy();
                    AudioManager.Instance.GameOverAudio();
                    StartCoroutine(TriggerSnake(collision.gameObject));
                }
            } else
                return;
        }
        else
            return; 
    }
    void StopMoveAndDestroy()
    {
        nc.NpcMove = false;
        nc.NPCTrans.GetComponent<Animator>().SetBool("Run", false);
        GameManager.Instance.CanShow = false;
        nc.time = 0;
        GameObject go = GameObject.Find("Seed");
        Destroy(go);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Seed")
        {
            StopMoveAndDestroy();
        }
        if (collision.name == "Portal")
        {
            StopMoveAndDestroy();
            PlayerPrefs.SetInt("seedNum", PlayerPrefs.GetInt("seedNum") + 5);
            GameManager.Instance.seedNumTxt.text = PlayerPrefs.GetInt("seedNum") + "";
            AudioManager.Instance.GameDoneAudio();
            Pass.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }

    IEnumerator TriggerSnake(GameObject snake)
    {
        Dead.Find("NPCImg").GetComponent<Animator>().SetBool("Bite", true);
        yield return new WaitForSeconds(1f);
        Dead.Find("DeadReason").GetComponent<Image>().sprite = biteImg;
        Dead.GetComponent<RectTransform>().localPosition = Vector3.zero;
    }
}
