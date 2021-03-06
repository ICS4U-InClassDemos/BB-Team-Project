﻿/*  Created by: 
 *  Project: Brick Breaker
 *  Date: 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Threading;
using BrickBreaker.Properties;

namespace BrickBreaker
{
    public partial class GameScreen : UserControl
    {
        #region global values

        //player1 button control keys - DO NOT CHANGE
        Boolean leftArrowDown, rightArrowDown, pKeyDown, gamePaused;

        // Game values
        public static int lives;

        // Paddle and Ball objects
        Paddle paddle;
        Ball ball;

        // list of all blocks for current level
        List<Block> blocks = new List<Block>();
        List<Powerups> Powerup = new List<Powerups>();

        // Brushes
        SolidBrush paddleBrush = new SolidBrush(Color.White);
        SolidBrush ballBrush = new SolidBrush(Color.White);
        SolidBrush blockBrush = new SolidBrush(Color.Red);
        

        #endregion

        public GameScreen()
        {
            InitializeComponent();
            OnStart();
            IanMethod();
        }

        public void IanMethod()
        {
            string type = "";
            Color c = Color.White;

            Random powerGen = new Random();

            foreach (Block b in blocks)
            {
                if (ball.BlockCollision(b))
                {
                    int powerupChance = powerGen.Next(1, 4);
                    int powerupType = powerGen.Next(1, 6);

                    if (powerupChance == 1)
                    {



                        if (powerupType == 1) // add 300 points
                        {
                            type = "points";
                            
                        }
                        else if (powerupType == 2) // add another ball
                        {
                            type = "ball";
                        }
                        else if (powerupType == 3) // add extra life
                        {
                            type = "life";
                            c = Color.Red;
                        }
                        else if (powerupType == 4) // bigger paddle
                        {
                            type = "big";
                            c = Color.Green;
                            
                        }
                        else if (powerupType == 5) // makes paddle faster
                        {
                            type = "fast";
                            c = Color.Pink;
                        }
                        SolidBrush powerBrush = new SolidBrush(c);
                        Powerups newPowerup = new Powerups(b.x + b.width / 2 - 5, b.y + b.height / 2 - 5, 10, type, powerBrush);
                        Powerup.Add(newPowerup);
                    }

                }
            }
            foreach (Powerups p in Powerup)
            {
                p.Move(3);
            }
                foreach (Powerups d in Powerup)
                {
                    Rectangle powerRec = new Rectangle(d.x, d.y, d.size, d.size);
                    Rectangle paddleRec = new Rectangle(paddle.x, paddle.y, paddle.width, paddle.height);

                    if (powerRec.IntersectsWith(paddleRec))
                    {
                        if (d.type == "points")
                        {
                        
                        }
                        else if (d.type == "ball")
                        {

                        }
                        else if (d.type == "life")
                        {
                            lives++;
                        }
                        else if (d.type == "big")
                        {
                         paddle.width += 80;
                        d.y = this.Width + 1;
                        }
                        else if (d.type == "fast")
                        {
                        paddle.speed += 1;
                        }
                    d.y = this.Height + 1;
                    }
                }
            int index = Powerup.FindIndex(b => b.y > this.Height);
            if (index >= 0 && Powerup.Count() > 0)
            {
                Powerup.RemoveAt(index);
            }
        }
        

        public void OnStart()
        {
            
            //set life counter
            lives = 3;

            //set all button presses to false.
            leftArrowDown = rightArrowDown = pKeyDown = false;


            // setup starting paddle values and create paddle object
            int paddleWidth = 80;
            int paddleHeight = 20;
            int paddleX = ((this.Width / 2) - (paddleWidth / 2));
            int paddleY = (this.Height - paddleHeight) - 60;
            int paddleSpeed = 8;
            paddle = new Paddle(paddleX, paddleY, paddleWidth, paddleHeight, paddleSpeed, Color.White);

            // setup starting ball values
            int ballX = this.Width / 2 - 10;
            int ballY = this.Height - paddle.height - 80;

            // Creates a new ball
            int xSpeed = 6;
            int ySpeed = 6;
            int ballSize = 20;
            ball = new Ball(ballX, ballY, xSpeed, ySpeed, ballSize);

            pauseLabel.Visible = false;
            menuButton.Enabled = false;
            menuButton.Visible = false;
            resumeButton.Enabled = false;
            resumeButton.Visible = false;

            #region Creates blocks for generic level. Need to replace with code that loads levels.

            //TODO - replace all the code in this region eventually with code that loads levels from xml files

            blocks.Clear();
            int x = 10;

            while (blocks.Count < 12)
            {
                x += 57;
                Block b1 = new Block(x, 10, 1);
                blocks.Add(b1);
            }

            #endregion

            // start the game engine loop
            gameTimer.Enabled = true;
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //player 1 button presses
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = true;
                    break;
                case Keys.Right:
                    rightArrowDown = true;
                    break;
                case Keys.P:
                    pKeyDown = true;
                    break;

                default:
                    break;
            }
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
            //player 1 button releases
            switch (e.KeyCode)
            {
                case Keys.Left:
                    leftArrowDown = false;
                    break;
                case Keys.Right:
                    rightArrowDown = false;
                    break;
                case Keys.P:
                    pKeyDown = false;
                    break;
            
                default:
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if(paddle.width > 80)
            {
                paddle.width--;
            }

            // Move the paddle
            if (leftArrowDown && paddle.x > 0)
            {
                paddle.Move("left");
            }
            if (rightArrowDown && paddle.x < (this.Width - paddle.width))
            {
                paddle.Move("right");
            }

            // Move ball
            ball.Move();

            IanMethod();

            // Check for collision with top and side walls
            ball.WallCollision(this);

            // Check for ball hitting bottom of screen
            if (ball.BottomCollision(this))
            {
                lives--;

                // Moves the ball back to origin
                ball.x = ((paddle.x - (ball.size / 2)) + (paddle.width / 2));
                ball.y = (this.Height - paddle.height) - 85;


                if (lives == 0)
                {
                    gameTimer.Enabled = false;
                    OnEnd();
                }
            }

            // Check for collision of ball with paddle, (incl. paddle movement)
            ball.PaddleCollision(paddle, leftArrowDown, rightArrowDown);

            // Check if ball has collided with any blocks
            foreach (Block b in blocks)
            {
                if (ball.BlockCollision(b))
                {
                    blocks.Remove(b);

                    if (blocks.Count == 0)
                    {
                        gameTimer.Enabled = false;
                        //OnWin();
                        OnEnd();
                    }

                    break;
                }
            }
            pauseScreenEnabled();
            //redraw the screen
            Refresh();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            gamePaused = false;
            OnEnd();
        }

        private void resumeButton_Click(object sender, EventArgs e)
        {
            pauseLabel.Visible = false;
            resumeButton.Enabled = false;
            resumeButton.Visible = false;
            menuButton.Enabled = false;
            menuButton.Visible = false;
            gamePaused = false;

            this.Focus();
        }
        // Both below functions error out when given time to load
        public void OnEnd()
        {
            gameTimer.Enabled = false;

            Form f = this.FindForm();
            f.Controls.Remove(this);

            GameOverScreen gos = new GameOverScreen();          
            f.Controls.Add(gos);

            gos.Focus();
        }
        //public void OnWin()
        //{
        //    gameTimer.Enabled = false;
        //    Form f = this.FindForm();

        //    f.Controls.Remove(this);
        //    playAgainButton ws = new playAgainButton();

        //    f.Controls.Add(ws);
        //}

        public void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            // Draws paddle
            paddleBrush.Color = paddle.colour;
            e.Graphics.FillRectangle(paddleBrush, paddle.x, paddle.y, paddle.width, paddle.height);

            // Draws blocks
            foreach (Block b in blocks)
            {
                if(b.hp == 1)
                {
                    e.Graphics.DrawImage(Resources.cobbleBrick_1hit, b.x, b.y, b.width, b.height);
                }
                else if(b.hp == 2)
                {
                    e.Graphics.DrawImage(Resources.cobbleBrick_2hits, b.x, b.y, b.width, b.height);

                }
                else if (b.hp == 3)
                {
                    e.Graphics.DrawImage(Resources.cobbleBrick_3hits, b.x, b.y, b.width, b.height);

                }


            }

            // Draws ball
            e.Graphics.FillRectangle(ballBrush, ball.x, ball.y, ball.size, ball.size);


            //Draws Powerup
            if (Powerup.Count() > 0)
            {
                foreach (Powerups p in Powerup)
                {
                    e.Graphics.FillRectangle(p.powerupBrush, p.x, p.y, p.size, p.size);
                }
            }
           
        }

        public void pauseScreenEnabled()
        {
            if (pKeyDown)
            {
                gamePaused = true;
            }
            if (gamePaused)
            {
                pauseLabel.Visible = true;
                gameTimer.Enabled = false;
                menuButton.Enabled = true;
                menuButton.Visible = true;
                resumeButton.Enabled = true;
                resumeButton.Visible = true;
            }
            else
            {
                gameTimer.Enabled = true;
            }
        }

        

    }
}
    

