using Farm;
using System.Collections.Generic;
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
        Walk_Right
    }

    private Dictionary<AnimState, string> animDict = new Dictionary<AnimState, string>();
    private float bufferTime = 0.0f;
    private float prevTime = 0.0f;
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
        StopIdle();
        SetAllAnimsFalse();
        if (move.x != 0)
        {
            PlayAnim(move.x < 0 ? AnimState.Walk_Left : AnimState.Walk_Right);
        }
        bufferTime = clips[1].length + 0.15f;
        prevTime = Time.time;

    }

    public void AnimStarted()
    {

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
