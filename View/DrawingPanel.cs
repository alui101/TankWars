using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WorldSpace;

namespace View
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class is used to draw the objects in the game.
    /// </summary>
    public class DrawingPanel : Panel
    {
        private World theWorld;

        // Tanks
        private Image blueTank;
        private Image darkTank;
        private Image yellowTank;
        private Image lightGreenTank;
        private Image greenTank;
        private Image orangeTank;
        private Image redTank;
        private Image purpleTank;

        // Enviorment
        private Image backGround;
        private Image wall;

        // Turrents
        private Image blueTurret;
        private Image darkTurret;
        private Image yellowTurret;
        private Image lightGreenTurret;
        private Image greenTurret;
        private Image orangeTurret;
        private Image redTurret;
        private Image purpleTurret;

        // Shots
        private Image blueShot;
        private Image darkShot;
        private Image yellowShot;
        private Image lightGreenShot;
        private Image greenShot;
        private Image orangeShot;
        private Image redShot;
        private Image purpleShot;

        // Players position
        private double playerX;
        private double playerY;

        // Determines if the user on a server
        private bool connected;

        // Used to animate explosions and beams
        private int numFrames;

        // Provided username 
        private string name;

        /// <summary>
        /// Preparing the malleable fields and images.
        /// </summary>
        /// <param name="w"> provided world</param>
        public DrawingPanel(World w)
        {
            //Initializing fields
            DoubleBuffered = true;
            connected = false;
            theWorld = w;
            numFrames = 0;
            connected = false;
            name = "";

            //Images

            backGround = Image.FromFile("..\\..\\..\\Resources\\Images\\Background.png");

            blueTank = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTank.png");
            darkTank = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTank.png");
            yellowTank = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTank.png");
            greenTank = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTank.png");
            lightGreenTank = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTank.png");
            orangeTank = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTank.png");
            redTank = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTank.png");
            purpleTank = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTank.png");

            blueTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\BlueTurret.png");
            darkTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\DarkTurret.png");
            yellowTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\YellowTurret.png");
            greenTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\GreenTurret.png");
            lightGreenTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\LightGreenTurret.png");
            orangeTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\OrangeTurret.png");
            redTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\RedTurret.png");
            purpleTurret = Image.FromFile("..\\..\\..\\Resources\\Images\\PurpleTurret.png");

            blueShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-blue.png");
            darkShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-grey.png");
            yellowShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-yellow.png");
            greenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png");
            lightGreenShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-green.png");
            orangeShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-white.png");
            redShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-red.png");
            purpleShot = Image.FromFile("..\\..\\..\\Resources\\Images\\shot-violet.png");

            wall = Image.FromFile("..\\..\\..\\Resources\\Images\\WallSprite.png");

        }

        // A delegate for DrawObjectWithTransform
        // Methods matching this delegate can draw whatever they want using e  
        public delegate void ObjectDrawer(object o, PaintEventArgs e);


        /// <summary>
        /// This method performs a translation and rotation to draw an object in the world.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        /// <param name="o">The object to draw</param>
        /// <param name="worldX">The X coordinate of the object in world space</param>
        /// <param name="worldY">The Y coordinate of the object in world space</param>
        /// <param name="angle">The orientation of the objec, measured in degrees clockwise from "up"</param>
        /// <param name="drawer">The drawer delegate. After the transformation is applied, the delegate is invoked to draw whatever it wants</param>
        private void DrawObjectWithTransform(PaintEventArgs e, object o, double worldX, double worldY, double angle, ObjectDrawer drawer)
        {
            // "push" the current transform
            System.Drawing.Drawing2D.Matrix oldMatrix = e.Graphics.Transform.Clone();

            e.Graphics.TranslateTransform((int)worldX, (int)worldY);
            e.Graphics.RotateTransform((float)angle);

            drawer(o, e);

            // "pop" the transform
            e.Graphics.Transform = oldMatrix;

        }

        /// <summary>
        /// Before any objects have been drawn, the background is drawn first.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void BackgroundDrawer(PaintEventArgs e)
        {
            Rectangle r = new Rectangle(-1000, -1000, 2000, 2000);
            e.Graphics.DrawImage(backGround, r);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the explosions.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void ExplosionDrawer(object o, PaintEventArgs e)
        {
            Explosion p = o as Explosion;

            if (numFrames >= 60) // explosion is going to expand with every frame
            {
                p.SetDeath(true);
                numFrames = 0;
            }

            if (!p.GetDeath()) // continues the animation until the explosion has ceased
            {

                int width = 8;
                int height = 8;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                {
                    // Creates a group of ellipses that then seperate in opposite directions, resembling an explosion
                    Rectangle r = new Rectangle(0 + numFrames, 0, width, height);
                    Rectangle r1 = new Rectangle(0, 0 + numFrames, width, height);
                    Rectangle r2 = new Rectangle(0 + numFrames, 0 + numFrames, width, height);
                    Rectangle r3 = new Rectangle(0 - numFrames, 0, width, height);
                    Rectangle r4 = new Rectangle(0, 0 - numFrames, width, height);
                    Rectangle r5 = new Rectangle(0 - numFrames, 0 - numFrames, width, height);
                    Rectangle r6 = new Rectangle(0 - numFrames, 0 + numFrames, width, height);
                    Rectangle r7 = new Rectangle(0 + numFrames, 0 - numFrames, width, height);
                    e.Graphics.FillEllipse(redBrush, r);
                    e.Graphics.FillEllipse(redBrush, r1);
                    e.Graphics.FillEllipse(redBrush, r2);
                    e.Graphics.FillEllipse(redBrush, r3);
                    e.Graphics.FillEllipse(redBrush, r4);
                    e.Graphics.FillEllipse(redBrush, r5);
                    e.Graphics.FillEllipse(redBrush, r6);
                    e.Graphics.FillEllipse(redBrush, r7);
                }
                numFrames++;
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the power ups.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void PowerUpDrawer(object o, PaintEventArgs e)
        {
            int width = 15;
            int height = 15;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
            using (System.Drawing.SolidBrush blueBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Blue))
            {
                // Circles are drawn starting from the top-left corner.
                // So if we want the circle centered on the powerup's location, we have to offset it
                // by half its size to the left (-width/2) and up (-height/2)
                Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
                Rectangle r1 = new Rectangle(-(width / 2), -(height / 2), width + 5, height + 5);

                e.Graphics.FillEllipse(blueBrush, r1);
                e.Graphics.FillEllipse(redBrush, r);
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the walls.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void WallDrawer(object o, PaintEventArgs e)
        {
            Walls wall = o as Walls;

            // the length between p1 and p2 will always be a multiple of the wall width (50 units)
            int width = 50;
            int height = 50;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);
            e.Graphics.DrawImage(this.wall, r);
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the scores.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void ScoreDrawer(object o, PaintEventArgs e)
        {
            // converting the provided dictionary and storing it into dictionary
            var dictionary = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            // obtaining a tank object from dictionary
            Tank tank = JsonConvert.DeserializeObject<Tank>(dictionary["Value"].ToString());

            using (Font font1 = new Font("Times New Roman", 20, FontStyle.Regular, GraphicsUnit.Pixel))
            {
                PointF pointF1 = new PointF(-25 - tank.GetName().Length * 4, 28);
                e.Graphics.DrawString(tank.GetName() + ": " + tank.GetScore(), font1, Brushes.White, pointF1);
            }
        }

        private void HpDrawer(object o, PaintEventArgs e)
        {
            var d = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            Tank tank = JsonConvert.DeserializeObject<Tank>(d["Value"].ToString());

            Rectangle r = new Rectangle(-30, -48, 59, 10);
            e.Graphics.DrawRectangle(Pens.Black, r);
            if (tank.GetHp() == 3)
            {
                using (System.Drawing.SolidBrush greenBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green))
                {
                    e.Graphics.FillRectangle(greenBrush, r);
                }
            }

            if (tank.GetHp() == 2)
            {
                using (System.Drawing.SolidBrush yellowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Yellow))
                {
                    Rectangle r2 = new Rectangle(-30, -48, 40, 10);
                    e.Graphics.FillRectangle(yellowBrush, r2);
                }
            }
            if (tank.GetHp() == 1)
            {
                using (System.Drawing.SolidBrush redBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                {
                    Rectangle r3 = new Rectangle(-30, -48, 20, 10);
                    e.Graphics.FillRectangle(redBrush, r3);
                }
            }

        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the projectiles.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void ProjDrawer(object o, PaintEventArgs e)
        {
            // converting the provided dictionary and storing it into dictionary
            var dictionary = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            // obtaining a projectile object from dictionary
            Projectile p = JsonConvert.DeserializeObject<Projectile>(dictionary["Value"].ToString());

            //Projectiles are drawn 30x30 pixels
            int width = 30;
            int height = 30;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

            int id = p.getOwner() % 8; // ensures that the first 8 users will get a unique color of projectile

            switch (id)
            {
                case 1:
                    e.Graphics.DrawImage(darkShot, r);
                    break;
                case 2:
                    e.Graphics.DrawImage(yellowShot, r);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenShot, r);
                    break;
                case 4:
                    e.Graphics.DrawImage(lightGreenShot, r);
                    break;
                case 5:
                    e.Graphics.DrawImage(orangeShot, r);
                    break;
                case 6:
                    e.Graphics.DrawImage(redShot, r);
                    break;
                case 7:
                    e.Graphics.DrawImage(purpleShot, r);
                    break;
                default:
                    e.Graphics.DrawImage(blueShot, r);
                    break;
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the beams.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void beamDrawer(object o, PaintEventArgs e)
        {
            // converting the provided dictionary and storing it into dictionary
            var dictionary = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            int id = (int)dictionary["Key"] % 8; // ensures that the first 8 users will get a unique color of beam

            using (Pen pen = new Pen(Color.White, 4.0f - numFrames)) // makes beam narrower, although this does not completely remove it
            {
                pen.DashStyle = DashStyle.Dash; // stylizes beam

                switch (id)
                {
                    case 1:
                        pen.Color = Color.Black;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 2:
                        pen.Color = Color.Yellow;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 3:
                        pen.Color = Color.Green;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 4:
                        pen.Color = Color.LightGreen;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 5:
                        pen.Color = Color.Orange;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 6:
                        pen.Color = Color.Red;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    case 7:
                        pen.Color = Color.Purple;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                    default:
                        pen.Color = Color.Blue;
                        e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, -2000));
                        break;
                }
            }

            numFrames++;
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the turrents.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void TurretDrawer(object o, PaintEventArgs e)
        {
            // converting the provided dictionary and storing it into dictionary
            var dictionary = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            // Turrets are drawn 50x50 pixels
            int width = 50;
            int height = 50;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

            int id = (int)dictionary["Key"] % 8; // ensures that the first 8 users will get a unique color of turrent

            switch (id)
            {
                case 1:
                    e.Graphics.DrawImage(darkTurret, r);
                    break;
                case 2:
                    e.Graphics.DrawImage(yellowTurret, r);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenTurret, r);
                    break;
                case 4:
                    e.Graphics.DrawImage(lightGreenTurret, r);
                    break;
                case 5:
                    e.Graphics.DrawImage(orangeTurret, r);
                    break;
                case 6:
                    e.Graphics.DrawImage(redTurret, r);
                    break;
                case 7:
                    e.Graphics.DrawImage(purpleTurret, r);
                    break;
                default:
                    e.Graphics.DrawImage(blueTurret, r);
                    break;
            }
        }

        /// <summary>
        /// Acts as a drawing delegate for DrawObjectWithTransform
        /// After performing the necessary transformation (translate/rotate)
        /// DrawObjectWithTransform will invoke this method that will draw the tanks.
        /// </summary>
        /// <param name="o">The object to draw</param>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        private void TankDrawer(object o, PaintEventArgs e)
        {
            // converting the provided dictionary and storing it into dictionary
            var dictionary = JObject.FromObject(o).ToObject<Dictionary<object, object>>();

            // Tanks are drawn 60x60 pixels
            int width = 60;
            int height = 60;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Rectangles are drawn starting from the top-left corner.
            // So if we want the rectangle centered on the player's location, we have to offset it
            // by half its size to the left (-width/2) and up (-height/2)
            Rectangle r = new Rectangle(-(width / 2), -(height / 2), width, height);

            int id = (int)dictionary["Key"] % 8; // ensures that the first 8 users will get a unique color of Tank

            switch (id)
            {
                case 1:
                    e.Graphics.DrawImage(darkTank, r);
                    break;
                case 2:
                    e.Graphics.DrawImage(yellowTank, r);
                    break;
                case 3:
                    e.Graphics.DrawImage(greenTank, r);
                    break;
                case 4:
                    e.Graphics.DrawImage(lightGreenTank, r);
                    break;
                case 5:
                    e.Graphics.DrawImage(orangeTank, r);
                    break;
                case 6:
                    e.Graphics.DrawImage(redTank, r);
                    break;
                case 7:
                    e.Graphics.DrawImage(purpleTank, r);
                    break;
                default:
                    e.Graphics.DrawImage(blueTank, r);
                    break;
            }
        }

        /// <summary>
        /// Draws all of the objects contained within this game.
        /// Invoked when the DrawingPanel needs to be re-drawn.
        /// </summary>
        /// <param name="e">PaintEventArgs to access the graphics (for drawing)</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            int ID = theWorld.getID();

            bool dead = theWorld.getDeath();

            // sometimes ID does not get assigned correctly, must be connected
            if (connected && ID != -1)
            {
                Tank player = theWorld.getTank(ID);

                if (player != null && !dead)
                {
                    this.playerX = player.GetLocation().GetX();
                    this.playerY = player.GetLocation().GetY();

                    // Center the view on the middle of the world,
                    // since the image and world use different coordinate systems
                    int viewSize = Size.Width; // view is square, so we can just use width
                    e.Graphics.TranslateTransform((float)-playerX + (viewSize / 2), (float)-playerY + (viewSize / 2));
                }

                if (dead)
                {
                    int viewSize = Size.Width; // view is square, so we can just use width
                    e.Graphics.TranslateTransform((float)-playerX + (viewSize / 2), (float)-playerY + (viewSize / 2));
                }
            }

            // draws the background
            BackgroundDrawer(e);

            lock (theWorld)
            {
                // Draw the players
                foreach (KeyValuePair<int, Tank> tank in theWorld.getTanks())
                {
                    // tank
                    DrawObjectWithTransform(e, tank, tank.Value.GetLocation().GetX(), tank.Value.GetLocation().GetY(), tank.Value.GetOrientation().ToAngle(), TankDrawer);

                    // turrent

                    DrawObjectWithTransform(e, tank, tank.Value.GetLocation().GetX(), tank.Value.GetLocation().GetY(), tank.Value.GetAim().ToAngle(), TurretDrawer);

                    //score
                    DrawObjectWithTransform(e, tank, tank.Value.GetLocation().GetX(), tank.Value.GetLocation().GetY(), 0, ScoreDrawer);
                    DrawObjectWithTransform(e, tank, tank.Value.GetLocation().GetX(), tank.Value.GetLocation().GetY(), 0, HpDrawer);
                }

                // Draw the projectiles
                foreach (KeyValuePair<int, Projectile> proj in theWorld.getProjectiles())
                {
                    DrawObjectWithTransform(e, proj, proj.Value.getLocation().GetX(), proj.Value.getLocation().GetY(), proj.Value.getOrientation().ToAngle(), ProjDrawer);
                }

                // Draw the powerups
                foreach (KeyValuePair<int, Powerups> powerup in theWorld.getPowerUps())
                {
                    DrawObjectWithTransform(e, powerup, powerup.Value.getLocation().GetX(), powerup.Value.getLocation().GetY(), 0, PowerUpDrawer);
                }

                // Draw the walls
                foreach (KeyValuePair<int, Walls> wall in theWorld.getWalls())
                {
                    //Walls will always be axis-aligned (purely horizontal or purely vertical, never diagonal).
                    //This means p1 and p2 will have either the same x value or the same y value.

                    if (wall.Value.getP1().GetY() == wall.Value.getP2().GetY())
                    {
                        double x1 = wall.Value.getP1().GetX();
                        double x2 = wall.Value.getP2().GetX();

                        double min = Math.Min(x1, x2);
                        double max = Math.Max(x1, x2);

                        // Enumerates through every block, incrementing by 50
                        for (double i = min; i <= max; i += 50)
                        {
                            DrawObjectWithTransform(e, wall, i, wall.Value.getP2().GetY(), 0, WallDrawer);
                        }
                    }

                    if (wall.Value.getP1().GetX() == wall.Value.getP2().GetX())
                    {
                        double y1 = wall.Value.getP1().GetY();
                        double y2 = wall.Value.getP2().GetY();

                        double min = Math.Min(y1, y2);
                        double max = Math.Max(y1, y2);

                        // Enumerates through every block, incrementing by 50
                        for (double i = min; i <= max; i += 50)
                        {
                            DrawObjectWithTransform(e, wall, wall.Value.getP2().GetX(), i, 0, WallDrawer);
                        }
                    }
                }

                // Draw the explosions
                foreach (Explosion exp in theWorld.getExplosion())
                {
                    if (!exp.GetDeath())
                    {
                        DrawObjectWithTransform(e, exp, exp.GetLocation().GetX(), exp.GetLocation().GetY(), 0, ExplosionDrawer);
                    }
                }

                List<KeyValuePair<int, Beams>> beamsThatNeedToBeRemoved = new List<KeyValuePair<int, Beams>>();
                // Draw the beams
                foreach (KeyValuePair<int, Beams> beam in theWorld.getBeams())
                {
                    double x = beam.Value.getOrigin().GetX();
                    double y = beam.Value.getOrigin().GetY();

                    double angle = beam.Value.getDirection().ToAngle();
                    DrawObjectWithTransform(e, beam, beam.Value.getOrigin().GetX(), beam.Value.getOrigin().GetY(), beam.Value.getDirection().ToAngle(), beamDrawer);

                    if (numFrames >= 60)
                    {
                        // collect the beams that need to be removed
                        beamsThatNeedToBeRemoved.Add(beam);
                        numFrames = 0;
                    }
                }

                // removing the beams to complete the animation, this was done in its own seperate loop as you cannot
                // remove while iterating simultaneously.
                foreach (KeyValuePair<int, Beams> beam in beamsThatNeedToBeRemoved)
                {
                    theWorld.removeBeam(beam.Key);
                }

            }

            // Do anything that Panel (from which we inherit) needs to do
            base.OnPaint(e);
        }

        /// <summary>
        /// Determines if the current user is connected to a server.
        /// </summary>
        /// <param name="connected"> determines if connected</param>
        public void setConnection(bool connected)
        {
            this.connected = connected;
        }

        /// <summary>
        /// Determines the current users username.
        /// </summary>
        /// <param name="name">provided username</param>
        public void setName(string name)
        {
            this.name = name;
        }

    }
}

