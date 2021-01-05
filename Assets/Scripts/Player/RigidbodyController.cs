using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyController : MonoBehaviour
{

    public CharacterController controller;
    public Transform camera;

    [Header("Physics")]
    public float characterSpeed;
    public float jumpSpeed;
    public float gravityScale;
    public LayerMask discludePlayer;
    public float distanceToGround;
    public float radius;


    [Header("Temps")]
    public Vector3 velocity;
    public bool grounded;
    public float verticalForce;

    private CapsuleCollider capCol;

    private void Awake()
    {
        capCol = this.GetComponent<CapsuleCollider>();
    }

    private Vector3 GainInput()
    {
        bool hasGround=false; RaycastHit groundInfo = new RaycastHit() ;

        (hasGround, groundInfo) = Surface();

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = Vector3.ClampMagnitude(input, 1) * characterSpeed;


        transform.eulerAngles = new Vector3(transform.eulerAngles.x, camera.transform.eulerAngles.y, transform.eulerAngles.z);

        input = transform.TransformDirection(input);

        input.y = 0;

        if (hasGround)
            input = Vector3.ProjectOnPlane(input, groundInfo.normal);

        input.y = Mathf.Clamp(input.y, -100, 0);

        

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            verticalForce += jumpSpeed;
        }

        return input;
    }

    (bool, RaycastHit) Surface ()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(transform.position, Vector3.down),out hitInfo, Mathf.Infinity, discludePlayer))
        {
            if (Vector3.Angle(hitInfo.normal, Vector3.up) > controller.slopeLimit)
                return (false, hitInfo);

            return (true, hitInfo);
        }
        else
        {
            return (false, hitInfo);
        }
    }

    void Gravity()
    {
        if (verticalForce > Physics.gravity.y * gravityScale && !grounded)
        {
            verticalForce += Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }

        if (grounded && verticalForce < 0f)
        {
            verticalForce = 0f;
        };

    }

    bool wasGrounded = false;
    public Vector3 currentVelocity;
    Vector3 lastPos;
    Vector3 lastVelocity;

    private void Update()
    {
        

        Vector3 input = GainInput();
        velocity = input * Time.deltaTime;
        velocity.y += verticalForce * Time.deltaTime;
        lastVelocity = velocity;

        controller.Move(velocity);

        

        wasGrounded = grounded;
        currentVelocity = (transform.position  - lastPos) / Time.deltaTime;
        lastPos = transform.position;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + Vector3.down * distanceToGround, radius);
    }

    void WallSlipCheck(Vector3 v)
    {
        // || (Mathf.Abs(input.magnitude*characterSpeed)>0.1f && velocity.magnitude<=0.05f) OTher Check for Stuckkledd
        if ((currentVelocity.y == 0 && verticalForce != 0))
        {
            Vector3 added = v;

            //Check all nearby colliders (except self)
            Collider[] c = Physics.OverlapSphere(transform.position, 20, discludePlayer);

            //Custom Collision Implementation
            foreach (Collider col in c)
            {
                if (col.isTrigger) continue;
                Vector3 penDir = new Vector3();
                float penDist = 0f;

                for (int i = 0; i < 2; i++)
                {
                    bool d = Physics.ComputePenetration(col, col.transform.position, col.transform.rotation, capCol, transform.position + added, transform.rotation, out penDir, out penDist);


                    if (d == false) continue;

                    transform.position += -penDir.normalized * penDist;

                }

            }

        }
    }

    
    public bool isGrounded()
    {
        Vector3 p1 = transform.position;
        float distanceToObstacle = 0;
        RaycastHit hit;
        // Cast a sphere wrapping character controller 0.1 meter down to check if it hits anything

        if (Physics.SphereCast(p1, radius, Vector3.down, out hit))
        {

            distanceToObstacle = hit.distance;
            if (distanceToObstacle < distanceToGround)
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit)
                    return false;

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void FixedUpdate()
    {
        grounded = isGrounded();
        Gravity();
        WallSlipCheck(lastVelocity);
    }

}
