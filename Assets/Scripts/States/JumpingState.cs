using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingState : State
{
    private float charge;
    private Vector3 angle;
    private int ticks = 100;
    public JumpingState(Character character, StateMachine stateMachine) : base(character, stateMachine){
    }

    public override void Enter()
    {
        base.Enter();
        charge = 0;
        character.jump_arc.enabled = true;
        character.jump_arc.positionCount = ticks;
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
      charge = (character.max_jump_force >= charge) ? charge + character.jump_charge_speed * Time.deltaTime : character.max_jump_force;
      angle = new Vector3(Camera.main.transform.forward.x, 1, Camera.main.transform.forward.z);
    }

    public override void PhysicsUpdate()
    {
      base.PhysicsUpdate();
      character.jump_arc.SetPositions(CalculateArcArray());
      if((!Mathf.Approximately(character.rigid_body.velocity.x , 0) 
      || !Mathf.Approximately(character.rigid_body.velocity.z , 0)) 
      && character.collision.on_ground){
          character.Stop();
      } else if(!character.collision.on_ground){
          character.Vector(1f, Vector3.zero);
      }
    }
    Vector3[] CalculateArcArray()
    {
        float grav = Mathf.Abs(character.getGravity());
        Vector3[] arcArray = new Vector3[ticks];
        float launchAngle = Mathf.Atan(angle.y/Mathf.Sqrt((angle.x * angle.x) + (angle.z * angle.z)));
        float velocity = charge * angle.magnitude;
        for (int t = 0; t < ticks; t++)
        {
            float val = t * Time.deltaTime;
            arcArray[t] = new Vector3(angle.x * charge * val, (velocity * val * Mathf.Sin(launchAngle)) - (.5f * grav * val * val), angle.z * charge * val) + character.transform.position;
        }

        return arcArray;
    }
}


