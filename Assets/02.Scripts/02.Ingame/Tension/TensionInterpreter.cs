﻿using System.Collections.Generic;
 using UnityEngine;

public class TensionEventInterpreter : MonoBehaviour
{
    private readonly List<SubTensionInterpreterBase> interpreters = new();

    private void Awake()
    {
        interpreters.Add(new OnScanNoTargetInterpreter(this, 1f));
        //interpreters.Add(new OnScanCompletedInterpreter(this));
    }

    private void OnEnable()
    {
        foreach (var interpreter in interpreters)
        {
            interpreter.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var interpreter in interpreters)
        {
            interpreter.Disable();
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        foreach (var interpreter in interpreters)
        {
            interpreter.Tick(deltaTime);
        }
    }
}
