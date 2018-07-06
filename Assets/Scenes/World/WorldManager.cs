﻿using System.Collections;
using UnityEngine;

/**
 * @author Pantelis Andrianakis
 */
public class WorldManager : MonoBehaviour
{
    public GameObject playerCharacter;

    [HideInInspector]
    public static WorldManager instance;
    [HideInInspector]
    ArrayList gameObjects = new ArrayList();
    [HideInInspector]
    private static readonly int visibilityRange = 3000;

    private void Start()
    {
        // Return if account name is empty.
        if (PlayerManager.instance == null || PlayerManager.instance.accountName == null)
        {
            return; // Return to login?
        }

        // Set instance.
        instance = this;

        // Change music.
        MusicManager.instance.PlayMusic(MusicManager.instance.EnterWorld);

        // Set player model.
        playerCharacter.GetComponent<MeshFilter>().mesh = GameObjectManager.instance.playerModels[PlayerManager.instance.selectedCharacterData.GetClassId()].GetComponent<MeshFilter>().mesh;
        playerCharacter.GetComponent<Renderer>().materials = GameObjectManager.instance.playerModels[PlayerManager.instance.selectedCharacterData.GetClassId()].GetComponent<Renderer>().materials;

        // Request world info from server.
        NetworkManager.instance.ChannelSend(new EnterWorldRequest(PlayerManager.instance.selectedCharacterData.GetName()));

        // Object distance forget task.
        StartCoroutine(DistanceCheck());
    }

    public void UpdateObject(int objectId, int classId, float posX, float posY, float posZ, int posHeading)
    {
        // Check for existing objects.
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.GetComponent<WorldObject>().objectId == objectId)
            {
                // TODO: Update object info.
                return;
            }
        }

        // Object does not exist. Instantiate.
        GameObject obj = Instantiate(GameObjectManager.instance.gameObjectList[classId], new Vector3(posX, posY, posZ), Quaternion.identity) as GameObject;

        // TODO: Proper appearance.

        // Assign object id.
        obj.AddComponent<WorldObject>();
        obj.GetComponent<WorldObject>().objectId = objectId;

        // Add RigidBody.
        Rigidbody rigidBody = obj.AddComponent<Rigidbody>();
        rigidBody.mass = 1;
        rigidBody.angularDrag = 0.05f;
        rigidBody.freezeRotation = true;

        // Add to game object list.
        gameObjects.Add(obj);
    }

    public void MoveObject(int objectId, float posX, float posY, float posZ)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.GetComponent<WorldObject>().objectId == objectId)
            {
                gameObject.GetComponent<Rigidbody>().MovePosition(Vector3.MoveTowards(gameObject.transform.position, new Vector3(posX, posY, posZ), 1));
                return;
            }
        }

        // Request unknown object info from server.
        NetworkManager.instance.ChannelSend(new ObjectInfoRequest(objectId));
    }

    public void DeleteObject(int objectId)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            if (gameObject.GetComponent<WorldObject>().objectId == objectId)
            {
                DeleteObject(gameObject);
                return;
            }
        }
    }

    private void DeleteObject(GameObject gameObject)
    {
        // Remove from objects list.
        gameObjects.Remove(gameObject);

        // Delete game object from world.
        Destroy(gameObject);
    }

    IEnumerator DistanceCheck()
    {
        while (true)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (Vector3.Distance(playerCharacter.transform.position, gameObject.transform.position) > visibilityRange)
                {
                    DeleteObject(gameObject);
                }
            }
            yield return new WaitForSeconds(3);
        }
    }
}
