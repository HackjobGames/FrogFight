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

    public float GroundSlideStart(Vector3 prev_vel){
        previous_velocity = prev_vel;
        return cur_tongue_distance;
    }

    public float GroundSlide(){
        Vector3 ground_velocity = previous_velocity.magnitude * new Vector3(previous_velocity.x, 0f, previous_velocity.z).normalized;
        rigid_body.velocity = ground_velocity;
        print(rigid_body.velocity);
        return cur_tongue_distance;
    }

    public void GroundSlideEnd(){
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
    }

    public void DisableTongue(bool grapple_engaged){
        if(grapple_engaged){
            Destroy(joint);
            Destroy(player_pivot_location);
        }
    }

    public void StopGrapple(bool grapple_engaged){
        var dir = (focal_point.position - head.position).normalized;
        head.rotation = Quaternion.LookRotation(dir);
        if(grapple_engaged){
            Destroy(joint);
            Destroy(player_pivot_location);
        }
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

      movement_machine.Initialize(standing_state);

      action_machine = new StateMachine();

      idle_state = new IdleState(this, action_machine);
      aiming_state = new AimingState(this, action_machine);
      grappling_state = new GrappleState(this, action_machine);

      action_machine.Initialize(idle_state);
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
