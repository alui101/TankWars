using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TankWars;

namespace WorldSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class represents a powerup. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]

    public class Powerups
    {
        // an int representing the powerup's unique ID.
        [JsonProperty(PropertyName = "power")]
        private int ID;

        // a Vector2D representing the location of the powerup.
        [JsonProperty(PropertyName = "loc")]
        private Vector2D loc;

        // a bool indicating if the powerup "died" (was collected by a player) on this frame.
        [JsonProperty(PropertyName = "died")]
        private bool died;

        /// <summary>
        /// default constructor.
        /// </summary>
        public Powerups()
        {
        }

        /// <summary>
        /// Retrieves the location.
        /// </summary>
        /// <returns>location of powerup</returns>
        public Vector2D getLocation()
        {
            return loc;
        }

        /// <summary>
        /// Determines if the powerup had been collected. 
        /// </summary>
        /// <returns>had been collected</returns>
        public bool getDeath()
        {
            return died;
        }

        /// <summary>
        /// Retrieves the powerups ID.
        /// </summary>
        /// <returns>powerups ID</returns>
        public int getID()
        {
            return ID;
        }

        /// <summary>
        /// Used when powerup is picked.
        /// </summary>
        /// <param name="dead"></param>
        public void SetDeath(bool dead)
        {
            this.died = dead;
        }

        /// <summary>
        /// Used when powerup is created.
        /// </summary>
        /// <param name="dead"></param>
        public void SetLocation(Vector2D loc)
        {
            this.loc = loc;
        }

        /// <summary>
        /// Used when powerup is created.
        /// </summary>
        /// <param name="dead"></param>
        public void SetID(int ID)
        {
            this.ID = ID;
        }

        // ----------------------------------Methods concerning collision--------------------------------


        /// <summary>
        /// Spawns a powerup in a random place in the world where it does not collide with wall.
        /// </summary>
        /// <param name="worldSize"> size of current world</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <param name="theWorld"> current world holding all components of the game</param>
        public static void SpawnPowerup(int worldSize, HashSet<Walls> walls,World theWorld)
        {
            Random rand = new Random();

            //random x within world
            int randX = rand.Next(-(worldSize / 2), worldSize / 2);
            //random y within world
            int randY = rand.Next(-(worldSize / 2), worldSize / 2);

            // Checks if the new random location collides with a wall. 
            while (Projectile.PrPCollidesWithWall(new Vector2D(randX, randY), walls))
            {
                randX = rand.Next(-(worldSize / 2) + 25, worldSize / 2 - 25);

                randY = rand.Next(-(worldSize / 2) + 25, worldSize / 2 - 25);
            }
            // Creates a power up then adds it to the world.
            Powerups powerup = new Powerups();
            powerup.SetDeath(false);
            powerup.SetID(theWorld.getPowerupID());
            powerup.SetLocation(new Vector2D(randX, randY));

            theWorld.addPowerUp(theWorld.getPowerupID(), powerup);

            theWorld.setPowerupID(theWorld.getPowerupID()+1);
        }

        /// <summary>
        /// Helper method that checks if a tank came into contact with a powerup.
        /// </summary>
        /// <param name="powerup"> a powerup with its location</param>
        /// <param name="theWorld"> current world holding all components of the game</param>
        public static void TankCollidesWithPowerUp(Powerups powerup, World theWorld)
        {
            // The x and y location of a projectile
            int pX = (int)powerup.getLocation().GetX();
            int pY = (int)powerup.getLocation().GetY();

            // Must determine if the projectile came into contact with any of the tanks
            foreach (Tank tank in theWorld.getTanks().Values)
            {

                // Adding and subtracting the width of the tank to get the range of the tank.
                int tankX1 = (int)tank.GetLocation().GetX() - 30;
                int tankX2 = (int)tank.GetLocation().GetX() + 30;
                int tankY1 = (int)tank.GetLocation().GetY() - 30;
                int tankY2 = (int)tank.GetLocation().GetY() + 30;

                // True if y collides
                bool yCollides = false;

                // Check if the y coordinates collide first
                if (pY <= tankY2 && pY >= tankY1)
                {
                    yCollides = true;
                }

                // Check if the x coordinates and y coordinates collides and if so it returns true.
                if (yCollides && pX <= tankX2 && pX >= tankX1)
                {
                    tank.AddCharge();
                    theWorld.setPowerUpDeath(powerup.getID(), true);
                    theWorld.addDeadPowerUp(powerup);
                }
            }
        }
    }
}
