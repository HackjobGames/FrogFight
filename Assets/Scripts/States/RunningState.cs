using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningState : State
{
    protected float speed;
    protected float velocity;
    private float horizontal_input;
    private float vertical_input;
    public RunningState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        horizontal_input = vertical_input = 0.0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
        horizontal_input = Input.GetAxis("Horizontal");
        vertical_input = Input.GetAxis("Vertical");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(Input.GetButtonDown("Jump")&&character.collision.on_ground){
            state_machine.ChangeState(character.jumping_state);
        } else if(!character.collision.on_ground){
            state_machine.ChangeState(character.falling_state);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector3 input_vector = new Vector3(horizontal_input,0,vertical_input);
        float speed_modifier = input_vector.magnitude;
        Vector3 direction = input_vector.normalized;

        if(input_vector.magnitude >= 0.1f){
            character.Move(speed_modifier, direction);
        } else {
            character.Stop();
        }
    }
}
