using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingState : State
{
    public StandingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }
    private float y_pos;
    public override void Enter()
    {
        base.Enter();
        character.TransitionAnimations(Character.Anim.Idle);
        character.rigid_body.useGravity = false;
        y_pos = character.stabilizer_transform.rotation.y;
    }
    public override void Exit()
    {
        base.Exit();
        character.rigid_body.useGravity = true;
        character.standing_cooldown.StartTimer();

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
        if(!character.collision.sphere_collided) {
            state_machine.ChangeState(character.falling_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Stop(y_pos);
    }
}
