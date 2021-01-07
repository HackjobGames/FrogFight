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
    [SerializeField]
    private Cinemachine.CinemachineFreeLook cam;
    [SerializeField]
    private Cinemachine.CinemachineFreeLook zoom_cam;
    [SerializeField]
    private GameObject crosshair;

    private float tongue_distance;
    private Vector3 mouthDiff;

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
            if(Physics.Raycast(mouth.position, (camera_hit.point- mouth.position).normalized, out player_hit, max_tongue_distance, is_grappleable)){
                hit_location = new GameObject();
                hit_location.transform.position = player_hit.point;
                hit_location.transform.parent = player_hit.transform;
                if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("MoveableObject")){
                    joint = player_hit.transform.gameObject.AddComponent<SpringJoint>();
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = player.position;
                } else if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")){
                    joint = player.gameObject.AddComponent<SpringJoint>();
                    joint.autoConfigureConnectedAnchor = false;
                    joint.connectedAnchor = hit_location.transform.position;
                }
                tongue_distance = Vector3.Distance(player.position, grapple_point);
                joint.minDistance = 0;
                joint.minDistance = tongue_distance;

                joint.spring = 999f;
                joint.damper = 0f;
                joint.massScale = 4.5f;

                tongue.positionCount = 2;

                return true;
            }
            return false;
        } else {
            return false;
        }

    }

    public void StopGrapple(){
        tongue.positionCount = 0;
        Destroy(joint);
        Destroy(hit_location);
    }

    public void RetractTongue() {
      joint.minDistance -= .1f;
    }

    public void UpdateTonguePositions(){
        tongue.positionCount = 2;
        Vector3 diff = hit_location.transform.position - mouth.position;
        tongue.SetPosition(0,mouth.position);
        tongue.SetPosition(1,hit_location.transform.position);
        
    }

    public void activate_main_camera(){
        cam.enabled = true;
        zoom_cam.enabled = false;
        crosshair.SetActive(false);
    }

    public void activate_zoom_camera(){
        cam.enabled = false;
        zoom_cam.enabled = true;
        crosshair.SetActive(true);
    }
    private void Start()
    {
        mouthDiff = player.transform.position - mouth.position;
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
