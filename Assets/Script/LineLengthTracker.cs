using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineLengthTracker : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public TextMeshProUGUI lengthText;

    private void Update()
    {
        // Ensure both the Line Renderer and TextMeshPro components are assigned
        if (lineRenderer != null && lengthText != null)
        {
            // Calculate the length of the Line Renderer
            float lineLength = CalculateLineLength(lineRenderer);

            // Update the TextMeshPro text with the line length
            lengthText.text = "Distence: " + lineLength.ToString("F1") + " Meters"; // Display with 2 decimal places
        }
    }

    private float CalculateLineLength(LineRenderer lineRenderer)
    {
        float length = 0f;

        // Iterate through each point of the Line Renderer
        for (int i = 1; i < lineRenderer.positionCount; i++)
        {
            Vector3 startPoint = lineRenderer.GetPosition(i - 1);
            Vector3 endPoint = lineRenderer.GetPosition(i);

            // Calculate the distance between two points and add it to the total length
            length += Vector3.Distance(startPoint, endPoint);
        }

        return length;
    }
}
