using System.Collections.Generic;
using UnityEngine;
using System;

public class InputHandler 
{
    public event Action<Vector2> OnDirectionKeyPressed;
    private List<ActionBinding> validInputs = new List<ActionBinding>();
    
    public void RegisterInput(params ActionBinding[] bindings) 
    {
        foreach(var binding in bindings)
        {
            validInputs.Add(binding);
        }
    }

    public void ProcessInputs()
    {
        foreach(var input in validInputs)
        {
            if(Input.GetKeyDown(input.Binding))
            {
                input.Action?.Invoke();
            }
        }
    }

}
