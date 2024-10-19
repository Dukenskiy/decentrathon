using UnityEngine;
using System.Collections.Generic;




public class Car_ai : MonoBehaviour
{

    public enum Level
    {
        first,
        second,
        third
    }
    public Level level;
    public LayerMask raycastLayers;

    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] wheelMeshes = new Transform[4];
    public float maxTorque = 120f;
    public float maxBrakeTorque = 500f;
    public float maxSteerAngle = 30f;
    public float brakeForce = 500f;
    public Vector3 centerOfMass = new Vector3(0, 0.45f, 0);

    private Rigidbody rigid_b;
    private List<Transform> way_points;
    private int current_waypoint_index = 0;

    private void Start()
    {
        maxTorque += (int)level * 15f;
        rigid_b = GetComponent<Rigidbody>();
        rigid_b.centerOfMass = centerOfMass;

        Transform parentTransform = transform.parent;

        if (parentTransform != null)
        {
            Road parentScript = parentTransform.GetComponent<Road>();

            if (parentScript != null)
            {
                way_points = parentScript.way_points;
            }
        }
    }

    private void Update()
    {
        Vector3 point = get_point();
        drive_to_point(point);
        sensors();
    }

    private void drive_to_point(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0;
        if ((direction.magnitude < 20.0f) && (GetComponent<Rigidbody>().linearVelocity.magnitude > 40))
        {

            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = brakeForce;
            }

        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                wheelColliders[i].brakeTorque = 0;
            }
            handleSteering();
        }
        if (direction.magnitude < 3.0f)
        {
            current_waypoint_index = (current_waypoint_index + 1) % way_points.Count;

        }
        handleMotor();
        updateWheelPositions();
    }

    private Vector3 get_point()
    {
        if (way_points.Count == 0) {; }
        Vector3 point = way_points[current_waypoint_index].position;
        return point;
    }

    private void handleMotor()
    {

        wheelColliders[2].motorTorque = maxTorque;
        wheelColliders[3].motorTorque = maxTorque;
    }

    private void handleSteering()
    {
        Vector3 steer_vector = transform.InverseTransformPoint(new Vector3(way_points[current_waypoint_index].position.x, transform.position.y, way_points[current_waypoint_index].position.z));
        float new_steer = maxSteerAngle * (steer_vector.x / steer_vector.magnitude);
        wheelColliders[0].steerAngle = new_steer;
        wheelColliders[1].steerAngle = new_steer;
    }

    private void updateWheelPositions()
    {
        for (int i = 0; i < wheelColliders.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;
            wheelColliders[i].GetWorldPose(out position, out rotation);
            wheelMeshes[i].position = position;
            wheelMeshes[i].rotation = rotation;
        }
    }

    private void sensors()
    {
        RaycastHit hit;
        Vector3 forward = transform.TransformDirection(Vector3.forward); 

        Vector3 frontLeft = transform.position + transform.right * -0.5f + transform.forward * 1f;
        Vector3 leftDirection = Quaternion.AngleAxis(-20, Vector3.up) * transform.forward;

        Vector3 frontRight = transform.position + transform.right * 0.5f + transform.forward * 1f;
        Vector3 rightDirection = Quaternion.AngleAxis(20, Vector3.up) * transform.forward;


        if (CheckRaycast(transform.position, forward, 2f))
        {
            wheelColliders[2].motorTorque = -maxTorque;
            wheelColliders[3].motorTorque = -maxTorque;
        }
        if (CheckRaycast(frontLeft, leftDirection))
        {
            wheelColliders[0].steerAngle += 5f;
            wheelColliders[1].steerAngle += 5f;
        }
        if (CheckRaycast(frontRight, rightDirection))
        {
            wheelColliders[0].steerAngle += -5f;
            wheelColliders[1].steerAngle += -5f;
        }

    }

    private bool CheckRaycast(Vector3 origin, Vector3 direction, float r_size = 10f)
    {
        RaycastHit hit;

        if (Physics.Raycast(origin, direction, out hit, r_size, raycastLayers))
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(origin, direction * r_size, Color.green); 
            return false;
        }

    }

}