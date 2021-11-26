using System;
using System.Collections.Generic;
using UnityEngine;

class ThreadManager : MonoBehaviour
{
    #region Fields
    static readonly List<Action> executeOnMainThread = new List<Action>();
    static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    static bool actionToExecuteOnMainThread;
    #endregion

    #region Methods
    private void FixedUpdate()
    {
        if (actionToExecuteOnMainThread)
            UpdateMain();
    }

    /// <summary>Sets an action to be executed on the main thread.</summary>
    /// <param name="action">The action to be executed on the main thread.</param>
    public static void ExecuteOnMainThread(Action action)
    {
        if (action == null)
        {
            Debug.Log("No action to execute on main thread!");
            return;
        }

        lock (executeOnMainThread)
        {
            executeOnMainThread.Add(action);
            actionToExecuteOnMainThread = true;
        }
    }

    /// <summary>Executes all code meant to run on the main thread. NOTE: Call this ONLY from the main thread.</summary>
    static void UpdateMain()
    {
        executeCopiedOnMainThread.Clear();
        lock (executeOnMainThread)
        {
            executeCopiedOnMainThread.AddRange(executeOnMainThread);
            executeOnMainThread.Clear();
            actionToExecuteOnMainThread = false;
        }

        for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
        {
            executeCopiedOnMainThread[i]();
        }
    }
    #endregion
}