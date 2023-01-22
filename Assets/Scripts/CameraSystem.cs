using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraSystem : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] bool useEdgeScrolling=false;
    [SerializeField] float fieldOfViewMax = 20;
    [SerializeField] float fieldOfViewMin = 5;
    [SerializeField] float followOffsetMin = 50;
    [SerializeField] float followOffsetMax = 200;
    [SerializeField] float followOffsetMinY = 200;
    [SerializeField] float followOffsetMaxY = 1100;
    float targetFieldOfView=50;
    Vector3 followOffset;

    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }
    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
        if (useEdgeScrolling)
        {
            HandleCameraMovementEdgeScrolling();
        }
        HandleCameraRotation();
        //HandleCameraZoom_FieldOfView();
        //HandleCameraZoom_MoveForward();
        HandleCameraMovement_LowerY();
    }

    private void HandleCameraMovement()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;


        


        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveSpeed * Time.deltaTime * moveDir;

    }


    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;

        
        int edgeScrollSize = 20;
        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;

        

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveSpeed * Time.deltaTime * moveDir;
    }

    private void HandleCameraRotation()
    {
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

        float rotateSpeed = 100f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraZoom_FieldOfView()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 1;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 1;
        }
        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);
        float zoomSpeed = 10f;
        cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime* zoomSpeed);
        
    }

    private void HandleCameraZoom_MoveForward()
    {
        Vector3 zoomDir = followOffset.normalized;
        float zoomAmount = 3f;
        if (Input.mouseScrollDelta.y>0)
        {
            followOffset -= zoomDir*zoomAmount; 
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset += zoomDir*zoomAmount;
        }

        if (followOffset.magnitude < followOffsetMin)
        {
            followOffset = zoomDir * followOffsetMin;
        }

        if (followOffset.magnitude > followOffsetMax)
        {
            followOffset = zoomDir * followOffsetMax;
        }

        float zoomSpeed = 10f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime*zoomSpeed);
        
    }


    private void HandleCameraMovement_LowerY()
    {
        float zoomAmount = 15f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -=  zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y +=  zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);

        float zoomSpeed = 10f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);

    }
}
