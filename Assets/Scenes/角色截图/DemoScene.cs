using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DemoScene : MonoBehaviour
{
       [Header("截图配置")]
       public Camera captureCamera; // 拖拽上面创建的CaptureCamera
       public Transform targetModel; // 拖拽需要截图的3D模型（可选，用于自动对准）
       public int screenshotWidth = 1024; // 截图宽度（建议2的倍数，如512/1024）
       public int screenshotHeight = 1024; // 截图高度
       public KeyCode captureKey = KeyCode.F12; // 触发截图的按键
       private string savePath = "Assets/Resources/UI/Role/"; // 保存路径
       private Animator animator;
       private bool isPaused = false;
       private RenderTexture _tempRT; // 临时渲染纹理

       private void Awake()
       {
            targetModel = GameObject.FindGameObjectWithTag("Role").transform;
            animator = targetModel.gameObject.GetComponent<Animator>();
       }

       void Start()
       {
           // 1. 初始化保存目录
           if (!Directory.Exists(savePath))
           {
               Directory.CreateDirectory(savePath);
           }
           
           // 3. 初始化渲染纹理（保留Alpha通道）
           _tempRT = new RenderTexture(screenshotWidth, screenshotHeight, 32, RenderTextureFormat.ARGB32);
           _tempRT.antiAliasing = 4; // 4倍抗锯齿，让模型边缘更平滑
           _tempRT.Create();
       }
   
       void Update()
       {
           // 按下按键触发截图
           if (Input.GetKeyDown(KeyCode.Space) && captureCamera != null)
           {
               CaptureTransparentModel();
           }

           if (!isPaused)
           {
               CheckAndPauseAtSecondFrame();
           }
       }
       private void CheckAndPauseAtSecondFrame()
       {
           AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
           
           AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);
           if (clipInfos.Length > 0)
           {
               AnimationClip idleClip = clipInfos[0].clip;
               float frameRate = idleClip.frameRate; // 动画帧率（如30帧/秒）
               float totalFrames = idleClip.length * frameRate; // 动画总帧数
               float currentFrame = stateInfo.normalizedTime * totalFrames; // 当前播放到的帧
               if (currentFrame >= 1f)
               {
                   isPaused = true;
                   animator.speed = 0;
               }
           }
       }
       /// <summary>
       /// 核心：捕获透明背景的3D模型截图
       /// </summary>
       private void CaptureTransparentModel()
       {
           // 1. 保存摄像机原有目标纹理（避免影响正常渲染）
           RenderTexture oldTarget = captureCamera.targetTexture;
           // 2. 让摄像机渲染到临时透明纹理
           captureCamera.targetTexture = _tempRT;
           captureCamera.Render(); // 强制渲染一次（关键：否则截图为黑）
   
           // 3. 读取渲染纹理中的像素（保留Alpha通道）
           RenderTexture.active = _tempRT;
           Texture2D screenshotTex = new Texture2D(screenshotWidth, screenshotHeight, TextureFormat.RGBA32, false);
           // 读取像素：从(0,0)开始，覆盖整个纹理
           screenshotTex.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
           screenshotTex.Apply(); // 应用像素数据
   
           // 4. 还原摄像机设置（必须！否则摄像机画面会消失）
           captureCamera.targetTexture = oldTarget;
           RenderTexture.active = null;
   
           // 5. 保存为PNG（只有PNG支持Alpha透明，JPG不行！）
           byte[] pngData = screenshotTex.EncodeToPNG();
           string fileName = $"{targetModel.gameObject.name}.png";
           string fullPath = Path.Combine(savePath, fileName);
           File.WriteAllBytes(fullPath, pngData);
   
           // 6. 释放内存（避免内存泄漏）
           Destroy(screenshotTex);
           SetImageToSprite(fullPath);
           Debug.Log($"透明背景截图完成！保存路径：{fullPath}");
       }
       private void SetImageToSprite(string imagePath)
       {
           // 刷新AssetDatabase，让Unity识别新生成的图片
           AssetDatabase.Refresh();

           // 转换为Unity的资源路径（确保路径格式正确）
           string assetPath = imagePath.Replace("\\", "/");

           // 获取图片的导入设置
           TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
           if (importer == null)
           {
               Debug.LogError($"无法获取图片导入设置：{assetPath}");
               return;
           }
           
           importer.textureType = TextureImporterType.Sprite;
           importer.spriteImportMode = SpriteImportMode.Single;
           importer.spritePixelsPerUnit = 100;
           importer.alphaIsTransparency = true;
           importer.wrapMode = TextureWrapMode.Clamp; // 防止拉伸时重复
           importer.filterMode = FilterMode.Bilinear; // 抗锯齿

           // 应用导入设置（必须调用，否则不生效）
           EditorUtility.SetDirty(importer);
           importer.SaveAndReimport();

           // 刷新资源库，使修改生效
           AssetDatabase.Refresh();
       }
   
       // 销毁时释放渲染纹理（重要）
       private void OnDestroy()
       {
           if (_tempRT != null)
           {
               _tempRT.Release();
               Destroy(_tempRT);
           }
       }
       
}
