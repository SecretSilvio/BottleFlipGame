using UnityEngine;

public class SuccessCheck : MonoBehaviour
{
    public int StreakCount = 0;

    private bool previousGrounded = true;
    public bool isGrounded { get; private set; } = true;
    public float height = 1.2f;
    public float width = 0.5f;
    public float groundBuffer = 0.1f;
    public float landingCooldown = 1f;
    public float timer = 0f;
    public LayerMask groundLayer;
    public ParticleSystem uprightStarPS;
    public ParticleSystem upsidedownStarPS;

    public AudioSource landingSound;
    private DotProductWater dpw;
    private TrailRenderer trailRenderer;

    private void Start()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        if (trailRenderer != null)
        {
            trailRenderer.enabled = false;
        }
        dpw = GetComponentInChildren<DotProductWater>();
    }

    // Update is called once per frame
    void Update()
    {
        // if landing cooldown is active, just count it down and return
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }
        if (isGrounded)
        {
            trailRenderer.enabled = false;
        }
        else
        {
            trailRenderer.enabled = true;
        }

            // check if grounded now
            var groundedCheck = isGroundedCheck();
        isGrounded = groundedCheck.isgrounded;
        // if previously grounded and still grounded, do nothing
        if (isGrounded && isGrounded == previousGrounded)
        {
            // do nothing
        }
        // if previously grounded and now not grounded, it means you have just lifted off
        else if (!isGrounded && isGrounded != previousGrounded)
        {
            // lifted off
        }
        // if previously not grounded and now grounded, it means you have just landed, start the landing cooldown to not land multiple times and set a flag that you won
        // NEED DEBUG TO NOT PLAY WIN WHEN RESETTING BOTTLE
        else if (isGrounded && isGrounded != previousGrounded)
        {
            timer = landingCooldown;
            if (groundedCheck.onSide)
            {
                // landed on side, do not count as success
                Debug.Log("Landed on Side!");
                StreakCount = 0;
            }
            else
            {
                // you win!
                GameEvents.OnFlipSuccess?.Invoke();
                OnSuccess(groundedCheck.upright);
                Debug.Log("You Landed Successfully!");
            }

        }
        // if previously not grounded and still not grounded, do nothing
        else if (!isGrounded && isGrounded == previousGrounded)
        {
            // do nothing
        }

        previousGrounded = isGrounded;
    }

    public (bool isgrounded, bool upright, bool onSide) isGroundedCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, height + groundBuffer, groundLayer))
        {
            return (true, true, false);
        }
        else
        {
            if (Physics.Raycast(transform.position, transform.up, out hit, height + groundBuffer, groundLayer))
            {
                return (true, false, false);
            }
            else
            {
                //check if grounded on the side
                if (Physics.Raycast(transform.position, Vector3.down, out hit, width + groundBuffer, groundLayer))
                {
                    return (true, false, true);
                }
                else
                {
                    return (false, false, false);
                }
            }
        }
    }

    public void OnSuccess(bool upright)
    {
        if (landingSound != null)
        {
        landingSound.pitch = Random.Range(0.95f, 1.05f);
        landingSound.Play();
        }
        StreakCount += 1;
        StartCoroutine(dpw.StickTheLanding());
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

    public void Reset()
    {
        if (isGrounded && !isGroundedCheck().onSide)
        {
            // reset called when upright, do nothing to streak
        }
        else
        {
            // reset streak if pressed mid throw or on side
            StreakCount = 0;
        }

        previousGrounded = true;
        isGrounded = true;
        timer = landingCooldown;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * (height + groundBuffer));
        Gizmos.DrawLine(transform.position, transform.position + transform.up * (height + groundBuffer));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * (width + groundBuffer));
    }
}
