using System.Collections.Generic;
using UnityEngine;

public class FollowPointsManager : MonoBehaviour
{
    public List<Zone> zone;

    [System.Serializable]
    public struct Zone
    { 
        public Zones zones;
        [SerializeField] public List<Transform> points; 
    }

    public enum Zones
    {
        Zone1,
        Zone2,
        Zone3,
        Zone4,
        Zone5,
        Zone6
    }
}