using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    Action<Vector3> onAnimatorMove;
    public void SetOnAnimatorMove(Action<Vector3> onAnimatorMove)
    {
        this.onAnimatorMove = onAnimatorMove;
    }

    private void OnAnimatorMove() 
    {
        onAnimatorMove?.Invoke(transform.position);
    }
}
