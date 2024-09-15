using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TankWars;

namespace WorldSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents a Tank. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        // an int representing the tank's unique ID.  
        [JsonProperty(PropertyName = "tank")]
        private int ID;

        // a Vector2D representing the tank's location. 
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        // a Vector2D representing the tank's orientation. 
        [JsonProperty(PropertyName = "bdir")]
        private Vector2D orientation;

        // a Vector2D representing the direction of the tank's turret (where it's aiming). 
        [JsonProperty(PropertyName = "tdir")]
        private Vector2D aiming;

        // a string representing the player's name.
        [JsonProperty(PropertyName = "name")]
        private string name;

        // and int representing the hit points of the tank.
        [JsonProperty(PropertyName = "hp")]
        private int hitPoints = 3;

        // an int representing the player's score.
        [JsonProperty(PropertyName = "score")]
        private int score = 0;

        // a bool indicating if the tank died on that frame. 
        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        // a bool indicating if the player controlling that tank disconnected on that frame.
        [JsonProperty(PropertyName = "dc")]
        private bool disconnected = false;

        // a bool indicating if the player joined on this frame. 
        [JsonProperty(PropertyName = "join")]
        private bool joined = false;

        /// <summary>
        /// Timer to keep track of when to respawn dead tanks.
        /// </summary>
        private Stopwatch deadWatch;

        /// <summary>
        /// True if tank has shot it's first shot, false otherwise.
        /// </summary>
        private bool firstShot;

        /// <summary>
        /// Frames since the last time this tank shot.
        /// </summary>
        private long framesShot;

        /// <summary>
        /// Frames since this tank died.
        /// </summary>
        private long framesSinceDeath;

        /// <summary>
        /// Number of beam charges.
        /// </summary>
        private int charges;

        /// <summary>
        /// Bool to check if this tank requested to shoot a beam.
        /// </summary>
        private bool isBeamReady;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Tank()
        {
            deadWatch = new Stopwatch();
            firstShot = false;
            charges = 0;
            framesSinceDeath = 0;
            framesShot = 0;
            isBeamReady = false;
        }

        /// <summary>
        /// Used to check if this tank requested to fire a beam
        /// </summary>
        /// <returns></returns>
        public bool IsBeamReady()
        {
            return isBeamReady;
        }

        /// <summary>
        /// Used when tank requests to fire a beam and when it actually fires the beam.
        /// </summary>
        /// <param name="ready"></param>
        public void SetIsBeamReady(bool ready)
        {
            this.isBeamReady = ready;
        }

        /// <summary>
        /// Retreives the number of beam charges.
        /// </summary>
        /// <returns>the number of beam charges</returns>
        public int GetCharges()
        {
            return charges;
        }

        /// <summary>
        /// Called when this tank picks up a powerup.
        /// </summary>
        public void AddCharge()
        {
            charges++;
        }

        /// <summary>
        /// Called when this tank fires a beam.
        /// </summary>
        public void ChargeUsed()
        {
            charges--;
        }

        /// <summary>
        /// Gets the the number of frames since the last this tank shot.
        /// </summary>
        /// <returns></returns>
        public long GetFramesShot()
        {
            return framesShot;
        }

        /// <summary>
        /// Sets the the number of frames since the last this tank shot.
        /// </summary>
        /// <returns></returns>
        public void SetFramesShot(long frames)
        {
            framesShot = frames;
        }

        /// <summary>
        /// Gets the the number of frames since last death.
        /// </summary>
        /// <returns></returns>
        public long GetFramesSinceDeath()
        {
            return framesSinceDeath;
        }

        /// <summary>
        /// Sets the the number of frames whe  this tank dies.
        /// </summary>
        /// <returns></returns>
        public void SetFramesAtDeath(long frames)
        {
            framesSinceDeath = frames;
        }

        /// <summary>
        /// Returns the death watch for this tank.
        /// </summary>
        /// <returns></returns>
        public Stopwatch GetDeathWatch()
        {
            return deadWatch;
        }

        /// <summary>
        /// True if tank has shot it's first shot, false otherwise.
        /// </summary>
        /// <returns></returns>
        public bool GetFirstShot()
        {
            return firstShot;
        }

        /// <summary>
        /// True if tank has shot it's first shot, false otherwise.
        /// </summary>
        /// <returns></returns>
        public void TanktShot()
        {
            firstShot = true;
        }

        /// <summary>
        /// Used when a tank dies.
        /// </summary>
        public void StartDeathWatch()
        {
            deadWatch.Start();
        }

        /// <summary>
        /// Used when a tank needs to respawn.
        /// </summary>
        public void ResetDeathWatch()
        {
            deadWatch.Reset();
        }

        /// <summary>
        /// Retrieves players name.
        /// </summary>
        /// <returns> players name</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Retrieves location of tank.
        /// </summary>
        /// <returns> location of tank</returns>
        public Vector2D GetLocation()
        {
            return location;
        }

        /// <summary>
        ///  Retrieves the information if a player disconnected.
        /// </summary>
        /// <returns> information on disconnection</returns>
        public bool GetDisconnected()
        {
            return disconnected;
        }

        /// <summary>
        /// Retrieves the orientation of the tank.
        /// </summary>
        /// <returns> orentation of tank</returns>
        public Vector2D GetOrientation()
        {
            return orientation;
        }

        /// <summary>
        ///  Retrieves the direction of the tanks turrent.
        /// </summary>
        /// <returns> direction of tanks turrent</returns>
        public Vector2D GetAim()
        {
            return aiming;
        }

        /// <summary>
        /// Sets the tank's location to the given parameter.
        /// </summary>
        /// <param name="loc"></param>
        public void SetLocation(Vector2D loc)
        {
            this.location = loc;
        }

        /// <summary>
        /// Determines where the tanks turrent is going to be aiming.
        /// </summary>
        /// <param name="aim"> direction where turrent is going to be aiming</param>
        public void SetAim(Vector2D aim)
        {
            aiming = aim;
        }

        /// <summary>
        /// Sets the tank's ID to the given parameter.
        /// </summary>
        /// <param name="ID"></param>
        public void SetID(int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// Sets the tank's name to the given parameter.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Sets the tank's body direction to the given parameter.
        /// </summary>
        /// <param name="orientation"></param>
        public void SetBDir(Vector2D orientation)
        {
            this.orientation = orientation;
        }

        /// <summary>
        /// Sets the tank's turret direction to the given parameter.
        /// </summary>
        /// <param name="aiming"></param>
        public void SetTDir(Vector2D aiming)
        {
            this.aiming = aiming;
        }

        /// <summary>
        /// Sets the tank's hitPoints to the given parameter.
        /// </summary>
        /// <param name="hp"></param>
        public void SetHp(int hp)
        {
            this.hitPoints = hp;
        }

        /// <summary>
        /// Sets the tank's death to the given parameter.
        /// </summary>
        /// <param name="died"></param>
        public void SetDeath(bool died)
        {
            this.died = died;
        }

        /// <summary>
        /// Sets the tank's disconnected status to the given parameter.
        /// </summary>
        /// <param name="dc"></param>
        public void SetDisconnection(bool dc)
        {
            this.disconnected = dc;
        }

        /// <summary>
        /// Sets the tank's join status to the given parameter.
        /// </summary>
        /// <param name="joined"></param>
        public void SetJoined(bool joined)
        {
            this.joined = joined;
        }

        /// <summary>
        /// Retrieves information if a tank died or not.
        /// </summary>
        /// <returns> ianformation on tanks vitality</returns>
        public bool IsDead()
        {
            return died;
        }

        /// <summary>
        /// Retrieves information if a player joined.
        /// </summary>
        /// <returns> information on a player joining</returns>
        public bool GetJoined()
        {
            return joined;
        }

        /// <summary>
        /// Retrieves tanks ID.
        /// </summary>
        /// <returns> tanks ID</returns>
        public int GetID()
        {
            return ID;
        }

        /// <summary>
        /// Retrieves the current HP of tank.
        /// </summary>
        /// <returns> tank HP</returns>
        public int GetHp()
        {
            return hitPoints;
        }

        /// <summary>
        /// Retrieves the score of a player.
        /// </summary>
        /// <returns> player score</returns>
        public int GetScore()
        {
            return score;
        }

        /// <summary>
        /// Adds 1 to the score.
        /// </summary>
        /// <returns></returns>
        public void AddScore()
        {
            score++;
        }

        // ----------------------------------Methods concerning collision--------------------------------


        /// <summary>
        /// Respawn's tanks by setting their location to a new random location.
        /// </summary>
        /// <param name="worldSize"> size of current world</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <returns>location of respawned tank</returns>
        public static Vector2D RespawnTank(int worldSize, HashSet<Walls> walls)
        {
            Random rand = new Random();

            //random x within world
            int randX = rand.Next(-(worldSize / 2), worldSize / 2);
            //random y within world
            int randY = rand.Next(-(worldSize / 2), worldSize / 2);

            // Checks if the new random location collides with a wall. 
            while (Tank.TankCollidesWithWall(new Vector2D(randX, randY), walls))
            {
                randX = rand.Next(-(worldSize / 2), worldSize / 2);

                randY = rand.Next(-(worldSize / 2), worldSize / 2);
            }

            return new Vector2D(randX, randY);
        }


        /// <summary>
        /// Spawns a tank and sets default values of a tank.
        /// </summary>
        /// <param name="name"> provided username</param>
        /// <param name="ID"> ID of the user</param>
        /// <param name="worldSize"> size of current world</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <returns> a fresh tank</returns>
        public static Tank SpawnTank(string name, int ID, int worldSize, HashSet<Walls> walls)
        {
            Random rand = new Random();
            Tank tank = new Tank();

            tank.SetID(ID);

            //random x within world
            int randX = rand.Next(-(worldSize / 2), worldSize / 2);
            //random y within world
            int randY = rand.Next(-(worldSize / 2), worldSize / 2);

            // Checks if the new random location if tank is 
            while (Tank.TankCollidesWithWall(new Vector2D(randX, randY), walls))
            {
                randX = rand.Next(-(worldSize / 2), worldSize / 2);

                randY = rand.Next(-(worldSize / 2), worldSize / 2);
            }

            tank.SetLocation(new Vector2D(randX, randY));

            // Providing default values
            tank.SetBDir(new Vector2D(0.0, -1.0));
            tank.SetAim(new Vector2D(-1.0, 0.0));
            tank.SetName(name);
            tank.SetHp(3);
            tank.SetDeath(false);
            tank.SetDisconnection(false);
            tank.SetJoined(true);

            return tank;
        }

        /// <summary>
        /// Helper method that checks if a tank collides with a wall.
        /// </summary>
        /// <param name="loc"> a tank with its location</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <returns> true if the tank collides with a wall, false otherwise</returns>
        public static bool TankCollidesWithWall(Vector2D loc, HashSet<Walls> walls)
        {
            // Adding and subtracting the width of the tank to get the range of the tank.
            int tankX1 = (int)loc.GetX() - 30;
            int tankX2 = (int)loc.GetX() + 30;
            int centerX = (int)loc.GetX();
            int tankY1 = (int)loc.GetY() - 30;
            int tankY2 = (int)loc.GetY() + 30;
            int centerY = (int)loc.GetY();

            // Must determine if the tank came into contact with any of the walls
            foreach (Walls wall in walls)
            {
                // Vertical wall logic
                if (wall.getP1().GetX() == wall.getP2().GetX())
                {
                    // Getting the ranges of the walls.
                    int topRange = (int)Math.Min(wall.getP1().GetY(), wall.getP2().GetY()) - 25;
                    int bottomRange = (int)Math.Max(wall.getP1().GetY(), wall.getP2().GetY()) + 25;
                    int leftRange = (int)wall.getP1().GetX() - 25;
                    int rightRange = leftRange + 50;

                    // True if y collides
                    bool yCollides = false;

                    // Check if the y coordinates collide first
                    if ((tankY1 <= bottomRange && tankY1 >= topRange) || (tankY2 <= bottomRange && tankY2 >= topRange))
                    {
                        yCollides = true;
                    }

                    // Check if the x coordinates and y coordinates collides and if so it returns true.
                    if (yCollides && ((tankX1 <= rightRange && tankX1 >= leftRange) || (tankX2 <= rightRange && tankX2 >= leftRange)
                        || (centerX <= rightRange && centerX >= leftRange)))
                    {
                        return true;
                    }
                }
                // Horizantal wall logic
                if (wall.getP1().GetY() == wall.getP2().GetY())
                {
                    // Getting the ranges of the walls.
                    int rightRange = (int)Math.Max(wall.getP1().GetX(), wall.getP2().GetX()) + 25;
                    int leftRange = (int)Math.Min(wall.getP1().GetX(), wall.getP2().GetX()) - 25;
                    int topRange = (int)wall.getP1().GetY() - 25;
                    int bottomRange = topRange + 50;

                    // True if x collides
                    bool xCollides = false;

                    // Check if the x coordinates collide first
                    if ((tankX1 <= rightRange && tankX1 >= leftRange) || (tankX2 <= rightRange && tankX2 >= leftRange))
                    {
                        xCollides = true;
                    }

                    // Check if the x coordinates and y coordinates collides and if so it returns true.
                    if (xCollides && ((tankY1 <= bottomRange && tankY1 >= topRange) || (tankY2 <= bottomRange && tankY2 >= topRange)
                        || (centerY <= bottomRange && centerY >= topRange)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Method to check if tank or projectile is out of boundry.
        /// </summary>
        /// <param name="loc">a tank or projectile with its location</param>
        /// <param name="worldSize"> size of current world</param>
        /// <returns> true if tank or projectile is out of bounds, false otherwise</returns>
        public static bool HitsBoundry(Vector2D loc, int worldSize)
        {
            // Checks if its out of bounds vertically  
            if (loc.GetY() > (worldSize / 2) || loc.GetY() < -(worldSize / 2))
            {
                return true;
            }

            // Checks if it's out of bounds horizantally
            if (loc.GetX() > (worldSize / 2) || loc.GetX() < -(worldSize / 2))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents an explosion, one that occurs after the destruction of a tank. 
    public class Explosion
    {
        // position of the explosion 
        private Vector2D location;

        // information on vitality of tank
        private bool dead;

        /// <summary>
        /// Constructor that determines the location of an explosion.
        /// </summary>
        /// <param name="loc"> position of explosion</param>
        public Explosion(Vector2D loc)
        {
            location = loc;
        }

        /// <summary>
        /// Retrieves location of explosion.
        /// </summary>
        /// <returns> explosion location</returns>
        public Vector2D GetLocation()
        {
            return location;
        }

        /// <summary>
        ///  Determines the destruction of tank.
        /// </summary>
        /// <param name="death"> destruction of tank</param>
        public void SetDeath(bool death)
        {
            dead = death;
        }

        /// <summary>
        /// Retrieves information on vitality of tank.
        /// </summary>
        /// <returns>information on viatlity of tank</returns>
        public bool GetDeath()
        {
            return dead;
        }
    }
}
