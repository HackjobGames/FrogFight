using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlidingState : State
{
    private float tongue_length;
    private float set_tongue_length;
    public GroundSlidingState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        set_tongue_length = character.GroundSlideStart();
    }
    public override void Exit()
    {
        base.Exit();
        character.GroundSlideEnd();
        character.ground_slide_timer.StartTimer();
    }

    public override void HandleInput()
    {
        base.HandleInput();

    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        MonoBehaviour.print(set_tongue_length + " ||| " + tongue_length);
        if(!character.collision.on_ground || tongue_length > set_tongue_length + 1){
            state_machine.ChangeState(character.swinging_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        tongue_length = character.GroundSlide();
    }
}