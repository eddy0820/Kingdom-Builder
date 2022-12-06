using UnityEngine;

public class VisibleJoints : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] Color rootColor = Color.green;
    [SerializeField] Color nodeColor = Color.white;
    [SerializeField] Color lineColor = Color.white;

    [Header("Root Node")]
    [SerializeField] Transform rootNode;

    [Header("Child Nodes")]
    [ReadOnly, SerializeField] Transform[] childNodes;

    private void OnDrawGizmos()
    {
        if (rootNode != null)
        {
            if (childNodes == null || childNodes.Length == 0)
            {
                // Get all joints to draw
                PopulateChildren();
            }


            foreach (Transform child in childNodes)
            {

                if (child == rootNode)
                {
                    // List includes the root, if root then larger, green cube
                    Gizmos.color = rootColor;
                    Gizmos.DrawCube(child.position, new Vector3(.1f, .1f, .1f));
                }
                else
                {
                    if(child.GetComponent<MeshRenderer>() == null)
                    {
                        Gizmos.color = lineColor;
                        Gizmos.DrawLine(child.position, child.parent.position);

                        Gizmos.color = nodeColor;
                        Gizmos.DrawCube(child.position, new Vector3(.01f, .01f, .01f));
                    }
                }
            }

        }
    }

    public void PopulateChildren()
    {
        childNodes = rootNode.GetComponentsInChildren<Transform>();
    }
}