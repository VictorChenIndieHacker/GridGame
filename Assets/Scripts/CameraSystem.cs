using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraSystem : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] bool useEdgeScrolling=false;
    [SerializeField] float targetOrthoMin=5;
    [SerializeField] float targetOrthoMax=16.6f;
    float targetOrthoSize;
    private void Awake()
    {
        targetOrthoSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
    }
    // Update is called once per frame
    void Update()
    {
        HandleCameraMovement();
        if (useEdgeScrolling)
        {
            HandleCameraMovementEdgeScrolling();
        }
        HandleCameraZoom();


    }

    private void HandleCameraMovement()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDir.y = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.y = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;




        inputDir.Normalize();
        //Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveSpeed * Time.deltaTime * inputDir;

    }


    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;

        
        int edgeScrollSize = 20;
        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.y = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.y = +1f;


        inputDir.Normalize();
        //Vector3 moveDir = transform.forward * inputDir.y + transform.right * inputDir.x;
        float moveSpeed = 50f;
        transform.position += moveSpeed * Time.deltaTime * inputDir;
    }

    

    private void HandleCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetOrthoSize -= 5;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            targetOrthoSize += 5;
        }

        targetOrthoSize = Mathf.Clamp(targetOrthoSize, targetOrthoMin, targetOrthoMax);

        float zoomSpeed = 10f;
        cinemachineVirtualCamera.m_Lens.OrthographicSize =
            Mathf.Lerp(cinemachineVirtualCamera.m_Lens.OrthographicSize, targetOrthoSize, Time.deltaTime * zoomSpeed);

    }
}
