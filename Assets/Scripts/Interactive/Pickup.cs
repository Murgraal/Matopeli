using UnityEngine;
using System;

public class Pickup : MonoBehaviour // This needs to go
{
    public event Action WhenPicked;

    private void OnTriggerEnter2D(Collider2D other) {
        
    }
}
