using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Emergency : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public LineRenderer lineRenderer; // Assign your LineRenderer prefab in the Inspector
    private Camera arCamera; // Reference to the AR camera
    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent for pathfinding

    private void Start()
    {
        arCamera = Camera.main; // Assumes you have a main camera set as the AR camera

        // Subscribe to the dropdown's onValueChanged event
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Disable the Line Renderer initially
        lineRenderer.enabled = false;

        // Create a NavMeshAgent for pathfinding
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.enabled = false; // Disable it initially
    }

    private void Update()
    {
        // Check if the Line Renderer is enabled and the NavMeshAgent is enabled
        if (lineRenderer.enabled && navMeshAgent.enabled)
        {
            // Update the Line Renderer's positions based on the NavMesh path
            DrawNavMeshPath(navMeshAgent.path);
        }
    }

    private void OnDropdownValueChanged(int optionIndex)
    {
        // Get the selected option from the dropdown
        string selectedOption = dropdown.options[optionIndex].text;

        // Find all GameObjects with the specified tag (e.g., "FireExit", "FireExtinguisher", "FireAlarm")
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(selectedOption);

        if (taggedObjects.Length == 0)
        {
            Debug.LogWarning("No objects with the selected tag found.");
            return;
        }

        // Find the closest object
        Transform closestObject = GetClosestObject(taggedObjects);

        if (closestObject != null)
        {
            // Enable the NavMeshAgent and set its destination to the closest object
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(closestObject.position);
        }
        else
        {
            Debug.LogWarning("No closest object found.");
        }
    }

    private Transform GetClosestObject(GameObject[] objects)
    {
        Transform closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(arCamera.transform.position, obj.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj.transform;
            }
        }
        return closestObject;
    }

    private void DrawNavMeshPath(NavMeshPath path)
    {
        // Set the Line Renderer positions to match the NavMesh path
        lineRenderer.positionCount = path.corners.Length;
        for (int i = 0; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }
}
