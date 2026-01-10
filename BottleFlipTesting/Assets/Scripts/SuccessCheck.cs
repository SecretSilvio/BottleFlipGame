using UnityEngine;

public class SuccessCheck : MonoBehaviour
{
    public bool isGrounded = true;
    public bool previousGrounded = true;
    public float height;
    public float groundBuffer = 0.1f;
    public float landingCooldown = 1f;
    public float timer = 0f;
    public LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        // if landing cooldown is active, just count it down and return
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        // check if grounded now
        isGrounded = isGroundedCheck();
        // if previously grounded and still grounded, do nothing
        if (isGrounded && isGrounded == previousGrounded)
        {
            previousGrounded = isGrounded;
            return;
        }
        // if previously grounded and now not grounded, it means you have just lifted off
        else if (!isGrounded && isGrounded != previousGrounded)
        {
            previousGrounded = isGrounded;
            return;
        }
        // if previously not grounded and now grounded, it means you have just landed, start the landing cooldown to not land multiple times and set a flag that you won
        else if (isGrounded && isGrounded != previousGrounded)
        {
            previousGrounded = isGrounded;
            timer = landingCooldown;
            // you win!
            Debug.Log("You Landed Successfully!");
            return;
        }
        // if previously not grounded and still not grounded, do nothing
        else if (!isGrounded && isGrounded == previousGrounded)
        {
            previousGrounded = isGrounded;
            return;
        }
    }

    public bool isGroundedCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, height + groundBuffer, groundLayer))
        {
            return true;
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.up, out hit, height + groundBuffer, groundLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (height + groundBuffer));
        Gizmos.DrawLine(transform.position, transform.position + transform.up * (height + groundBuffer));
    }
}
