using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Tracking : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown _navTargetDropDown;
    [SerializeField]
    private List<string> _navTargetTags = new List<string>(); // List of target tags

    private NavMeshPath _path; // Current Calculated Path
    private LineRenderer _lineRenderer; // LineRenderer To Display Path
    private Transform _closestTarget; // The closest target GameObject with the associated tag

    public GameObject arCamera;

    private bool _lineToggle = false;

    private void Start()
    {
        _path = new NavMeshPath();
        _lineRenderer = transform.GetComponent<LineRenderer>();
        _lineRenderer.enabled = _lineToggle;

        // Initialize the closest target to null
        _closestTarget = null;
    }

    private void Update()
    {
        if (_lineToggle && _closestTarget != null)
        {
            NavMesh.CalculatePath(arCamera.transform.position, _closestTarget.position, NavMesh.AllAreas, _path);
            _lineRenderer.positionCount = _path.corners.Length;
            _lineRenderer.SetPositions(_path.corners);
        }

        // Check for the closest target and update if necessary
        UpdateClosestTarget();
    }

    public void SetCurrentNavTarget(int selectedValue)
    {
        _closestTarget = null; // Reset the closest target

        string selectedText = _navTargetDropDown.options[selectedValue].text;

        if (_navTargetTags.Contains(selectedText))
        {
            if (!_lineToggle)
            {
                ToggleVisibility();
            }

            // Find all GameObjects with the selected tag
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(selectedText);

            if (targetObjects.Length > 0)
            {
                // Find the closest target among the objects with the specified tag
                _closestTarget = FindClosestTarget(targetObjects);
            }
        }
    }

    public void ToggleVisibility()
    {
        _lineToggle = !_lineToggle;
        _lineRenderer.enabled = _lineToggle;
        Debug.Log("Toggle Line Vis");
    }

    private void UpdateClosestTarget()
    {
        if (_closestTarget != null)
        {
            // Calculate the distance to the current closest target
            float currentDistance = Vector3.Distance(arCamera.transform.position, _closestTarget.position);

            // Find all GameObjects with the specified tag
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(_closestTarget.tag);

            if (targetObjects.Length > 0)
            {
                // Find the new closest target among the objects with the specified tag
                Transform newClosestTarget = FindClosestTarget(targetObjects);

                if (newClosestTarget != _closestTarget)
                {
                    // If a new closest target is found, update the target
                    _closestTarget = newClosestTarget;
                }
            }
        }
    }

    private Transform FindClosestTarget(GameObject[] targets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(arCamera.transform.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target.transform;
            }
        }
        return closestTarget;
    }
}
