using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Attack,
    Run,
    Catapult
}

class AudioObject
{
    public GameObject audioObj;
    public AudioSource audioSource;

    public AudioObject(GameObject audioObj, AudioSource audioSource)
    {
        this.audioObj = audioObj;
        this.audioSource = audioSource;
    }
}

public class AudioManager : Singleton<AudioManager>
{
    private Transform musicNode;
    private AudioSource bgAudioSource;
    private Dictionary<int, Dictionary<AudioType, AudioObject>> audioDict;
    private Dictionary<string, AudioClip> allSoundDict;

    private void Awake()
    {
        audioDict = new Dictionary<int, Dictionary<AudioType, AudioObject>>();
        allSoundDict = new Dictionary<string, AudioClip>();
    }

    void Start()
    {
        LoadAllAudioClip();
        if (musicNode == null)
        {
            GameObject musicObj = new GameObject("MusicNode");
            musicNode = musicObj.transform;
            DontDestroyOnLoad(musicObj);
        }

        if (bgAudioSource == null)
        {
            GameObject audioObj = new GameObject();
            audioObj.transform.parent = musicNode;
            bgAudioSource = audioObj.AddComponent<AudioSource>();
            audioObj.name = "BGMObj";
            bgAudioSource.loop = true;
        }

        PlayBGM(8);
    }

    private void LoadAllAudioClip()
    {
        AudioClip[] allSounds = Resources.LoadAll<AudioClip>("");
        foreach (var soundClip in allSounds)
        {
            if (soundClip != null && !string.IsNullOrEmpty(soundClip.name))
            {
                // 避免重复名称（如果有重名，后面的会覆盖前面的，建议资源名称唯一）
                if (!allSoundDict.ContainsKey(soundClip.name))
                {
                    allSoundDict.Add(soundClip.name, soundClip);
                }
                else
                {
                    Debug.Log($"发现重名音效：{soundClip.name}，已忽略重复项！");
                }
            }
        }
    }

    //播放背景音
    public void PlayBGM(int mapId)
    {
        // MapLayerInfo mapLayerInfo = Ui.Instance.GetMapLayerInfo(mapId);
        // AudioClip audioClip = mapLayerInfo.GetMusicClip();
        // bgAudioSource.clip = audioClip;
        // bgAudioSource.Play();
    }

    //调整背景音量
    public void SetBgAudioVolume(float volume)
    {
        bgAudioSource.volume = volume;
    }

    //播放音效
    public void PlayAudio(GameObject target, AudioType audioType, string name)
    {
        int gId = target.GetInstanceID();
        Dictionary<AudioType, AudioObject> audioObjDic;
        if (!audioDict.ContainsKey(gId))
        {
            audioDict.Add(gId, new Dictionary<AudioType, AudioObject>());
        }

        audioObjDic = audioDict[gId];
        AudioSource audioSource;
        if (audioObjDic.ContainsKey(audioType))
        {
            audioSource = audioObjDic[audioType].audioSource;
        }
        else
        {
            audioSource = CreateAudioSource(ref audioObjDic, audioType, name);
        }

        audioSource.clip = GetAudioClip(name);
        audioSource.Play();
        // if (!audioSource.isPlaying)
        // {
        // }
    }

    private AudioSource CreateAudioSource(ref Dictionary<AudioType, AudioObject> audioObjDic, AudioType audioType, string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.parent = musicNode;
        AudioSource audioSource = obj.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(name);
        AudioObject audioObj = new AudioObject(obj, audioSource);
        audioObjDic[audioType] = audioObj;
        return audioSource;
    }

    private AudioClip GetAudioClip(string name)
    {
        if (allSoundDict.ContainsKey(name))
        {
            return allSoundDict[name];
        }

        Debug.Log("没有 " + name + "音效");
        return null;
    }
}