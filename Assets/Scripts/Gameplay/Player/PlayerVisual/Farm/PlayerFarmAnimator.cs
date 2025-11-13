using DG.Tweening;
using Farm;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerFarmAnimator : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerFarmInput farmInput;
    [SerializeField] private AnimationClip[] clips;

    public enum AnimState
    {
        Idle = 0,
        Digging,
        Walk_Left,
        Walk_Right,
        Walk_Up,
        Walk_Down
    }

    private Dictionary<AnimState, string> animDict = new Dictionary<AnimState, string>();
    private float bufferTime = 0.0f;
    private float prevTime = 0.0f;

    public delegate void OnGroundPound();
    public OnGroundPound AnimReachedGroundPound;

    public Animator PlayerAnimator => playerAnimator;

    private void OnEnable()
    {
        farmInput.OnFarmInput += FarmPressed;
        farmInput.OnMovementEvent += MovementPressed;
    }

    private void OnDisable()
    {
        farmInput.OnFarmInput -= FarmPressed;
        farmInput.OnMovementEvent -= MovementPressed;
    }

    private void Awake()
    {
        animDict.Add(AnimState.Idle, "idleBool");
        animDict.Add(AnimState.Digging, "digBool");
        animDict.Add(AnimState.Walk_Left, "leftMoveBool");
        animDict.Add(AnimState.Walk_Right, "rightMoveBool");
        animDict.Add(AnimState.Walk_Down, "downMoveBool");
        animDict.Add(AnimState.Walk_Up, "upMoveBool");
    }

    private void Start()
    {
        PlayAnim(AnimState.Idle);
    }

    private void FarmPressed(bool farming)
    {
        SetAllAnimsFalse();
        PlayAnim(AnimState.Digging);
        bufferTime = clips[0].length;
        prevTime = Time.time;
    }

    private void MovementPressed(Vector2 move)
    {
        if (playerAnimator.GetBool("digBool") == true) { return; }
        StopIdle();
        SetAllAnimsFalse();
        if (move.x != 0)
        {
            PlayAnim(move.x < 0 ? AnimState.Walk_Left : AnimState.Walk_Right);
        }
        if (move.y != 0)
        {
            PlayAnim(move.y < 0 ? AnimState.Walk_Down : AnimState.Walk_Up);
        }
        bufferTime = clips[1].length + 0.15f;
        prevTime = Time.time;

    }

    public void GroundPound()
    {
        print(AnimReachedGroundPound);
        Camera.main.transform.DOShakePosition(0.1f, 0.1f);
        AnimReachedGroundPound?.Invoke();
    }

    public void AnimEnded(AnimState endedAnimState)
    {
        playerAnimator.SetBool(animDict[endedAnimState], false);
    }

    private void PlayAnim(AnimState currAnimState)
    {
        playerAnimator.SetBool(animDict[currAnimState], true);
    }

    private void StopIdle()
    {
        playerAnimator.SetBool(animDict[AnimState.Idle], false);
    }
    private void SetAllAnimsFalse()
    {
        foreach (var anim in animDict.Values)
        {
            playerAnimator.SetBool(anim, false);
        }
    }

    private void Update()
    {
        
        if (Time.time - prevTime >= bufferTime)
        {
            PlayAnim(AnimState.Idle);
        }
        
    }
}
