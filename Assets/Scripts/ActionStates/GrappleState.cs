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
    }
    public override void Exit()
    {
        base.Exit();
        character.StopGrapple();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(character.movement_machine.cur_state != character.swinging_state){
            state_machine.ChangeState(character.idle_state);
        }
        // character.UpdateTonguePositions();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}