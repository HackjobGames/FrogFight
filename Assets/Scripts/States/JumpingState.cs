using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    private float charge;
    private Vector3 angle;
    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine){
    }

    public override void Enter()
    {
        base.Enter();
        charge = 0;
        character.jump_arc.enabled = true;
    }
    public override void Exit()
    {
        base.Exit();
        character.jump_arc.enabled = false;
    }

    public override void HandleInput()
    {
        base.HandleInput();
        if(Input.GetButtonUp("Jump")) {
          character.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f); 
          character.ApplyImpulse(angle * charge);
          state_machine.ChangeState(character.falling_state);
        }
    }
    public override void LogicUpdate()
    {
      base.LogicUpdate();
      charge = (character.max_jump_force >= charge) ? charge + Time.deltaTime : character.max_jump_force;
      angle = new Vector3(Camera.main.transform.forward.x, 1, Camera.main.transform.forward.z);
      character.jump_arc.positionCount = 300;
      character.jump_arc.SetPositions(CalculateArcArray());
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    Vector3[] CalculateArcArray()
    {
        float grav = Mathf.Abs(Physics.gravity.y);
        print(grav);
        int ticks = 300;
        Vector3[] arcArray = new Vector3[ticks];
        float launchAngle = Mathf.Atan(angle.y/Mathf.Sqrt((angle.x * angle.x) + (angle.z * angle.z)));
        print(launchAngle);
        float velocity = charge * angle.magnitude;
        
        for (int t = 0; t < ticks; t++)
        {
            float val = t * Time.deltaTime;
            arcArray[t] = new Vector3(angle.x * charge * val, (velocity * val * Mathf.Sin(launchAngle)) - (.5f * grav * val * val), angle.z * charge * val) + character.transform.position;
        }

        return arcArray;
    }
}


