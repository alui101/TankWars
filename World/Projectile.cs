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
    /// This class represents a Projectile. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        // an int representing the projectile's unique ID.
        [JsonProperty(PropertyName = "proj")]
        private int ID;

        // a Vector2D representing the projectile's location.
        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        // a Vector2D representing the projectile's orientation.
        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;

        // a bool representing if the projectile died on this frame (hit something or left the bounds of the world).
        [JsonProperty(PropertyName = "died")]
        private bool died;

        // an int representing the ID of the tank that created the projectile.
        [JsonProperty(PropertyName = "owner")]
        private int owner;

        /// <summary>
        /// default constructor.
        /// </summary>
        public Projectile()
        {
        }

        /// <summary>
        /// Constructor to use when creating projectiles in the server.
        /// </summary>
        /// <param name="ID"> unique ID identifying a projectile</param>
        /// <param name="loc"> location of projectile</param>
        /// <param name="orientation"> orientation of projectile</param>
        /// <param name="died"> indicates wheter it has died</param>
        /// <param name="owner"> contains information on who owns the projectile</param>
        public Projectile(int ID, Vector2D loc, Vector2D orientation, bool died, int owner)
        {
            this.ID = ID;
            this.location = loc;
            this.orientation = orientation;
            this.died = died;
            this.owner = owner;
        }

        /// <summary>
        ///  Sets the projectiles location.
        /// </summary>
        public void setLocation(Vector2D location)
        {
            this.location = location;
        }

        /// <summary>
        /// Sets the validity of a projectile.
        /// </summary>
        /// <param name="died"> determines if projectile died</param>
        public void setDeath(bool died)
        {
            this.died = died;
        }

        /// <summary>
        /// Retrieves the projectiles location.
        /// </summary>
        /// <returns> projectiles location</returns>
        public Vector2D getLocation()
        {
            return location;
        }

        /// <summary>
        /// Retrieves the projectiles orientation.
        /// </summary>
        /// <returns> projectiles orientation</returns>
        public Vector2D getOrientation()
        {
            return orientation;
        }

        /// <summary>
        /// Determines if the projectile died.
        /// </summary>
        /// <returns> if projectile died</returns>
        public bool getDeath()
        {
            return died;
        }

        /// <summary>
        /// Retrieves a projectiles ID.
        /// </summary>
        /// <returns> projectiles ID</returns>
        public int getID()
        {
            return ID;
        }

        /// <summary>
        /// Retrieves the ID of the tank that shot the projectile.
        /// </summary>
        /// <returns> ID of tank that shot</returns>
        public int getOwner()
        {
            return owner;
        }

        // ----------------------------------Methods concerning collision and physics--------------------------------


        /// <summary>
        /// Determines motion of a projectile.
        /// </summary>
        /// <param name="proj">provided projectile</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <param name="worldSize"> size of current world</param>
        /// <param name="theWorld"> current world holding all components of the game</param>
        /// <param name="gameMode"> determines what gamemode is on</param>
        /// <returns></returns>
        public static bool ProcessProj(Projectile proj, HashSet<Walls> walls,int worldSize, World theWorld,bool gameMode)
        {
            Vector2D velocity = proj.getOrientation() * 25;

            Vector2D oldLoc = proj.getLocation();
            Vector2D newLoc = velocity + oldLoc;

            // If it collides with a wall or a tank, and if it goes out of boundary the projectile should die.
            if (PrPCollidesWithWall(proj.getLocation(),walls) || ProjCollidesWithTank(proj,theWorld, gameMode) || Tank.HitsBoundry(proj.getLocation(),worldSize))
            {
                proj.setDeath(true);
                return true; // true if proj died.
            }
            else
            {
                proj.setLocation(newLoc);
            }
            return false;
        }

        /// <summary>
        /// Helper method that checks if a projectile or a powerup colides with a wall.
        /// </summary>
        /// <param name="p"> location of powerup/projectile</param>
        /// <param name="walls"> contains all of the walls of the world</param>
        /// <returns>a bool that determines if a collision occured</returns>
        public static bool PrPCollidesWithWall(Vector2D p, HashSet<Walls> walls)
        {
            // The x and y location of a projectile
            int pX = (int)p.GetX();
            int pY = (int)p.GetY();

            // Must determine if the projectile came into contact with any of the walls
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
                    if (pY <= bottomRange && pY >= topRange)
                    {
                        yCollides = true;
                    }

                    // Check if the x coordinates and y coordinates collides and if so it returns true.
                    if (yCollides && pX <= rightRange && pX >= leftRange)
                    {
                        return true;
                    }
                }
                // Horizantal wall logic
                if (wall.getP1().GetY() == wall.getP2().GetY())
                {
                    //// Getting the ranges of the walls.
                    int rightRange = (int)Math.Max(wall.getP1().GetX(), wall.getP2().GetX()) + 25;
                    int leftRange = (int)Math.Min(wall.getP1().GetX(), wall.getP2().GetX()) - 25;

                    int topRange = (int)wall.getP1().GetY() - 25;
                    int bottomRange = topRange + 50;

                    // True if y collides
                    bool yCollides = false;

                    // Check if the y coordinates collide first
                    if (pY <= bottomRange && pY >= topRange)
                    {
                        yCollides = true;
                    }

                    // Check if the x coordinates and y coordinates collides and if so it returns true.
                    if (yCollides && pX <= rightRange && pX >= leftRange)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Helper method that checks if a projectile hits a tank.
        /// </summary>
        /// <param name="proj">a projectile with its location</param>
        /// <param name="theWorld"> current world holding all components of the game</param>
        /// <param name="gameMode"> determines what gamemode is on</param>
        /// <returns>a bool that determines if a collision occured</returns>
        public static bool ProjCollidesWithTank(Projectile proj, World theWorld, bool gameMode)
        {
            // The x and y location of a projectile
            int pX = (int)proj.getLocation().GetX();
            int pY = (int)proj.getLocation().GetY();

            // Must determine if the projectile came into contact with any of the tanks
            foreach (Tank tank in theWorld.getTanks().Values)
            {
                // Collision detection on own tank is avoided
                if (proj.getOwner() == tank.GetID())
                {
                    continue;
                }

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
                    // If the game mode is turned on
                    if (gameMode)
                    {
                        // Checks if the projectiles owner and the tank that got hit are on the same team.
                        if (proj.getOwner() % 2 == tank.GetID() % 2)
                        {
                            return true;
                        }
                    }

                    // If tank is at 1 hp then kill it and add the score to the projectiles owner.
                    if (tank.GetHp() == 1)
                    {
                        tank.SetDeath(true);
                        tank.SetHp(0);
                        theWorld.addDeadTank(tank);
                        theWorld.getTank(proj.getOwner()).AddScore();
                    }
                    else if (tank.GetHp() == 0)
                    {// If tank is dead don't detect it.
                        return false;
                    }
                    else
                    {
                        tank.SetHp(tank.GetHp() - 1); // Tank took a point of damage
                    }
                    return true;
                }

            }
            return false;

        }

    }
}
