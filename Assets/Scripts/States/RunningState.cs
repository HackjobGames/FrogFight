using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : State
{
    protected float speed;
    protected float velocity;
    private float horizontal_input;
    private float vertical_input;
    private float turnSpeed = .1f;
    private Transform cam;
    public RunningState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        horizontal_input = vertical_input = 0.0f;
        cam = Camera.main.gameObject.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Input.GetButtonDown("Jump")&&character.collision.on_ground){
            state_machine.ChangeState(character.jumping_state);
        } else if(Input.GetMouseButtonDown(0)){
            state_machine.ChangeState(character.swinging_state);
        } else if(!character.collision.on_ground){
            state_machine.ChangeState(character.falling_state);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector3 input_vector = new Vector3(horizontal_input,0,vertical_input);
        float speed_modifier = input_vector.magnitude;
        Vector3 dir = input_vector.normalized;
        print(input_vector);
        if(input_vector.magnitude >= 0.1f){
          float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
          float angle = Mathf.SmoothDampAngle(character.transform.eulerAngles.y, targetAngle, ref turnSpeed, .1f);
          character.transform.rotation = Quaternion.Euler(0f, angle, 0f); 
          Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
          character.Move(speed_modifier, moveDir.normalized);
        } else {
          character.Stop();
        }
    }
}
