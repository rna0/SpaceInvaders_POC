using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInvaders
{
    public partial class Form1 : Form
    {
        bool goleft;
        bool goright;
        int speed = 1;
        int score = 0;
        bool isPressed;
        int totalEnemies;
        int playerSpeed = 6;

        public Form1()
        {
            //start window, get number of enemies and create mousewheel event  
            InitializeComponent();
            totalEnemies = get_totalEnemies();
            MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }
        void Form1_MouseWheel(Object sender, MouseEventArgs e)
        {
            //inc or dec by 1 or -1 from invaders movement by using scroll bar
            int speed_inc = speed / Math.Abs(speed);
            if (e.Delta > 0 && Math.Abs(speed + speed_inc) < 4)
            {
                speed += speed_inc;
            }
            else if (e.Delta < 0 && Math.Abs(speed - speed_inc) > 0)
            {
                speed -= speed_inc;
            }
        }
        private int get_totalEnemies()
        {
            //checking if the tag 'invader' in all picture control
            int i=0;
            foreach (Control x in Controls)
            {
                if (x is PictureBox && (string)x.Tag == "invader")
                {
                    i++;
                }
            }
            return i;
        }
        private void keyisdown( object sender, KeyEventArgs e)
        {
            //if key down, check if right or left for bool in timer
            if (e.KeyCode == Keys.Left)
            {
                goleft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
            }
            //if space is pressed: player shoots
            if (e.KeyCode == Keys.Space && !isPressed)
            {
                isPressed = true;
                makeBullet();
            }
        }
        private void keyisup(object sender, KeyEventArgs e)
        {
            // left and right released
            if (e.KeyCode == Keys.Left)
            {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = false;
            }
            //if space is released: shoot released
            if (isPressed)
            {
                isPressed = false;
            }
        }
        private void change_side(bool on_left)
        {
            //when invader tuch the right corner all invaders change direction
            //when invader tuch the left corner all invaders change direction and move down
            speed = -speed;
            foreach (Control x in Controls)
            {
                if (x is PictureBox && (string)x.Tag == "invader")
                {
                    if (on_left)
                        ((PictureBox)x).Top += ((PictureBox)x).Height/4;
                    ((PictureBox)x).Left += speed*3;
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //this function is called every time the timer tool is changed (20 milseconds)
            //put player ship and score in place
            player.Top = Height-100;
            score_label.Top = Height - 75;
            //player moving left and right unless out of bounds
            if (goleft && player.Left > 0)
            {
                player.Left -= playerSpeed;
            }
            else if (goright && player.Left+75<Width)
            {
                player.Left += playerSpeed;
            }

            // end of player moving left and right

            //check for enemy is on form. enemies moving on the form
            foreach (Control x in Controls)
            {
                if (x is PictureBox && (string)x.Tag == "invader")
                {
                    if (((PictureBox)x).Bounds.IntersectsWith(player.Bounds))
                    {
                        gameOver();
                    }
                    if (((PictureBox)x).Left + speed > Width - 51)
                    {
                        change_side(false);
                        break;
                    }
                    else if (((PictureBox)x).Left - speed < 1)
                    {
                        change_side(true);
                    }

                }
            }
            //move on screen
            foreach (Control x in Controls)
                if (x is PictureBox && (string)x.Tag == "invader")
                    ((PictureBox)x).Left += speed;
            // end of enemies moving on the form

            //animating the bullets and removing them when the have left the scene
            foreach (Control y in Controls)
            {
                if (y is PictureBox && (string)y.Tag == "bullet")
                {
                    y.Top -= 20;
                    if (((PictureBox)y).Top < 10)// a bit lower than top screen
                    {
                        Controls.Remove(y);
                    }
                }
            }
            // end of animating the bullets.

            // bullet and enemy collision start
            foreach (Control i in Controls)
            {
                foreach (Control j in Controls)
                {
                    if (i is PictureBox && (string)i.Tag == "invader")
                    {
                        if (j is PictureBox && (string)j.Tag == "bullet")
                        {
                            if (i.Bounds.IntersectsWith(j.Bounds))
                            {
                                score++;
                                Controls.Remove(i);
                                Controls.Remove(j);
                            }
                        }
                    }
                }
            }
            // bullet and enemy collision end

            // keeping and showing score
            score_label.Text = "Score : " + score;
            if (score > totalEnemies - 1)
            {
                gameOver();
                //MessageBox.Show("You Win");
            }
            // end of keeping and showing score.
        }
        private void makeBullet()
        {
            PictureBox bullet = new PictureBox();
            bullet.Image = Properties.Resources.pew1;
            bullet.Size = new Size(5, 20);
            bullet.Tag = "bullet";
            bullet.Left = player.Left + player.Width / 2;
            bullet.Top = player.Top - 20;
            Controls.Add(bullet);
            bullet.BringToFront();
        }
        private void gameOver()
        {
            timer1.Stop();
            PictureBox sign = new PictureBox();
            if (score == totalEnemies)
            {
                sign.Image = Properties.Resources.victory;
                score_label.Text += ", You Win";
            }
            else
            { 
                sign.Image = Properties.Resources.game_over;
                score_label.Text += ", Game Over";
            }

            sign.Size = new Size(Width/2, Height/3);
            sign.Left = (ClientRectangle.Width - sign.Size.Width)/2;
            sign.Top = (ClientRectangle.Height - sign.Size.Height)/2;
            Controls.Add(sign);
            sign.SizeMode = PictureBoxSizeMode.StretchImage;
            sign.BringToFront();

            //for (int i = 2; i < 200; i+=1)
            //{
            //    sign.Size = new Size(20*i, 5*i);
                
            //}

        }
    }
}
