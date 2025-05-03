
# CrabNet - Decentralized Topology API

CrabNet is a lightweight C# .NET API designed for use in multiplayer games made with Unity.

It provides functionality for estabilishing connectings to allow data to be sent and received, allowing users to decide how to utilise this.

# Installation

The package can be downloaded and dropped into the Unity asset folder.

# How To use

The API consists of 3 distinct sections: the logic server, the physics server, and the client.

The logic server is used to handle all game logic that can be considerd important such as player items or level progression logic. This helps improve the security of the game by handling important externally.

The physics server is used to handle all game physics such as players: players, NPCs, or networked objetcs. This server is hosted by a client, reducing the load on external servers which helps drive down running costs.

The client handles all of the personal data of a player, used to connect and communicate with the servers and other clients.

To add new networked functions to your game, you will need to create a new function in the Send class and a function to handle it in the Receive class. For these functions to be mapped to each other, you will then need to add the packet (which can simply the function name) to the Packet class enum. Then finally make sure you add it to the InitializeData function in the Client/Server class.

This will now allow you to call the Send fucntion wherever you need it in your game code.

For help on how to format and write this code, the exisiting functions such as the Welcome message showcase how to do it.

# How It Is Used in The Demo Game

The demo uses the API to split the level progrssion logic, which is specific doors opening, from the client and handle it vai the logic server.

The physics server then is responsible for handling all other fucntions which are: player position and rotation, player spawning, player shooting, player disconnects, enemy target, enemy damaged, enemy position and rotation, spawn enemy, spawn boss, and relaying sector 1 clear and sector 2 clear.

The client is then responsible for handling all the player's actions and then transmitting them to the server. As well as receiving and handling data, such as another player's actions or a sector's state, from the server.