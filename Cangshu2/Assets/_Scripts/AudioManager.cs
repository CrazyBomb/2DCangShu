using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get {
            if (!instance)
            {
                instance = FindObjectOfType<AudioManager>();
                if (!instance)
                {
                    GameObject obj = new GameObject("AudioManager");
                    obj.AddComponent<AudioSource>();
                    instance = obj.AddComponent<AudioManager>();
                    DontDestroyOnLoad(obj);
                }
            }                           
            return instance;
        }
    }

    AudioSource AS;
    private AudioClip cutAudio;
    private AudioClip overAudio;
    private AudioClip doneAudio;

    public void GetResources()
    {
        AS = GetComponent<AudioSource>();
        cutAudio = Resources.Load<AudioClip>("Audio/挖土");
        overAudio = Resources.Load<AudioClip>("Audio/游戏失败");
        doneAudio = Resources.Load<AudioClip>("Audio/游戏胜利");
    }
    /// <summary>
    /// 掘土音效
    /// </summary>
    public void CutAudio()
    {
        if (!AS.isPlaying)
            AS.PlayOneShot(cutAudio);      
    }
    /// <summary>
    /// 游戏结束音效
    /// </summary>
    public void GameOverAudio()
    {
        Camera.main.GetComponent<AudioSource>().Stop();
        AS.PlayOneShot(overAudio);
    }
    /// <summary>
    /// 游戏胜利音效
    /// </summary>
    public void GameDoneAudio()
    {
        Camera.main.GetComponent<AudioSource>().Stop();
        AS.PlayOneShot(doneAudio);
    }
}
