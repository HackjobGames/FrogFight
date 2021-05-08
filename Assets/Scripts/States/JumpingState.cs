using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine){
    }

    public override void Enter()
    {
        base.Enter();
        Vector3 angle = new Vector3(Camera.main.transform.forward.x, 1, Camera.main.transform.forward.z);
        character.TransitionAnimations(Character.Anim.Jump);
        character.Jump(angle);
        state_machine.ChangeState(character.falling_state);
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();

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


