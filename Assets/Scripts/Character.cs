using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Character : NetworkBehaviour
{
    public StateMachine movement_machine;
    public State standing_state;
    public State jumping_state;
    public State falling_state;
    public State swinging_state;
    public State ground_sliding_state;

    public StateMachine action_machine;    
    public State aiming_state;
    public State grappling_state;
    public State idle_state;
    public LineRenderer jump_arc;
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
    public float max_jump_force = 30f;
    [SerializeField]
    public float jump_charge_speed = 1000f;

    private float gravity = -9.81f;

    public PlayerCollision collision;
   [SerializeField]
    private LayerMask is_grappleable;

    public Rigidbody rigid_body;
    private ConfigurableJoint joint;
    private ConfigurableJoint player_joint;
    [SerializeField]
    private float max_tongue_distance = 100000f;
    public float initial_tongue_distance { get; private set; }
    public float cur_tongue_distance { get; private set; }
    [SerializeField]
    private Transform mouth;
    [SerializeField]
    private Transform main_camera;
    [SerializeField]
    private Transform player;
    private GameObject hit_location;
    private GameObject player_pivot_location;
    [SerializeField]
    private GameObject crosshair;
    [SerializeField]
    private GameObject aim_marker_prefab;
    private GameObject aim_marker;
    private MeshRenderer aim_marker_mesh;
    private RaycastHit camera_hit;
    private RaycastHit tongue_hit;

    [SerializeField]
    private Transform head;
    [SerializeField]
    private Transform focal_point;
    private Quaternion head_initial_pos;
    private CableComponent cable_component;
    [SerializeField]
    private Material tongue_material;
    private Vector3 previous_velocity;
    [SerializeField]
    private float slide_speed = 25f;
    private Vector3 slide_direction;
    private float turnSpeed = .1f;

    public Transform look_at;

    private float max_tongue_strength = 1f;
    private float max_air_speed = 5f;
    private float tongue_dampen = 20f;
    
    public Transform frog;



    public void Move(float speed_modifier, Vector3 direction){
        if(Mathf.Abs(rigid_body.velocity.magnitude) > max_speed) {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, dec_speed * Time.deltaTime);
        } else {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, acc_speed * Time.deltaTime);
        }
    }

    public void Vector(float speed_modifier, Vector3 direction){
        var vector = direction * speed * speed_modifier;
        rigid_body.AddForce(Vector3.up * gravity + vector,ForceMode.Acceleration);
    }

    public void Fall(){
        float horizontal_input = Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");
        Vector3 input_vector = new Vector3(horizontal_input,0,vertical_input);
        float speed_modifier = input_vector.magnitude;
        Vector3 dir = input_vector.normalized;
        if(input_vector.magnitude >= 0.1f){
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + Camera.main.gameObject.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, .1f);
            // transform.rotation = Quaternion.Euler(0f, angle, 0f); 
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector(speed_modifier, moveDir.normalized);
        } else {
            Vector(1f, Vector3.zero);
        }
    }
    public void Stop(){
      if(action_machine.cur_state != grappling_state
        && (!Mathf.Approximately(rigid_body.velocity.x , 0) 
        || !Mathf.Approximately(rigid_body.velocity.z , 0))
        && collision.on_ground){
          rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, Vector3.zero, stop_speed * Time.deltaTime);
      }
    }

    public void ApplyImpulse(Vector3 force){
        rigid_body.AddForce(force, ForceMode.Impulse);
    }
    public bool StartGrapple(){
        if(Physics.Raycast(look_at.position, main_camera.forward, out camera_hit, max_tongue_distance, is_grappleable) &&
          Physics.Raycast(mouth.position, (camera_hit.point- mouth.position).normalized, out tongue_hit, max_tongue_distance, is_grappleable)) {
            hit_location = new GameObject();
            hit_location.transform.position = tongue_hit.point;
            hit_location.transform.parent = tongue_hit.transform;
            initial_tongue_distance = Vector3.Distance(player.position, tongue_hit.transform.position);
            cur_tongue_distance = initial_tongue_distance;

            hit_location.AddComponent<CableComponent>();
            cable_component = hit_location.GetComponent<CableComponent>();
            cable_component.endPoint = mouth;
            cable_component.cableMaterial = tongue_material;
            cable_component.cableLength = initial_tongue_distance;
            return true;
        } else {
            return false;
        }
    }

    public void DisableTongue(){
        Destroy(joint);
        Destroy(player_joint);
        Destroy(player_pivot_location);
    }

    public void StopGrapple(){
        var dir = (focal_point.position - head.position).normalized;
        head.rotation = Quaternion.LookRotation(dir);

        Destroy(joint);
        Destroy(player_joint);
        Destroy(player_pivot_location);
        Destroy(hit_location);
    }

    public void UpdateTonguePositions(){
        cur_tongue_distance = Vector3.Distance(mouth.position, hit_location.transform.position);

        var dir = (hit_location.transform.position - head.position).normalized;
        var rotation = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Slerp(head.rotation, rotation, Time.deltaTime * 2f);
        if(cable_component.line != null){
            cable_component.line.SetPosition(cable_component.segments, mouth.position);
            cable_component.cableLength = cur_tongue_distance;
        }
    }

    public bool ApplyTongueForce() {
      Vector3 dist = hit_location.transform.position - head.position;
      Vector3 force = (dist.magnitude > max_tongue_strength) ? dist * max_tongue_strength/dist.magnitude : dist;
      force *= tongue_dampen;
      rigid_body.AddForce(force, ForceMode.Impulse);
      return(max_tongue_distance >= dist.magnitude);
  }

    public float getGravity() {
        return gravity;
    }

    public void AimMarkerUpdate(){
      if(Physics.Raycast(main_camera.position, main_camera.forward, out camera_hit, max_tongue_distance, is_grappleable) &&
          Physics.Raycast(mouth.position, (camera_hit.point- mouth.position).normalized, out tongue_hit, max_tongue_distance, is_grappleable)) {
          if(!aim_marker_mesh.enabled){
            aim_marker_mesh.enabled = true;
          }
          
          aim_marker.transform.position = tongue_hit.point;
        } else if(aim_marker_mesh.enabled){
          aim_marker_mesh.enabled = false;
        }
    }

    private void Start()
    {
      if(this.isLocalPlayer) {
        main_camera.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
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
        rigid_body.isKinematic = false;
      }

      jump_arc = GetComponent<LineRenderer>();
      collision = GetComponent<PlayerCollision>();
      aim_marker = Instantiate(aim_marker_prefab) as GameObject;
      aim_marker_mesh = aim_marker.GetComponent<MeshRenderer>();
      jump_arc.enabled = false;
      aim_marker_mesh.enabled = false;
      head_initial_pos = head.rotation;
      head.rotation = head_initial_pos;
    }

    private void Update()
    {
      if(this.isLocalPlayer) {
        movement_machine.cur_state.HandleInput();
        action_machine.cur_state.HandleInput();
        action_machine.cur_state.LogicUpdate();
        movement_machine.cur_state.LogicUpdate();
      }
    }

    private void FixedUpdate() 
    {
      if (this.isLocalPlayer) {
        movement_machine.cur_state.PhysicsUpdate();
        action_machine.cur_state.PhysicsUpdate();
      }
    }
}
