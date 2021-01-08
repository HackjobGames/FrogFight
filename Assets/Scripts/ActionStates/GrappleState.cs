using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleState : State
{

    private bool grapple_engaged = false;
    
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
                character.enable_tongue();
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        character.reset_gravity();
        character.StopGrapple(grapple_engaged);
        grapple_engaged = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetMouseButtonUp(0)){
            state_machine.ChangeState(character.idle_state);
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        character.UpdateTonguePositions();
        if(grapple_engaged){
            if(character.rigid_body.velocity.y < 0){
                character.increase_gravity();
            } else if(character.rigid_body.velocity.y >= 0){
                character.decrease_gravity();
            }
        } else {
            if(character.rigid_body.velocity.y <= 0){
                grapple_engaged = true;
                character.enable_tongue();
            }
        }

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}