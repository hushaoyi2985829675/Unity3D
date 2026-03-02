using System;
using System.Collections;
using System.Collections.Generic;
using RoleNs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

public class Move : MonoBehaviour
{
    private CharacterState characterState;
    private RoleInfo roleInfo;
    //当前阵营
    private RoleCamp roleCamp = RoleCamp.SwordShield;
    CharacterController character;
    Vector3 inputValue = new Vector3();
    Vector2 inputDelta = new Vector2();
    float gravity;
    private Rect pixelArea;
    private Vector2 mousePos;
    [SerializeField]
    private bool isJump;
    [SerializeField]
    private bool jumpLock;
    private float velocityY;
    public bool isMoveKey;
    private AnimationEvent animationEvent;
    private InputEvent inputEvent;
    private Vector3 attackMove;
    [SerializeField]
    private bool lastGround;
    private PlayerState moveState = PlayerState.Idle;
    private void Awake()
    {
        character = GetComponent<CharacterController>();
        gravity = Physics.gravity.y;
        // 将相对屏幕的Rect转为实际像素坐标
        pixelArea = new Rect(
            0.6f * Screen.width,
            0,
            Screen.width,
            Screen.height
        );
        characterState = GetComponent<CharacterState>();
        animationEvent = GetComponent<AnimationEvent>();
        inputEvent = GetComponent<InputEvent>();
    }

    private void Start()
    {
        roleInfo = characterState.GetRoleInfo();
        animationEvent.AddAnimationEvent(OnAnimationAction);
        inputEvent.jumpEvent += OnJumpAction;
        inputEvent.moveEvent += OnMoveAction;
        inputEvent.touchEvent += OnTouchAction;
        inputEvent.deltaEvent += OnDeltaAction;
    }

    private void OnGUI()
    {
        GUI.color = new Color(1, 0, 0, 0.005f); // RGBA：红、绿、蓝、透明度
        GUI.DrawTexture(pixelArea, Texture2D.whiteTexture); // 用白色纹理填充矩形
        GUI.color = Color.black;
        GUI.Box(pixelArea, "");
        GUI.color = Color.white;
    }

    public void OnMoveAction(InputValue value)
    {
        inputValue.x = value.Get<Vector2>().x;
        inputValue.z = value.Get<Vector2>().y;
        if (Mathf.Abs(inputValue.x) > 0 ||  Mathf.Abs(inputValue.z) > 0)
        {
            characterState.AddPlayerState(PlayerState.Walk);
            isMoveKey = true;
        }
        else
        {
            isMoveKey = false;
            characterState.RemovePlayerState(PlayerState.Walk);
        }
    }

    public void OnJumpAction(InputValue value)
    {
        if (isJump || characterState.IsCurStatePriorityHigher(PlayerState.Jump))
        {
            return;
        }
        isJump = true;
        characterState.AddPlayerState(PlayerState.Jump);
        animationEvent.PostAnimationEvent("JumpTrigger");
        jumpLock = true;
    }

    public void OnAnimationAction(string eventName)
    {
        switch (eventName)
        {
            case "AttackJumpEnd":
                if (characterState.GetGround())
                {
                    isJump = false;
                    jumpLock = false;
                    characterState.RemovePlayerState(PlayerState.Jump);
                }
                break;
            case "Jump":
                jumpLock = false;
                velocityY = Mathf.Sqrt(2 * roleInfo.jumpSpeed * -gravity); 
                break;
            case "JumpEndStart":
                jumpLock = true;
                break;
            case "JumpEnd":
                characterState.RemovePlayerState(PlayerState.Jump);
                isJump = false;
                jumpLock = false;
                break;
            case "AttackMoveStart":
                attackMove = new Vector3(0, 0, 0.7f);
                break;
            case "AttackMoveEnd":
                attackMove = new Vector3(0, 0, 0);
                break;
            case "SkillJump":
                attackMove = new Vector3(0, 0, 1);
                velocityY = Mathf.Sqrt(2 * 2.5f * -gravity); 
                break;
        }
    }

    public void OnTouchAction(InputValue value)
    {
        mousePos = Mouse.current.position.ReadValue();
    }

    public void OnDeltaAction(InputValue value)
    {
        if (pixelArea.Contains(mousePos))
        {
            inputDelta = value.Get<Vector2>();
            transform.localRotation *= Quaternion.Euler(0, inputDelta.x * 0.5f, 0);
        }
    }

    void Update()
    {
        lastGround =  character.isGrounded;
        //移动
        move();
        if (!character.isGrounded && !lastGround && !isJump && characterState.GetPlayerState() != PlayerState.Skill)
        {
            isJump = true;
            characterState.AddPlayerState(PlayerState.Jump);
            // EditorApplication.isPaused = true;
        }
        
        //增加重力
        AddGravity();
        characterState.SetVelocity(transform.InverseTransformDirection(character.velocity));
        characterState.SetGround(character.isGrounded);
        characterState.SetLastGround(lastGround);
    }

    void move()
    {
        float localSpeed = roleInfo.moveSpeed;
        // if (characterState.GetPlayerState() != PlayerState.Walk)
        // {
        //     localSpeed *= 0.5f;
        // }
        var moveDir = transform.TransformDirection(inputValue) * (Time.deltaTime * localSpeed);
        //跳跃优化
        if (velocityY < 0)
        {
            velocityY += gravity * 3 * Time.deltaTime;
        }
        else
        {
            velocityY += gravity * 1f * Time.deltaTime;
        }
        moveDir.y = velocityY * Time.deltaTime;
        PlayerState state = characterState.GetPlayerState();
        if (state == PlayerState.Attack || state == PlayerState.Skill)
        {
            Vector3 att = transform.TransformDirection(attackMove) * (Time.deltaTime * 2);
            moveDir.z = att.z * localSpeed;
            moveDir.x = att.x * localSpeed;
        }
        if (jumpLock || state == PlayerState.SwordShield)
        {
            moveDir.x = 0;
            moveDir.z = 0;
        }
        character.Move(moveDir);
    }

    void AddGravity()
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

    private void FixedUpdate()
    {
       
    }
}
