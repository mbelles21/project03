using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [System.Serializable]
    public class WallSection
    {
        public GameObject completeWall;
        public GameObject doorwayOpen;
    }

    public WallSection[] wallSections = new WallSection[8];
    public Transform[] doorPoints = new Transform[8];

    void Start()
    {
        // Initialize door points
        for (int i = 0; i < doorPoints.Length; i++)
        {
            if (doorPoints[i] != null && doorPoints[i].GetComponent<DoorPoint>() == null)
            {
                doorPoints[i].gameObject.AddComponent<DoorPoint>();
            }
        }

        // Start with all walls showing
        for (int i = 0; i < wallSections.Length; i++)
        {
            ShowWall(i);
        }
    }

    public void ShowWall(int index)
    {
        if (index >= 0 && index < wallSections.Length)
        {
            if (wallSections[index].completeWall != null)
                wallSections[index].completeWall.SetActive(true);
            if (wallSections[index].doorwayOpen != null)
                wallSections[index].doorwayOpen.SetActive(false);
        }
    }

    public void ShowDoorway(int index)
    {
        if (index >= 0 && index < wallSections.Length)
        {
            if (wallSections[index].completeWall != null)
                wallSections[index].completeWall.SetActive(false);
            if (wallSections[index].doorwayOpen != null)
                wallSections[index].doorwayOpen.SetActive(true);
        }
    }
}