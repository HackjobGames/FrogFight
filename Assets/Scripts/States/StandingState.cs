using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingState : State
{
    public StandingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetButton("Jump")&&character.collision.on_ground &&character.action_machine.cur_state != character.grappling_state){
            state_machine.ChangeState(character.jumping_state);
        } else if(!character.collision.on_ground){
            state_machine.ChangeState(character.falling_state);
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(character.action_machine.cur_state != character.grappling_state){
            if((!Mathf.Approximately(character.rigid_body.velocity.x , 0) 
            || !Mathf.Approximately(character.rigid_body.velocity.z , 0)) 
            && character.collision.on_ground){
                character.Stop();
            } else if(!character.collision.on_ground){
                character.Vector(1f, Vector3.zero);
            }
        }
    }
}
