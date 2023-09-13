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
    [SerializeField]
    private GameObject flashingBallPrefab; // Reference to the flashing ball prefab

    private NavMeshPath _path; // Current Calculated Path
    private LineRenderer _lineRenderer; // LineRenderer To Display Path
    private Dictionary<string, List<Transform>> _taggedObjects = new Dictionary<string, List<Transform>>(); // Organize objects by tag

    public GameObject arCamera;

    private bool _lineToggle = false;
    private Transform _closestTarget; // The closest target GameObject with the associated tag

    private void Start()
    {
        _path = new NavMeshPath();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = _lineToggle;

        // Initialize the tagged objects dictionary
        InitializeTaggedObjects();

        // Initialize closest target to null
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

    private void InitializeTaggedObjects()
    {
        // Fill the tagged objects dictionary with objects having the specified tags
        foreach (string tag in _navTargetTags)
        {
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(tag);
            _taggedObjects[tag] = new List<Transform>(targetObjects.Length);
            foreach (GameObject obj in targetObjects)
            {
                _taggedObjects[tag].Add(obj.transform);
            }
        }
    }

    private GameObject _currentFlashingBall;

    public void SetCurrentNavTarget(int selectedValue)
    {
        // Reset the closest target
        _closestTarget = null;

        string selectedText = _navTargetDropDown.options[selectedValue].text;

        if (_taggedObjects.ContainsKey(selectedText))
        {
            if (!_lineToggle)
            {
                ToggleVisibility();
            }

            List<Transform> targetObjects = _taggedObjects[selectedText];

            if (targetObjects.Count > 0)
            {
                // Find the closest target among the objects with the specified tag
                _closestTarget = FindClosestTarget(targetObjects);

                if (_closestTarget != null)
                {
                    // Destroy the previously instantiated flashing ball
                    if (_currentFlashingBall != null)
                    {
                        Destroy(_currentFlashingBall);
                    }

                    // Instantiate a new flashing ball at the closest target's position
                    _currentFlashingBall = Instantiate(flashingBallPrefab, _closestTarget.position, Quaternion.identity);
                }
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

            if (_taggedObjects.ContainsKey(_closestTarget.tag))
            {
                List<Transform> targetObjects = _taggedObjects[_closestTarget.tag];

                if (targetObjects.Count > 0)
                {
                    // Find the new closest target among the objects with the specified tag
                    Transform newClosestTarget = FindClosestTarget(targetObjects);

                    if (newClosestTarget != _closestTarget)
                    {
                        // If a new closest target is found, update the target
                        _closestTarget = newClosestTarget;

                        // Destroy the previously instantiated flashing ball
                        if (_currentFlashingBall != null)
                        {
                            Destroy(_currentFlashingBall);
                        }

                        // Instantiate a new flashing ball at the new closest target's position
                        _currentFlashingBall = Instantiate(flashingBallPrefab, _closestTarget.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    private Transform FindClosestTarget(List<Transform> targets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(arCamera.transform.position, target.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }
        return closestTarget;
    }
}
