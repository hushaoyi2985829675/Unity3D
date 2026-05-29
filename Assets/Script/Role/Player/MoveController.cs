using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    private CharacterState characterState;
    private RoleInfo roleInfo;
    private CharacterController character;
    private Vector3 inputValue = new Vector3();
    private Vector2 inputDelta = new Vector2();
    [Header("一些参数")]
    private float gravity = -9.81f;
    private float transitionScale = 0f;
    public bool isJump = false;
    [SerializeField]
    private bool jumpLock;
    public bool jumpTrigger;
    private Transform body;
    public float velocityY;
    public bool isMoveKey;
    private AnimationEvent animationEvent;
    private Vector3 attackMove;
    [SerializeField]
    private bool lastGround;

    private string aniName;
    private string aniTrigger;
    private void Awake()
    {
        character = gameObject.GetComponent<CharacterController>();
        characterState = GetComponent<CharacterState>();
        character.center = new Vector3(0, 1, 0);
        gravity = Physics.gravity.y;
        body = transform.Find("Body");
        animationEvent = GetComponent<AnimationEvent>();
    }

    private void Start()
    {
        roleInfo = characterState.GetRoleInfo();
        animationEvent.AddAnimationEvent(OnAnimationAction);
    }

    // private void OnGUI()
    // {
    //     GUI.color = new Color(1, 0, 0, 0.005f); // RGBA：红、绿、蓝、透明度
    //     GUI.DrawTexture(pixelArea, Texture2D.whiteTexture); // 用白色纹理填充矩形
    //     GUI.color = Color.black;
    //     GUI.Box(pixelArea, "");
    //     GUI.color = Color.white;
    // }

    public void OnMoveInput(InputValue value)
    {
        Vector2 v = value.Get<Vector2>();
        inputValue.x = v.x;
        inputValue.z = v.y;
    }

    public void OnJumpInput(InputValue value)
    {
        Debug.Log("空格2222");
        if (isJump)
        {
            return;
        }
        isJump = true;
        // jumpLock = true;
        // characterState.PlayAnimation("JumpStart");
        jumpTrigger = true;
        Debug.Log("空格");
    }

    public void OnAnimationAction(string eventName)
    {
        // switch (eventName)
        // {
        //     case "AttackJumpEnd":
        //         if (characterState.GetGround())
        //         {
        //             isJump = false;
        //             jumpLock = false;
        //             characterState.RemovePlayerState(PlayerState.Jump);
        //         }
        //         break;
        //     case "Jump":
        //         // jumpLock = false;
        //         // jumpTrigger = true;
        //         break;
        //     case "JumpEndStart":
        //         jumpLock = true;
        //         break;
        //     case "JumpEnd":
        //         characterState.RemovePlayerState(PlayerState.Jump);
        //         isJump = false;
        //         jumpLock = false;
        //         break;
        //     case "AttackMoveStart":
        //         attackMove = new Vector3(0, 0, 0.7f);
        //         break;
        //     case "AttackMoveEnd":
        //         attackMove = new Vector3(0, 0, 0);
        //         break;
        //     case "SkillJump":
        //         attackMove = new Vector3(0, 0, 1);
        //         velocityY = Mathf.Sqrt(2 * 2.5f * -gravity);
        //         break;
        // }
    }


    private void Update()
    {
        aniName = !isJump ? "Move" : "";
        aniTrigger = "";
        Vector3 moveDir = new Vector3(0, 0, 0);
        //移动
        if (inputValue.sqrMagnitude > 0)
        {
            Transform cTrans = Camera.main.transform;

            Vector3 camForward = cTrans.forward;
            Vector3 camRight = cTrans.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();
            Vector3 inputDir = new Vector3(inputValue.x, 0f, inputValue.z);
            moveDir = (camRight * inputDir.x + camForward * inputDir.z) * roleInfo.runSpeed;
            float signedAngle = Vector3.SignedAngle(body.forward, moveDir, Vector3.up);
            body.rotation = Quaternion.Slerp(body.rotation,body.rotation * Quaternion.Euler(0f, signedAngle, 0f),characterState.turnSpeed * Time.deltaTime);

        }
        else
        {
            transitionScale = 0;
        }
        //跳跃
        if (isJump)
        {
            // characterState.AddPlayerState(PlayerState.Jump);
            // animationEvent.PostAnimationEvent("JumpTrigger");
            if (jumpTrigger)
            {
                velocityY = Mathf.Sqrt(2 * roleInfo.jumpSpeed * -gravity);
                jumpTrigger = false;
                aniName = "JumpStart";
            }
            
            characterState.SetAnimatorFloat("VelocityY", velocityY);

        }

        AddGravity();
        moveDir.y = velocityY;
        
        if (!jumpLock)
        {
            character.Move(moveDir * Time.deltaTime);
        }
        
        // 上一帧地面（本帧移动前的着地状态），供状态与动画用
        
        characterState.SetAnimatorBool("Ground", character.isGrounded);
        characterState.SetAnimatorBool("LastGround", lastGround);
        characterState.PlayAnimation(aniName);
        if (!character.isGrounded && !lastGround && !isJump && characterState.GetPlayerState() != PlayerState.Skill)
        {
            isJump = true;
            aniName = "JumpAir";
        }
        if(!lastGround && character.isGrounded)
        {
            isJump = false;
            aniName = "Move";
        }
        
        characterState.SetAnimatorTrigger(aniTrigger);
        characterState.SetAnimatorFloat("Velocity", inputValue.sqrMagnitude);
        characterState.SetVelocity(transform.InverseTransformDirection(character.velocity));
        characterState.SetGround(character.isGrounded);
        characterState.SetLastGround(lastGround);
        lastGround = character.isGrounded;
    }

    private void ApplyMove()
    {

        // var moveDir = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * inputValue * (Time.deltaTime * localSpeed);
        // //跳跃优化
        // if (velocityY < 0)
        // {
        //     velocityY += gravity * 3 * Time.deltaTime;
        // }
        // else
        // {
        //     velocityY += gravity * 1f * Time.deltaTime;
        // }
        // moveDir.y = velocityY * Time.deltaTime;
        // PlayerState state = characterState.GetPlayerState();
        // if (state == PlayerState.Attack || state == PlayerState.Skill)
        // {
        //     Vector3 att = transform.TransformDirection(attackMove) * (Time.deltaTime * 2);
        //     moveDir.z = att.z * localSpeed;
        //     moveDir.x = att.x * localSpeed;
        // }
        // if (jumpLock || state == PlayerState.SwordShield)
        // {
        //     moveDir.x = 0;
        //     moveDir.z = 0;
        // }

    }
    private void AddGravity()
    {
        if (character.isGrounded && !isJump)
        {
            velocityY = -2f;
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

    }
}
