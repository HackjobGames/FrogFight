using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingState : State
{
    private float horizontal_input;
    private float vertical_input;
    private float turnSpeed = .1f;
    private Transform cam;
    public FallingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

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
        //if(Mathf.Abs(character.rigid_body.velocity.magnitude) < 0.1f) {
        if(character.collision.on_ground) {
            state_machine.ChangeState(character.standing_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(character.action_machine.cur_state != character.grappling_state){
            character.Fall();
        }
        character.Stop();
    }
}

