using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerNetworkMovement : NetworkBehaviour
{
    private const float TransitionDuration = 0.2f;
    private const float JumpForce = 5f;
    private const string JumpTriggerParameter = "jump";
    private const string RollTriggerParameter = "down";
    private const string RightBool = "right";
    private const string LeftBool = "left";
    private const string DeathAnimBool = "death";
    [SerializeField] private GameObject character;
    private readonly Vector3 leftPos = new(0, 0, -1);
    private readonly Vector3 centrePos = new(0, 0, 0);
    private readonly Vector3 rightPos = new(0, 0, 1);
    private Animator CharacterAnimator;
    private Rigidbody characterRb;
    private NetworkMecanimAnimator _networkAnimator;
    private void Awake()
    {
        CharacterAnimator = character.GetComponent<Animator>();
        characterRb = character.GetComponent<Rigidbody>();
        _networkAnimator =character.GetComponent<NetworkMecanimAnimator>();
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.IsMoveRight)
            {
                NetworkManager.bufferedInput.IsMoveRight = false;
                SetCharacterAnimation(RightBool);
                StartCoroutine(MoveToPosition(rightPos));
            }
            else if (data.IsMoveCentre)
            {
                NetworkManager.bufferedInput.IsMoveCentre = false;
                if(transform.position==leftPos)
                    SetCharacterAnimation(RightBool);
                else if (transform.position == rightPos)
                    SetCharacterAnimation(LeftBool);
                StartCoroutine(MoveToPosition(centrePos));
            }
            else if (data.IsMoveLeft)
            {
                NetworkManager.bufferedInput.IsMoveLeft = false;
                SetCharacterAnimation(LeftBool);
                StartCoroutine(MoveToPosition(leftPos));
            }
            if (data.IsJump)
            {
                NetworkManager.bufferedInput.IsJump = false;
                _networkAnimator.SetTrigger(JumpTriggerParameter);
                characterRb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
        }
    }
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < TransitionDuration)
        {
            transform.position = Vector3.Lerp(startingPos, targetPosition, elapsedTime / TransitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CharacterAnimator.SetBool(RightBool, false);
        CharacterAnimator.SetBool(LeftBool, false);
        transform.position = targetPosition;
    }

    private void SetCharacterAnimation(string boolName)
    {
        _networkAnimator.Animator.SetBool(boolName, true);
    }
}
