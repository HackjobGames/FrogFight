using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleState : State
{

    private bool grapple_engaged = false;
    private bool ground_sliding = false;
    

    private float horizontal_input;
    private float vertical_input;
    private float turnSpeed = .1f;

    private Vector3 prev_vel;
    private float tongue_length;
    private float set_tongue_length;
    private Transform cam;

    public GrappleState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        if(!character.StartGrapple()){
            state_machine.ChangeState(character.idle_state);
        } else {
            if(character.collision.on_ground){
                grapple_engaged = true;
                ground_sliding = true;
                character.EnableTongue();
            }
        }
        horizontal_input = vertical_input = 0.0f;
        cam = Camera.main.gameObject.transform;
    }
    public override void Exit()
    {
        base.Exit();
        character.ResetGravity();
        character.StopGrapple(grapple_engaged);
        grapple_engaged = false;
        ground_sliding = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetMouseButtonUp(0)){
            state_machine.ChangeState(character.idle_state);
        }
        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        character.UpdateTonguePositions();
        if(grapple_engaged){
            if(character.rigid_body.velocity.y < 0){
                character.IncreaseGravity();
            } else if(character.rigid_body.velocity.y >= 0){
                character.DecreaseGravity();
            }
        } else {
            if(character.rigid_body.velocity.y <= 0 || character.cur_tongue_distance > character.initial_tongue_distance){
                grapple_engaged = true;
                character.EnableTongue();
            }
        }

        if(character.collision.on_ceiling){
            state_machine.ChangeState(character.idle_state);
        }

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(grapple_engaged){
            if(character.collision.on_ground && !ground_sliding){
                MonoBehaviour.print("enable ground slide");
                ground_sliding = true;
                set_tongue_length = character.GroundSlideStart(prev_vel);
                character.DisableTongue(grapple_engaged);
                
            } else if((!character.collision.on_ground || tongue_length > set_tongue_length) 
                        && ground_sliding){
                ground_sliding = false;
                character.EnableTongue();
            } else {
                prev_vel = character.rigid_body.velocity;
            }
        }
        if(ground_sliding){
            tongue_length = character.GroundSlide();
        }
        else {
            Vector3 input_vector = new Vector3(horizontal_input,0,vertical_input);
            float speed_modifier = input_vector.magnitude;
            Vector3 dir = input_vector.normalized;
            if(input_vector.magnitude >= 0.1f){
                float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(character.transform.eulerAngles.y, targetAngle, ref turnSpeed, .1f);
                character.transform.rotation = Quaternion.Euler(0f, angle, 0f); 
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                character.Vector(speed_modifier, moveDir.normalized);
            } else {
                character.Vector(1f, Vector3.zero);
            }
        }
    }
}