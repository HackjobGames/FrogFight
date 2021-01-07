using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public StateMachine movement_machine;
    public State standing_state;
    public State jumping_state;
    public State falling_state;
    public State swinging_state;

    public StateMachine action_machine;    
    public State aiming_state;
    public State grappling_state;
    public State idle_state;
    public LineRenderer jump_arc;
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
    [SerializeField]
    private LineRenderer tongue;
    private Vector3 grapple_point;
   [SerializeField]
    private LayerMask is_grappleable;

    private Rigidbody rigid_body;
    private SpringJoint joint;
    [SerializeField]
    private float max_tongue_distance = 100f;
    [SerializeField]
    private Transform mouth;
    [SerializeField]
    private Transform main_camera;
    [SerializeField]
    private Transform player;
    private GameObject hit_location;
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
    public bool StartGrapple(){
        RaycastHit camera_hit;
        RaycastHit player_hit;
        if(Physics.Raycast(main_camera.position, main_camera.forward, out camera_hit, max_tongue_distance, is_grappleable)){
            hit_location = new GameObject();
            if(Physics.Raycast(mouth.position, (camera_hit.point- mouth.position).normalized, out player_hit, max_tongue_distance, is_grappleable)){
                if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("MoveableObject")){
                    hit_location.transform.position = player_hit.point;
                    hit_location.transform.parent = player_hit.transform;
                    joint = player_hit.transform.gameObject.AddComponent<SpringJoint>();
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = player.position;
                } else if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")){
                    hit_location.transform.position = player_hit.point;
                    hit_location.transform.parent = player_hit.transform;
                    joint = player.gameObject.AddComponent<SpringJoint>();
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = hit_location.transform.position;
                }
            }
            float dist_from_point = Vector3.Distance(player.position, grapple_point);
            joint.maxDistance = dist_from_point * .8f;
            joint.minDistance = dist_from_point * .3f;

            joint.spring = 10f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            tongue.positionCount = 2;
            return true;
        } else {
            return false;
        }

    }

    public void StopGrapple(){
        tongue.positionCount = 0;
        Destroy(joint);
        Destroy(hit_location);
    }

    public void UpdateTonguePositions(){
        tongue.positionCount = 2;
        tongue.SetPosition(0,mouth.position);
        tongue.SetPosition(1,hit_location.transform.position);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rigid_body = GetComponent<Rigidbody>();
        collision = GetComponent<Collision>();
        jump_arc.enabled = false;

        movement_machine = new StateMachine();
        standing_state = new  StandingState(this,movement_machine);
        jumping_state = new JumpingState(this,movement_machine);
        falling_state = new FallingState(this,movement_machine);
        swinging_state = new SwingingState(this,movement_machine);

        movement_machine.Initialize(standing_state);

        action_machine = new StateMachine();

        idle_state = new IdleState(this, action_machine);
        aiming_state = new AimingState(this, action_machine);
        grappling_state = new GrappleState(this, action_machine);

        action_machine.Initialize(idle_state);
    }


    private void Update()
    {
        movement_machine.cur_state.HandleInput();

        movement_machine.cur_state.LogicUpdate();

        action_machine.cur_state.HandleInput();

        action_machine.cur_state.LogicUpdate();
    }

    private void FixedUpdate() 
    {
        movement_machine.cur_state.PhysicsUpdate();

        action_machine.cur_state.PhysicsUpdate();

        rigid_body.AddForce(Vector3.up * gravity,ForceMode.Acceleration);
    }
}
