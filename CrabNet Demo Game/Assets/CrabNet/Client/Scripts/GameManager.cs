﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    /// <summary>
    /// Singleton this.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    /// <summary>
    /// Spwan player with (id, username, positio, rotation) for this client instance.
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="_username"></param>
    /// <param name="_position"></param>
    /// <param name="_rotation"></param>
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId) //if id is the local player
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);

        }
        else //if id is not the local player
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        // Adds player (id, username) to player dictionary.
        _player.GetComponent<PlayerManager>().id = _id;
        _player.GetComponent<PlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }
}