using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingState : State
{
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
        if(Input.GetMouseButtonUp(0)){
            state_machine.ChangeState(character.falling_state);
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        character.UpdateTonguePositions();

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}