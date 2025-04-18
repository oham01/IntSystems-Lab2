using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float runSpeed;
    public float gotHayDestroyDelay;
    private bool hitByHay;

    public float dropDestroyDelay; // 1
    private Collider myCollider; // 2
    private Rigidbody myRigidbody;


    private SheepSpawner sheepSpawner;

    // Sheep heart
    public float heartOffset; // 1
    public GameObject heartPrefab; // 2
    
    // Dodging sheep
    public float dodgeSpeed = 2f;
    public float dodgeIntervalMin = 1f;
    public float dodgeIntervalMax = 3f;
    public float dodgeAmount = 1f;

    private float dodgeTimer;
    private float dodgeDirection;
    private float horizontalLimit = 22f; // Match hay machine boundary

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();
        SetNextDodge(); // Initialize the next dodge of the sheep

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);

        // Side-to-side movement randomly
        if (!hitByHay)
        {
            Vector3 dodgeVector = Vector3.right * dodgeDirection * dodgeSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + dodgeVector;

            // Clamp within horizontal boundaries
            if (Mathf.Abs(newPos.x) < horizontalLimit)
            {
                transform.position = newPos;
            }

            dodgeTimer -= Time.deltaTime;
            if (dodgeTimer <= 0)
            {
                SetNextDodge();
            }
        }
    }

    private void HitByHay()
    {
        GameStateManager.Instance.SavedSheep();

        SoundManager.Instance.PlaySheepHitClip();

        sheepSpawner.RemoveSheepFromList(gameObject);
        hitByHay = true; // 1
        runSpeed = 0; // 2
        Destroy(gameObject, gotHayDestroyDelay); // 3

        Instantiate(heartPrefab, transform.position + new Vector3(0, heartOffset, 0), Quaternion.identity);
        TweenScale tweenScale = gameObject.AddComponent<TweenScale>(); ; // 1
        tweenScale.targetScale = 0; // 2
        tweenScale.timeToReachTarget = gotHayDestroyDelay; // 3
        

    }

    private void OnTriggerEnter(Collider other) // 1
    {
        if (other.CompareTag("Hay") && !hitByHay) // 2
        {
            Destroy(other.gameObject); // 3
            HitByHay(); // 4

        }
        else if (other.CompareTag("DropSheep"))
        {
            Drop();
        }
    }

    private void Drop()
    {
        SoundManager.Instance.PlaySheepDroppedClip();

        GameStateManager.Instance.DroppedSheep();
        sheepSpawner.RemoveSheepFromList(gameObject);
        myRigidbody.isKinematic = false; // 1
        myCollider.isTrigger = false; // 2
        Destroy(gameObject, dropDestroyDelay); // 3
    }

    public void SetSpawner(SheepSpawner spawner)
    {
        sheepSpawner = spawner;
    }

    private void SetNextDodge()
    {
        dodgeTimer = Random.Range(dodgeIntervalMin, dodgeIntervalMax);
        dodgeDirection = Random.value > 0.5f ? 1f : -1f; // left or right depending on randomness
    }

}

