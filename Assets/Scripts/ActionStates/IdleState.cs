using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public IdleState(Character character, StateMachine stateMachine) : base(character, stateMachine){

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
        if(Input.GetMouseButtonDown(0)){
            if(character.StartGrapple()){
                state_machine.ChangeState(character.grappling_state);
                character.movement_machine.ChangeState(character.swinging_state);
            } else {
                character.StopGrapple();
            }
        }
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        character.AimMarkerUpdate();
        if (Player.localPlayer.dead) {
          state_machine.ChangeState(character.spectate_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
