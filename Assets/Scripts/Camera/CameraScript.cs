using TMPro;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] TMP_InputField movementInput;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float zoomSpeed;

    private float horizontalInput;
    private float verticalInput;
    private float scrollInput;

    public void SetSpeed()
    {
        if (movementInput.text != null)
        {
            movementSpeed = float.Parse(movementInput.text);
        }
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(horizontalInput * movementSpeed, verticalInput * movementSpeed, 0);
        mainCamera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
    }
}
