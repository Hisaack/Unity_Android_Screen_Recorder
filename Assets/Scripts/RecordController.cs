using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using System.IO;
#if UNITY_IOS
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif
public class RecordController : MonoBehaviour
{
#if UNITY_ANDROID
    private AndroidJavaObject mScreenRecorder;
#endif
    private const string videoName = "NH";
    private string VIDEO_URL = "";
    private string pathToGallery_URL;
    private bool isPlayingVideo = false;
    private bool isInFireworkFrame = false;
    private void Start()
    {
        VIDEO_URL = Application.persistentDataPath + "/" + videoName + ".mp4";
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            mScreenRecorder = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            int width = Screen.width > 720 ? 720 : Screen.width;
            int height = Screen.width > 720 ? (int)(Screen.height * 720f / Screen.width) : Screen.height;
            mScreenRecorder.Call("setupVideo", (int)(1f * width * height / 100 * 240 * 7), 60);
            pathToGallery_URL = "/../../../../DCIM/Videos";
        }
#endif
    }
    private void OnEnable()
    {
        StartCoroutine(PrepareRecord());
    }
    private void OnDisable()
    {
        ReleaseRecord();
    }
    public IEnumerator PrepareRecord()
    {
        yield return new WaitForSeconds(1f);
#if UNITY_ANDROID && ! UNITY_EDITOR
        mScreenRecorder.Call("setFileName", videoName);
        mScreenRecorder.Call("prepareRecord");
#endif
    }
    public void ReleaseRecord()
    {
#if UNITY_ANDROID && ! UNITY_EDITOR
        mScreenRecorder.Call("cleanUpRecord");
#endif
    }
    //Set up video name and start recording video
    public void OnClickStartRecording()
    {
#if UNITY_ANDROID && ! UNITY_EDITOR
        mScreenRecorder.Call("startRecording");
#endif
    }
    public void OnClickStopRecord()
    {
#if UNITY_ANDROID && ! UNITY_EDITOR
        mScreenRecorder.Call("stopRecording");
        SaveVideoAndroid();
		StartCoroutine(PrepareRecord());
#endif
    }
    public void SaveVideoAndroid()
    {
        DateTime now = DateTime.Now;
        string fileName = "Video_" + now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + ".mp4";
        if (!Directory.Exists(Application.persistentDataPath + pathToGallery_URL))
            Directory.CreateDirectory(Application.persistentDataPath + pathToGallery_URL);
        File.Copy(VIDEO_URL, Application.persistentDataPath + pathToGallery_URL + "/" + fileName);
    }
    private void OnGetRecordState(string message)
    {
    }
}
