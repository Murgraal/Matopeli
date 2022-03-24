using UnityEngine;
using System;

public struct ActionBinding
{
    public KeyCode Binding;
    public Action Action;
    
    public ActionBinding(KeyCode binding, Action action)
    {
        this.Binding = binding;
        this.Action = action;
    }
}
