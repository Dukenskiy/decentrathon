using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider[] wheelColliders = new WheelCollider[4];
    public Transform[] wheelMeshes = new Transform[4];
    public float maxTorque = 200f;
    public float maxBrakeTorque = 500f;
    public float maxSteerAngle = 30f;
    public float brakeForce = 500f;

    public Rigidbody rb;

    private void Start()
    {
    }

    private void Update()
    {
        updateWheelPositions();
    }

    private void FixedUpdate()
    {
        handleMotor();
        handleSteering();
    }

    protected virtual void handleMotor()
    {
        float motorInput = Input.GetAxis("Vertical") * maxTorque;
        float brakeInput = Input.GetKey(KeyCode.Space) ? brakeForce : 0f;

        
        wheelColliders[2].motorTorque = motorInput;
        wheelColliders[3].motorTorque = motorInput;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].brakeTorque = brakeInput;
        }
    }

    protected virtual void handleSteering()
    {
        float steerInput = Input.GetAxis("Horizontal") * maxSteerAngle;

        wheelColliders[0].steerAngle = steerInput;
        wheelColliders[1].steerAngle = steerInput;
    }

    protected void updateWheelPositions()
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
}
