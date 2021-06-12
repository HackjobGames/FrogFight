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
    }
    public override void Exit()
    {
        base.Exit(); 
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetMouseButtonUp(0) || !character.hit_location){
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
        character.Fall();
        if(!character.ApplyTongueForce()) {
          state_machine.ChangeState(character.falling_state);
        };
    }
}