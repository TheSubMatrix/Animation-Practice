using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    CharacterController _controller;
    [SerializeField] Animator _animator;
    [SerializeField] float speed = 6f;
    [SerializeField] float turnSmoothTime = 0.1f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] Camera cam;

    Vector3 _moveDirection = Vector3.zero;
    Vector3 _verticalVelocity = Vector3.zero;
    float turningVelocity;
    float _gravity = - Physics.gravity.magnitude;
    const float runAnimSpeed = 6;
    bool isEmoting = false;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {

        if (_controller.isGrounded && _verticalVelocity.y < -2)
        {
            _verticalVelocity.y = -2;
        }
        if(!_controller.isGrounded)
        {
            _verticalVelocity.y += _gravity * Time.deltaTime;
            _controller.Move(_verticalVelocity * Time.deltaTime);
            if(_verticalVelocity.y > 0)
            {
                _animator.SetBool("Is Falling", true);
            }
        }
        else
        {
            _animator.SetBool("Is Jumping", false);
            _animator.SetBool("Is Falling", false);
        }
        
        _animator.SetBool("Is Grounded", _controller.isGrounded);


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if(Input.GetButtonDown("Jump") && _controller.isGrounded && !isEmoting)
        {
            _animator.SetBool("Is Jumping", true);
            _verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2 * _gravity);
        }
        Vector3 desiredMovementDirection = new Vector3(horizontalInput,0f, verticalInput).normalized;

        if(Input.GetButtonDown("Emote 1") && !isEmoting && _controller.isGrounded && desiredMovementDirection.magnitude < 0.1)
        {
            isEmoting = true;
            _animator.SetTrigger("Emote 1");
        }
        if (Input.GetButtonDown("Emote 2") && !isEmoting && _controller.isGrounded && desiredMovementDirection.magnitude < 0.1)
        {
            isEmoting = true;
            _animator.SetTrigger("Emote 2");
        }
        if (Input.GetButtonDown("Emote 3") && !isEmoting && _controller.isGrounded && desiredMovementDirection.magnitude < 0.1)
        {
            isEmoting = true;
            _animator.SetTrigger("Emote 3");
        }


        if (desiredMovementDirection.magnitude >= 0.1 && !isEmoting)
        {
            float desiredAngle = Mathf.Atan2(desiredMovementDirection.x, desiredMovementDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float calculatedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, desiredAngle, ref turningVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, calculatedAngle, 0f);
            _moveDirection = (Quaternion.Euler(0f, desiredAngle, 0f) * Vector3.forward).normalized;
            Vector3 moveVelocity = _moveDirection * speed * Time.deltaTime;
            _controller.Move(moveVelocity);
            _animator.SetFloat("X Velocity", Vector3.Dot(transform.forward, _controller.velocity));
            _animator.SetFloat("Z Velocity", Vector3.Dot(transform.right, _controller.velocity));
            _animator.SetFloat("Velocity Magnitude", (Mathf.Abs(Vector3.Dot(transform.right, _controller.velocity)) + Mathf.Abs(Vector3.Dot(transform.forward, _controller.velocity))) / runAnimSpeed);

        }
        else
        {
            _animator.SetFloat("X Velocity", 0);
            _animator.SetFloat("Z Velocity", 0);
            _animator.SetFloat("Velocity Magnitude", 0);
        }
    }
    public void FinishEmote()
    {
        isEmoting = false;
    }
}
