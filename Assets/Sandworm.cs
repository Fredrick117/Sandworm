using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandworm : MonoBehaviour
{
    // Cooldowns and gameplay specific values
    [SerializeField]
    private float burrowTime = 1f;

    [SerializeField]
    private float burrowCooldown = 5.0f;

    [SerializeField]
    private float changeDirectionCooldown = 0.1f;

    [SerializeField]
    private int maxSegmentsPerFood = 3;

    // Worm state
    [HideInInspector]
    public bool isDead = false;

    // Burrowing
    [HideInInspector]
    public bool isBurrowed = false;

    private bool isBurrowOnCooldown = false;

    private Coroutine burrowCoroutine;

    // Directional movement
    private bool canChangeDirection = true;

    private Coroutine directionCoroutine;

    private Vector2 direction = Vector2.left;

    // Sandworm segments
    [SerializeField]
    private Transform segmentPrefab;

    private List<Transform> segments;

    // Misc
    private GameManager manager;

    private Transform sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetChild(0);
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        segments = new List<Transform>();
        segments.Add(this.transform);

        // Start the game with three segments
        AddSegment();
        AddSegment();
        AddSegment();
    }

    // Update is called once per frame
    void Update()
    {
        // Burrowing
        if (Input.GetKeyDown(KeyCode.Space) && !isBurrowed && !isBurrowOnCooldown && !isDead)
        {
            if (burrowCoroutine != null)
            {
                StopCoroutine(Burrow());
            }

            burrowCoroutine = StartCoroutine(Burrow());
        }

        // Directional movement
        if (!isBurrowed && canChangeDirection && !isDead)
        {
            // Up
            if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down && direction != Vector2.up)
            {
                direction = Vector2.up;
                if (directionCoroutine != null)
                { 
                    StopCoroutine(ChangeDirectionCooldown());
                }

                directionCoroutine = StartCoroutine(ChangeDirectionCooldown());
                sprite.transform.eulerAngles = new Vector3(0, 0, -90);
            }
            // Down
            else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up && direction != Vector2.down)
            {
                direction = Vector2.down;
                if (directionCoroutine != null)
                {
                    StopCoroutine(ChangeDirectionCooldown());
                }

                directionCoroutine = StartCoroutine(ChangeDirectionCooldown());
                sprite.transform.eulerAngles = new Vector3(0, 0, 90);
            }
            // Right
            else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left && direction != Vector2.right)
            {
                direction = Vector2.right;
                if (directionCoroutine != null)
                {
                    StopCoroutine(ChangeDirectionCooldown());
                }

                directionCoroutine = StartCoroutine(ChangeDirectionCooldown());
                sprite.transform.eulerAngles = new Vector3(0, 0, 180);
            }
            // Left
            else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right && direction != Vector2.left)
            {
                direction = Vector2.left;
                if (directionCoroutine != null)
                {
                    StopCoroutine(ChangeDirectionCooldown());
                }

                directionCoroutine = StartCoroutine(ChangeDirectionCooldown());
                sprite.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            // Segment movement
            for (int i = segments.Count - 1; i > 0; i--)
            {
                segments[i].position = segments[i - 1].position;
                segments[i].GetChild(0).eulerAngles = segments[i - 1].GetChild(0).eulerAngles;
            }

            this.transform.position = new Vector3(Mathf.Round(this.transform.position.x) + direction.x, Mathf.Round(this.transform.position.y) + direction.y, isBurrowed ? -10f : 0f);
        }
    }

    private IEnumerator Burrow()
    {
        isBurrowed = true;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);

        yield return new WaitForSeconds(burrowTime);

        isBurrowed = false;
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
        StartCoroutine(BurrowCooldown());
    }

    private IEnumerator BurrowCooldown()
    {
        isBurrowOnCooldown = true;
        yield return new WaitForSeconds(burrowCooldown);
        isBurrowOnCooldown = false;
    }

    private IEnumerator ChangeDirectionCooldown()
    {
        canChangeDirection = false;
        yield return new WaitForSeconds(changeDirectionCooldown);
        canChangeDirection = true;
    }

    private void AddSegment()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.name += " " + (segments.Count).ToString();
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    // Collision handling
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Spice")
        {
            int numSegments = Random.Range(0, maxSegmentsPerFood + 1);
            for (int i = 0; i < numSegments; i++)
            {
                AddSegment();
            }
        }
        else if (collision.tag == "WrapWall")
        {
            string location = collision.gameObject.GetComponent<WrapWall>().location;

            if (location == "Top")
            {
                this.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y * -1), isBurrowed ? -10 : 0);
            }
            else if (location == "Bottom")
            {
                this.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y * -1), isBurrowed ? -10 : 0);
            }
            else if (location == "Left")
            {
                this.transform.position = new Vector3((this.transform.position.x * -1), this.transform.position.y, isBurrowed ? -10 : 0);
            }
            else if (location == "Right")
            {
                this.transform.position = new Vector3((this.transform.position.x * -1), this.transform.position.y, isBurrowed ? -10 : 0);
            }
        }
        else if (collision.tag == "Sandworm" && collision.gameObject.tag != "First" && !isBurrowed)
        {
            isDead = true;
            manager.GameOver();
        }
    }
}
