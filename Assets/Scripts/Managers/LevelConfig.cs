using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class LevelConfig
{
    [System.Serializable] public class UserObject
    {
        [SerializeField] private string name;
        [SerializeField] private Vector3 position;

        public string Name { get => name; set => name = value; }
        public Vector3 Position { get => position; set => position = value; }
    }

    [SerializeField] private int rating = 0;
    [SerializeField] private int attemptCounter = 0;
    [SerializeField] private int hitCounter = 0;
    [SerializeField] private int lostGoats = 0;
    [SerializeField] private List<UserObject> userObjects = new List<UserObject>();

    public int Rating { get => rating; set => rating = value; }
    public int AttemptCounter { get => attemptCounter; set => attemptCounter = value; }
    public int HitCounter { get => hitCounter; set => hitCounter = value; }
    public int LostGoats { get => lostGoats; set => lostGoats = value; }
    public List<UserObject> UserObjects { get => userObjects; set => userObjects = value; }
}
