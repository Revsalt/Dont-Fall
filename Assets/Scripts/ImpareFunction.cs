using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ImpareFunction : MonoBehaviour
{
    public ImparedFunction[] imparedFunctions;

    public void ImparedFunc(int i)
    {
        imparedFunctions[i].imparedFunction.Invoke();
    }
}

[System.Serializable]
public class ImparedFunction
{
    public UnityEvent imparedFunction;
}
