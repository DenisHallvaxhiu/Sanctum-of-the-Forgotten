using UnityEngine;
using System;

public class PlayerEvents : MonoBehaviour {
    //Events
    public event Action<bool> Move;
    public event Action<bool> Sprint;
    public event Action Jump;
    public event Action<bool> Crouch;


    public void RaiseMove(bool value) {
        Move?.Invoke(value);
    }

    public void RaiseSprint(bool isSprinting) {
        Sprint?.Invoke(isSprinting);
    }

    public void RaiseJump() {
        Jump?.Invoke();
    }

    public void RaiseCrouch(bool isCrouching) {
        Crouch?.Invoke(isCrouching);
    }

}
