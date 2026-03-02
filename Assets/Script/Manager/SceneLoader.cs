using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class SceneInfo
{
    public string sceneName;
    public GameObject sceneRoot;
    public Scene scene;
    public SceneBase sceneBase;

    public SceneInfo(string sceneName, GameObject sceneRoot,Scene scene,SceneBase sceneBase)
    {
        this.sceneName = sceneName;
        this.sceneRoot = sceneRoot;
        this.scene = scene;
        this.sceneBase = sceneBase;
    }
}

public class SceneLoader : Singleton<SceneLoader>
{
    private SceneInfo mainSceneInfo;
    private SceneInfo UISceneInfo;

    public void Awake()
    {
        LoadAsyncScene("MainScene", false);
        // GameObject sceneRoot = GameObject.Find("Root");
        // mainSceneInfo = new SceneInfo("MainScene",sceneRoot,SceneManager.GetSceneByName("MainScene"),null);
        // Debug.Log(mainSceneInfo.sceneRoot);
    }

    public void LoadAsyncScene(string sceneName,bool isMainScene,Action<SceneBase> callback = null)
    { 
        LoadSceneMode mode = isMainScene ? LoadSceneMode.Single : LoadSceneMode.Additive;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName,mode);
        if (asyncOperation == null)
        {
            Debug.LogError("场景为空: " + sceneName);
            return;
        }
        asyncOperation.completed += operation =>
        {
            // new SceneInfo()
            Scene scene = SceneManager.GetSceneByName(sceneName);
            GameObject root = GetGameObjectByName(scene, "Root");
            SceneBase sceneBase = GetSceneBase(root);
            if (!isMainScene)
            {
                UISceneInfo = new SceneInfo(sceneName, root, scene, sceneBase);
                ShowScene(UISceneInfo,false);
            }
            else
            {
                mainSceneInfo = new SceneInfo(sceneName, root, scene, sceneBase);
            }
            if (sceneBase != null)
            {
                sceneBase.onEnter();   
            }
            callback?.Invoke(sceneBase);
        };
    }

    public void RemoveScene(string sceneName)
    {
        if (UISceneInfo.sceneName == sceneName)
        {
            ShowScene(UISceneInfo,true);
            if (UISceneInfo.sceneBase != null)
            {
                UISceneInfo.sceneBase.onExit();
            }
            SceneManager.UnloadSceneAsync(UISceneInfo.sceneName);
            UISceneInfo = null;
        }
    }

    private GameObject GetGameObjectByName(Scene scene,string name)
    {
        foreach (GameObject obj in scene.GetRootGameObjects())
        {
            if (obj.name == name)
            {
                return obj;
            }
        }
        return null;
    }
    private SceneBase GetSceneBase(GameObject root)
    {
        SceneBase sceneBases = root.transform.GetComponentInChildren<SceneBase>(true);
        return sceneBases;
    }

    private void ShowScene(SceneInfo sceneInfo,bool hide)
    {
//        sceneInfo.sceneRoot.SetActive(hide);
    }
    // private IEnumerable Loader(string sceneName,bool isMainScene,params object[] data)
    // {
    //   
    //     yield return asyncOperation;
    //     
    // }
}
