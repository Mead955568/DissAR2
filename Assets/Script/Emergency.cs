using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Emergency : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown objectTypeDropdown;
    [SerializeField]
    private LineRenderer lineRenderer;
    private Camera arCamera;

    private NavMeshPath navMeshPath;
    private string selectedObjectType = "";

    private void Start()
    {
        arCamera = Camera.main;

        objectTypeDropdown.onValueChanged.AddListener(_ => SetSelectedObjectType());

        // Initialize the NavMeshPath
        navMeshPath = new NavMeshPath();
        lineRenderer.enabled = false; // Initially, LineRenderer is disabled
    }

    private void Update()
    {
        // Check if the Line Renderer should be enabled
        if (!string.IsNullOrEmpty(selectedObjectType))
        {
            // Find all GameObjects with the specified tag (e.g., "FireExit", "FireExtinguish", "FireAlarm")
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(selectedObjectType);

            if (taggedObjects.Length == 0)
            {
                Debug.LogWarning("No objects with the selected tag found.");
                return;
            }

            // Find the closest object
            Transform closestObject = GetClosestObject(taggedObjects);

            if (closestObject != null)
            {
                // Calculate the NavMesh path to the closest object
                NavMesh.CalculatePath(arCamera.transform.position, closestObject.position, NavMesh.AllAreas, navMeshPath);

                if (navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.LogWarning("Path calculation Success.");

                    // Enable the Line Renderer and set its positions to the calculated path
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = navMeshPath.corners.Length;
                    lineRenderer.SetPositions(navMeshPath.corners);
                }
                else
                {
                    Debug.LogWarning("Path calculation failed.");
                }
            }
            else
            {
                Debug.LogWarning("No closest object found.");
            }
        }
    }

    private void SetSelectedObjectType()
    {
        selectedObjectType = objectTypeDropdown.options[objectTypeDropdown.value].text;
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
}
