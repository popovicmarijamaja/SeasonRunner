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
    private Transform leftPos;
    private Transform centrePos;
    private Transform rightPos;
    private Animator CharacterAnimator;
    private Rigidbody characterRb;
    private NetworkMecanimAnimator _networkAnimator;

    private void Awake()
    {
        CharacterAnimator = GetComponent<Animator>();
        characterRb = GetComponent<Rigidbody>();
        _networkAnimator =GetComponent<NetworkMecanimAnimator>();
    }
    public void GetPos(Transform left, Transform centre, Transform right)
    {
        leftPos = left;
        centrePos = centre;
        rightPos = right;
    }
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (data.IsMoveRight)
            {
                NetworkManager.bufferedInput.IsMoveRight = false;
                SetCharacterAnimation(RightBool);
                StartCoroutine(MoveToPosition(rightPos.position));
            }
            else if (data.IsMoveCentre)
            {
                NetworkManager.bufferedInput.IsMoveCentre = false;
                if(transform.position.z == leftPos.position.z)
                    SetCharacterAnimation(RightBool);
                else if (transform.position.z == rightPos.position.z)
                    SetCharacterAnimation(LeftBool);
                StartCoroutine(MoveToPosition(centrePos.position));
            }
            else if (data.IsMoveLeft)
            {
                NetworkManager.bufferedInput.IsMoveLeft = false;
                SetCharacterAnimation(LeftBool);
                StartCoroutine(MoveToPosition(leftPos.position));
            }
            if (data.IsJump)
            {
                NetworkManager.bufferedInput.IsJump = false;
                _networkAnimator.SetTrigger(JumpTriggerParameter);
                characterRb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
            if (data.IsCrawl)
            {
                NetworkManager.bufferedInput.IsCrawl = false;
                _networkAnimator.SetTrigger(RollTriggerParameter);
            }
        }
    }
    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;

        while (elapsedTime < TransitionDuration)
        {
            float newZ = Mathf.Lerp(startingPos.z, targetPosition.z, elapsedTime / TransitionDuration);
            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set only the Z position to the target, keeping X and Y at their original values.
        transform.position = new Vector3(transform.position.x, transform.position.y, targetPosition.z);

        CharacterAnimator.SetBool(RightBool, false);
        CharacterAnimator.SetBool(LeftBool, false);
    }

    private void SetCharacterAnimation(string boolName)
    {
        _networkAnimator.Animator.SetBool(boolName, true);
    }
}
