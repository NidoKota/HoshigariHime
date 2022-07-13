using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScenarioController;
using UniRx;
using UnityEngine.Rendering;
using WipeCirclePPS;

/// <summary>
/// Scenarioが表示されるかテストする
/// </summary>
public class ScenarioTest : MonoBehaviour
{
    public Volume volume;
    public ScenarioInput input;
    
    void OnEnable()
    {
        WipeCircle wipeCircle;
        if (volume.profile.TryGet(out wipeCircle))
        {
            wipeCircle.radius.value = 0;
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(x => Fade.FadeOut(wipeCircle, 1.5f));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X)) input.PlayScenario();
        if (Input.GetKeyDown(KeyCode.Z)) input.scenarioDisplayBase.Next();
        if (Input.GetKeyDown(KeyCode.C)) input.scenarioDisplayBase.ForceStop();
    }
}
