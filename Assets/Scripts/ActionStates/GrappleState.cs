using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleState : State
{
    public GrappleState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        if(!character.StartGrapple()){
            state_machine.ChangeState(character.idle_state);
        }
    }
    public override void Exit()
    {
        base.Exit();
        Debug.Log("exit grapple");
        character.StopGrapple();
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

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}