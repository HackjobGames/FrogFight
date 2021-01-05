using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public StateMachine movement_machine;
    public State standing_state;
    public State jumping_state;
    public State falling_state;
    [SerializeField]
    private float gravity = -9.81f;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float max_speed = 5f;
    [SerializeField]
    private float acc_speed = 11f;
    [SerializeField]
    private float dec_speed = 11f;
    [SerializeField]
    private float stop_speed = 11f;
    [SerializeField]
    public float max_jump_force = 15f;
    [SerializeField]
    public float jump_charge_speed = 10f;
    public Collision collision;

    private Rigidbody rigid_body;
    public void Move(float speed_modifier, Vector3 direction){
        if(Mathf.Abs(rigid_body.velocity.magnitude) > max_speed) {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, dec_speed * Time.deltaTime);
        } else {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, acc_speed * Time.deltaTime);
        }
    }

    public void Stop(){
        rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, Vector3.zero, stop_speed * Time.deltaTime);
    }

    public void ApplyImpulse(Vector3 force){
        rigid_body.AddForce(force, ForceMode.Impulse);
    }
    private void Start()
    {
        rigid_body = GetComponent<Rigidbody>();
        collision = GetComponent<Collision>();

        movement_machine = new StateMachine();
        standing_state = new  StandingState(this,movement_machine);
        jumping_state = new JumpingState(this,movement_machine);
        falling_state = new FallingState(this,movement_machine);

        movement_machine.Initialize(standing_state);
    }


    private void Update()
    {
        movement_machine.cur_state.HandleInput();

        movement_machine.cur_state.LogicUpdate();
    }

    private void FixedUpdate() {
        movement_machine.cur_state.PhysicsUpdate();

        rigid_body.AddForce(Vector3.up * gravity,ForceMode.Acceleration);
    }
}
