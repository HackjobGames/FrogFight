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
    public float max_jump_force = 15f;
    [SerializeField]
    public float jump_charge_speed = 10f;

    [SerializeField]
    private float min_gravity = -7f;
    [SerializeField]
    private float max_gravity = -20f;
    [SerializeField]
    private float default_gravity = -9.81f;
    [SerializeField]
    private float gravity_acc = .05f;
    private float cur_gravity;

    public Collision collision;
   [SerializeField]
    private LayerMask is_grappleable;

    public Rigidbody rigid_body { get; private set; }
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
    private Cinemachine.CinemachineFreeLook cam;
    [SerializeField]
    private Cinemachine.CinemachineFreeLook zoom_cam;
    [SerializeField]
    private GameObject crosshair;

    private RaycastHit camera_hit;
    private RaycastHit player_hit;

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
    public Timer ground_slide_timer; 



    public void Move(float speed_modifier, Vector3 direction){
        if(Mathf.Abs(rigid_body.velocity.magnitude) > max_speed) {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, dec_speed * Time.deltaTime);
        } else {
            rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, direction * speed * speed_modifier, acc_speed * Time.deltaTime);
        }
    }

    public void Vector(float speed_modifier, Vector3 direction){
        var vector = direction * speed * speed_modifier;
        rigid_body.AddForce(Vector3.up * cur_gravity + vector,ForceMode.Acceleration);
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
            transform.rotation = Quaternion.Euler(0f, angle, 0f); 
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector(speed_modifier, moveDir.normalized);
        } else {
            Vector(1f, Vector3.zero);
        }
    }
    public void Stop(){
        rigid_body.velocity = Vector3.MoveTowards(rigid_body.velocity, Vector3.zero, stop_speed * Time.deltaTime);
    }

    public void ApplyImpulse(Vector3 force){
        rigid_body.AddForce(force, ForceMode.Impulse);
    }
    public bool StartGrapple(){
        if(Physics.Raycast(main_camera.position, main_camera.forward, out camera_hit, max_tongue_distance, is_grappleable) &&
          Physics.Raycast(mouth.position, (camera_hit.point- mouth.position).normalized, out player_hit, max_tongue_distance, is_grappleable)) {
            hit_location = new GameObject();
            hit_location.transform.position = player_hit.point;
            hit_location.transform.parent = player_hit.transform;
            initial_tongue_distance = Vector3.Distance(player.position, player_hit.transform.position);
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

    public void EnableTongue(){
        if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("MoveableObject")){
            joint = player_hit.transform.gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = player.position;
        } else if(player_hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground")){
            joint = hit_location.gameObject.AddComponent<ConfigurableJoint>();
            hit_location.GetComponent<Rigidbody>().isKinematic = true;

            player_pivot_location = new GameObject();
            player_pivot_location.transform.position = mouth.position;
            player_joint = player_pivot_location.AddComponent<ConfigurableJoint>();
            player_joint.connectedBody = rigid_body;
            player_joint.anchor = new Vector3(0, 1, 0);
            player_joint.xMotion = ConfigurableJointMotion.Locked;
            player_joint.yMotion = ConfigurableJointMotion.Locked;
            player_joint.zMotion = ConfigurableJointMotion.Locked;

            joint.connectedBody = player_pivot_location.GetComponent<Rigidbody>();
            joint.axis = new Vector3(1, 1, 1);
            joint.connectedAnchor = hit_location.transform.position;
            joint.anchor = new Vector3(0, -1, 0);
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            
        } else {
            return;
        }

        float dist_from_point = Vector3.Distance(mouth.position, player_hit.transform.position);
    }
    public void SetGroundVelocity(){
        previous_velocity = rigid_body.velocity;
    }
    public float GroundSlideStart(){
        rigid_body.velocity = new Vector3(rigid_body.velocity.x, 0f, rigid_body.velocity.z);
        slide_direction = new Vector3(previous_velocity.x, 0f, previous_velocity.z);
        if(slide_direction.magnitude < .5){
            Vector3 dir_to_rope = hit_location.transform.position - mouth.position;
            slide_direction = new Vector3(dir_to_rope.x, 0f, dir_to_rope.z).normalized;
        } else {
             slide_direction = slide_direction.normalized;
        }
        return cur_tongue_distance;
    }

    public float GroundSlide(){
        Vector3 ground_velocity;
        if(previous_velocity.magnitude < slide_speed){
            ground_velocity = Vector3.Lerp(rigid_body.velocity, slide_direction * slide_speed,.05f);
        } else {
           ground_velocity = previous_velocity.magnitude * slide_direction;
        }
        rigid_body.velocity = ground_velocity;

        return cur_tongue_distance;
    }

    public void GroundSlideEnd(){
        rigid_body.velocity = new Vector3(rigid_body.velocity.x, rigid_body.velocity.magnitude, rigid_body.velocity.z);
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

    public void ActivateMainCamera(){
        cam.enabled = true;
        zoom_cam.enabled = false;
        crosshair.SetActive(false);
    }

    public void ActivateZoomCamera(){
        cam.enabled = false;
        zoom_cam.enabled = true;
        crosshair.SetActive(true);
    }

    public void IncreaseGravity(){
        cur_gravity = Mathf.Lerp(cur_gravity, max_gravity, gravity_acc);
    }

    public void DecreaseGravity(){
        cur_gravity = Mathf.Lerp(cur_gravity, min_gravity, gravity_acc);
    }

    public void ResetGravity(){
        cur_gravity = default_gravity;
    }

    public float getGravity() {
        return cur_gravity;
    }

    private void Start()
    {
      if(this.isLocalPlayer) {
        main_camera.gameObject.SetActive(true);
        cam.gameObject.SetActive(true);
        zoom_cam.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;

      }
      rigid_body = GetComponent<Rigidbody>();
      collision = GetComponent<Collision>();
      jump_arc.enabled = false;
      cur_gravity = default_gravity;
      head_initial_pos = head.rotation;
      head.rotation = head_initial_pos;

      movement_machine = new StateMachine();
      standing_state = new  StandingState(this,movement_machine);
      jumping_state = new JumpingState(this,movement_machine);
      falling_state = new FallingState(this,movement_machine);
      swinging_state = new SwingingState(this,movement_machine);
      ground_sliding_state = new GroundSlidingState(this,movement_machine);

      movement_machine.Initialize(standing_state);

      action_machine = new StateMachine();

      idle_state = new IdleState(this, action_machine);
      aiming_state = new AimingState(this, action_machine);
      grappling_state = new GrappleState(this, action_machine);

      action_machine.Initialize(idle_state);

      GameObject ground_timer_object = new GameObject();
      ground_slide_timer = ground_timer_object.AddComponent<Timer>();
      ground_slide_timer.SetTimer(1f);
    }


    private void Update()
    {
      if(this.isLocalPlayer) {
        movement_machine.cur_state.HandleInput();



        action_machine.cur_state.HandleInput();


      }
      action_machine.cur_state.LogicUpdate();
      movement_machine.cur_state.LogicUpdate();
    }

    private void FixedUpdate() 
    {
      movement_machine.cur_state.PhysicsUpdate();
      action_machine.cur_state.PhysicsUpdate();
    }
}
