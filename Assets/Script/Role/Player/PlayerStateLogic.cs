using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateLogic : MonoBehaviour
{
    CharacterState characterState;
    protected Dictionary<PlayerState, UnitState> unitStateList;
    PlayerState curState;

    [Header("一些变量")]
    private Vector3 inputValue = new Vector3();
    protected virtual void Start()
    {
        unitStateList = new Dictionary<PlayerState, UnitState>();
        unitStateList.Add(PlayerState.Walk, new UnitState(OnWalkEnter, OnWalkUpdate, OnWalkExit));
        unitStateList.Add(PlayerState.Attack, new UnitState(OnJumpEnter, OnJumpUpdate, OnJumpExit));
        unitStateList[curState].Enter();
    }

    private void Update()
    {
        unitStateList[curState].Enter();
    }

    #region 按键检测
    public void OnMoveInput(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        inputValue.x = v.x;
        inputValue.z = v.y;
    }

    #endregion

    private void ChangeState(PlayerState state)
    {
        if(curState == state)
        {
            return;
        }
        unitStateList[curState].Exit();
        curState = state;
        unitStateList[curState].Enter();
    }
    private void OnWalkEnter()
    {
        characterState.PlayAnimation("Walk");
    }
    private void OnWalkUpdate()
    {
        
    }
     private void OnWalkExit()
    {
        
    }
    private void OnJumpEnter()
    {
        
    }
     private void OnJumpUpdate()
    {
        
    }
    private void OnJumpExit()
    {
        
    }
}
