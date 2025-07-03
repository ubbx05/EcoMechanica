using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
            moveY += 1f;
        if (Input.GetKey(KeyCode.S))
            moveY -= 1f;
        if (Input.GetKey(KeyCode.A))
            moveX -= 1f;
        if (Input.GetKey(KeyCode.D))
            moveX += 1f;

        Vector3 move = new Vector3(moveX, moveY, 0f).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;
    }
}
