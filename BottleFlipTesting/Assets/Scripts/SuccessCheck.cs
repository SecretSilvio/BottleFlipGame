using UnityEngine;

public class SuccessCheck : MonoBehaviour
{
    public bool previousGrounded = true;
    public float height;
    public float groundBuffer = 0.1f;
    public float landingCooldown = 1f;
    public float timer = 0f;
    public LayerMask groundLayer;
    public ParticleSystem uprightStarPS;
    public ParticleSystem upsidedownStarPS;

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
        var groundedCheck = isGroundedCheck();
        // if previously grounded and still grounded, do nothing
        if (groundedCheck.isGrounded && groundedCheck.isGrounded == previousGrounded)
        {
            previousGrounded = groundedCheck.isGrounded;
            return;
        }
        // if previously grounded and now not grounded, it means you have just lifted off
        else if (!groundedCheck.isGrounded && groundedCheck.isGrounded != previousGrounded)
        {
            previousGrounded = groundedCheck.isGrounded;
            return;
        }
        // if previously not grounded and now grounded, it means you have just landed, start the landing cooldown to not land multiple times and set a flag that you won
        // NEED DEBUG TO NOT PLAY WIN WHEN RESETTING BOTTLE
        else if (groundedCheck.isGrounded && groundedCheck.isGrounded != previousGrounded)
        {
            previousGrounded = groundedCheck.isGrounded;
            timer = landingCooldown;
            // you win!
            GameEvents.OnFlipSuccess?.Invoke();
            OnSuccess(groundedCheck.upright);
            Debug.Log("You Landed Successfully!");
            return;
        }
        // if previously not grounded and still not grounded, do nothing
        else if (!groundedCheck.isGrounded && groundedCheck.isGrounded == previousGrounded)
        {
            previousGrounded = groundedCheck.isGrounded;
            return;
        }
    }

    public (bool isGrounded, bool upright) isGroundedCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, height + groundBuffer, groundLayer))
        {
            return (true, true);
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.up, out hit, height + groundBuffer, groundLayer))
            {
                return (true, false);
            }
            else
            {
                return (false, false);
            }
        }
    }

    public void OnSuccess(bool upright)
    {
        // Additional logic for success can be added here
        if (upright)
        {
            Debug.Log("Landed Upright!");
            uprightStarPS.Play();

        }
        else
        {
            Debug.Log("Landed Upside Down!");
            upsidedownStarPS.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (height + groundBuffer));
        Gizmos.DrawLine(transform.position, transform.position + transform.up * (height + groundBuffer));
    }
}
