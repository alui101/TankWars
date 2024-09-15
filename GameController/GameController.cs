using NetworkUtil;
using System.Text.RegularExpressions;
using Model;
using Newtonsoft.Json;
using WorldSpace;
using TankWars;


namespace Controller
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class is responsible for parsing information received from the NetworkController,
    /// updating the model, and Frequently informing the view about the changing world.
    /// </summary>
    public class GameController
    {
        // delegates and events

        public delegate void ServerUpdateHandler();

        public event ServerUpdateHandler UpdateArrived;

        public delegate void errorMessages();

        public event errorMessages sendMessage;

        // Object that represents the world
        private World theWorld;

        // State representing the connection with the server
        SocketState theServer;

        // Determines the name of player

        private string playerName;

        // Determines the id

        private int ID;

        // Determines the world size

        private int worldSize;

        // Determines if its moving or where its moving

        private string moving;

        // Determines what kind of shot is bieng fired

        private string fire;

        // Represents key inputs

        private bool up;
        private bool right;
        private bool left;
        private bool down;


        public Vector2D tdir = new Vector2D();

        /// <summary>
        /// Preparing the fields and key inputs.
        /// </summary>
        public GameController()
        {
            //Initializing fields

            theWorld = new World();
            theServer = null;
            playerName = "player";
            ID = -1;
            worldSize = 0;

            moving = "none";
            fire = "none";

            up = false;
            right = false;
            left = false;
            down = false;
        }

        /// <summary>
        /// Begins the process of connecting to the server.
        /// </summary>
        /// <param name="addr"> the server address</param>
        public void Connect(string addr, string name)
        {
            playerName = name;
            Networking.ConnectToServer(OnConnect, addr, 11000);
        }

        /// <summary>
        /// Method to be invoked by the networking library when a connection is made.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void OnConnect(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                // inform the view
                sendMessage();
                return;
            }

            theServer = state;

            Networking.Send(state.TheSocket, playerName + "\n");

            // Start an event loop to receive messages from the server
            state.OnNetworkAction = ReceiveID;
            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library when 
        /// data is available.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ReceiveID(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                // inform the view
                sendMessage();
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            if (parts.Length >= 1)
            {
                ID = int.Parse(parts[0]);
                state.OnNetworkAction = ReceiveWorldSize;
            }

            // Continue the event loop
            // state.OnNetworkAction has not been changed, 
            // so this same method (ReceiveMessage) 
            // will be invoked when more data arrives
            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library when 
        /// data is available.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ReceiveWorldSize(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                // inform the view
                sendMessage();
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            if (parts.Length >= 2)
            {
                worldSize = int.Parse(parts[1]);
                state.OnNetworkAction = ReceiveJson;
            }

            Networking.GetData(state);
        }

        /// <summary>
        /// Method to be invoked by the networking library when 
        /// data is available.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ReceiveJson(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                // inform the view
                sendMessage();
                return;
            }
            lock (theWorld)
            {
                ProcessMessages(state);
            }

            Networking.GetData(state);

            // Notify any listeners (the view) that a new game world has arrived from the server
            if (UpdateArrived != null)
                UpdateArrived();

        }


        /// <summary>
        /// Process any buffered messages separated by '\n'
        /// Then inform the view.
        /// </summary>
        /// <param name="state">The socket on which to send the data</param>
        private void ProcessMessages(SocketState state)
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

                if (p[0] == '{')
                {
                    //    //Check if p is a tank
                    if (p[2] == 't')
                    {

                        Tank tank = JsonConvert.DeserializeObject<Tank>(p);

                        // if tank is new and is not dead then add it
                        if (theWorld.getTank(tank.GetID()) is null && !tank.IsDead() && !tank.GetDisconnected() && tank.GetHp() != 0)
                        {
                            if (tank.GetID() == ID)
                            {
                                theWorld.setID(ID);

                                theWorld.setDeath(false);

                            }

                            theWorld.addTank(tank.GetID(), tank);

                        }
                        // If tank exists and is not dead or dissconnected then remove old tank and add new one
                        if (theWorld.getTank(tank.GetID()) != null && !tank.IsDead() && !tank.GetDisconnected())
                        {
                            // Remove tank with same ID
                            theWorld.removeTankID(tank.GetID());
                            theWorld.addTank(tank.GetID(), tank);
                        }
                        // Removes tank if it dies
                        if (theWorld.getTank(tank.GetID()) != null && tank.IsDead())
                        {
                            if (tank.GetID() == ID)
                            {
                                theWorld.setDeath(true);
                                theWorld.removeTankID(tank.GetID());
                                theWorld.addExplosion(new Explosion(tank.GetLocation()));
                            }
                            else
                            {
                                theWorld.removeTankID(tank.GetID());
                                theWorld.addExplosion(new Explosion(tank.GetLocation()));
                            }
                        }

                        // Removes tank when it gets disconnected
                        if (theWorld.getTank(tank.GetID()) != null && tank.GetDisconnected())
                        {
                            theWorld.removeTankID(tank.GetID());
                            theWorld.addExplosion(new Explosion(tank.GetLocation()));
                        }

                        if (theWorld.getTank(tank.GetID()) != null && tank.GetHp() == 0)
                        {
                            theWorld.removeTankID(tank.GetID());
                        }

                    }

                    // Check if p is a projectile
                    if (p[2] == 'p' && p[3] == 'r')
                    {
                        Projectile proj = JsonConvert.DeserializeObject<Projectile>(p);

                        // if projectile does not exist and is not dead
                        if (theWorld.getProj(proj.getID()) == null && !proj.getDeath())
                        {
                            theWorld.addProjectile(proj.getID(), proj);
                        }

                        // if projectile exists and is not dead
                        if (theWorld.getProj(proj.getID()) != null && !proj.getDeath())
                        {
                            theWorld.RemoveProj(proj.getID());
                            theWorld.addProjectile(proj.getID(), proj);
                        }

                        // if projectile is dead
                        if (theWorld.getProj(proj.getID()) != null && proj.getDeath())
                        {
                            theWorld.RemoveProj(proj.getID());
                        }
                    }

                    //Check if p is a wall
                    if (p[2] == 'w')
                    {
                        Walls wall = JsonConvert.DeserializeObject<Walls>(p);
                        //Add to world
                        theWorld.addWall(wall.getID(), wall);
                    }

                    // Check if p is a beam
                    if (p[2] == 'b')
                    {

                        Beams beam = JsonConvert.DeserializeObject<Beams>(p);
                        //Add to world
                        theWorld.addBeam(beam.getID(), beam);
                    }

                    // Check if p is a powerup
                    if (p[2] == 'p' && p[3] == 'o')
                    {
                        Powerups powerup = JsonConvert.DeserializeObject<Powerups>(p);
                        // if powerup does not exist and is not dead
                        if (theWorld.getPowerUp(powerup.getID()) == null && !powerup.getDeath())
                        {
                            theWorld.addPowerUp(powerup.getID(), powerup);
                        }

                        // if PowerUp exists and is not dead
                        if (theWorld.getPowerUp(powerup.getID()) != null && !powerup.getDeath())
                        {
                            theWorld.removePowerUp(powerup.getID());
                            theWorld.addPowerUp(powerup.getID(), powerup);
                        }

                        // if PowerUp is dead
                        if (theWorld.getPowerUp(powerup.getID()) != null && powerup.getDeath())
                        {
                            theWorld.removePowerUp(powerup.getID());
                        }
                    }
                }



                // Then remove it from the SocketState's growable buffer
                state.RemoveData(0, p.Length);

            }

            ProcessInputs();
        }

        /// <summary>
        /// Send input information to the server.
        /// </summary>
        private void ProcessInputs()
        {
            tdir.Normalize();

            ControlCommand cmd = new ControlCommand(moving, fire, tdir);

            Networking.Send(theServer.TheSocket, JsonConvert.SerializeObject(cmd) + "\n");

        }

        /// <summary>
        /// Determines which direction the user is moving, if they are even moving at all.
        /// </summary>
        /// <param name="up">determines if up is pressed</param>
        public void setUp(bool up)
        {
            if (up)
            {
                this.up = up;
                moving = "up";
            }
            else
            {
                this.up = up;
                if (down)
                {
                    moving = "down";
                }
                else if (left)
                {
                    moving = "left";
                }
                else if (right)
                {
                    moving = "right";
                }
                else
                {
                    moving = "none";
                }
            }
        }

        /// <summary>
        /// Determines which direction the user is moving, if they are even moving at all.
        /// </summary>
        /// <param name="down">determines if down is pressed</param>
        public void setDown(bool down)
        {
            if (down)
            {
                this.down = down;
                moving = "down";
            }
            else
            {
                this.down = down;

                if (up)
                {
                    moving = "up";
                }
                else if (left)
                {
                    moving = "left";
                }
                else if (right)
                {
                    moving = "right";
                }
                else
                {
                    moving = "none";
                }
            }
        }

        /// <summary>
        /// Determines which direction the user is moving, if they are even moving at all.
        /// </summary>
        /// <param name="left"> determines if left was pressed</param>
        public void setLeft(bool left)
        {
            if (left)
            {
                this.left = left;
                moving = "left";
            }
            else
            {
                this.left = left;
                if (up)
                {
                    moving = "up";
                }
                else if (down)
                {
                    moving = "down";
                }
                else if (right)
                {
                    moving = "right";
                }
                else
                {
                    moving = "none";
                }
            }
        }
        /// <summary>
        /// Determines which direction the user is moving, if they are even moving at all.
        /// </summary>
        /// <param name="right"> determines if right is pressed</param>
        public void setRight(bool right)
        {
            if (right)
            {
                this.right = right;
                moving = "right";
            }
            else
            {
                this.right = right;
                if (up)
                {
                    moving = "up";
                }
                else if (left)
                {
                    moving = "left";
                }
                else if (down)
                {
                    moving = "down";
                }
                else
                {
                    moving = "none";
                }
            }
        }

        /// <summary>
        /// Determines which projectile is being shot.
        /// </summary>
        /// <param name="fire"> the type of projectile</param>
        public void setFire(string fire)
        {
            this.fire = fire;
        }

        /// <summary>
        /// Determines the the direction of the tanks turrent.
        /// </summary>
        /// <param name="pos"></param>
        public void setTdir(Vector2D pos)
        {
            this.tdir = pos;
        }

        /// <summary>
        /// Retrieves the space that holds all of the objects
        /// </summary>
        /// <returns>current world</returns>
        public World getWorld()
        {
            return theWorld;
        }

    }
}
