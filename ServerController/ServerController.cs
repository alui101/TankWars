using Model;
using NetworkUtil;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TankWars;
using WorldSpace;

namespace ServerControllerSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class updates the model according to the game mechanics, keeping track of clients, and broadcasting the world to them.
    /// </summary>
    public class ServerController
    {
        /// <summary>
        /// A map of clients that are connected, each with an ID.
        /// </summary>
        private Dictionary<long, SocketState> clients;

        /// <summary>
        /// The world state
        /// </summary>
        private World theWorld;

        /// <summary>
        /// Contains the information in the settings file.
        /// </summary>
        private XmlParser settings;

        /// <summary>
        /// List of walls parsed from the settings file.
        /// </summary>
        private HashSet<Walls> walls;

        /// <summary>
        /// Helps us assign tank IDs.
        /// </summary>
        private int tankID;

        /// <summary>
        /// Helps us assign projectiles IDs.
        /// </summary>
        private int projID;

        /// <summary>
        /// Helps us assign beam IDs.
        /// </summary>
        private int beamID;

        /// <summary>
        /// Dictionary of the commmands received from the clients.
        /// </summary>
        private Dictionary<int, ControlCommand> commands;

        /// <summary>
        /// List of the tanks that have disconnected.
        /// </summary>
        private HashSet<Tank> disconnectedTanks;

        /// <summary>
        /// List of projectiles that have died(hit something or left the bounds of the world)
        /// </summary>
        private HashSet<Projectile> deadProjectiles;

        /// <summary>
        /// List of beams that have been utilized
        /// </summary>
        private HashSet<Beams> deadBeams;

        /// <summary>
        /// Represents the number of frames since the last powerup spawned.
        /// </summary>
        private long framesSincePowerupSpawned;

        /// <summary>
        /// Amount of frames that have passed since game started.
        /// </summary>
        private long frames;

        /// <summary>
        /// Zero argument constructor that sets the default value of the fields.
        /// </summary>
        public ServerController()
        {
            frames = 0;

            theWorld = new World();

            theWorld.SetPowerupDelay();

            //Parse the xml file and save the values to private fields
            settings = new XmlParser("..\\..\\..\\..\\Resources\\settings.xml");

            walls = settings.GetWalls();

            // Include all of the walls into the world
            foreach (Walls wall in walls)
            {
                theWorld.addWall(wall.getID(), wall);
            }

            clients = new Dictionary<long, SocketState>();
            commands = new Dictionary<int, ControlCommand>();

            disconnectedTanks = new HashSet<Tank>();
            deadBeams = new HashSet<Beams>();
            deadProjectiles = new HashSet<Projectile>();

            // IDs
            tankID = 0;
            projID = 0;
            beamID = 0;

            framesSincePowerupSpawned = 0;
        }

        /// <summary>
        /// Called everytime a frame passes.
        /// </summary>
        public void IncrementFrames()
        {
            frames++;
        }

        /// <summary>
        /// Start accepting Tcp sockets connections from clients
        /// </summary>
        public void StartServer()
        {
            try
            {
                // This begins an "event loop"
                Networking.StartServer(NewClientConnected, 11000);
                Console.WriteLine("Server is running. Accepting new clients:");
            }
            catch
            {
                Console.WriteLine("Error occured whilst trying to start server.");
            }
        }

        /// <summary>
        /// Manages clients that have attempted a connected.
        /// </summary>
        /// <param name="state">The SocketState representing the new client</param>
        private void NewClientConnected(SocketState state)
        {
            if (state.ErrorOccurred)
                return;

            Console.WriteLine("Accepted new connection.");

            // change the state's network action to the 
            // receive handler so we can process data when something
            // happens on the network
            state.OnNetworkAction = ReceiveName;

            Networking.GetData(state);
        }

        /// <summary>
        /// Manages information provided by the client and sends information on the world.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ReceiveName(SocketState state)
        {
            // Remove the client if they aren't still connected
            if (state.ErrorOccurred)
            {
                RemoveClient(state.ID);

                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            if (parts.Length >= 1)
            {
                string name = parts[0].Substring(0, parts[0].Length - 1);
                Console.WriteLine("Player(" + state.ID + ") " + "\"" + name + "\"" + " joined.");
                lock (clients)
                {
                    clients[state.ID] = state;
                }
                Networking.Send(state.TheSocket, state.ID + "\n" + settings.GetWorldSize() + "\n");
                foreach (Walls wall in walls)
                {
                    string serializedWall = JsonConvert.SerializeObject(wall);
                    Networking.Send(state.TheSocket, serializedWall + "\n");
                }
                lock (clients)
                {
                    // If the game mode is enabled we must determine which player 
                    // belongs to which team, and provide an identifier.
                    if (settings.GetGameMode())
                    {
                        string teamName = "";
                        if (tankID % 2 == 0)
                        {
                            teamName = "TEAM1 : " + name;
                        }
                        else
                        {
                            teamName = "TEAM2 : " + name;
                        }
                        // Creates a new tank
                        theWorld.addTank(tankID, Tank.SpawnTank(teamName, tankID, settings.GetWorldSize(), walls));
                    }
                    // If the game mode is disabled then the default name is used.
                    else
                    {
                        theWorld.addTank(tankID, Tank.SpawnTank(name, tankID, settings.GetWorldSize(), walls));
                    }
                }
                tankID++;
                state.OnNetworkAction = ReceiveCommands;
            }

            // Continue the event loop that receives messages from this client untill we get the name or they disconnect
            Networking.GetData(state);
        }

        /// <summary>
        /// Handles command requests from the client.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ReceiveCommands(SocketState state)
        {
            // Remove the client if they aren't still connected
            if (state.ErrorOccurred)
            {
                RemoveClient(state.ID);
                return;
            }

            lock (commands)
            {
                ProcessCommands(state);
            }
            // Continue the event loop that receives messages from this client
            Networking.GetData(state);
        }

        /// <summary>
        /// Given the data that has arrived so far, 
        /// potentially from multiple Receivecommands operations, 
        /// determine if we have enough to make a complete message,
        /// and process/gather it.
        /// </summary>
        /// <param name="sender">The SocketState that represents the client</param>
        private void ProcessCommands(SocketState state)
        {
            string totalData = state.GetData();

            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            // Loop until we have processed all messages.
            // We may have received more than one.
            foreach (string p in parts)
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                // If client sends malformed data dissconnect them.
                try
                {
                    if (p[0] == '{')
                    {
                        ControlCommand command = JsonConvert.DeserializeObject<ControlCommand>(p);

                        // Used to keep track of beam requests made by clients because commands are coming in faster than the frame rate.
                        if (command.GetFiring() == "alt")
                        {
                            Tank tank = theWorld.getTank((int)state.ID);

                            if (tank.GetCharges() > 0 && tank.GetHp() > 0)
                            {
                                tank.SetIsBeamReady(true);
                            }
                        }
                        // Obtaining commands.
                        if (!commands.TryGetValue((int)state.ID, out _))
                        {
                            commands.Add((int)state.ID, command);
                        }
                        else
                        {
                            commands[(int)state.ID] = command;
                        }
                    }
                }
                catch
                {
                    RemoveClient(state.ID);
                }
                // Remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);
            }
        }

        /// <summary>
        /// Removes a client from the clients dictionary
        /// </summary>
        /// <param name="id">The ID of the client</param>
        private void RemoveClient(long id)
        {
            Console.WriteLine("Client " + id + " disconnected");
            lock (clients)
            {
                clients.Remove(id);

                // When a client is removed, we must destroy tank.
                theWorld.getTank((int)id).SetDisconnection(true);
                theWorld.getTank((int)id).SetDeath(true);
                theWorld.getTank((int)id).SetHp(0);

                disconnectedTanks.Add(theWorld.getTank((int)id));
                theWorld.removeTankID((int)id);
            }
        }

        /// <summary>
        /// Sends the state of the world to the clients.
        /// </summary>
        public void UpdateWorld()
        {
            lock (clients)
            {
                // captures everything that is going to be sent.
                string send = "";

                // Checks if there is less then 2 powerups in the world.
                if (theWorld.getPowerUps().Count < 2)
                {
                    // Checks if enough time has passed to be able to spawn a powerup.
                    if (frames - framesSincePowerupSpawned >= theWorld.GetPowerupDelay())
                    {
                        // spawns powerup
                        int thisPowerUp = theWorld.getPowerupID();
                        Powerups.SpawnPowerup(settings.GetWorldSize(), walls, theWorld);
                        send += JsonConvert.SerializeObject(theWorld.getPowerUp(thisPowerUp)) + "\n";
                        theWorld.SetPowerupDelay();
                        framesSincePowerupSpawned = frames;
                    }

                }

                // Loops through the clients and send them the active power ups.
                foreach (KeyValuePair<long, SocketState> client in clients)
                {
                    if (send.Length > 0)
                    {
                        Networking.Send(client.Value.TheSocket, send);
                    }
                }

                // Prepare for next capture
                send = "";

                // Determine if a tank came into contact with any powerup
                foreach (KeyValuePair<int, Powerups> p in theWorld.getPowerUps())
                {
                    Powerups.TankCollidesWithPowerUp(p.Value, theWorld);
                }

                // Loops through the clients and send them the active tanks.
                foreach (KeyValuePair<long, SocketState> client in clients)
                {
                    // Used to determine if a specified tank exists
                    Tank tank = theWorld.getTank((int)client.Key);

                    // Determines if the client and tank exists
                    if (clients.TryGetValue(client.Key, out SocketState state) && tank != null)
                    {
                        // Processes the commands sent by the client and applies them on their tank.
                        lock (commands)
                        {
                            if (tank.GetHp() > 0)
                            {
                                ProcessControls(tank);
                            }
                        }

                        // If tank is dead but it succeeds in receiving the send then we proceed.
                        if (tank.IsDead())
                        {
                            tank.SetJoined(false);

                            // If tank is dead check if it is ready to respawn, if it is respawn it, if not do nothing.
                            if ((frames - tank.GetFramesSinceDeath() >= settings.GetRespawnRate()) && tank.GetHp() == 0)
                            {
                                tank.SetLocation(Tank.RespawnTank(settings.GetWorldSize(), walls));
                                tank.SetHp(3);
                            }

                            // Server sends the other alive tanks to this client.
                            foreach (KeyValuePair<int, Tank> tankInWorld in theWorld.getTanks())
                            {
                                if (tankInWorld.Value.GetID() != tank.GetID())
                                {
                                    send += JsonConvert.SerializeObject(tankInWorld.Value) + "\n";
                                }
                            }

                            if (send.Length > 0)
                            {
                                Networking.Send(state.TheSocket, send);
                            }
                            send = "";
                        }
                        // If we fail in sending the "Alive" client the processed tank then that means there is
                        // something wrong with the client so we don't try to do anything else.
                        if (!tank.IsDead() && Networking.Send(state.TheSocket, JsonConvert.SerializeObject(tank) + "\n"))
                        {
                            tank.SetJoined(false);

                            // If tank is dead check if it is ready to respawn, if it is respawn it, if not do nothing.
                            if ((frames - tank.GetFramesSinceDeath() >= settings.GetRespawnRate()) && tank.GetHp() == 0)
                            {
                                tank.SetLocation(Tank.RespawnTank(settings.GetWorldSize(), walls));
                                tank.SetHp(3);
                            }

                            // Server sends the other alive tanks to this client.
                            foreach (KeyValuePair<int, Tank> tankInWorld in theWorld.getTanks())
                            {
                                if (tankInWorld.Value.GetID() != tank.GetID() && tankInWorld.Value.GetHp() != 0)
                                {
                                    send += JsonConvert.SerializeObject(tankInWorld.Value) + "\n";
                                }
                            }

                            if (send.Length > 0)
                            {
                                Networking.Send(state.TheSocket, send);
                            }
                            send = "";
                        }
                    }
                }

                // Loop to add all the projectiles to send, so that we send it to each client.
                foreach (KeyValuePair<int, Projectile> projInWorld in theWorld.getProjectiles())
                {
                    if (!projInWorld.Value.getDeath())
                    {
                        if (Projectile.ProcessProj(projInWorld.Value, walls, settings.GetWorldSize(), theWorld, settings.GetGameMode()))
                        {
                            deadProjectiles.Add(projInWorld.Value);
                        }
                    }
                    send += JsonConvert.SerializeObject(projInWorld.Value) + "\n";
                }

                // Loop to add all the beams to send, so that we send it to each client.
                foreach (KeyValuePair<int, Beams> beams in theWorld.getBeams())
                {
                    deadBeams.Add(beams.Value);
                    Beams.ProcessBeam(beams.Value, theWorld, settings.GetGameMode());
                    send += JsonConvert.SerializeObject(beams.Value) + "\n";
                }

                // Loops through every client to send them the projectiles and beams.
                foreach (KeyValuePair<long, SocketState> client in clients)
                {
                    if (send.Length > 0)
                    {
                        Networking.Send(client.Value.TheSocket, send);
                    }
                }
                send = "";

                // Removes dead projectiles from the world.
                foreach (Projectile proj in deadProjectiles)
                {
                    theWorld.RemoveProj(proj.getID());
                }

                // Removes dead powerups from the world
                foreach (Powerups p in theWorld.getDeadPowerUps())
                {
                    theWorld.removePowerUp(p.getID());
                }

                // Removes dead beams from the world
                foreach (Beams b in deadBeams)
                {
                    theWorld.removeBeam(b.getID());
                }

                // Clears the list of deadProjectiles so that we don't send the same deadProjectiles over and over again.
                if (deadProjectiles.Count > 0)
                {
                    deadProjectiles.Clear();
                }

                // Clears the list of deadBeams so that we don't send the same deadBeams over and over again.
                if (deadBeams.Count > 0)
                {
                    deadBeams.Clear();
                }

                // Loops through each client to send the disconnected tanks, dead tanks and dead powerups.
                foreach (KeyValuePair<long, SocketState> client in clients)
                {
                    foreach (Tank tank in disconnectedTanks)
                    {
                        send += JsonConvert.SerializeObject(tank) + "\n";
                    }
                    if (send.Length > 0)
                    {
                        Networking.Send(client.Value.TheSocket, send);
                    }
                    send = "";
                    foreach (Tank tank in theWorld.getDeadTanks())
                    {
                        send += JsonConvert.SerializeObject(tank) + "\n";
                        // Starts the death timer, that determines when tank respawns.
                        tank.SetFramesAtDeath(frames);
                    }
                    if (send.Length > 0)
                    {
                        Networking.Send(client.Value.TheSocket, send);
                    }
                    send = "";
                    foreach (Powerups p in theWorld.getDeadPowerUps())
                    {
                        send += JsonConvert.SerializeObject(p) + "\n";
                    }
                    if (send.Length > 0)
                    {
                        Networking.Send(client.Value.TheSocket, send);
                    }
                    send = "";
                }

                // All dead tanks been sent, set back to default values.
                foreach (Tank tank in theWorld.getDeadTanks())
                {
                    tank.SetDeath(false);
                }

                // Clears the list of dead tanks so that we don't send the same dead tanks over and over again.
                if (theWorld.getDeadTanks().Count > 0)
                {
                    theWorld.ClearDeadTanks();
                }

                // Clears the list of disconnected tanks so that we don't send the same disconnected tanks over and over again.
                if (disconnectedTanks.Count > 0)
                {
                    disconnectedTanks.Clear();
                }
                // Clears the list of dead powerups so that we don't send the same dead powerup over and over again.
                if (theWorld.getDeadPowerUps().Count > 0)
                {
                    theWorld.ClearDeadPowerUps();
                }
            }
        }

        /// <summary>
        /// Applies the controls the client sent to their tank.
        /// </summary>
        /// <param name="tank"> tank whos controls are to be processed</param>
        private void ProcessControls(Tank tank)
        {
            if (commands.TryGetValue(tank.GetID(), out ControlCommand command))
            {
                // Sets the aim of the tank
                Vector2D newAim = new Vector2D(command.GetTdir());
                tank.SetAim(newAim);

                // Checks if tank is ready to fire a beam.
                if (tank.IsBeamReady())
                {
                    if (tank.GetCharges() > 0)
                    {
                        tank.ChargeUsed();
                        Beams beam = new Beams(beamID, tank.GetLocation(), tank.GetAim(), tank.GetID());
                        theWorld.addBeam(beamID, beam);
                        beamID++;
                        tank.SetIsBeamReady(false);
                    }
                }

                if (command.GetMoving() == "left")
                {
                    Vector2D oldLoc = tank.GetLocation();
                    Vector2D newLoc = new Vector2D(oldLoc.GetX() - 3, oldLoc.GetY());

                    // If tank is out of bounds then wrap it around appropriately.
                    if (Tank.HitsBoundry(tank.GetLocation(), settings.GetWorldSize()))
                    {
                        newLoc = new Vector2D(-oldLoc.GetX() - 3, oldLoc.GetY());
                        tank.SetLocation(newLoc);
                    }
                    else if (!Tank.TankCollidesWithWall(newLoc, walls))
                    {
                        tank.SetLocation(newLoc);
                    }
                    Vector2D newBdir = new Vector2D(-1, 0);
                    tank.SetBDir(newBdir);
                }
                else if (command.GetMoving() == "right")
                {
                    Vector2D oldLoc = tank.GetLocation();
                    Vector2D newLoc = new Vector2D(oldLoc.GetX() + 3, oldLoc.GetY());

                    // If tank is out of bounds then wrap it around appropriately.
                    if (Tank.HitsBoundry(tank.GetLocation(), settings.GetWorldSize()))
                    {
                        newLoc = new Vector2D(-oldLoc.GetX() + 3, oldLoc.GetY());
                        tank.SetLocation(newLoc);
                    }
                    else if (!Tank.TankCollidesWithWall(newLoc, walls))
                    {
                        tank.SetLocation(newLoc);
                    }
                    Vector2D newBdir = new Vector2D(1, 0);
                    tank.SetBDir(newBdir);
                }
                else if (command.GetMoving() == "down")
                {
                    Vector2D oldLoc = tank.GetLocation();
                    Vector2D newLoc = new Vector2D(oldLoc.GetX(), oldLoc.GetY() + 3);

                    // If tank is out of bounds then wrap it around appropriately.
                    if (Tank.HitsBoundry(tank.GetLocation(), settings.GetWorldSize()))
                    {
                        newLoc = new Vector2D(oldLoc.GetX(), -oldLoc.GetY() + 3);
                        tank.SetLocation(newLoc);
                    }
                    else if (!Tank.TankCollidesWithWall(newLoc, walls))
                    {
                        tank.SetLocation(newLoc);
                    }

                    Vector2D newBdir = new Vector2D(0, 1);
                    tank.SetBDir(newBdir);
                }
                else if (command.GetMoving() == "up")
                {
                    Vector2D oldLoc = tank.GetLocation();
                    Vector2D newLoc = new Vector2D(oldLoc.GetX(), oldLoc.GetY() - 3);

                    // If tank is out of bounds then wrap it around appropriately.
                    if (Tank.HitsBoundry(tank.GetLocation(), settings.GetWorldSize()))
                    {
                        newLoc = new Vector2D(oldLoc.GetX(), -oldLoc.GetY() - 3);
                        tank.SetLocation(newLoc);
                    }
                    else if (!Tank.TankCollidesWithWall(newLoc, walls))
                    {
                        tank.SetLocation(newLoc);
                    }

                    Vector2D newBdir = new Vector2D(0, -1);
                    tank.SetBDir(newBdir);
                }

                // Manages main projectile and its firing delay
                if (command.GetFiring() == "main")
                {
                    if (tank.GetFirstShot()) // After the first shot we must time its delay
                    {
                        if (frames - tank.GetFramesShot() >= settings.GetFramesPerShot()) // The delay
                        {
                            // Creating the projectile
                            Projectile proj = new Projectile(projID, tank.GetLocation(), tank.GetAim(), false, tank.GetID());
                            theWorld.addProjectile(projID, proj);
                            tank.SetFramesShot(frames);
                            projID++;
                        }
                    }
                    else
                    {
                        tank.TanktShot();
                        tank.SetFramesShot(frames);
                        Projectile proj = new Projectile(projID, tank.GetLocation(), tank.GetAim(), false, tank.GetID());
                        theWorld.addProjectile(projID, proj);
                        projID++;
                    }
                }

            }

        }

        /// <summary>
        /// Gets the value of MSPerFrame
        /// </summary>
        /// <returns> Amount of MS per frame</returns>
        public int GetMSPerFrame()
        {
            return settings.GetMSPerFrame();
        }
    }
}
