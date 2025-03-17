using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [Header("타겟 설정")]
    [SerializeField] Transform target;
    [SerializeField] Vector3 targetOffset = Vector3.zero;
    
    [Header("회전 설정")]
    [SerializeField] float rotationSpeed = 5.0f;
    [SerializeField] float minVerticalAngle = -80.0f;
    [SerializeField] float maxVerticalAngle = 80.0f;
    
    [Header("줌 설정")]
    [SerializeField] float zoomSpeed = 10.0f;
    [SerializeField] float minDistance = 5.0f;
    [SerializeField] float maxDistance = 30.0f;
    
    [Header("댐핑 설정")]
    [SerializeField] float movementDampening = 5.0f;
    [SerializeField] float rotationDampening = 5.0f;
    
    float currentDistance;
    float targetDistance;
    float currentXRotation;
    float currentYRotation;
    float targetXRotation;
    float targetYRotation;
    
    // 마우스 입력 감지
    bool isRotating = false;
    
    // 초기화
    void Start()
    {
        if (target == null)
        {
            GameObject targetObject = new GameObject("CameraTarget");
            targetObject.transform.position = Vector3.zero;
            target = targetObject.transform;
        }
        
        Vector3 angles = transform.eulerAngles;
        targetXRotation = currentXRotation = angles.y;
        targetYRotation = currentYRotation = angles.x;
 
        targetDistance = currentDistance = Vector3.Distance(transform.position, target.position);

        UpdateCamera();
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void LateUpdate()
    {
        UpdateCamera();
    }
    
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotating = true;
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            isRotating = false;
        }

        if (isRotating)
        {
            targetXRotation += Input.GetAxis("Mouse X") * rotationSpeed;
            targetYRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
            
            targetYRotation = Mathf.Clamp(targetYRotation, minVerticalAngle, maxVerticalAngle);
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            targetDistance -= scrollInput * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
    }
    
    void UpdateCamera()
    {
        currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, Time.deltaTime * rotationDampening);
        currentYRotation = Mathf.Lerp(currentYRotation, targetYRotation, Time.deltaTime * rotationDampening);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * movementDampening);
       
        Quaternion rotation = Quaternion.Euler(currentYRotation, currentXRotation, 0);        

        Vector3 targetPosition = target.position + targetOffset;
        Vector3 direction = rotation * Vector3.back;
        Vector3 desiredPosition = targetPosition + direction * currentDistance;        

        transform.position = desiredPosition;
        transform.LookAt(targetPosition);
    }    

    public void SetTarget(Transform newTarget, bool instant = false)
    {
        if (newTarget != null)
        {
            target = newTarget;
            
            if (instant)
            {
                UpdateCamera();
            }
        }
    }
    
    public void ResetCamera(float newDistance = 15f, float xRotation = 45f, float yRotation = 30f)
    {
        targetXRotation = currentXRotation = xRotation;
        targetYRotation = currentYRotation = yRotation;
        targetDistance = currentDistance = newDistance;
    }
}