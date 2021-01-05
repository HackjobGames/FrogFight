using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    private float charge;
    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine){
    }

    public override void Enter()
    {
        base.Enter();
        charge = 0;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetButtonUp("Jump")) {
          float force = (character.max_jump_force >= charge) ? charge : character.max_jump_force;
          character.ApplyImpulse(Vector3.up * force);
          state_machine.ChangeState(character.falling_state);
        }
    }
    public override void LogicUpdate()
    {
      base.LogicUpdate();
      print(charge);
      charge += character.jump_charge_speed * Time.deltaTime;
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
