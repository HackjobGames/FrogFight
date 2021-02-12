using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollState : State
{
    private float horizontal_input;
    private float vertical_input;
    private float turnSpeed = .1f;
    private Transform cam;
    public RagdollState(Character character, StateMachine stateMachine) : base(character, stateMachine){

    }

    public override void Enter()
    {
        base.Enter();
        character.rigid_body.constraints = RigidbodyConstraints.None;
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
        if(character.collision.on_ground){
            state_machine.ChangeState(character.standing_state);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        character.Vector(1, Vector3.zero);
    }
}

