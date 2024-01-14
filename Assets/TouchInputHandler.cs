using UnityEngine;

public class TouchInputHandler : MonoBehaviour
{
    private float rotationSpeed = 5f;
    private float scaleSpeed = 0.01f;

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 1)
        {
            // Rotation with one touch
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotationX = touch.deltaPosition.y * rotationSpeed * Time.deltaTime;
                float rotationY = -touch.deltaPosition.x * rotationSpeed * Time.deltaTime;

                transform.Rotate(Vector3.right, rotationX, Space.World);
                transform.Rotate(Vector3.up, rotationY, Space.World);
            }
        }
        else if (Input.touchCount == 2)
        {
            // Scaling with two touches
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Scale the object
            Vector3 newScale = transform.localScale + Vector3.one * deltaMagnitudeDiff * scaleSpeed;
            newScale = Vector3.Max(newScale, Vector3.one * 0.1f); // Minimum scale
            newScale = Vector3.Min(newScale, Vector3.one * 5f);  // Maximum scale
            transform.localScale = newScale;
        }
    }
}