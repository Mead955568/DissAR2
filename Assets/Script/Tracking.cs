using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Tracking : MonoBehaviour
{
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
        // Check for the closest target and update if necessary
        UpdateClosestTarget();

        if (_lineToggle && _closestTarget != null)
        {
            NavMesh.CalculatePath(arCamera.transform.position, _closestTarget.position, NavMesh.AllAreas, _path);
            _lineRenderer.positionCount = _path.corners.Length;
            _lineRenderer.SetPositions(_path.corners);
        }
    }

    private void UpdateClosestTarget()
    {
        GameObject[] taggedObjects = FindAllWithTaggedObjects();

        if (taggedObjects.Length > 0)
        {
            // Find the new closest target among the objects with the specified tag
            Transform newClosestTarget = FindClosestTarget(taggedObjects);

            if (newClosestTarget != _closestTarget)
            {
                // If a new closest target is found, update the target
                _closestTarget = newClosestTarget;
            }
        }
    }

    private GameObject[] FindAllWithTaggedObjects()
    {
        List<GameObject> taggedObjects = new List<GameObject>();

        foreach (string tag in _navTargetTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            taggedObjects.AddRange(objects);
        }

        return taggedObjects.ToArray();
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
