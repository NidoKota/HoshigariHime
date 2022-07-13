using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// fps数を計測する
/// </summary>
public class FPSCounter : MonoBehaviour
{
    TextMeshPro fPSText;
    int frameCount;
    float prevTime;
    float fPS;
    float time;
    static ForwardDisplay forwardDisplay;

    //↓のコメントを外すとゲームを起動したら勝手にfps数を計り画面に表示する
    //[RuntimeInitializeOnLoadMethod]
    static void Ini()
    {
        GameObject fPSView = new GameObject("FPSView");
        TextMeshPro textMeshPro = fPSView.AddComponent<TextMeshPro>();
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.fontSize = 0.25f;
        fPSView.AddComponent<FPSCounter>();
        forwardDisplay = fPSView.AddComponent<ForwardDisplay>();
        forwardDisplay.cam = Camera.main.transform;
        forwardDisplay.position = new Vector3(0.385f, 0.55f, 1f);
        forwardDisplay.updatePosition = true;
        DontDestroyOnLoad(fPSView);
    }

    void Start()
    {
        fPSText = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if (!forwardDisplay.cam) forwardDisplay.cam = Camera.main.transform;

        ++frameCount;

        time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fPS = frameCount / time;
            fPSText.SetText("{0:0}fps", fPS);
            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
        if (fPS >= 59) fPSText.color = Color.white;
        else fPSText.color = Color.red;
    }
}
