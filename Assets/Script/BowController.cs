using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    private Animator bowAnimator; // Reference to the Animator
    public InventorySystem inventory; // Reference to your inventory system
    public string arrowItemName = "Arrow"; // Name of the arrow item in the inventory
    private bool isDrawing = false;

    public string arrowPrefabPath = "Arrow"; // Path in the Resources folder (without file extension)

    private GameObject arrowPrefab; // The loaded arrow prefab
    public Transform spawnPosition;

    public float shootingForce = 100;

    public AudioSource bow;
    public AudioClip loadBow;
    public AudioClip shotBow;

    private void Start()
    {
        bowAnimator = GetComponent<Animator>();
        LoadArrowPrefab();
    }
    private void LoadArrowPrefab()
    {
        // Load the arrow prefab from the specified path in the Resources folder
        arrowPrefab = Resources.Load<GameObject>(arrowPrefabPath);

        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab not found at path: " + arrowPrefabPath);
        }
    }

    void Update()
    {
        HandleBowDrawing();
    }

    private void HandleBowDrawing()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            StartDraw();

        }

        if (Input.GetMouseButtonUp(1) && isDrawing) // Right mouse button released
        {
            CancelDraw();
        }

        if (Input.GetMouseButtonDown(0) && isDrawing) // Left mouse button pressed
        {
            ReleaseArrow();
        }
    }

    private void StartDraw()
    {
        isDrawing = true;
        bowAnimator.SetBool("IsDrawing", true); // Set the Animator parameter
        bow.PlayOneShot(loadBow);

    }

    private void CancelDraw()
    {
        isDrawing = false;
        bowAnimator.SetBool("IsDrawing", false); // Reset the Animator parameter
    }

    private void ReleaseArrow()
    {
        if (!isDrawing) return;

        isDrawing = false;
        bowAnimator.SetBool("IsDrawing", false);
        bow.PlayOneShot(shotBow);


        ShootArrow();
    }


    private void ShootArrow()
    {
        Vector3 shootingDirection = CalculateDirection().normalized;

        // Instantiate the bullet
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition.position, Quaternion.identity);
        arrow.transform.SetParent(null);

        // Poiting the bullet to face the shooting direction
        arrow.transform.forward = shootingDirection;

        // Shoot the bullet
        arrow.GetComponent<Rigidbody>().AddForce(shootingDirection * shootingForce, ForceMode.Impulse);
    }


    public Vector3 CalculateDirection()
    {
        // Shooting from the middle of the screen to check where are we pointing at
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Hitting Something
            targetPoint = hit.point;
        }
        else
        {
            // Shooting at the air
            targetPoint = ray.GetPoint(100);
        }

        // Returning the shooting direction and spread
        return targetPoint - spawnPosition.position;
    }

}
