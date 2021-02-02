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
                character.EnableTongue();
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        character.ResetGravity();
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
            character.SwingCircularArc();
            if(character.collision.on_ground){
                //character.DisableTongue(grapple_engaged);
                grapple_engaged = false;
            }

            if(character.rigid_body.velocity.y < 0){
                character.IncreaseGravity();
            } else if(character.rigid_body.velocity.y >= 0){
                character.DecreaseGravity();
            }
        } else {
            if(character.rigid_body.velocity.y <= 0 || character.cur_tongue_distance > character.initial_tongue_distance && !grapple_engaged){
                MonoBehaviour.print("engage");
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
        if(grapple_engaged){
            //character.GrapplePhysics();
            //character.SwingGravity();
        }
        base.PhysicsUpdate();
    }
}