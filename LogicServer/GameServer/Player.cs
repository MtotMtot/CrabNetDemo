using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class Player
    {
        /// <summary>The unique ID of the player.</summary>
        public int id;
        /// <summary>The username of the player.</summary>
        public string username;
        public Player(int _id, string _username)
        {
            id = _id;
            username = _username;
        }

        public void Update()
        {
            
        }
    }
}