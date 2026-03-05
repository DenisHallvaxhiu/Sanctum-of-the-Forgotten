using UnityEngine;
using System;

public class PlayerEvents : MonoBehaviour {
    //Events
    public event Action<bool> Move;
    public event Action<bool> Sprint;
    public event Action Jump;


    public void RaiseMove(bool value) {
        Move?.Invoke(value);
    }

    public void RaiseSprint(bool isSprinting) {
        Sprint?.Invoke(isSprinting);
    }

    public void RaiseJump() {
        Jump?.Invoke();
    }
    
}
