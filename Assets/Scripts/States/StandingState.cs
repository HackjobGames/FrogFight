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
        if(Input.GetButton("Jump") && character.action_machine.cur_state != character.grappling_state){
            state_machine.ChangeState(character.jumping_state);
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(!character.collision.on_ground) {
            state_machine.ChangeState(character.falling_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Stop();
    }
}
