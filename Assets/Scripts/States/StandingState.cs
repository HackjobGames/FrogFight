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
        if(Input.GetButtonDown("Jump")&&character.collision.on_ground){
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
        if(character.rigid_body.velocity.x > .1f || character.rigid_body.velocity.z > .1f){
            character.Stop();
        }
    }
}
