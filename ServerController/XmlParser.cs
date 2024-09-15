using System;
using System.Collections.Generic;
using System.Xml;
using TankWars;
using WorldSpace;

namespace ServerControllerSpace
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class Reads an XML settings file to determine the rules and settings of the game.
    /// </summary>
    class XmlParser
    {
        /// <summary>
        /// List of walls parsed from the settings file.
        /// </summary>
        private HashSet<Walls> walls;

        /// <summary>
        /// The universe size.
        /// </summary>
        private int worldSize;

        /// <summary>
        /// Current game mode(one of two options).
        /// </summary>
        private bool GameMode;

        /// <summary>
        ///  Mili Seconds per frame.
        /// </summary>
        private int MSPerFrame;

        /// <summary>
        /// Delay between firing projectiles.
        /// </summary>
        private int framesPerShot;

        /// <summary>
        /// How long it takes to respawn after you die.
        /// </summary>
        private int respawnRate;

        /// <summary>
        /// Used to ID walls.
        /// </summary>
        private int wallID;

        /// <summary>
        /// A constructor that takes in the filePath of a xml file and parses it.
        /// </summary>
        /// <param name="filePath"></param>
        public XmlParser(string filePath)
        {
            walls = new HashSet<Walls>();
            wallID = 0;
            try
            {

                using (XmlReader reader = XmlReader.Create(filePath))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            // captures provided values, gets more complex when capturing walls.
                            switch (reader.Name)
                            {
                                case "GameMode":
                                    reader.Read();
                                    GameMode = Convert.ToBoolean(reader.Value);
                                    break;
                                case "UniverseSize":
                                    reader.Read();
                                    worldSize = Convert.ToInt32(reader.Value);
                                    break;
                                case "MSPerFrame":
                                    reader.Read();
                                    MSPerFrame = Convert.ToInt32(reader.Value);
                                    break;
                                case "FramesPerShot":
                                    reader.Read();
                                    framesPerShot = Convert.ToInt32(reader.Value);
                                    break;
                                case "RespawnRate":
                                    reader.Read();
                                    respawnRate = Convert.ToInt32(reader.Value);
                                    break;
                                case "Wall":
                                    // Used to capture point values of walls
                                    int p1X = 0;
                                    int p2X = 0;
                                    int p1Y = 0;
                                    int p2Y = 0;

                                    bool doneP1 = false; // indicates when it capures a walls P1
                                    bool doneP2 = false; // indicates when it captures a walls P2
                                    while (reader.Read())
                                    {
                                        if (reader.IsStartElement())
                                        {
                                            switch (reader.Name)
                                            {
                                                case "p1": // Captures the first point P1
                                                    while (reader.Read())
                                                    {
                                                        if (doneP1)
                                                        {
                                                            break;
                                                        }
                                                        if (reader.IsStartElement())
                                                        {
                                                            switch (reader.Name)
                                                            {
                                                                case "x":
                                                                    reader.Read();
                                                                    p1X = Convert.ToInt32(reader.Value);
                                                                    break;
                                                                case "y":
                                                                    reader.Read();
                                                                    p1Y = Convert.ToInt32(reader.Value);
                                                                    doneP1 = true;
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                                case "p2": // Captures the second point P2
                                                    while (reader.Read())
                                                    {
                                                        if (doneP2)
                                                        {
                                                            break;
                                                        }

                                                        if (reader.IsStartElement())
                                                        {
                                                            switch (reader.Name)
                                                            {
                                                                case "x":
                                                                    reader.Read();
                                                                    p2X = Convert.ToInt32(reader.Value);
                                                                    break;
                                                                case "y":
                                                                    reader.Read();
                                                                    p2Y = Convert.ToInt32(reader.Value);
                                                                    doneP2 = true;
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }

                                        }

                                        // Once both points are captured from a specific wall, the wall can be made and included.
                                        if (doneP1 && doneP2) 
                                        {
                                            Walls wall = new Walls();
                                            wall.setP1(new Vector2D(p1X, p1Y));
                                            wall.setP2(new Vector2D(p2X, p2Y));
                                            wall.setID(wallID);
                                            walls.Add(wall);
                                            wallID++;
                                            break;
                                        }
                                    }
                                    break; // break for the wall
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Settings file is corrupted");
            }
        }

        /// <summary>
        /// Retrieves the chosen gamemode.
        /// </summary>
        /// <returns> a gamemode</returns>
        public bool GetGameMode()
        {
            return GameMode;
        }

        /// <summary>
        /// Returns the world size from the settings file.
        /// </summary>
        /// <returns>a worldsize</returns>
        public int GetWorldSize()
        {
            return worldSize;
        }

        /// <summary>
        /// Returns the MSPerFrame from the settings file.
        /// </summary>
        /// <returns>a ms perframe</returns>
        public int GetMSPerFrame()
        {
            return MSPerFrame;
        }

        /// <summary>
        /// Returns the framesPerShot from the settings file.
        /// </summary>
        /// <returns>a frames per shot</returns>
        public int GetFramesPerShot()
        {
            return framesPerShot;
        }

        /// <summary>
        /// Returns the respawnRate from the settings file.
        /// </summary>
        /// <returns>a spawn rate</returns>
        public int GetRespawnRate()
        {
            return respawnRate;
        }


        /// <summary>
        /// Returns the list of walls from the settings.
        /// </summary>
        /// <returns> provided walls</returns>
        public HashSet<Walls> GetWalls()
        {
            return walls;
        }

    }
}
