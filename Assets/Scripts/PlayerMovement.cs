using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static readonly int HashMove = Animator.StringToHash("Move");

    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;

    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 이렇게 해쉬로 변환된 놈을 들고 있는걸 사용하는걸 기본으로
        playerAnimator.SetFloat(HashMove, playerInput.Move);
        //playerAnimator.SetFloat(Animator.StringToHash("Move"), playerInput.Move);
    }

    // 디버그로 물리랑 랜더랑 다른지 확인하기
    private void FixedUpdate()
    {
        float angle = playerInput.Rotate * rotateSpeed * Time.deltaTime;
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0f, angle, 0f));

        Vector3 delta = playerInput.Move * transform.forward * moveSpeed * Time.deltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + delta);
    }
}
