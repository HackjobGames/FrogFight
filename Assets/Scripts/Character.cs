using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public StateMachine movement_machine;
    public State running_state;
    public State jumping_state;
    public State swinging_state;
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
    private float max_tongue_distance = 100f;
    public float jump_force = 11f;
    public Collision collision;
    [SerializeField]
    private LineRenderer tongue;
    private Vector3 grapple_point;
     [SerializeField]
    private LayerMask is_grappleable;

    private Rigidbody rigid_body;
    private SpringJoint joint;

      [SerializeField]
    private Transform mouth;
    [SerializeField]
    private Transform main_camera;
    [SerializeField]
    private Transform player;
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

    public void StartGrapple(){
        RaycastHit hit;

        if(Physics.Raycast(main_camera.position, main_camera.forward, out hit, max_tongue_distance, is_grappleable)){
            grapple_point = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapple_point;

            float dist_from_point = Vector3.Distance(player.position, grapple_point);
            joint.maxDistance = dist_from_point * .8f;
            joint.minDistance = dist_from_point * .3f;

            joint.spring = 10f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            tongue.positionCount = 2;
        }

    }

    public void StopGrapple(){
        tongue.positionCount = 0;
        Destroy(joint);
    }

    public void UpdateTonguePositions(){
        tongue.SetPosition(0,mouth.position);
        tongue.SetPosition(1,grapple_point);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rigid_body = GetComponent<Rigidbody>();
        collision = GetComponent<Collision>();

        movement_machine = new StateMachine();
        running_state = new  RunningState(this,movement_machine);
        jumping_state = new JumpingState(this,movement_machine);
        falling_state = new FallingState(this,movement_machine);
        swinging_state = new SwingingState(this,movement_machine);

        movement_machine.Initialize(running_state);
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
