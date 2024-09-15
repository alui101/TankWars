using System;
using System.Drawing;
using System.Windows.Forms;
using Controller;
using TankWars;

namespace View
{
    /// <summary>
    /// Author : Ali Hassoun & Emiliano Plaza, Spring 2021
    /// 
    /// University of Utah
    /// 
    /// This class registers basic event handlers for user inputs and controller events. It also displays error messages.
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// The controller object that handles 
        /// communication and logic
        /// </summary>
        private GameController controller;

        /// <summary>
        /// component where all of the objects are drawn
        /// </summary>
        private DrawingPanel drawingPanel;

        /// <summary>
        /// Initializes components and sets 'default' values for the GUI.
        /// </summary>
        /// <param name="c"> provided controller</param>
        public Form1(GameController c)
        {
            InitializeComponent();

            this.Text = "PewPewPew";

            controller = c;

            // Default values for server and name boxes
            serverBox.Text = "localhost";
            nameBox.Text = "player";

            // event handlers for managing information provided by the controller
            controller.UpdateArrived += OnFrame;
            controller.sendMessage += displayErrorMessage;

            // Implements drawing panel

            drawingPanel = new DrawingPanel(controller.getWorld());
            drawingPanel.Size = new Size(900, 900);
            drawingPanel.Location = new Point(-39 + this.ClientSize.Width / 2 - drawingPanel.Size.Width / 2, 35);
            drawingPanel.BackColor = Color.Black;

            drawingPanel.Enabled = false;

            this.Controls.Add(drawingPanel);

            this.AutoSize = false;

        }

        /// <summary>
        /// Handler for the controller's sendMessage event.
        /// Provides a way to display connection errors and to allow users to attempt another connection.
        /// </summary>
        private void displayErrorMessage()
        {
            MessageBox.Show("unable to connect to server");

            // accesses any form control from a thread
            Invoke(new Action(() =>
            {
                ConnectButton.Enabled = true;
                serverBox.Enabled = true;
                nameBox.Enabled = true;
            }));
        }

        /// <summary>
        /// Handler for the controller's UpdateArrived event.
        /// </summary>
        private void OnFrame()
        {
            // Invalidate this form and all its children
            // This will cause the form to redraw as soon as it can
            try
            {
                MethodInvoker invalidator = new MethodInvoker(() => drawingPanel.Refresh());
                this.Invoke(invalidator);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Allows the user to connect to a server at a certain address.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (serverBox.Text.Length == 0)
            {
                MessageBox.Show("Please enter a server address");
            }
            else
            {
                if (nameBox.Text.Length > 16)
                {
                    MessageBox.Show("Name cannot be longer than 16 characters.");
                }
                else if(nameBox.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a name.");
                }
                else
                {
                    controller.Connect(serverBox.Text, nameBox.Text);
                    drawingPanel.setConnection(true);
                    drawingPanel.setName(nameBox.Text);

                    // must not allow any more input once client has joined a server
                    ConnectButton.Enabled = false;
                    serverBox.Enabled = false;
                    nameBox.Enabled = false;
                }
            }

        }

        /// <summary>
        /// Handles the command keys when pressed.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // The View should just translate the raw input data and then pass that to the controller.
            // The controller should decide what to do when those inputs happen.
            // This allows us to avoid complications with a Forms reference in the controller.

            if (e.KeyCode == Keys.S)
            {
                controller.setDown(true);
            }
            else if (e.KeyCode == Keys.W)
            {
                controller.setUp(true);
            }
            else if (e.KeyCode == Keys.D)
            {
                controller.setRight(true);
            }
            else if (e.KeyCode == Keys.A)
            {
                controller.setLeft(true);
            }

            // Prevent other key handlers from running
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the command keys when released.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S)
            {
                controller.setDown(false);
            }
            else if (e.KeyCode == Keys.W)
            {
                controller.setUp(false);
            }
            else if (e.KeyCode == Keys.D)
            {
                controller.setRight(false);
            }
            else if (e.KeyCode == Keys.A)
            {
                controller.setLeft(false);
            }
            else if (e.KeyCode == Keys.Q)
            {
                Application.Exit();
            }
        }


        /// <summary>
        /// Handles the mouse input when pressed.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            // user demands a normal projectile
            if (e.Button == MouseButtons.Left)
            {
                controller.setFire("main");
            }

            // user demands a beam attack
            if (e.Button == MouseButtons.Right)
            {
                controller.setFire("alt");
            }

        }

        /// <summary>
        /// Handles the mouse input when released.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // manages the intervals from which the beam attack can be fired
            if (e.Button == MouseButtons.Right)
            {
                controller.getWorld().setIsBeamShot(false);
            }

            // user is preparing next shot, nothing is being fired
            controller.setFire("none");
        }

        /// <summary>
        /// Handles the position of the tanks turrent by tracking the position of the cursor with a Vector.
        /// </summary>
        /// <param name="sender">the object which has raised the event</param>
        /// <param name="e">an object which inherits from EventArgs</param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            //a Vector2D representing where the player wants to aim their turret.
            Vector2D pos = new Vector2D((e.Location.X - this.drawingPanel.Width / 2), (e.Location.Y - this.drawingPanel.Height / 2) - 35);

            pos.Normalize();
            controller.setTdir(pos);
        }

        private void controlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("W:" + "\t" + "\t" + "Move up" + "\n" + "A:" + "\t" + "\t" + "Move left" + "\n" + "S:" + "\t" + "\t" + "Move down" + "\n"
                + "D:" + "\t" + "\t" + "Move right" + "\n" + "Mouse:" + "\t" + "\t" + "Aim" + "\n" + "Left Click:" + "\t" + "Fire projectile" + "\n"
                + "Right Click:" + "\t" + "Fire beam" + "\n" + "Q:" + "\t" + "\t" + "Quit" + "\n");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TankWars student/staff solution" + "\n" +
                "Artwork by Jolie Uk and Alex Smith" + "\n" + "Game design and implementation by Ali Hassoun" + "\n" +
                    "& Emiliano Plaza & Daniel Kopta" + "\n" + "CS 3500 Spring 2021, University of Utah");
        }
    }
}
