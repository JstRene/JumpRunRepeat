using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private float platformSpeed;

    [SerializeField]
    private Vector3 start;

    [SerializeField]
    private Vector3 end;

    private Vector3 velocity;

    void FixedUpdate()
    {
        float pingPong = Mathf.PingPong(Time.fixedTime * this.platformSpeed, 1.0f);
        var newPosition = Vector3.Lerp(this.start, this.end, pingPong);

        this.velocity = (newPosition - this.transform.localPosition) / Time.fixedDeltaTime;
        this.transform.localPosition = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return this.velocity;
    }
}
