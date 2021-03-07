using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingState : State
{
    private bool grapple_engaged = false;
    public SwingingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        character.EnableTongue();
    }
    public override void Exit()
    {
        base.Exit(); 
        character.DisableTongue();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(character.collision.on_ground && !character.ground_slide_timer.is_active){
            character.ResetGravity();
            state_machine.ChangeState(character.ground_sliding_state);
        } else {
            character.SetGroundVelocity();
        }

        if(character.rigid_body.velocity.y < 0){
            character.IncreaseGravity();
        } else{
            character.DecreaseGravity();
        }

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Fall();
    }
}