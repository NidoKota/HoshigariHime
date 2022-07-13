using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// UniRxを1番初めに必ず起動させる
/// </summary>
public class MainThreadDispatcherInstantiate
{
    //ゲームが起動したら勝手に生成する
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Instantiate()
    {
        GameObject go = new GameObject("MainThreadDispatcher");
        go.AddComponent<MainThreadDispatcher>();
        Object.DontDestroyOnLoad(go);
    }
}
