using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputEvent : MonoBehaviour
{
   public Action<InputValue> jumpEvent;
   public Action<InputValue> moveEvent;
   public Action<InputValue> deltaEvent;
   public Action<InputValue> touchEvent;
   public Action<InputValue> swordShieldEvent;
   public Action<InputValue> attackEvent;
   
   private void OnJumpInput(InputValue value)
   {
      jumpEvent.Invoke(value);
   }

   private void OnMoveInput(InputValue value)
   {
      moveEvent?.Invoke(value);
   }

   private void OnDeltaInput(InputValue value)
   {
      deltaEvent?.Invoke(value);
   }
   private void OnTouchInput(InputValue value)
   {
      touchEvent?.Invoke(value);
   }

   private void OnSwordShieldInput(InputValue value)
   {
      swordShieldEvent?.Invoke(value);
   }

   private void OnAttackInput(InputValue value)
   {
      attackEvent?.Invoke(value);
   }
}
