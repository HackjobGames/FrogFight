using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingState : State
{
    public AimingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        //character.ActivateZoomCamera();
    }
    public override void Exit()
    {
        base.Exit();
        // character.ActivateMainCamera();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetMouseButtonDown(0)){
            state_machine.ChangeState(character.grappling_state);
        } else if(Input.GetMouseButtonUp(1)){
            state_machine.ChangeState(character.idle_state);
        } 
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
