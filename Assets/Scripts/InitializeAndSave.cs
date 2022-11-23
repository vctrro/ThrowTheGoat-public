using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeAndSave : MonoBehaviour
{
    private SceneController sceneController;
    private LevelConfig levelConfig;
    private string objectName;
    private LevelConfig.UserObject thisObject;

    void Start()
    {
        sceneController = GameObject.Find("SceneController").GetComponent<SceneController>();
        sceneController.OnThrowGoat.AddListener(Throw);
        levelConfig = sceneController.levelConfig;
        objectName = gameObject.name;

        LevelConfig.UserObject temp = new LevelConfig.UserObject(); 
        foreach (var item in levelConfig.UserObjects)
        {
            if(item.Name == objectName)
            {
                temp = item;
                break;
            }
        }

        if (temp.Name != null)
        {
            transform.position = temp.Position;
            thisObject = temp;
        }
        else
        {
            temp.Name = objectName;
            temp.Position = transform.position;
            levelConfig.UserObjects.Add(temp);
            thisObject = levelConfig.UserObjects[levelConfig.UserObjects.Count-1];
            // Debug.Log(thisObject.Name);
        }
    }

    private void Throw(int throwCount)
    {
        //Save position
        if (throwCount == 1)
        {
            thisObject.Position = transform.position;
        }
    }
}
