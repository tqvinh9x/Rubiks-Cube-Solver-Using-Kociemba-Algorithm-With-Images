using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TwoPhaseSolver;

namespace KociembaSolver
{
    public partial class Form1 : Form
    {
        private enum Colors { red, orange, white, yellow, green, blue }
        Colors[] myCube = new Colors[54];
        Cube c = TwoPhaseSolver.Move.randmove(1).apply(new Cube());
        Cubie[] corners = new Cubie[8];
        Cubie[] edges = new Cubie[12];
        List<string> lstMerged = new List<string>();
        public Form1()
        {
            InitializeComponent();
            InitializeColor();
            label1.Text = "";
            setEventForButton();
            Text = "Tran Quang Vinh";
            this.Location = new Point(0, 0);
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
            panel1.Height = Screen.PrimaryScreen.WorkingArea.Size.Height * 3 / 5;
            panel1.Width = Screen.PrimaryScreen.WorkingArea.Size.Width / 3;
            panel1.Location = new Point(0, 0);
            panel2.Height = Screen.PrimaryScreen.WorkingArea.Size.Height * 2 / 5;
            panel2.Width = Screen.PrimaryScreen.WorkingArea.Size.Width / 3;
            panel2.Location = new Point(0, Screen.PrimaryScreen.WorkingArea.Size.Height * 3 / 5);
            panel3.Height = Screen.PrimaryScreen.WorkingArea.Size.Height;
            panel3.Width = Screen.PrimaryScreen.WorkingArea.Size.Width * 2 / 3;
            panel3.Location = new Point(Screen.PrimaryScreen.WorkingArea.Size.Width / 3, 0);
            //calculateAverageMoves();
        }
        private void setEventForButton()
        {
            for (int i = 0; i < 54; i++)
            {
                string s = "button" + i;
                Button btn = this.Controls.Find(s, true).FirstOrDefault() as Button;
                btn.Click += Button_ChangeColor;
            }
            string[] colors = { "Red", "Green", "Yellow", "Orange", "Blue", "White" };
            foreach (string color in colors)
            {
                Button btn = this.Controls.Find(color, true).FirstOrDefault() as Button;
                btn.Click += Button_ChangeRadio;
            }
        }
        private void InitializeColor()
        {
            for (int i = 0; i < 54; i++)
            {
                myCube[i] = Colors.white;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Button_ChangeColor(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            Color currentColor = new Color();
            if (radioGreen.Checked)
            {
                currentColor = Color.Green;
            }
            else if (radioRed.Checked)
            {
                currentColor = Color.Red;
            }
            else if (radioYellow.Checked)
            {
                currentColor = Color.Yellow;
            }
            else if (radioOrange.Checked)
            {
                currentColor = Color.Orange;
            }
            else if (radioBlue.Checked)
            {
                currentColor = Color.Blue;
            }
            else
            {
                currentColor = Color.White;
            }
            if (btn.BackColor == currentColor)
            {
                btn.BackColor = Color.White;
            }
            else btn.BackColor = currentColor;
        }
        private void Button_ChangeRadio(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string radioName = "radio" + btn.Name;
            RadioButton RadioBtn = this.Controls.Find(radioName, true).FirstOrDefault() as RadioButton;
            RadioBtn.Checked = true;
        }
        private void buttonSolve_Click(object sender, EventArgs e)
        {
            getCurrentScramble();
            CenterOrient();
            BindingStateForKociembaCube();
            if (checkFinished() == true)
            {
                MessageBox.Show("This cube is already solved.\n\nTry something harder :)");
            }
            else
            {
                Solve(ref myCube);
                if (checkFinished() == true)
                {
                    //execute kociemba algorithm
                    c.corners = corners;
                    c.edges = edges;
                    List<string> result = Search.patternSolve(c, TwoPhaseSolver.Move.None, 22, printInfo: true);
                    label1.Text = "Phase 1: " + result[0] + "\nPhase 2: " + result[1];
                    label1.Font = new Font("Comic Sans MS", 9);
                    label2.Text = "Solution: " + result[2] + " moves";

                    //pictures
                    CleanPanel();
                    lstMerged.Clear();
                    pictureTranslator(result[0] + result[1], ref lstMerged);
                    int startX = 0, startY = 0;
                    int pictureSize = panel3.Width / 6;
                    for (int step = 0; step < lstMerged.Count; step++)
                    {
                        PictureBox pic = new PictureBox();
                        pic.Location = new Point(startX, startY);
                        pic.BorderStyle = BorderStyle.Fixed3D;
                        Label lblStep = new Label();
                        lblStep.Parent = pic;
                        lblStep.Location = new Point(0, pictureSize - 20);
                        lblStep.BackColor = Color.Transparent;
                        lblStep.Text = "Step " + (step + 1);
                        try
                        {
                            Bitmap bmp = new Bitmap("../../pictures/" + lstMerged[step] + ".png");
                            Bitmap newImage = new Bitmap(bmp, pictureSize, pictureSize);
                            pic.Image = newImage;
                            pic.Width = pictureSize;
                            pic.Height = pictureSize;
                            panel3.Controls.Add(pic);
                            startX += pictureSize;
                            if (startX + pictureSize > panel3.Width)
                            {
                                startX = 0;
                                startY += pictureSize;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("picture named \"" + lstMerged[step] + "\".png not found.");
                            break;
                        }
                    }
                    PictureBox picDone = new PictureBox();
                    picDone.Location = new Point(startX, startY);
                    picDone.BorderStyle = BorderStyle.Fixed3D;
                    Label lblStepDone = new Label();
                    lblStepDone.Parent = picDone;
                    lblStepDone.Location = new Point(0, pictureSize - 20);
                    lblStepDone.BackColor = Color.Transparent;
                    lblStepDone.ForeColor = Color.Green;
                    lblStepDone.Text = "Done.";
                    try
                    {
                        Bitmap bmp = new Bitmap("../../pictures/done.png");
                        Bitmap newImage = new Bitmap(bmp, pictureSize, pictureSize);
                        picDone.Image = newImage;
                        picDone.Width = pictureSize;
                        picDone.Height = pictureSize;
                        panel3.Controls.Add(picDone);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    MessageBox.Show("This cube cannot be solved.");
                }
            }
        }
        private void BindingStateForKociembaCube()
        {
            #region Corner0
            //0-0
            if (myCube[8] == Colors.white && myCube[11] == Colors.green && myCube[18] == Colors.red)
            {
                corners[0] = new Cubie(0, 0);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.white && myCube[18] == Colors.green)
            {
                corners[0] = new Cubie(0, 2);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.red && myCube[18] == Colors.white)
            {
                corners[0] = new Cubie(0, 1);
            }
            //0-1
            else if (myCube[8] == Colors.white && myCube[11] == Colors.orange && myCube[18] == Colors.green)
            {
                corners[0] = new Cubie(1, 0);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.white && myCube[18] == Colors.orange)
            {
                corners[0] = new Cubie(1, 2);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.green && myCube[18] == Colors.white)
            {
                corners[0] = new Cubie(1, 1);
            }
            //0-2
            else if (myCube[8] == Colors.white && myCube[11] == Colors.blue && myCube[18] == Colors.orange)
            {
                corners[0] = new Cubie(2, 0);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.white && myCube[18] == Colors.blue)
            {
                corners[0] = new Cubie(2, 2);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.orange && myCube[18] == Colors.white)
            {
                corners[0] = new Cubie(2, 1);
            }
            //0-3
            else if (myCube[8] == Colors.white && myCube[11] == Colors.red && myCube[18] == Colors.blue)
            {
                corners[0] = new Cubie(3, 0);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.white && myCube[18] == Colors.red)
            {
                corners[0] = new Cubie(3, 2);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.blue && myCube[18] == Colors.white)
            {
                corners[0] = new Cubie(3, 1);
            }
            //0-4
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.red && myCube[18] == Colors.green)
            {
                corners[0] = new Cubie(4, 0);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.yellow && myCube[18] == Colors.red)
            {
                corners[0] = new Cubie(4, 2);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.green && myCube[18] == Colors.yellow)
            {
                corners[0] = new Cubie(4, 1);
            }
            //0-5
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.green && myCube[18] == Colors.orange)
            {
                corners[0] = new Cubie(5, 0);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.yellow && myCube[18] == Colors.green)
            {
                corners[0] = new Cubie(5, 2);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.orange && myCube[18] == Colors.yellow)
            {
                corners[0] = new Cubie(5, 1);
            }
            //0-6
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.orange && myCube[18] == Colors.blue)
            {
                corners[0] = new Cubie(6, 0);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.yellow && myCube[18] == Colors.orange)
            {
                corners[0] = new Cubie(6, 2);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.blue && myCube[18] == Colors.yellow)
            {
                corners[0] = new Cubie(6, 1);
            }
            //0-7
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.blue && myCube[18] == Colors.red)
            {
                corners[0] = new Cubie(7, 0);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.yellow && myCube[18] == Colors.blue)
            {
                corners[0] = new Cubie(7, 2);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.red && myCube[18] == Colors.yellow)
            {
                corners[0] = new Cubie(7, 1);
            }
            #endregion
            #region Corner1
            //1-0
            if (myCube[6] == Colors.white && myCube[38] == Colors.green && myCube[9] == Colors.red)
            {
                corners[1] = new Cubie(0, 0);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.white && myCube[9] == Colors.green)
            {
                corners[1] = new Cubie(0, 2);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.red && myCube[9] == Colors.white)
            {
                corners[1] = new Cubie(0, 1);
            }
            //1-1
            else if (myCube[6] == Colors.white && myCube[38] == Colors.orange && myCube[9] == Colors.green)
            {
                corners[1] = new Cubie(1, 0);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.white && myCube[9] == Colors.orange)
            {
                corners[1] = new Cubie(1, 2);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.green && myCube[9] == Colors.white)
            {
                corners[1] = new Cubie(1, 1);
            }
            //1-2
            else if (myCube[6] == Colors.white && myCube[38] == Colors.blue && myCube[9] == Colors.orange)
            {
                corners[1] = new Cubie(2, 0);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.white && myCube[9] == Colors.blue)
            {
                corners[1] = new Cubie(2, 2);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.orange && myCube[9] == Colors.white)
            {
                corners[1] = new Cubie(2, 1);
            }
            //1-3
            else if (myCube[6] == Colors.white && myCube[38] == Colors.red && myCube[9] == Colors.blue)
            {
                corners[1] = new Cubie(3, 0);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.white && myCube[9] == Colors.red)
            {
                corners[1] = new Cubie(3, 2);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.blue && myCube[9] == Colors.white)
            {
                corners[1] = new Cubie(3, 1);
            }
            //1-4
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.red && myCube[9] == Colors.green)
            {
                corners[1] = new Cubie(4, 0);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.yellow && myCube[9] == Colors.red)
            {
                corners[1] = new Cubie(4, 2);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.green && myCube[9] == Colors.yellow)
            {
                corners[1] = new Cubie(4, 1);
            }
            else //1-5
            if (myCube[6] == Colors.yellow && myCube[38] == Colors.green && myCube[9] == Colors.orange)
            {
                corners[1] = new Cubie(5, 0);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.yellow && myCube[9] == Colors.green)
            {
                corners[1] = new Cubie(5, 2);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.orange && myCube[9] == Colors.yellow)
            {
                corners[1] = new Cubie(5, 1);
            }
            else //1-6
            if (myCube[6] == Colors.yellow && myCube[38] == Colors.orange && myCube[9] == Colors.blue)
            {
                corners[1] = new Cubie(6, 0);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.yellow && myCube[9] == Colors.orange)
            {
                corners[1] = new Cubie(6, 2);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.blue && myCube[9] == Colors.yellow)
            {
                corners[1] = new Cubie(6, 1);
            }
            //1-7
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.blue && myCube[9] == Colors.red)
            {
                corners[1] = new Cubie(7, 0);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.yellow && myCube[9] == Colors.blue)
            {
                corners[1] = new Cubie(7, 2);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.red && myCube[9] == Colors.yellow)
            {
                corners[1] = new Cubie(7, 1);
            }
            #endregion
            #region Corner2
            //2-0
            if (myCube[0] == Colors.white && myCube[29] == Colors.green && myCube[36] == Colors.red)
            {
                corners[2] = new Cubie(0, 0);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.white && myCube[36] == Colors.green)
            {
                corners[2] = new Cubie(0, 2);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.red && myCube[36] == Colors.white)
            {
                corners[2] = new Cubie(0, 1);
            }
            //2-1
            else if (myCube[0] == Colors.white && myCube[29] == Colors.orange && myCube[36] == Colors.green)
            {
                corners[2] = new Cubie(1, 0);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.white && myCube[36] == Colors.orange)
            {
                corners[2] = new Cubie(1, 2);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.green && myCube[36] == Colors.white)
            {
                corners[2] = new Cubie(1, 1);
            }
            //2-2
            else if (myCube[0] == Colors.white && myCube[29] == Colors.blue && myCube[36] == Colors.orange)
            {
                corners[2] = new Cubie(2, 0);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.white && myCube[36] == Colors.blue)
            {
                corners[2] = new Cubie(2, 2);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.orange && myCube[36] == Colors.white)
            {
                corners[2] = new Cubie(2, 1);
            }
            //2-3
            else if (myCube[0] == Colors.white && myCube[29] == Colors.red && myCube[36] == Colors.blue)
            {
                corners[2] = new Cubie(3, 0);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.white && myCube[36] == Colors.red)
            {
                corners[2] = new Cubie(3, 2);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.blue && myCube[36] == Colors.white)
            {
                corners[2] = new Cubie(3, 1);
            }
            //2-4
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.red && myCube[36] == Colors.green)
            {
                corners[2] = new Cubie(4, 0);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.yellow && myCube[36] == Colors.red)
            {
                corners[2] = new Cubie(4, 2);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.green && myCube[36] == Colors.yellow)
            {
                corners[2] = new Cubie(4, 1);
            }
            //2-5
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.green && myCube[36] == Colors.orange)
            {
                corners[2] = new Cubie(5, 0);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.yellow && myCube[36] == Colors.green)
            {
                corners[2] = new Cubie(5, 2);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.orange && myCube[36] == Colors.yellow)
            {
                corners[2] = new Cubie(5, 1);
            }
            //2-6
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.orange && myCube[36] == Colors.blue)
            {
                corners[2] = new Cubie(6, 0);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.yellow && myCube[36] == Colors.orange)
            {
                corners[2] = new Cubie(6, 2);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.blue && myCube[36] == Colors.yellow)
            {
                corners[2] = new Cubie(6, 1);
            }
            //2-7
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.blue && myCube[36] == Colors.red)
            {
                corners[2] = new Cubie(7, 0);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.yellow && myCube[36] == Colors.blue)
            {
                corners[2] = new Cubie(7, 2);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.red && myCube[36] == Colors.yellow)
            {
                corners[2] = new Cubie(7, 1);
            }
            #endregion
            #region Corner3
            //3-0
            if (myCube[2] == Colors.white && myCube[20] == Colors.green && myCube[27] == Colors.red)
            {
                corners[3] = new Cubie(0, 0);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.white && myCube[27] == Colors.green)
            {
                corners[3] = new Cubie(0, 2);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.red && myCube[27] == Colors.white)
            {
                corners[3] = new Cubie(0, 1);
            }
            //3-1
            else if (myCube[2] == Colors.white && myCube[20] == Colors.orange && myCube[27] == Colors.green)
            {
                corners[3] = new Cubie(1, 0);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.white && myCube[27] == Colors.orange)
            {
                corners[3] = new Cubie(1, 2);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.green && myCube[27] == Colors.white)
            {
                corners[3] = new Cubie(1, 1);
            }
            //3-2
            else if (myCube[2] == Colors.white && myCube[20] == Colors.blue && myCube[27] == Colors.orange)
            {
                corners[3] = new Cubie(2, 0);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.white && myCube[27] == Colors.blue)
            {
                corners[3] = new Cubie(2, 2);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.orange && myCube[27] == Colors.white)
            {
                corners[3] = new Cubie(2, 1);
            }
            //3-3
            else if (myCube[2] == Colors.white && myCube[20] == Colors.red && myCube[27] == Colors.blue)
            {
                corners[3] = new Cubie(3, 0);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.white && myCube[27] == Colors.red)
            {
                corners[3] = new Cubie(3, 2);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.blue && myCube[27] == Colors.white)
            {
                corners[3] = new Cubie(3, 1);
            }
            //3-4
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.red && myCube[27] == Colors.green)
            {
                corners[3] = new Cubie(4, 0);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.yellow && myCube[27] == Colors.red)
            {
                corners[3] = new Cubie(4, 2);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.green && myCube[27] == Colors.yellow)
            {
                corners[3] = new Cubie(4, 1);
            }
            //3-5
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.green && myCube[27] == Colors.orange)
            {
                corners[3] = new Cubie(5, 0);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.yellow && myCube[27] == Colors.green)
            {
                corners[3] = new Cubie(5, 2);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.orange && myCube[27] == Colors.yellow)
            {
                corners[3] = new Cubie(5, 1);
            }
            //3-6
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.orange && myCube[27] == Colors.blue)
            {
                corners[3] = new Cubie(6, 0);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.yellow && myCube[27] == Colors.orange)
            {
                corners[3] = new Cubie(6, 2);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.blue && myCube[27] == Colors.yellow)
            {
                corners[3] = new Cubie(6, 1);
            }
            //3-7
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.blue && myCube[27] == Colors.red)
            {
                corners[3] = new Cubie(7, 0);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.yellow && myCube[27] == Colors.blue)
            {
                corners[3] = new Cubie(7, 2);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.red && myCube[27] == Colors.yellow)
            {
                corners[3] = new Cubie(7, 1);
            }
            #endregion
            #region Corner4
            //4-0
            if (myCube[47] == Colors.white && myCube[24] == Colors.green && myCube[17] == Colors.red)
            {
                corners[4] = new Cubie(0, 0);
            }
            else if (myCube[47] == Colors.red && myCube[24] == Colors.white && myCube[17] == Colors.green)
            {
                corners[4] = new Cubie(0, 2);
            }
            else if (myCube[47] == Colors.green && myCube[24] == Colors.red && myCube[17] == Colors.white)
            {
                corners[4] = new Cubie(0, 1);
            }
            //4-1
            else if (myCube[47] == Colors.white && myCube[24] == Colors.orange && myCube[17] == Colors.green)
            {
                corners[4] = new Cubie(1, 0);
            }
            else if (myCube[47] == Colors.green && myCube[24] == Colors.white && myCube[17] == Colors.orange)
            {
                corners[4] = new Cubie(1, 2);
            }
            else if (myCube[47] == Colors.orange && myCube[24] == Colors.green && myCube[17] == Colors.white)
            {
                corners[4] = new Cubie(1, 1);
            }
            //4-2
            else if (myCube[47] == Colors.white && myCube[24] == Colors.blue && myCube[17] == Colors.orange)
            {
                corners[4] = new Cubie(2, 0);
            }
            else if (myCube[47] == Colors.orange && myCube[24] == Colors.white && myCube[17] == Colors.blue)
            {
                corners[4] = new Cubie(2, 2);
            }
            else if (myCube[47] == Colors.blue && myCube[24] == Colors.orange && myCube[17] == Colors.white)
            {
                corners[4] = new Cubie(2, 1);
            }
            //4-3
            else if (myCube[47] == Colors.white && myCube[24] == Colors.red && myCube[17] == Colors.blue)
            {
                corners[4] = new Cubie(3, 0);
            }
            else if (myCube[47] == Colors.blue && myCube[24] == Colors.white && myCube[17] == Colors.red)
            {
                corners[4] = new Cubie(3, 2);
            }
            else if (myCube[47] == Colors.red && myCube[24] == Colors.blue && myCube[17] == Colors.white)
            {
                corners[4] = new Cubie(3, 1);
            }
            //4-4
            else if (myCube[47] == Colors.yellow && myCube[24] == Colors.red && myCube[17] == Colors.green)
            {
                corners[4] = new Cubie(4, 0);
            }
            else if (myCube[47] == Colors.green && myCube[24] == Colors.yellow && myCube[17] == Colors.red)
            {
                corners[4] = new Cubie(4, 2);
            }
            else if (myCube[47] == Colors.red && myCube[24] == Colors.green && myCube[17] == Colors.yellow)
            {
                corners[4] = new Cubie(4, 1);
            }
            //4-5
            else if (myCube[47] == Colors.yellow && myCube[24] == Colors.green && myCube[17] == Colors.orange)
            {
                corners[4] = new Cubie(5, 0);
            }
            else if (myCube[47] == Colors.orange && myCube[24] == Colors.yellow && myCube[17] == Colors.green)
            {
                corners[4] = new Cubie(5, 2);
            }
            else if (myCube[47] == Colors.green && myCube[24] == Colors.orange && myCube[17] == Colors.yellow)
            {
                corners[4] = new Cubie(5, 1);
            }
            //4-6
            else if (myCube[47] == Colors.yellow && myCube[24] == Colors.orange && myCube[17] == Colors.blue)
            {
                corners[4] = new Cubie(6, 0);
            }
            else if (myCube[47] == Colors.blue && myCube[24] == Colors.yellow && myCube[17] == Colors.orange)
            {
                corners[4] = new Cubie(6, 2);
            }
            else if (myCube[47] == Colors.orange && myCube[24] == Colors.blue && myCube[17] == Colors.yellow)
            {
                corners[4] = new Cubie(6, 1);
            }
            //4-7
            else if (myCube[47] == Colors.yellow && myCube[24] == Colors.blue && myCube[17] == Colors.red)
            {
                corners[4] = new Cubie(7, 0);
            }
            else if (myCube[47] == Colors.red && myCube[24] == Colors.yellow && myCube[17] == Colors.blue)
            {
                corners[4] = new Cubie(7, 2);
            }
            else if (myCube[47] == Colors.blue && myCube[24] == Colors.red && myCube[17] == Colors.yellow)
            {
                corners[4] = new Cubie(7, 1);
            }
            #endregion
            #region Corner5
            //5-0
            if (myCube[45] == Colors.white && myCube[15] == Colors.green && myCube[44] == Colors.red)
            {
                corners[5] = new Cubie(0, 0);
            }
            else if (myCube[45] == Colors.red && myCube[15] == Colors.white && myCube[44] == Colors.green)
            {
                corners[5] = new Cubie(0, 2);
            }
            else if (myCube[45] == Colors.green && myCube[15] == Colors.red && myCube[44] == Colors.white)
            {
                corners[5] = new Cubie(0, 1);
            }
            //5-1
            else if (myCube[45] == Colors.white && myCube[15] == Colors.orange && myCube[44] == Colors.green)
            {
                corners[5] = new Cubie(1, 0);
            }
            else if (myCube[45] == Colors.green && myCube[15] == Colors.white && myCube[44] == Colors.orange)
            {
                corners[5] = new Cubie(1, 2);
            }
            else if (myCube[45] == Colors.orange && myCube[15] == Colors.green && myCube[44] == Colors.white)
            {
                corners[5] = new Cubie(1, 1);
            }
            //5-2
            else if (myCube[45] == Colors.white && myCube[15] == Colors.blue && myCube[44] == Colors.orange)
            {
                corners[5] = new Cubie(2, 0);
            }
            else if (myCube[45] == Colors.orange && myCube[15] == Colors.white && myCube[44] == Colors.blue)
            {
                corners[5] = new Cubie(2, 2);
            }
            else if (myCube[45] == Colors.blue && myCube[15] == Colors.orange && myCube[44] == Colors.white)
            {
                corners[5] = new Cubie(2, 1);
            }
            //5-3
            else if (myCube[45] == Colors.white && myCube[15] == Colors.red && myCube[44] == Colors.blue)
            {
                corners[5] = new Cubie(3, 0);
            }
            else if (myCube[45] == Colors.blue && myCube[15] == Colors.white && myCube[44] == Colors.red)
            {
                corners[5] = new Cubie(3, 2);
            }
            else if (myCube[45] == Colors.red && myCube[15] == Colors.blue && myCube[44] == Colors.white)
            {
                corners[5] = new Cubie(3, 1);
            }
            //5-4
            else if (myCube[45] == Colors.yellow && myCube[15] == Colors.red && myCube[44] == Colors.green)
            {
                corners[5] = new Cubie(4, 0);
            }
            else if (myCube[45] == Colors.green && myCube[15] == Colors.yellow && myCube[44] == Colors.red)
            {
                corners[5] = new Cubie(4, 2);
            }
            else if (myCube[45] == Colors.red && myCube[15] == Colors.green && myCube[44] == Colors.yellow)
            {
                corners[5] = new Cubie(4, 1);
            }
            //5-5
            else if (myCube[45] == Colors.yellow && myCube[15] == Colors.green && myCube[44] == Colors.orange)
            {
                corners[5] = new Cubie(5, 0);
            }
            else if (myCube[45] == Colors.orange && myCube[15] == Colors.yellow && myCube[44] == Colors.green)
            {
                corners[5] = new Cubie(5, 2);
            }
            else if (myCube[45] == Colors.green && myCube[15] == Colors.orange && myCube[44] == Colors.yellow)
            {
                corners[5] = new Cubie(5, 1);
            }
            //5-6
            else if (myCube[45] == Colors.yellow && myCube[15] == Colors.orange && myCube[44] == Colors.blue)
            {
                corners[5] = new Cubie(6, 0);
            }
            else if (myCube[45] == Colors.blue && myCube[15] == Colors.yellow && myCube[44] == Colors.orange)
            {
                corners[5] = new Cubie(6, 2);
            }
            else if (myCube[45] == Colors.orange && myCube[15] == Colors.blue && myCube[44] == Colors.yellow)
            {
                corners[5] = new Cubie(6, 1);
            }
            //5-7
            else if (myCube[45] == Colors.yellow && myCube[15] == Colors.blue && myCube[44] == Colors.red)
            {
                corners[5] = new Cubie(7, 0);
            }
            else if (myCube[45] == Colors.red && myCube[15] == Colors.yellow && myCube[44] == Colors.blue)
            {
                corners[5] = new Cubie(7, 2);
            }
            else if (myCube[45] == Colors.blue && myCube[15] == Colors.red && myCube[44] == Colors.yellow)
            {
                corners[5] = new Cubie(7, 1);
            }
            #endregion
            #region Corner6
            //6-0
            if (myCube[51] == Colors.white && myCube[42] == Colors.green && myCube[35] == Colors.red)
            {
                corners[6] = new Cubie(0, 0);
            }
            else if (myCube[51] == Colors.red && myCube[42] == Colors.white && myCube[35] == Colors.green)
            {
                corners[6] = new Cubie(0, 2);
            }
            else if (myCube[51] == Colors.green && myCube[42] == Colors.red && myCube[35] == Colors.white)
            {
                corners[6] = new Cubie(0, 1);
            }
            //6-1
            else if (myCube[51] == Colors.white && myCube[42] == Colors.orange && myCube[35] == Colors.green)
            {
                corners[6] = new Cubie(1, 0);
            }
            else if (myCube[51] == Colors.green && myCube[42] == Colors.white && myCube[35] == Colors.orange)
            {
                corners[6] = new Cubie(1, 2);
            }
            else if (myCube[51] == Colors.orange && myCube[42] == Colors.green && myCube[35] == Colors.white)
            {
                corners[6] = new Cubie(1, 1);
            }
            //6-2
            else if (myCube[51] == Colors.white && myCube[42] == Colors.blue && myCube[35] == Colors.orange)
            {
                corners[6] = new Cubie(2, 0);
            }
            else if (myCube[51] == Colors.orange && myCube[42] == Colors.white && myCube[35] == Colors.blue)
            {
                corners[6] = new Cubie(2, 2);
            }
            else if (myCube[51] == Colors.blue && myCube[42] == Colors.orange && myCube[35] == Colors.white)
            {
                corners[6] = new Cubie(2, 1);
            }
            //6-3
            else if (myCube[51] == Colors.white && myCube[42] == Colors.red && myCube[35] == Colors.blue)
            {
                corners[6] = new Cubie(3, 0);
            }
            else if (myCube[51] == Colors.blue && myCube[42] == Colors.white && myCube[35] == Colors.red)
            {
                corners[6] = new Cubie(3, 2);
            }
            else if (myCube[51] == Colors.red && myCube[42] == Colors.blue && myCube[35] == Colors.white)
            {
                corners[6] = new Cubie(3, 1);
            }
            //6-4
            else if (myCube[51] == Colors.yellow && myCube[42] == Colors.red && myCube[35] == Colors.green)
            {
                corners[6] = new Cubie(4, 0);
            }
            else if (myCube[51] == Colors.green && myCube[42] == Colors.yellow && myCube[35] == Colors.red)
            {
                corners[6] = new Cubie(4, 2);
            }
            else if (myCube[51] == Colors.red && myCube[42] == Colors.green && myCube[35] == Colors.yellow)
            {
                corners[6] = new Cubie(4, 1);
            }
            //6-5
            else if (myCube[51] == Colors.yellow && myCube[42] == Colors.green && myCube[35] == Colors.orange)
            {
                corners[6] = new Cubie(5, 0);
            }
            else if (myCube[51] == Colors.orange && myCube[42] == Colors.yellow && myCube[35] == Colors.green)
            {
                corners[6] = new Cubie(5, 2);
            }
            else if (myCube[51] == Colors.green && myCube[42] == Colors.orange && myCube[35] == Colors.yellow)
            {
                corners[6] = new Cubie(5, 1);
            }
            //6-6
            else if (myCube[51] == Colors.yellow && myCube[42] == Colors.orange && myCube[35] == Colors.blue)
            {
                corners[6] = new Cubie(6, 0);
            }
            else if (myCube[51] == Colors.blue && myCube[42] == Colors.yellow && myCube[35] == Colors.orange)
            {
                corners[6] = new Cubie(6, 2);
            }
            else if (myCube[51] == Colors.orange && myCube[42] == Colors.blue && myCube[35] == Colors.yellow)
            {
                corners[6] = new Cubie(6, 1);
            }
            //6-7
            else if (myCube[51] == Colors.yellow && myCube[42] == Colors.blue && myCube[35] == Colors.red)
            {
                corners[6] = new Cubie(7, 0);
            }
            else if (myCube[51] == Colors.red && myCube[42] == Colors.yellow && myCube[35] == Colors.blue)
            {
                corners[6] = new Cubie(7, 2);
            }
            else if (myCube[51] == Colors.blue && myCube[42] == Colors.red && myCube[35] == Colors.yellow)
            {
                corners[6] = new Cubie(7, 1);
            }
            #endregion
            #region Corner7
            //7-0
            if (myCube[53] == Colors.white && myCube[33] == Colors.green && myCube[26] == Colors.red)
            {
                corners[7] = new Cubie(0, 0);
            }
            else if (myCube[53] == Colors.red && myCube[33] == Colors.white && myCube[26] == Colors.green)
            {
                corners[7] = new Cubie(0, 2);
            }
            else if (myCube[53] == Colors.green && myCube[33] == Colors.red && myCube[26] == Colors.white)
            {
                corners[7] = new Cubie(0, 1);
            }
            //7-1
            else if (myCube[53] == Colors.white && myCube[33] == Colors.orange && myCube[26] == Colors.green)
            {
                corners[7] = new Cubie(1, 0);
            }
            else if (myCube[53] == Colors.green && myCube[33] == Colors.white && myCube[26] == Colors.orange)
            {
                corners[7] = new Cubie(1, 2);
            }
            else if (myCube[53] == Colors.orange && myCube[33] == Colors.green && myCube[26] == Colors.white)
            {
                corners[7] = new Cubie(1, 1);
            }
            //7-2
            else if (myCube[53] == Colors.white && myCube[33] == Colors.blue && myCube[26] == Colors.orange)
            {
                corners[7] = new Cubie(2, 0);
            }
            else if (myCube[53] == Colors.orange && myCube[33] == Colors.white && myCube[26] == Colors.blue)
            {
                corners[7] = new Cubie(2, 2);
            }
            else if (myCube[53] == Colors.blue && myCube[33] == Colors.orange && myCube[26] == Colors.white)
            {
                corners[7] = new Cubie(2, 1);
            }
            //7-3
            else if (myCube[53] == Colors.white && myCube[33] == Colors.red && myCube[26] == Colors.blue)
            {
                corners[7] = new Cubie(3, 0);
            }
            else if (myCube[53] == Colors.blue && myCube[33] == Colors.white && myCube[26] == Colors.red)
            {
                corners[7] = new Cubie(3, 2);
            }
            else if (myCube[53] == Colors.red && myCube[33] == Colors.blue && myCube[26] == Colors.white)
            {
                corners[7] = new Cubie(3, 1);
            }
            //7-4
            else if (myCube[53] == Colors.yellow && myCube[33] == Colors.red && myCube[26] == Colors.green)
            {
                corners[7] = new Cubie(4, 0);
            }
            else if (myCube[53] == Colors.green && myCube[33] == Colors.yellow && myCube[26] == Colors.red)
            {
                corners[7] = new Cubie(4, 2);
            }
            else if (myCube[53] == Colors.red && myCube[33] == Colors.green && myCube[26] == Colors.yellow)
            {
                corners[7] = new Cubie(4, 1);
            }
            //7-5
            else if (myCube[53] == Colors.yellow && myCube[33] == Colors.green && myCube[26] == Colors.orange)
            {
                corners[7] = new Cubie(5, 0);
            }
            else if (myCube[53] == Colors.orange && myCube[33] == Colors.yellow && myCube[26] == Colors.green)
            {
                corners[7] = new Cubie(5, 2);
            }
            else if (myCube[53] == Colors.green && myCube[33] == Colors.orange && myCube[26] == Colors.yellow)
            {
                corners[7] = new Cubie(5, 1);
            }
            //7-6
            else if (myCube[53] == Colors.yellow && myCube[33] == Colors.orange && myCube[26] == Colors.blue)
            {
                corners[7] = new Cubie(6, 0);
            }
            else if (myCube[53] == Colors.blue && myCube[33] == Colors.yellow && myCube[26] == Colors.orange)
            {
                corners[7] = new Cubie(6, 2);
            }
            else if (myCube[53] == Colors.orange && myCube[33] == Colors.blue && myCube[26] == Colors.yellow)
            {
                corners[7] = new Cubie(6, 1);
            }
            //7-7
            else if (myCube[53] == Colors.yellow && myCube[33] == Colors.blue && myCube[26] == Colors.red)
            {
                corners[7] = new Cubie(7, 0);
            }
            else if (myCube[53] == Colors.red && myCube[33] == Colors.yellow && myCube[26] == Colors.blue)
            {
                corners[7] = new Cubie(7, 2);
            }
            else if (myCube[53] == Colors.blue && myCube[33] == Colors.red && myCube[26] == Colors.yellow)
            {
                corners[7] = new Cubie(7, 1);
            }
            #endregion

            #region Edge0
            //0-0
            if (myCube[5] == Colors.white && myCube[19] == Colors.red)
            {
                edges[0] = new Cubie(0, 0);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.white)
            {
                edges[0] = new Cubie(0, 1);
            }
            //0-1
            else if (myCube[5] == Colors.white && myCube[19] == Colors.green)
            {
                edges[0] = new Cubie(1, 0);
            }
            else if (myCube[5] == Colors.green && myCube[19] == Colors.white)
            {
                edges[0] = new Cubie(1, 1);
            }
            //0-2
            else if (myCube[5] == Colors.white && myCube[19] == Colors.orange)
            {
                edges[0] = new Cubie(2, 0);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.white)
            {
                edges[0] = new Cubie(2, 1);
            }
            //0-3
            else if (myCube[5] == Colors.white && myCube[19] == Colors.blue)
            {
                edges[0] = new Cubie(3, 0);
            }
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.white)
            {
                edges[0] = new Cubie(3, 1);
            }
            //0-4
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.red)
            {
                edges[0] = new Cubie(4, 0);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.yellow)
            {
                edges[0] = new Cubie(4, 1);
            }
            //0-5
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.green)
            {
                edges[0] = new Cubie(5, 0);
            }
            else if (myCube[5] == Colors.green && myCube[19] == Colors.yellow)
            {
                edges[0] = new Cubie(5, 1);
            }
            //0-6
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.orange)
            {
                edges[0] = new Cubie(6, 0);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.yellow)
            {
                edges[0] = new Cubie(6, 1);
            }
            //0-7
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.blue)
            {
                edges[0] = new Cubie(7, 0);
            }
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.yellow)
            {
                edges[0] = new Cubie(7, 1);
            }
            //0-8
            else if (myCube[5] == Colors.green && myCube[19] == Colors.red)
            {
                edges[0] = new Cubie(8, 0);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.green)
            {
                edges[0] = new Cubie(8, 1);
            }
            //0-9
            else if (myCube[5] == Colors.green && myCube[19] == Colors.orange)
            {
                edges[0] = new Cubie(9, 0);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.green)
            {
                edges[0] = new Cubie(9, 1);
            }
            //0-10
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.orange)
            {
                edges[0] = new Cubie(10, 0);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.blue)
            {
                edges[0] = new Cubie(10, 1);
            }
            //0-11
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.red)
            {
                edges[0] = new Cubie(11, 0);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.blue)
            {
                edges[0] = new Cubie(11, 1);
            }
            #endregion
            #region Edge1
            //1-0
            if (myCube[7] == Colors.white && myCube[10] == Colors.red)
            {
                edges[1] = new Cubie(0, 0);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.white)
            {
                edges[1] = new Cubie(0, 1);
            }
            //1-1
            else if (myCube[7] == Colors.white && myCube[10] == Colors.green)
            {
                edges[1] = new Cubie(1, 0);
            }
            else if (myCube[7] == Colors.green && myCube[10] == Colors.white)
            {
                edges[1] = new Cubie(1, 1);
            }
            //1-2
            else if (myCube[7] == Colors.white && myCube[10] == Colors.orange)
            {
                edges[1] = new Cubie(2, 0);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.white)
            {
                edges[1] = new Cubie(2, 1);
            }
            //1-3
            else if (myCube[7] == Colors.white && myCube[10] == Colors.blue)
            {
                edges[1] = new Cubie(3, 0);
            }
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.white)
            {
                edges[1] = new Cubie(3, 1);
            }
            //1-4
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.red)
            {
                edges[1] = new Cubie(4, 0);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.yellow)
            {
                edges[1] = new Cubie(4, 1);
            }
            //1-5
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.green)
            {
                edges[1] = new Cubie(5, 0);
            }
            else if (myCube[7] == Colors.green && myCube[10] == Colors.yellow)
            {
                edges[1] = new Cubie(5, 1);
            }
            //1-6
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.orange)
            {
                edges[1] = new Cubie(6, 0);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.yellow)
            {
                edges[1] = new Cubie(6, 1);
            }
            //1-7
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.blue)
            {
                edges[1] = new Cubie(7, 0);
            }
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.yellow)
            {
                edges[1] = new Cubie(7, 1);
            }
            //1-8
            else if (myCube[7] == Colors.green && myCube[10] == Colors.red)
            {
                edges[1] = new Cubie(8, 0);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.green)
            {
                edges[1] = new Cubie(8, 1);
            }
            //1-9
            else if (myCube[7] == Colors.green && myCube[10] == Colors.orange)
            {
                edges[1] = new Cubie(9, 0);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.green)
            {
                edges[1] = new Cubie(9, 1);
            }
            //1-10
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.orange)
            {
                edges[1] = new Cubie(10, 0);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.blue)
            {
                edges[1] = new Cubie(10, 1);
            }
            //1-11
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.red)
            {
                edges[1] = new Cubie(11, 0);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.blue)
            {
                edges[1] = new Cubie(11, 1);
            }
            #endregion
            #region Edge2
            //2-0
            if (myCube[3] == Colors.white && myCube[37] == Colors.red)
            {
                edges[2] = new Cubie(0, 0);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.white)
            {
                edges[2] = new Cubie(0, 1);
            }
            //2-1
            else if (myCube[3] == Colors.white && myCube[37] == Colors.green)
            {
                edges[2] = new Cubie(1, 0);
            }
            else if (myCube[3] == Colors.green && myCube[37] == Colors.white)
            {
                edges[2] = new Cubie(1, 1);
            }
            //2-2
            else if (myCube[3] == Colors.white && myCube[37] == Colors.orange)
            {
                edges[2] = new Cubie(2, 0);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.white)
            {
                edges[2] = new Cubie(2, 1);
            }
            //2-3
            else if (myCube[3] == Colors.white && myCube[37] == Colors.blue)
            {
                edges[2] = new Cubie(3, 0);
            }
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.white)
            {
                edges[2] = new Cubie(3, 1);
            }
            //2-4
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.red)
            {
                edges[2] = new Cubie(4, 0);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.yellow)
            {
                edges[2] = new Cubie(4, 1);
            }
            //2-5
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.green)
            {
                edges[2] = new Cubie(5, 0);
            }
            else if (myCube[3] == Colors.green && myCube[37] == Colors.yellow)
            {
                edges[2] = new Cubie(5, 1);
            }
            //2-6
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.orange)
            {
                edges[2] = new Cubie(6, 0);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.yellow)
            {
                edges[2] = new Cubie(6, 1);
            }
            //2-7
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.blue)
            {
                edges[2] = new Cubie(7, 0);
            }
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.yellow)
            {
                edges[2] = new Cubie(7, 1);
            }
            //2-8
            else if (myCube[3] == Colors.green && myCube[37] == Colors.red)
            {
                edges[2] = new Cubie(8, 0);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.green)
            {
                edges[2] = new Cubie(8, 1);
            }
            //2-9
            else if (myCube[3] == Colors.green && myCube[37] == Colors.orange)
            {
                edges[2] = new Cubie(9, 0);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.green)
            {
                edges[2] = new Cubie(9, 1);
            }
            //2-10
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.orange)
            {
                edges[2] = new Cubie(10, 0);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.blue)
            {
                edges[2] = new Cubie(10, 1);
            }
            //2-11
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.red)
            {
                edges[2] = new Cubie(11, 0);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.blue)
            {
                edges[2] = new Cubie(11, 1);
            }
            #endregion
            #region Edge3
            //3-0
            if (myCube[1] == Colors.white && myCube[28] == Colors.red)
            {
                edges[3] = new Cubie(0, 0);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.white)
            {
                edges[3] = new Cubie(0, 1);
            }
            //3-1
            else if (myCube[1] == Colors.white && myCube[28] == Colors.green)
            {
                edges[3] = new Cubie(1, 0);
            }
            else if (myCube[1] == Colors.green && myCube[28] == Colors.white)
            {
                edges[3] = new Cubie(1, 1);
            }
            //3-2
            else if (myCube[1] == Colors.white && myCube[28] == Colors.orange)
            {
                edges[3] = new Cubie(2, 0);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.white)
            {
                edges[3] = new Cubie(2, 1);
            }
            //3-3
            else if (myCube[1] == Colors.white && myCube[28] == Colors.blue)
            {
                edges[3] = new Cubie(3, 0);
            }
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.white)
            {
                edges[3] = new Cubie(3, 1);
            }
            //3-4
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.red)
            {
                edges[3] = new Cubie(4, 0);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.yellow)
            {
                edges[3] = new Cubie(4, 1);
            }
            //3-5
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.green)
            {
                edges[3] = new Cubie(5, 0);
            }
            else if (myCube[1] == Colors.green && myCube[28] == Colors.yellow)
            {
                edges[3] = new Cubie(5, 1);
            }
            //3-6
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.orange)
            {
                edges[3] = new Cubie(6, 0);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.yellow)
            {
                edges[3] = new Cubie(6, 1);
            }
            //3-7
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.blue)
            {
                edges[3] = new Cubie(7, 0);
            }
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.yellow)
            {
                edges[3] = new Cubie(7, 1);
            }
            //3-8
            else if (myCube[1] == Colors.green && myCube[28] == Colors.red)
            {
                edges[3] = new Cubie(8, 0);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.green)
            {
                edges[3] = new Cubie(8, 1);
            }
            //3-9
            else if (myCube[1] == Colors.green && myCube[28] == Colors.orange)
            {
                edges[3] = new Cubie(9, 0);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.green)
            {
                edges[3] = new Cubie(9, 1);
            }
            //3-10
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.orange)
            {
                edges[3] = new Cubie(10, 0);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.blue)
            {
                edges[3] = new Cubie(10, 1);
            }
            //3-11
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.red)
            {
                edges[3] = new Cubie(11, 0);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.blue)
            {
                edges[3] = new Cubie(11, 1);
            }
            #endregion
            #region Edge4
            //4-0
            if (myCube[50] == Colors.white && myCube[25] == Colors.red)
            {
                edges[4] = new Cubie(0, 0);
            }
            else if (myCube[50] == Colors.red && myCube[25] == Colors.white)
            {
                edges[4] = new Cubie(0, 1);
            }
            //4-1
            else if (myCube[50] == Colors.white && myCube[25] == Colors.green)
            {
                edges[4] = new Cubie(1, 0);
            }
            else if (myCube[50] == Colors.green && myCube[25] == Colors.white)
            {
                edges[4] = new Cubie(1, 1);
            }
            //4-2
            else if (myCube[50] == Colors.white && myCube[25] == Colors.orange)
            {
                edges[4] = new Cubie(2, 0);
            }
            else if (myCube[50] == Colors.orange && myCube[25] == Colors.white)
            {
                edges[4] = new Cubie(2, 1);
            }
            //4-3
            else if (myCube[50] == Colors.white && myCube[25] == Colors.blue)
            {
                edges[4] = new Cubie(3, 0);
            }
            else if (myCube[50] == Colors.blue && myCube[25] == Colors.white)
            {
                edges[4] = new Cubie(3, 1);
            }
            //4-4
            else if (myCube[50] == Colors.yellow && myCube[25] == Colors.red)
            {
                edges[4] = new Cubie(4, 0);
            }
            else if (myCube[50] == Colors.red && myCube[25] == Colors.yellow)
            {
                edges[4] = new Cubie(4, 1);
            }
            //4-5
            else if (myCube[50] == Colors.yellow && myCube[25] == Colors.green)
            {
                edges[4] = new Cubie(5, 0);
            }
            else if (myCube[50] == Colors.green && myCube[25] == Colors.yellow)
            {
                edges[4] = new Cubie(5, 1);
            }
            //4-6
            else if (myCube[50] == Colors.yellow && myCube[25] == Colors.orange)
            {
                edges[4] = new Cubie(6, 0);
            }
            else if (myCube[50] == Colors.orange && myCube[25] == Colors.yellow)
            {
                edges[4] = new Cubie(6, 1);
            }
            //4-7
            else if (myCube[50] == Colors.yellow && myCube[25] == Colors.blue)
            {
                edges[4] = new Cubie(7, 0);
            }
            else if (myCube[50] == Colors.blue && myCube[25] == Colors.yellow)
            {
                edges[4] = new Cubie(7, 1);
            }
            //4-8
            else if (myCube[50] == Colors.green && myCube[25] == Colors.red)
            {
                edges[4] = new Cubie(8, 0);
            }
            else if (myCube[50] == Colors.red && myCube[25] == Colors.green)
            {
                edges[4] = new Cubie(8, 1);
            }
            //4-9
            else if (myCube[50] == Colors.green && myCube[25] == Colors.orange)
            {
                edges[4] = new Cubie(9, 0);
            }
            else if (myCube[50] == Colors.orange && myCube[25] == Colors.green)
            {
                edges[4] = new Cubie(9, 1);
            }
            //4-10
            else if (myCube[50] == Colors.blue && myCube[25] == Colors.orange)
            {
                edges[4] = new Cubie(10, 0);
            }
            else if (myCube[50] == Colors.orange && myCube[25] == Colors.blue)
            {
                edges[4] = new Cubie(10, 1);
            }
            //4-11
            else if (myCube[50] == Colors.blue && myCube[25] == Colors.red)
            {
                edges[4] = new Cubie(11, 0);
            }
            else if (myCube[50] == Colors.red && myCube[25] == Colors.blue)
            {
                edges[4] = new Cubie(11, 1);
            }
            #endregion
            #region Edge5
            //5-0
            if (myCube[46] == Colors.white && myCube[16] == Colors.red)
            {
                edges[5] = new Cubie(0, 0);
            }
            else if (myCube[46] == Colors.red && myCube[16] == Colors.white)
            {
                edges[5] = new Cubie(0, 1);
            }
            //5-1
            else if (myCube[46] == Colors.white && myCube[16] == Colors.green)
            {
                edges[5] = new Cubie(1, 0);
            }
            else if (myCube[46] == Colors.green && myCube[16] == Colors.white)
            {
                edges[5] = new Cubie(1, 1);
            }
            //5-2
            else if (myCube[46] == Colors.white && myCube[16] == Colors.orange)
            {
                edges[5] = new Cubie(2, 0);
            }
            else if (myCube[46] == Colors.orange && myCube[16] == Colors.white)
            {
                edges[5] = new Cubie(2, 1);
            }
            //5-3
            else if (myCube[46] == Colors.white && myCube[16] == Colors.blue)
            {
                edges[5] = new Cubie(3, 0);
            }
            else if (myCube[46] == Colors.blue && myCube[16] == Colors.white)
            {
                edges[5] = new Cubie(3, 1);
            }
            //5-4
            else if (myCube[46] == Colors.yellow && myCube[16] == Colors.red)
            {
                edges[5] = new Cubie(4, 0);
            }
            else if (myCube[46] == Colors.red && myCube[16] == Colors.yellow)
            {
                edges[5] = new Cubie(4, 1);
            }
            //5-5
            else if (myCube[46] == Colors.yellow && myCube[16] == Colors.green)
            {
                edges[5] = new Cubie(5, 0);
            }
            else if (myCube[46] == Colors.green && myCube[16] == Colors.yellow)
            {
                edges[5] = new Cubie(5, 1);
            }
            //5-6
            else if (myCube[46] == Colors.yellow && myCube[16] == Colors.orange)
            {
                edges[5] = new Cubie(6, 0);
            }
            else if (myCube[46] == Colors.orange && myCube[16] == Colors.yellow)
            {
                edges[5] = new Cubie(6, 1);
            }
            //5-7
            else if (myCube[46] == Colors.yellow && myCube[16] == Colors.blue)
            {
                edges[5] = new Cubie(7, 0);
            }
            else if (myCube[46] == Colors.blue && myCube[16] == Colors.yellow)
            {
                edges[5] = new Cubie(7, 1);
            }
            //5-8
            else if (myCube[46] == Colors.green && myCube[16] == Colors.red)
            {
                edges[5] = new Cubie(8, 0);
            }
            else if (myCube[46] == Colors.red && myCube[16] == Colors.green)
            {
                edges[5] = new Cubie(8, 1);
            }
            //5-9
            else if (myCube[46] == Colors.green && myCube[16] == Colors.orange)
            {
                edges[5] = new Cubie(9, 0);
            }
            else if (myCube[46] == Colors.orange && myCube[16] == Colors.green)
            {
                edges[5] = new Cubie(9, 1);
            }
            //5-10
            else if (myCube[46] == Colors.blue && myCube[16] == Colors.orange)
            {
                edges[5] = new Cubie(10, 0);
            }
            else if (myCube[46] == Colors.orange && myCube[16] == Colors.blue)
            {
                edges[5] = new Cubie(10, 1);
            }
            //5-11
            else if (myCube[46] == Colors.blue && myCube[16] == Colors.red)
            {
                edges[5] = new Cubie(11, 0);
            }
            else if (myCube[46] == Colors.red && myCube[16] == Colors.blue)
            {
                edges[5] = new Cubie(11, 1);
            }
            #endregion
            #region Edge6
            //6-0
            if (myCube[48] == Colors.white && myCube[43] == Colors.red)
            {
                edges[6] = new Cubie(0, 0);
            }
            else if (myCube[48] == Colors.red && myCube[43] == Colors.white)
            {
                edges[6] = new Cubie(0, 1);
            }
            //6-1
            else if (myCube[48] == Colors.white && myCube[43] == Colors.green)
            {
                edges[6] = new Cubie(1, 0);
            }
            else if (myCube[48] == Colors.green && myCube[43] == Colors.white)
            {
                edges[6] = new Cubie(1, 1);
            }
            //6-2
            else if (myCube[48] == Colors.white && myCube[43] == Colors.orange)
            {
                edges[6] = new Cubie(2, 0);
            }
            else if (myCube[48] == Colors.orange && myCube[43] == Colors.white)
            {
                edges[6] = new Cubie(2, 1);
            }
            //6-3
            else if (myCube[48] == Colors.white && myCube[43] == Colors.blue)
            {
                edges[6] = new Cubie(3, 0);
            }
            else if (myCube[48] == Colors.blue && myCube[43] == Colors.white)
            {
                edges[6] = new Cubie(3, 1);
            }
            //6-4
            else if (myCube[48] == Colors.yellow && myCube[43] == Colors.red)
            {
                edges[6] = new Cubie(4, 0);
            }
            else if (myCube[48] == Colors.red && myCube[43] == Colors.yellow)
            {
                edges[6] = new Cubie(4, 1);
            }
            //6-5
            else if (myCube[48] == Colors.yellow && myCube[43] == Colors.green)
            {
                edges[6] = new Cubie(5, 0);
            }
            else if (myCube[48] == Colors.green && myCube[43] == Colors.yellow)
            {
                edges[6] = new Cubie(5, 1);
            }
            //6-6
            else if (myCube[48] == Colors.yellow && myCube[43] == Colors.orange)
            {
                edges[6] = new Cubie(6, 0);
            }
            else if (myCube[48] == Colors.orange && myCube[43] == Colors.yellow)
            {
                edges[6] = new Cubie(6, 1);
            }
            //6-7
            else if (myCube[48] == Colors.yellow && myCube[43] == Colors.blue)
            {
                edges[6] = new Cubie(7, 0);
            }
            else if (myCube[48] == Colors.blue && myCube[43] == Colors.yellow)
            {
                edges[6] = new Cubie(7, 1);
            }
            //6-8
            else if (myCube[48] == Colors.green && myCube[43] == Colors.red)
            {
                edges[6] = new Cubie(8, 0);
            }
            else if (myCube[48] == Colors.red && myCube[43] == Colors.green)
            {
                edges[6] = new Cubie(8, 1);
            }
            //6-9
            else if (myCube[48] == Colors.green && myCube[43] == Colors.orange)
            {
                edges[6] = new Cubie(9, 0);
            }
            else if (myCube[48] == Colors.orange && myCube[43] == Colors.green)
            {
                edges[6] = new Cubie(9, 1);
            }
            //6-10
            else if (myCube[48] == Colors.blue && myCube[43] == Colors.orange)
            {
                edges[6] = new Cubie(10, 0);
            }
            else if (myCube[48] == Colors.orange && myCube[43] == Colors.blue)
            {
                edges[6] = new Cubie(10, 1);
            }
            //6-11
            else if (myCube[48] == Colors.blue && myCube[43] == Colors.red)
            {
                edges[6] = new Cubie(11, 0);
            }
            else if (myCube[48] == Colors.red && myCube[43] == Colors.blue)
            {
                edges[6] = new Cubie(11, 1);
            }
            #endregion
            #region Edge7
            //7-0
            if (myCube[52] == Colors.white && myCube[34] == Colors.red)
            {
                edges[7] = new Cubie(0, 0);
            }
            else if (myCube[52] == Colors.red && myCube[34] == Colors.white)
            {
                edges[7] = new Cubie(0, 1);
            }
            //7-1
            else if (myCube[52] == Colors.white && myCube[34] == Colors.green)
            {
                edges[7] = new Cubie(1, 0);
            }
            else if (myCube[52] == Colors.green && myCube[34] == Colors.white)
            {
                edges[7] = new Cubie(1, 1);
            }
            //7-2
            else if (myCube[52] == Colors.white && myCube[34] == Colors.orange)
            {
                edges[7] = new Cubie(2, 0);
            }
            else if (myCube[52] == Colors.orange && myCube[34] == Colors.white)
            {
                edges[7] = new Cubie(2, 1);
            }
            //7-3
            else if (myCube[52] == Colors.white && myCube[34] == Colors.blue)
            {
                edges[7] = new Cubie(3, 0);
            }
            else if (myCube[52] == Colors.blue && myCube[34] == Colors.white)
            {
                edges[7] = new Cubie(3, 1);
            }
            //7-4
            else if (myCube[52] == Colors.yellow && myCube[34] == Colors.red)
            {
                edges[7] = new Cubie(4, 0);
            }
            else if (myCube[52] == Colors.red && myCube[34] == Colors.yellow)
            {
                edges[7] = new Cubie(4, 1);
            }
            //7-5
            else if (myCube[52] == Colors.yellow && myCube[34] == Colors.green)
            {
                edges[7] = new Cubie(5, 0);
            }
            else if (myCube[52] == Colors.green && myCube[34] == Colors.yellow)
            {
                edges[7] = new Cubie(5, 1);
            }
            //7-6
            else if (myCube[52] == Colors.yellow && myCube[34] == Colors.orange)
            {
                edges[7] = new Cubie(6, 0);
            }
            else if (myCube[52] == Colors.orange && myCube[34] == Colors.yellow)
            {
                edges[7] = new Cubie(6, 1);
            }
            //7-7
            else if (myCube[52] == Colors.yellow && myCube[34] == Colors.blue)
            {
                edges[7] = new Cubie(7, 0);
            }
            else if (myCube[52] == Colors.blue && myCube[34] == Colors.yellow)
            {
                edges[7] = new Cubie(7, 1);
            }
            //7-8
            else if (myCube[52] == Colors.green && myCube[34] == Colors.red)
            {
                edges[7] = new Cubie(8, 0);
            }
            else if (myCube[52] == Colors.red && myCube[34] == Colors.green)
            {
                edges[7] = new Cubie(8, 1);
            }
            //7-9
            else if (myCube[52] == Colors.green && myCube[34] == Colors.orange)
            {
                edges[7] = new Cubie(9, 0);
            }
            else if (myCube[52] == Colors.orange && myCube[34] == Colors.green)
            {
                edges[7] = new Cubie(9, 1);
            }
            //7-10
            else if (myCube[52] == Colors.blue && myCube[34] == Colors.orange)
            {
                edges[7] = new Cubie(10, 0);
            }
            else if (myCube[52] == Colors.orange && myCube[34] == Colors.blue)
            {
                edges[7] = new Cubie(10, 1);
            }
            //7-11
            else if (myCube[52] == Colors.blue && myCube[34] == Colors.red)
            {
                edges[7] = new Cubie(11, 0);
            }
            else if (myCube[52] == Colors.red && myCube[34] == Colors.blue)
            {
                edges[7] = new Cubie(11, 1);
            }
            #endregion
            #region Edge8
            //8-0
            if (myCube[14] == Colors.white && myCube[21] == Colors.red)
            {
                edges[8] = new Cubie(0, 0);
            }
            else if (myCube[14] == Colors.red && myCube[21] == Colors.white)
            {
                edges[8] = new Cubie(0, 1);
            }
            //8-1
            else if (myCube[14] == Colors.white && myCube[21] == Colors.green)
            {
                edges[8] = new Cubie(1, 0);
            }
            else if (myCube[14] == Colors.green && myCube[21] == Colors.white)
            {
                edges[8] = new Cubie(1, 1);
            }
            //8-2
            else if (myCube[14] == Colors.white && myCube[21] == Colors.orange)
            {
                edges[8] = new Cubie(2, 0);
            }
            else if (myCube[14] == Colors.orange && myCube[21] == Colors.white)
            {
                edges[8] = new Cubie(2, 1);
            }
            //8-3
            else if (myCube[14] == Colors.white && myCube[21] == Colors.blue)
            {
                edges[8] = new Cubie(3, 0);
            }
            else if (myCube[14] == Colors.blue && myCube[21] == Colors.white)
            {
                edges[8] = new Cubie(3, 1);
            }
            //8-4
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.red)
            {
                edges[8] = new Cubie(4, 0);
            }
            else if (myCube[14] == Colors.red && myCube[21] == Colors.yellow)
            {
                edges[8] = new Cubie(4, 1);
            }
            //8-5
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.green)
            {
                edges[8] = new Cubie(5, 0);
            }
            else if (myCube[14] == Colors.green && myCube[21] == Colors.yellow)
            {
                edges[8] = new Cubie(5, 1);
            }
            //8-6
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.orange)
            {
                edges[8] = new Cubie(6, 0);
            }
            else if (myCube[14] == Colors.orange && myCube[21] == Colors.yellow)
            {
                edges[8] = new Cubie(6, 1);
            }
            //8-7
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.blue)
            {
                edges[8] = new Cubie(7, 0);
            }
            else if (myCube[14] == Colors.blue && myCube[21] == Colors.yellow)
            {
                edges[8] = new Cubie(7, 1);
            }
            //8-8
            else if (myCube[14] == Colors.green && myCube[21] == Colors.red)
            {
                edges[8] = new Cubie(8, 0);
            }
            else if (myCube[14] == Colors.red && myCube[21] == Colors.green)
            {
                edges[8] = new Cubie(8, 1);
            }
            //8-9
            else if (myCube[14] == Colors.green && myCube[21] == Colors.orange)
            {
                edges[8] = new Cubie(9, 0);
            }
            else if (myCube[14] == Colors.orange && myCube[21] == Colors.green)
            {
                edges[8] = new Cubie(9, 1);
            }
            //8-10
            else if (myCube[14] == Colors.blue && myCube[21] == Colors.orange)
            {
                edges[8] = new Cubie(10, 0);
            }
            else if (myCube[14] == Colors.orange && myCube[21] == Colors.blue)
            {
                edges[8] = new Cubie(10, 1);
            }
            //8-11
            else if (myCube[14] == Colors.blue && myCube[21] == Colors.red)
            {
                edges[8] = new Cubie(11, 0);
            }
            else if (myCube[14] == Colors.red && myCube[21] == Colors.blue)
            {
                edges[8] = new Cubie(11, 1);
            }
            #endregion
            #region Edge9
            //9-0
            if (myCube[12] == Colors.white && myCube[41] == Colors.red)
            {
                edges[9] = new Cubie(0, 0);
            }
            else if (myCube[12] == Colors.red && myCube[41] == Colors.white)
            {
                edges[9] = new Cubie(0, 1);
            }
            //9-1
            else if (myCube[12] == Colors.white && myCube[41] == Colors.green)
            {
                edges[9] = new Cubie(1, 0);
            }
            else if (myCube[12] == Colors.green && myCube[41] == Colors.white)
            {
                edges[9] = new Cubie(1, 1);
            }
            //9-2
            else if (myCube[12] == Colors.white && myCube[41] == Colors.orange)
            {
                edges[9] = new Cubie(2, 0);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.white)
            {
                edges[9] = new Cubie(2, 1);
            }
            //9-3
            else if (myCube[12] == Colors.white && myCube[41] == Colors.blue)
            {
                edges[9] = new Cubie(3, 0);
            }
            else if (myCube[12] == Colors.blue && myCube[41] == Colors.white)
            {
                edges[9] = new Cubie(3, 1);
            }
            //9-4
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.red)
            {
                edges[9] = new Cubie(4, 0);
            }
            else if (myCube[12] == Colors.red && myCube[41] == Colors.yellow)
            {
                edges[9] = new Cubie(4, 1);
            }
            //9-5
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.green)
            {
                edges[9] = new Cubie(5, 0);
            }
            else if (myCube[12] == Colors.green && myCube[41] == Colors.yellow)
            {
                edges[9] = new Cubie(5, 1);
            }
            //9-6
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.orange)
            {
                edges[9] = new Cubie(6, 0);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.yellow)
            {
                edges[9] = new Cubie(6, 1);
            }
            //9-7
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.blue)
            {
                edges[9] = new Cubie(7, 0);
            }
            else if (myCube[12] == Colors.blue && myCube[41] == Colors.yellow)
            {
                edges[9] = new Cubie(7, 1);
            }
            //9-8
            else if (myCube[12] == Colors.green && myCube[41] == Colors.red)
            {
                edges[9] = new Cubie(8, 0);
            }
            else if (myCube[12] == Colors.red && myCube[41] == Colors.green)
            {
                edges[9] = new Cubie(8, 1);
            }
            //9-9
            else if (myCube[12] == Colors.green && myCube[41] == Colors.orange)
            {
                edges[9] = new Cubie(9, 0);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.green)
            {
                edges[9] = new Cubie(9, 1);
            }
            //9-10
            else if (myCube[12] == Colors.blue && myCube[41] == Colors.orange)
            {
                edges[9] = new Cubie(10, 0);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.blue)
            {
                edges[9] = new Cubie(10, 1);
            }
            //9-11
            else if (myCube[12] == Colors.blue && myCube[41] == Colors.red)
            {
                edges[9] = new Cubie(11, 0);
            }
            else if (myCube[12] == Colors.red && myCube[41] == Colors.blue)
            {
                edges[9] = new Cubie(11, 1);
            }
            #endregion
            #region Edge10
            //10-0
            if (myCube[32] == Colors.white && myCube[39] == Colors.red)
            {
                edges[10] = new Cubie(0, 0);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.white)
            {
                edges[10] = new Cubie(0, 1);
            }
            //10-1
            else if (myCube[32] == Colors.white && myCube[39] == Colors.green)
            {
                edges[10] = new Cubie(1, 0);
            }
            else if (myCube[32] == Colors.green && myCube[39] == Colors.white)
            {
                edges[10] = new Cubie(1, 1);
            }
            //10-2
            else if (myCube[32] == Colors.white && myCube[39] == Colors.orange)
            {
                edges[10] = new Cubie(2, 0);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.white)
            {
                edges[10] = new Cubie(2, 1);
            }
            //10-3
            else if (myCube[32] == Colors.white && myCube[39] == Colors.blue)
            {
                edges[10] = new Cubie(3, 0);
            }
            else if (myCube[32] == Colors.blue && myCube[39] == Colors.white)
            {
                edges[10] = new Cubie(3, 1);
            }
            //10-4
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.red)
            {
                edges[10] = new Cubie(4, 0);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.yellow)
            {
                edges[10] = new Cubie(4, 1);
            }
            //10-5
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.green)
            {
                edges[10] = new Cubie(5, 0);
            }
            else if (myCube[32] == Colors.green && myCube[39] == Colors.yellow)
            {
                edges[10] = new Cubie(5, 1);
            }
            //10-6
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.orange)
            {
                edges[10] = new Cubie(6, 0);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.yellow)
            {
                edges[10] = new Cubie(6, 1);
            }
            //10-7
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.blue)
            {
                edges[10] = new Cubie(7, 0);
            }
            else if (myCube[32] == Colors.blue && myCube[39] == Colors.yellow)
            {
                edges[10] = new Cubie(7, 1);
            }
            //10-8
            else if (myCube[32] == Colors.green && myCube[39] == Colors.red)
            {
                edges[10] = new Cubie(8, 0);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.green)
            {
                edges[10] = new Cubie(8, 1);
            }
            //10-9
            else if (myCube[32] == Colors.green && myCube[39] == Colors.orange)
            {
                edges[10] = new Cubie(9, 0);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.green)
            {
                edges[10] = new Cubie(9, 1);
            }
            //10-10
            else if (myCube[32] == Colors.blue && myCube[39] == Colors.orange)
            {
                edges[10] = new Cubie(10, 0);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.blue)
            {
                edges[10] = new Cubie(10, 1);
            }
            //10-11
            else if (myCube[32] == Colors.blue && myCube[39] == Colors.red)
            {
                edges[10] = new Cubie(11, 0);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.blue)
            {
                edges[10] = new Cubie(11, 1);
            }
            #endregion
            #region Edge11
            //11-0
            if (myCube[30] == Colors.white && myCube[23] == Colors.red)
            {
                edges[11] = new Cubie(0, 0);
            }
            else if (myCube[30] == Colors.red && myCube[23] == Colors.white)
            {
                edges[11] = new Cubie(0, 1);
            }
            //11-1
            else if (myCube[30] == Colors.white && myCube[23] == Colors.green)
            {
                edges[11] = new Cubie(1, 0);
            }
            else if (myCube[30] == Colors.green && myCube[23] == Colors.white)
            {
                edges[11] = new Cubie(1, 1);
            }
            //11-2
            else if (myCube[30] == Colors.white && myCube[23] == Colors.orange)
            {
                edges[11] = new Cubie(2, 0);
            }
            else if (myCube[30] == Colors.orange && myCube[23] == Colors.white)
            {
                edges[11] = new Cubie(2, 1);
            }
            //11-3
            else if (myCube[30] == Colors.white && myCube[23] == Colors.blue)
            {
                edges[11] = new Cubie(3, 0);
            }
            else if (myCube[30] == Colors.blue && myCube[23] == Colors.white)
            {
                edges[11] = new Cubie(3, 1);
            }
            //11-4
            else if (myCube[30] == Colors.yellow && myCube[23] == Colors.red)
            {
                edges[11] = new Cubie(4, 0);
            }
            else if (myCube[30] == Colors.red && myCube[23] == Colors.yellow)
            {
                edges[11] = new Cubie(4, 1);
            }
            //11-5
            else if (myCube[30] == Colors.yellow && myCube[23] == Colors.green)
            {
                edges[11] = new Cubie(5, 0);
            }
            else if (myCube[30] == Colors.green && myCube[23] == Colors.yellow)
            {
                edges[11] = new Cubie(5, 1);
            }
            //11-6
            else if (myCube[30] == Colors.yellow && myCube[23] == Colors.orange)
            {
                edges[11] = new Cubie(6, 0);
            }
            else if (myCube[30] == Colors.orange && myCube[23] == Colors.yellow)
            {
                edges[11] = new Cubie(6, 1);
            }
            //11-7
            else if (myCube[30] == Colors.yellow && myCube[23] == Colors.blue)
            {
                edges[11] = new Cubie(7, 0);
            }
            else if (myCube[30] == Colors.blue && myCube[23] == Colors.yellow)
            {
                edges[11] = new Cubie(7, 1);
            }
            //11-8
            else if (myCube[30] == Colors.green && myCube[23] == Colors.red)
            {
                edges[11] = new Cubie(8, 0);
            }
            else if (myCube[30] == Colors.red && myCube[23] == Colors.green)
            {
                edges[11] = new Cubie(8, 1);
            }
            //11-9
            else if (myCube[30] == Colors.green && myCube[23] == Colors.orange)
            {
                edges[11] = new Cubie(9, 0);
            }
            else if (myCube[30] == Colors.orange && myCube[23] == Colors.green)
            {
                edges[11] = new Cubie(9, 1);
            }
            //11-10
            else if (myCube[30] == Colors.blue && myCube[23] == Colors.orange)
            {
                edges[11] = new Cubie(10, 0);
            }
            else if (myCube[30] == Colors.orange && myCube[23] == Colors.blue)
            {
                edges[11] = new Cubie(10, 1);
            }
            //11-11
            else if (myCube[30] == Colors.blue && myCube[23] == Colors.red)
            {
                edges[11] = new Cubie(11, 0);
            }
            else if (myCube[30] == Colors.red && myCube[23] == Colors.blue)
            {
                edges[11] = new Cubie(11, 1);
            }
            #endregion

            //string s = "";
            //for (int i = 0; i < 9; i++)
            //{
            //    s += edges[i] + "-" + i;
            //}
            //MessageBox.Show(s);
        }
        private bool checkFinished()
        {
            CenterOrient();
            for (int i = 0; i < 54; i++)
            {
                if (i < 9 && myCube[i] != Colors.white) return false;
                else if (i > 8 && i < 18 && myCube[i] != Colors.green) return false;
                else if (i > 17 && i < 27 && myCube[i] != Colors.red) return false;
                else if (i > 26 && i < 36 && myCube[i] != Colors.blue) return false;
                else if (i > 35 && i < 45 && myCube[i] != Colors.orange) return false;
                else if (i > 44 && i < 54 && myCube[i] != Colors.yellow) return false;
            }
            return true;
        }
        private void DoR(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[8];
            Colors temp2 = myCube[5];
            Colors temp3 = myCube[2];
            myCube[8] = myCube[17];
            myCube[17] = myCube[53];
            myCube[53] = myCube[27];
            myCube[27] = temp1;
            myCube[5] = myCube[14];
            myCube[14] = myCube[50];
            myCube[50] = myCube[30];
            myCube[30] = temp2;
            myCube[2] = myCube[11];
            myCube[11] = myCube[47];
            myCube[47] = myCube[33];
            myCube[33] = temp3;
            //face
            temp1 = myCube[18];
            temp2 = myCube[19];
            myCube[18] = myCube[24];
            myCube[24] = myCube[26];
            myCube[26] = myCube[20];
            myCube[20] = temp1;
            myCube[19] = myCube[21];
            myCube[21] = myCube[25];
            myCube[25] = myCube[23];
            myCube[23] = temp2;
        }
        private void DoU(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[9];
            Colors temp2 = myCube[10];
            Colors temp3 = myCube[11];
            myCube[9] = myCube[18];
            myCube[18] = myCube[27];
            myCube[27] = myCube[36];
            myCube[36] = temp1;
            myCube[10] = myCube[19];
            myCube[19] = myCube[28];
            myCube[28] = myCube[37];
            myCube[37] = temp2;
            myCube[11] = myCube[20];
            myCube[20] = myCube[29];
            myCube[29] = myCube[38];
            myCube[38] = temp3;
            //face
            temp1 = myCube[0];
            temp2 = myCube[1];
            myCube[0] = myCube[6];
            myCube[6] = myCube[8];
            myCube[8] = myCube[2];
            myCube[2] = temp1;
            myCube[1] = myCube[3];
            myCube[3] = myCube[7];
            myCube[7] = myCube[5];
            myCube[5] = temp2;

        }
        private void DoF(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[6];
            Colors temp2 = myCube[7];
            Colors temp3 = myCube[8];
            myCube[6] = myCube[44];
            myCube[44] = myCube[47];
            myCube[47] = myCube[18];
            myCube[18] = temp1;
            myCube[7] = myCube[41];
            myCube[41] = myCube[46];
            myCube[46] = myCube[21];
            myCube[21] = temp2;
            myCube[8] = myCube[38];
            myCube[38] = myCube[45];
            myCube[45] = myCube[24];
            myCube[24] = temp3;
            //face
            temp1 = myCube[9];
            temp2 = myCube[10];
            myCube[9] = myCube[15];
            myCube[15] = myCube[17];
            myCube[17] = myCube[11];
            myCube[11] = temp1;
            myCube[10] = myCube[12];
            myCube[12] = myCube[16];
            myCube[16] = myCube[14];
            myCube[14] = temp2;
        }
        private void DoL(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[6];
            Colors temp2 = myCube[3];
            Colors temp3 = myCube[0];
            myCube[6] = myCube[29];
            myCube[29] = myCube[51];
            myCube[51] = myCube[15];
            myCube[15] = temp1;
            myCube[3] = myCube[32];
            myCube[32] = myCube[48];
            myCube[48] = myCube[12];
            myCube[12] = temp2;
            myCube[0] = myCube[35];
            myCube[35] = myCube[45];
            myCube[45] = myCube[9];
            myCube[9] = temp3;
            //face
            temp1 = myCube[36];
            temp2 = myCube[37];
            myCube[36] = myCube[42];
            myCube[42] = myCube[44];
            myCube[44] = myCube[38];
            myCube[38] = temp1;
            myCube[37] = myCube[39];
            myCube[39] = myCube[43];
            myCube[43] = myCube[41];
            myCube[41] = temp2;
        }
        private void DoB(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[0];
            Colors temp2 = myCube[1];
            Colors temp3 = myCube[2];
            myCube[0] = myCube[20];
            myCube[20] = myCube[53];
            myCube[53] = myCube[42];
            myCube[42] = temp1;
            myCube[1] = myCube[23];
            myCube[23] = myCube[52];
            myCube[52] = myCube[39];
            myCube[39] = temp2;
            myCube[2] = myCube[26];
            myCube[26] = myCube[51];
            myCube[51] = myCube[36];
            myCube[36] = temp3;
            //face
            temp1 = myCube[27];
            temp2 = myCube[28];
            myCube[27] = myCube[33];
            myCube[33] = myCube[35];
            myCube[35] = myCube[29];
            myCube[29] = temp1;
            myCube[28] = myCube[30];
            myCube[30] = myCube[34];
            myCube[34] = myCube[32];
            myCube[32] = temp2;
        }
        private void DoD(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[15];
            Colors temp2 = myCube[16];
            Colors temp3 = myCube[17];
            myCube[15] = myCube[42];
            myCube[42] = myCube[33];
            myCube[33] = myCube[24];
            myCube[24] = temp1;
            myCube[16] = myCube[43];
            myCube[43] = myCube[34];
            myCube[34] = myCube[25];
            myCube[25] = temp2;
            myCube[17] = myCube[44];
            myCube[44] = myCube[35];
            myCube[35] = myCube[26];
            myCube[26] = temp3;
            //face
            temp1 = myCube[45];
            temp2 = myCube[46];
            myCube[45] = myCube[51];
            myCube[51] = myCube[53];
            myCube[53] = myCube[47];
            myCube[47] = temp1;
            myCube[46] = myCube[48];
            myCube[48] = myCube[52];
            myCube[52] = myCube[50];
            myCube[50] = temp2;
        }
        private void DoRPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[8];
            Colors temp2 = myCube[5];
            Colors temp3 = myCube[2];
            myCube[8] = myCube[27];
            myCube[27] = myCube[53];
            myCube[53] = myCube[17];
            myCube[17] = temp1;
            myCube[5] = myCube[30];
            myCube[30] = myCube[50];
            myCube[50] = myCube[14];
            myCube[14] = temp2;
            myCube[2] = myCube[33];
            myCube[33] = myCube[47];
            myCube[47] = myCube[11];
            myCube[11] = temp3;
            //face
            temp1 = myCube[18];
            temp2 = myCube[19];
            myCube[18] = myCube[20];
            myCube[20] = myCube[26];
            myCube[26] = myCube[24];
            myCube[24] = temp1;
            myCube[19] = myCube[23];
            myCube[23] = myCube[25];
            myCube[25] = myCube[21];
            myCube[21] = temp2;
        }
        private void DoUPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[9];
            Colors temp2 = myCube[10];
            Colors temp3 = myCube[11];
            myCube[9] = myCube[36];
            myCube[36] = myCube[27];
            myCube[27] = myCube[18];
            myCube[18] = temp1;
            myCube[10] = myCube[37];
            myCube[37] = myCube[28];
            myCube[28] = myCube[19];
            myCube[19] = temp2;
            myCube[11] = myCube[38];
            myCube[38] = myCube[29];
            myCube[29] = myCube[20];
            myCube[20] = temp3;
            //face
            temp1 = myCube[0];
            temp2 = myCube[1];
            myCube[0] = myCube[2];
            myCube[2] = myCube[8];
            myCube[8] = myCube[6];
            myCube[6] = temp1;
            myCube[1] = myCube[5];
            myCube[5] = myCube[7];
            myCube[7] = myCube[3];
            myCube[3] = temp2;
        }
        private void DoFPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[6];
            Colors temp2 = myCube[7];
            Colors temp3 = myCube[8];
            myCube[6] = myCube[18];
            myCube[18] = myCube[47];
            myCube[47] = myCube[44];
            myCube[44] = temp1;
            myCube[7] = myCube[21];
            myCube[21] = myCube[46];
            myCube[46] = myCube[41];
            myCube[41] = temp2;
            myCube[8] = myCube[24];
            myCube[24] = myCube[45];
            myCube[45] = myCube[38];
            myCube[38] = temp3;
            //face
            temp1 = myCube[9];
            temp2 = myCube[10];
            myCube[9] = myCube[11];
            myCube[11] = myCube[17];
            myCube[17] = myCube[15];
            myCube[15] = temp1;
            myCube[10] = myCube[14];
            myCube[14] = myCube[16];
            myCube[16] = myCube[12];
            myCube[12] = temp2;
        }
        private void DoLPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[6];
            Colors temp2 = myCube[3];
            Colors temp3 = myCube[0];
            myCube[6] = myCube[15];
            myCube[15] = myCube[51];
            myCube[51] = myCube[29];
            myCube[29] = temp1;
            myCube[3] = myCube[12];
            myCube[12] = myCube[48];
            myCube[48] = myCube[32];
            myCube[32] = temp2;
            myCube[0] = myCube[9];
            myCube[9] = myCube[45];
            myCube[45] = myCube[35];
            myCube[35] = temp3;
            //face
            temp1 = myCube[36];
            temp2 = myCube[37];
            myCube[36] = myCube[38];
            myCube[38] = myCube[44];
            myCube[44] = myCube[42];
            myCube[42] = temp1;
            myCube[37] = myCube[41];
            myCube[41] = myCube[43];
            myCube[43] = myCube[39];
            myCube[39] = temp2;
        }
        private void DoBPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[0];
            Colors temp2 = myCube[1];
            Colors temp3 = myCube[2];
            myCube[0] = myCube[42];
            myCube[42] = myCube[53];
            myCube[53] = myCube[20];
            myCube[20] = temp1;
            myCube[1] = myCube[39];
            myCube[39] = myCube[52];
            myCube[52] = myCube[23];
            myCube[23] = temp2;
            myCube[2] = myCube[36];
            myCube[36] = myCube[51];
            myCube[51] = myCube[26];
            myCube[26] = temp3;
            //face
            temp1 = myCube[27];
            temp2 = myCube[28];
            myCube[27] = myCube[29];
            myCube[29] = myCube[35];
            myCube[35] = myCube[33];
            myCube[33] = temp1;
            myCube[28] = myCube[32];
            myCube[32] = myCube[34];
            myCube[34] = myCube[30];
            myCube[30] = temp2;
        }
        private void DoDPrime(ref Colors[] myCube)
        {
            //side
            Colors temp1 = myCube[15];
            Colors temp2 = myCube[16];
            Colors temp3 = myCube[17];
            myCube[15] = myCube[24];
            myCube[24] = myCube[33];
            myCube[33] = myCube[42];
            myCube[42] = temp1;
            myCube[16] = myCube[25];
            myCube[25] = myCube[34];
            myCube[34] = myCube[43];
            myCube[43] = temp2;
            myCube[17] = myCube[26];
            myCube[26] = myCube[35];
            myCube[35] = myCube[44];
            myCube[44] = temp3;
            //face
            temp1 = myCube[45];
            temp2 = myCube[46];
            myCube[45] = myCube[47];
            myCube[47] = myCube[53];
            myCube[53] = myCube[51];
            myCube[51] = temp1;
            myCube[46] = myCube[50];
            myCube[50] = myCube[52];
            myCube[52] = myCube[48];
            myCube[48] = temp2;
        }
        private void DoF2(ref Colors[] myCube)
        {
            DoF(ref myCube);
            DoF(ref myCube);
        }
        private void DoR2(ref Colors[] myCube)
        {
            DoR(ref myCube);
            DoR(ref myCube);
        }
        private void DoU2(ref Colors[] myCube)
        {
            DoU(ref myCube);
            DoU(ref myCube);
        }
        private void DoL2(ref Colors[] myCube)
        {
            DoL(ref myCube);
            DoL(ref myCube);
        }
        private void DoD2(ref Colors[] myCube)
        {
            DoD(ref myCube);
            DoD(ref myCube);
        }
        private void DoB2(ref Colors[] myCube)
        {
            DoB(ref myCube);
            DoB(ref myCube);
        }
        private void DoY(ref Colors[] myCube)
        {
            DoU(ref myCube);
            DoDPrime(ref myCube);
            Colors temp1 = myCube[12];
            Colors temp2 = myCube[13];
            Colors temp3 = myCube[14];
            myCube[12] = myCube[21];
            myCube[21] = myCube[30];
            myCube[30] = myCube[39];
            myCube[39] = temp1;
            myCube[13] = myCube[22];
            myCube[22] = myCube[31];
            myCube[31] = myCube[40];
            myCube[40] = temp2;
            myCube[14] = myCube[23];
            myCube[23] = myCube[32];
            myCube[32] = myCube[41];
            myCube[41] = temp3;
        }
        private void DoYPrime(ref Colors[] myCube)
        {
            DoUPrime(ref myCube);
            DoD(ref myCube);
            Colors temp1 = myCube[12];
            Colors temp2 = myCube[13];
            Colors temp3 = myCube[14];
            myCube[12] = myCube[39];
            myCube[39] = myCube[30];
            myCube[30] = myCube[21];
            myCube[21] = temp1;
            myCube[13] = myCube[40];
            myCube[40] = myCube[31];
            myCube[31] = myCube[22];
            myCube[22] = temp2;
            myCube[14] = myCube[41];
            myCube[41] = myCube[32];
            myCube[32] = myCube[23];
            myCube[23] = temp3;
        }
        private void DoY2(ref Colors[] myCube)
        {
            DoY(ref myCube);
            DoY(ref myCube);
        }
        private void Dor(ref Colors[] myCube)
        {
            DoR(ref myCube);
            Colors temp1 = myCube[1];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[7];
            myCube[1] = myCube[10];
            myCube[10] = myCube[46];
            myCube[46] = myCube[34];
            myCube[34] = temp1;
            myCube[4] = myCube[13];
            myCube[13] = myCube[49];
            myCube[49] = myCube[31];
            myCube[31] = temp2;
            myCube[7] = myCube[16];
            myCube[16] = myCube[52];
            myCube[52] = myCube[28];
            myCube[28] = temp3;
        }
        private void Dor2(ref Colors[] myCube)
        {
            Dor(ref myCube);
            Dor(ref myCube);
        }
        private void DorPrime(ref Colors[] myCube)
        {
            DoRPrime(ref myCube);
            Colors temp1 = myCube[1];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[7];
            myCube[1] = myCube[34];
            myCube[34] = myCube[46];
            myCube[46] = myCube[10];
            myCube[10] = temp1;
            myCube[4] = myCube[31];
            myCube[31] = myCube[49];
            myCube[49] = myCube[13];
            myCube[13] = temp2;
            myCube[7] = myCube[28];
            myCube[28] = myCube[52];
            myCube[52] = myCube[16];
            myCube[16] = temp3;
        }
        private void Dof(ref Colors[] myCube)
        {
            DoF(ref myCube);
            Colors temp1 = myCube[3];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[5];
            myCube[3] = myCube[43];
            myCube[43] = myCube[50];
            myCube[50] = myCube[19];
            myCube[19] = temp1;
            myCube[4] = myCube[40];
            myCube[40] = myCube[49];
            myCube[49] = myCube[22];
            myCube[22] = temp2;
            myCube[5] = myCube[37];
            myCube[37] = myCube[48];
            myCube[48] = myCube[25];
            myCube[25] = temp3;
        }
        private void DofPrime(ref Colors[] myCube)
        {
            DoFPrime(ref myCube);
            Colors temp1 = myCube[3];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[5];
            myCube[3] = myCube[19];
            myCube[19] = myCube[50];
            myCube[50] = myCube[43];
            myCube[43] = temp1;
            myCube[4] = myCube[22];
            myCube[22] = myCube[49];
            myCube[49] = myCube[40];
            myCube[40] = temp2;
            myCube[5] = myCube[25];
            myCube[25] = myCube[48];
            myCube[48] = myCube[37];
            myCube[37] = temp3;
        }
        private void Dof2(ref Colors[] myCube)
        {
            Dof(ref myCube);
            Dof(ref myCube);
        }
        private void DoM2(ref Colors[] myCube)
        {
            swap(ref myCube[1], ref myCube[46]);
            swap(ref myCube[4], ref myCube[49]);
            swap(ref myCube[7], ref myCube[52]);
            swap(ref myCube[10], ref myCube[34]);
            swap(ref myCube[13], ref myCube[31]);
            swap(ref myCube[16], ref myCube[28]);

        }
        private void DoX(ref Colors[] myCube)
        {
            DoR(ref myCube);
            DoLPrime(ref myCube);
            Colors temp1 = myCube[1];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[7];
            myCube[1] = myCube[10];
            myCube[10] = myCube[46];
            myCube[46] = myCube[34];
            myCube[34] = temp1;
            myCube[4] = myCube[13];
            myCube[13] = myCube[49];
            myCube[49] = myCube[31];
            myCube[31] = temp2;
            myCube[7] = myCube[16];
            myCube[16] = myCube[52];
            myCube[52] = myCube[28];
            myCube[28] = temp3;
        }
        private void DoXPrime(ref Colors[] myCube)
        {
            DoRPrime(ref myCube);
            DoL(ref myCube);
            Colors temp1 = myCube[1];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[7];
            myCube[1] = myCube[34];
            myCube[34] = myCube[46];
            myCube[46] = myCube[10];
            myCube[10] = temp1;
            myCube[4] = myCube[31];
            myCube[31] = myCube[49];
            myCube[49] = myCube[13];
            myCube[13] = temp2;
            myCube[7] = myCube[28];
            myCube[28] = myCube[52];
            myCube[52] = myCube[16];
            myCube[16] = temp3;
        }
        private void DoX2(ref Colors[] myCube)
        {
            DoX(ref myCube);
            DoX(ref myCube);
        }
        private void DoZ(ref Colors[] myCube)
        {
            DoF(ref myCube);
            DoBPrime(ref myCube);
            Colors temp1 = myCube[3];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[5];
            myCube[3] = myCube[43];
            myCube[43] = myCube[50];
            myCube[50] = myCube[19];
            myCube[19] = temp1;
            myCube[4] = myCube[40];
            myCube[40] = myCube[49];
            myCube[49] = myCube[22];
            myCube[22] = temp2;
            myCube[5] = myCube[37];
            myCube[37] = myCube[48];
            myCube[48] = myCube[25];
            myCube[25] = temp3;
        }
        private void DoZPrime(ref Colors[] myCube)
        {
            DoFPrime(ref myCube);
            DoB(ref myCube);
            Colors temp1 = myCube[3];
            Colors temp2 = myCube[4];
            Colors temp3 = myCube[5];
            myCube[3] = myCube[19];
            myCube[19] = myCube[50];
            myCube[50] = myCube[43];
            myCube[43] = temp1;
            myCube[4] = myCube[22];
            myCube[22] = myCube[49];
            myCube[49] = myCube[40];
            myCube[40] = temp2;
            myCube[5] = myCube[25];
            myCube[25] = myCube[48];
            myCube[48] = myCube[37];
            myCube[37] = temp3;
        }
        private void DoZ2(ref Colors[] myCube)
        {
            DoZ(ref myCube);
            DoZ(ref myCube);
        }
        private void CenterOrient()
        {
            if (myCube[13] == Colors.green)
            {
                if (myCube[4] == Colors.white)
                {
                    //already oriented
                }
                else if (myCube[4] == Colors.red)
                {
                    execute("z", ref myCube);
                }
                else if (myCube[4] == Colors.yellow)
                {
                    execute("z2", ref myCube);
                }
                else if (myCube[4] == Colors.orange)
                {
                    execute("z'", ref myCube);
                }
            }
            else if (myCube[13] == Colors.red)
            {
                if (myCube[4] == Colors.white)
                {
                    execute("y'", ref myCube);
                }
                else if (myCube[4] == Colors.blue)
                {
                    execute("zy'", ref myCube);
                }
                else if (myCube[4] == Colors.yellow)
                {
                    execute("z2y'", ref myCube);
                }
                else if (myCube[4] == Colors.green)
                {
                    execute("z'y'", ref myCube);
                }
            }
            else if (myCube[13] == Colors.blue)
            {
                if (myCube[4] == Colors.white)
                {
                    execute("y2", ref myCube);
                }
                else if (myCube[4] == Colors.orange)
                {
                    execute("zy2", ref myCube);
                }
                else if (myCube[4] == Colors.yellow)
                {
                    execute("z2y2", ref myCube);
                }
                else if (myCube[4] == Colors.red)
                {
                    execute("z'y2", ref myCube);
                }
            }
            else if (myCube[13] == Colors.orange)
            {
                if (myCube[4] == Colors.white)
                {
                    execute("y", ref myCube);
                }
                else if (myCube[4] == Colors.green)
                {
                    execute("zy", ref myCube);
                }
                else if (myCube[4] == Colors.yellow)
                {
                    execute("z2y", ref myCube);
                }
                else if (myCube[4] == Colors.blue)
                {
                    execute("z'y", ref myCube);
                }
            }
            else if (myCube[13] == Colors.white)
            {
                if (myCube[4] == Colors.blue)
                {
                    execute("x", ref myCube);
                }
                else if (myCube[4] == Colors.red)
                {
                    execute("zx", ref myCube);
                }
                else if (myCube[4] == Colors.green)
                {
                    execute("z2x", ref myCube);
                }
                else if (myCube[4] == Colors.orange)
                {
                    execute("z'x", ref myCube);
                }
            }
            else if (myCube[13] == Colors.yellow)
            {
                if (myCube[4] == Colors.green)
                {
                    execute("x'", ref myCube);
                }
                else if (myCube[4] == Colors.red)
                {
                    execute("zx'", ref myCube);
                }
                else if (myCube[4] == Colors.blue)
                {
                    execute("z2x'", ref myCube);
                }
                else if (myCube[4] == Colors.orange)
                {
                    execute("z'x'", ref myCube);
                }
            }
        }
        private void swap(ref Colors a, ref Colors b)
        {
            Colors temp = a;
            a = b;
            b = temp;
        }
        private void Solve(ref Colors[] myCube)
        {
            doCross(ref myCube);
            doLayer2(ref myCube);
            TopCross(ref myCube);
            Oll(ref myCube);
            Last4Corners(ref myCube);
            GreenAlwaysInFront(ref myCube);
            Last4Edges(ref myCube);
        }
        private void doCross(ref Colors[] myCube)
        {
            //solve the first cross edge
            if (myCube[16] == Colors.yellow && myCube[46] == Colors.green)
            {
                execute("F2U'R'F", ref myCube);
            }
            else if (myCube[25] == Colors.yellow && myCube[50] == Colors.green)
            {
                execute("RF", ref myCube);
            }
            else if (myCube[25] == Colors.green && myCube[50] == Colors.yellow)
            {
                execute("D'", ref myCube);
            }
            else if (myCube[34] == Colors.yellow && myCube[52] == Colors.green)
            {
                execute("B2UR'F", ref myCube);
            }
            else if (myCube[34] == Colors.green && myCube[52] == Colors.yellow)
            {
                execute("D2", ref myCube);
            }
            else if (myCube[43] == Colors.yellow && myCube[48] == Colors.green)
            {
                execute("L'F'", ref myCube);
            }
            else if (myCube[43] == Colors.green && myCube[48] == Colors.yellow)
            {
                execute("D", ref myCube);

            }
            else if (myCube[12] == Colors.green && myCube[41] == Colors.yellow)
            {
                execute("F'", ref myCube);
            }
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.green)
            {
                execute("LD", ref myCube);
            }
            else if (myCube[14] == Colors.green && myCube[21] == Colors.yellow)
            {
                execute("F", ref myCube);
            }
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.green)
            {
                execute("R'D'", ref myCube);
            }
            else if (myCube[23] == Colors.green && myCube[30] == Colors.yellow)
            {
                execute("RD'", ref myCube);
            }
            else if (myCube[23] == Colors.yellow && myCube[30] == Colors.green)
            {
                execute("R2F", ref myCube);
            }
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.green)
            {
                execute("L'D", ref myCube);
            }
            else if (myCube[32] == Colors.green && myCube[39] == Colors.yellow)
            {
                execute("L2F'", ref myCube);
            }
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.green)
            {
                execute("F2", ref myCube);
            }
            else if (myCube[7] == Colors.green && myCube[10] == Colors.yellow)
            {
                execute("U'R'F", ref myCube);
            }
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.green)
            {
                execute("UF2", ref myCube);
            }
            else if (myCube[5] == Colors.green && myCube[19] == Colors.yellow)
            {
                execute("R'F", ref myCube);
            }
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.green)
            {
                execute("U2F2", ref myCube);
            }
            else if (myCube[1] == Colors.green && myCube[28] == Colors.yellow)
            {
                execute("UR'F", ref myCube);
            }
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.green)
            {
                execute("U'F2", ref myCube);
            }
            else if (myCube[3] == Colors.green && myCube[37] == Colors.yellow)
            {
                execute("LF'", ref myCube);
            }
            else
            {
                //already in correct position
            }
            DoY(ref myCube);
            //solve the second cross edge
            if (myCube[16] == Colors.yellow && myCube[46] == Colors.red)
            {
                execute("F2U'R'F", ref myCube);
            }
            else if (myCube[25] == Colors.yellow && myCube[50] == Colors.red)
            {
                execute("RF", ref myCube);
            }
            else if (myCube[25] == Colors.red && myCube[50] == Colors.yellow)
            {
                execute("L'D'L", ref myCube);
            }
            else if (myCube[34] == Colors.yellow && myCube[52] == Colors.red)
            {
                execute("B2UR'F", ref myCube);
            }
            else if (myCube[34] == Colors.red && myCube[52] == Colors.yellow)
            {
                execute("L'D2L", ref myCube);
            }
            else if (myCube[12] == Colors.red && myCube[41] == Colors.yellow)
            {
                execute("F'", ref myCube);
            }
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.red)
            {
                execute("FU'R'F", ref myCube);
            }
            else if (myCube[14] == Colors.red && myCube[21] == Colors.yellow)
            {
                execute("F", ref myCube);
            }
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.red)
            {
                execute("RUF2", ref myCube);
            }
            else if (myCube[23] == Colors.yellow && myCube[30] == Colors.red)
            {
                execute("R2F", ref myCube);
            }
            else if (myCube[23] == Colors.red && myCube[30] == Colors.yellow)
            {
                execute("R'UF2", ref myCube);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.yellow)
            {
                execute("B'U2F2", ref myCube);
            }
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.red)
            {
                execute("B'UR'F", ref myCube);
            }
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.red)
            {
                execute("F2", ref myCube);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.yellow)
            {
                execute("U'R'F", ref myCube);
            }
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.red)
            {
                execute("UF2", ref myCube);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.yellow)
            {
                execute("R'F", ref myCube);
            }
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.red)
            {
                execute("U2F2", ref myCube);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.yellow)
            {
                execute("UR'F", ref myCube);
            }
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.red)
            {
                execute("U'F2", ref myCube);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.yellow)
            {
                execute("LF'L'", ref myCube);
            }
            else
            {
                //already in correct position
            }
            DoY(ref myCube);
            //solve the third cross edge
            if (myCube[16] == Colors.yellow && myCube[46] == Colors.blue)
            {
                execute("F2U'R'F", ref myCube);
            }
            else if (myCube[25] == Colors.yellow && myCube[50] == Colors.blue)
            {
                execute("RF", ref myCube);
            }
            else if (myCube[25] == Colors.blue && myCube[50] == Colors.yellow)
            {
                execute("R2UF2", ref myCube);
            }
            else if (myCube[12] == Colors.blue && myCube[41] == Colors.yellow)
            {
                execute("F'", ref myCube);
            }
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.blue)
            {
                execute("FU'R'F", ref myCube);
            }
            else if (myCube[14] == Colors.blue && myCube[21] == Colors.yellow)
            {
                execute("F", ref myCube);
            }
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.blue)
            {
                execute("RUF2", ref myCube);
            }
            else if (myCube[23] == Colors.yellow && myCube[30] == Colors.blue)
            {
                execute("R2F", ref myCube);
            }
            else if (myCube[23] == Colors.blue && myCube[30] == Colors.yellow)
            {
                execute("R'UF2", ref myCube);
            }
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.blue)
            {
                execute("LU'L'F2", ref myCube);
            }
            else if (myCube[32] == Colors.blue && myCube[39] == Colors.yellow)
            {
                execute("LU2L'R'F", ref myCube);
            }
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.blue)
            {
                execute("F2", ref myCube);
            }
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.yellow)
            {
                execute("U'R'F", ref myCube);
            }
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.blue)
            {
                execute("UF2", ref myCube);
            }
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.yellow)
            {
                execute("R'F", ref myCube);
            }
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.blue)
            {
                execute("U2F2", ref myCube);
            }
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.yellow)
            {
                execute("UR'F", ref myCube);
            }
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.blue)
            {
                execute("U'F2", ref myCube);
            }
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.yellow)
            {
                execute("LF'L'", ref myCube);
            }
            else
            {
                //already correct
            }
            DoY(ref myCube);
            //solve the last cross edge
            if (myCube[16] == Colors.yellow && myCube[46] == Colors.orange)
            {
                execute("F2U'R'FR", ref myCube);
            }
            else if (myCube[7] == Colors.yellow && myCube[10] == Colors.orange)
            {
                execute("F2", ref myCube);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.yellow)
            {
                execute("U'R'FR", ref myCube);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.yellow)
            {
                execute("F'", ref myCube);
            }
            else if (myCube[12] == Colors.yellow && myCube[41] == Colors.orange)
            {
                execute("FU'R'FR", ref myCube);
            }
            else if (myCube[14] == Colors.orange && myCube[21] == Colors.yellow)
            {
                execute("F", ref myCube);
            }
            else if (myCube[14] == Colors.yellow && myCube[21] == Colors.orange)
            {
                execute("RUR'F2", ref myCube);
            }
            else if (myCube[23] == Colors.yellow && myCube[30] == Colors.orange)
            {
                execute("R2FR2", ref myCube);
            }
            else if (myCube[23] == Colors.orange && myCube[30] == Colors.yellow)
            {
                execute("R'URF2", ref myCube);
            }
            else if (myCube[32] == Colors.yellow && myCube[39] == Colors.orange)
            {
                execute("LU'L'F2", ref myCube);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.yellow)
            {
                execute("LU2L'R'FR", ref myCube);
            }
            else if (myCube[5] == Colors.yellow && myCube[19] == Colors.orange)
            {
                execute("UF2", ref myCube);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.yellow)
            {
                execute("R'FR", ref myCube);
            }
            else if (myCube[1] == Colors.yellow && myCube[28] == Colors.orange)
            {
                execute("U2F2", ref myCube);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.yellow)
            {
                execute("UR'FR", ref myCube);
            }
            else if (myCube[3] == Colors.yellow && myCube[37] == Colors.orange)
            {
                execute("U'F2", ref myCube);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.yellow)
            {
                execute("LF'L'", ref myCube);
            }
            else
            {
                //already correct
            }
        }
        private void doLayer2(ref Colors[] myCube)
        {
            //solve the first bottom-corner
            if (myCube[17] == Colors.yellow && myCube[24] == Colors.orange && myCube[47] == Colors.green)
            {
                execute("RUR'U'RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[17] == Colors.green && myCube[24] == Colors.yellow && myCube[47] == Colors.orange)
            {
                execute("RUR'U'RUR'", ref myCube);
            }
            else if (myCube[26] == Colors.orange && myCube[33] == Colors.green && myCube[53] == Colors.yellow)
            {
                execute("R'U2RU'RUR'", ref myCube);
            }
            else if (myCube[26] == Colors.yellow && myCube[33] == Colors.orange && myCube[53] == Colors.green)
            {
                execute("R'U2RU'F'U'F", ref myCube);
            }
            else if (myCube[26] == Colors.green && myCube[33] == Colors.yellow && myCube[53] == Colors.orange)
            {
                execute("R'URURUR'", ref myCube);
            }
            else if (myCube[35] == Colors.orange && myCube[42] == Colors.green && myCube[51] == Colors.yellow)
            {
                execute("LU2L'F'U'F", ref myCube);
            }
            else if (myCube[35] == Colors.yellow && myCube[42] == Colors.orange && myCube[51] == Colors.green)
            {
                execute("LU2L'RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[35] == Colors.green && myCube[42] == Colors.yellow && myCube[51] == Colors.orange)
            {
                execute("LU2L'RUR'", ref myCube);
            }
            else if (myCube[44] == Colors.orange && myCube[15] == Colors.green && myCube[45] == Colors.yellow)
            {
                execute("L'U'LRUR'", ref myCube);
            }
            else if (myCube[44] == Colors.yellow && myCube[15] == Colors.orange && myCube[45] == Colors.green)
            {
                execute("L'U'LF'U'F", ref myCube);
            }
            else if (myCube[44] == Colors.green && myCube[15] == Colors.yellow && myCube[45] == Colors.orange)
            {
                execute("L'ULU'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.yellow && myCube[18] == Colors.green)
            {
                execute("F'U'F", ref myCube);
            }
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.green && myCube[18] == Colors.orange)
            {
                execute("RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.orange && myCube[18] == Colors.yellow)
            {
                execute("RUR'", ref myCube);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.orange && myCube[27] == Colors.yellow)
            {
                execute("URUR'", ref myCube);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.yellow && myCube[27] == Colors.green)
            {
                execute("U2RU'R'", ref myCube);
            }
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.green && myCube[27] == Colors.orange)
            {
                execute("R2FR2F'", ref myCube);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.yellow && myCube[36] == Colors.green)
            {
                execute("RU2R'", ref myCube);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.orange && myCube[36] == Colors.yellow)
            {
                execute("U2RUR'", ref myCube);
            }
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.green && myCube[36] == Colors.orange)
            {
                execute("UR2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.green && myCube[9] == Colors.orange)
            {
                execute("U2R2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.yellow && myCube[9] == Colors.green)
            {
                execute("RU'R'", ref myCube);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.orange && myCube[9] == Colors.yellow)
            {
                execute("U'RUR'", ref myCube);
            }
            else
            {
                //already correct
            }

            //solve the first layer-2 edge
            if (myCube[14] == Colors.green && myCube[21] == Colors.orange)
            {
                execute("RU'R2U2RU'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.orange && myCube[30] == Colors.green)
            {
                execute("R'U2R2U'R'U'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.green && myCube[30] == Colors.orange)
            {
                execute("R'U'R2U'R2UR2UR'y", ref myCube);
            }
            else if (myCube[32] == Colors.green && myCube[39] == Colors.orange)
            {
                execute("LU'L'URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[32] == Colors.orange && myCube[39] == Colors.green)
            {
                execute("LU2L'U'R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[12] == Colors.orange && myCube[41] == Colors.green)
            {
                execute("L'ULR'FRF'RUR'y", ref myCube);
            }
            else if (myCube[12] == Colors.green && myCube[41] == Colors.orange)
            {
                execute("L'U'LURU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[7] == Colors.green && myCube[10] == Colors.orange)
            {
                execute("URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.green)
            {
                execute("U2R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.green)
            {
                execute("U'R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.green && myCube[19] == Colors.orange)
            {
                execute("U2RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.green)
            {
                execute("R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[1] == Colors.green && myCube[28] == Colors.orange)
            {
                execute("U'RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.green && myCube[37] == Colors.orange)
            {
                execute("RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.green)
            {
                execute("UR'FRF'RUR'y", ref myCube);
            }
            else
            {
                //already correct
                execute("y", ref myCube);
            }

            //solve the second bottom-corner
            if (myCube[17] == Colors.red && myCube[24] == Colors.yellow && myCube[47] == Colors.green)
            {
                execute("RUR'U'RUR'", ref myCube);
            }
            else if (myCube[17] == Colors.yellow && myCube[24] == Colors.green && myCube[47] == Colors.red)
            {
                execute("RU'R2FRF'", ref myCube);
            }
            else if (myCube[26] == Colors.green && myCube[33] == Colors.red && myCube[53] == Colors.yellow)
            {
                execute("R'U2RU'RUR'", ref myCube);
            }
            else if (myCube[26] == Colors.yellow && myCube[33] == Colors.green && myCube[53] == Colors.red)
            {
                execute("R'U2R2U'R'", ref myCube);
            }
            else if (myCube[26] == Colors.red && myCube[33] == Colors.yellow && myCube[53] == Colors.green)
            {
                execute("R'URURUR'", ref myCube);
            }
            else if (myCube[35] == Colors.green && myCube[42] == Colors.red && myCube[51] == Colors.yellow)
            {
                execute("LU2L'F'U'F", ref myCube);
            }
            else if (myCube[35] == Colors.yellow && myCube[42] == Colors.green && myCube[51] == Colors.red)
            {
                execute("LU2L'RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[35] == Colors.red && myCube[42] == Colors.yellow && myCube[51] == Colors.green)
            {
                execute("LU2L'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.green && myCube[11] == Colors.yellow && myCube[18] == Colors.red)
            {
                execute("URU'R'", ref myCube);
            }
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.red && myCube[18] == Colors.green)
            {
                execute("RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.green && myCube[18] == Colors.yellow)
            {
                execute("RUR'", ref myCube);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.green && myCube[27] == Colors.yellow)
            {
                execute("URUR'", ref myCube);
            }
            else if (myCube[2] == Colors.green && myCube[20] == Colors.yellow && myCube[27] == Colors.red)
            {
                execute("U2RU'R'", ref myCube);
            }
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.red && myCube[27] == Colors.green)
            {
                execute("R2FR2F'", ref myCube);
            }
            else if (myCube[0] == Colors.green && myCube[29] == Colors.yellow && myCube[36] == Colors.red)
            {
                execute("RU2R'", ref myCube);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.green && myCube[36] == Colors.yellow)
            {
                execute("U2RUR'", ref myCube);
            }
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.red && myCube[36] == Colors.green)
            {
                execute("UR2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.red && myCube[9] == Colors.green)
            {
                execute("U2R2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.green && myCube[38] == Colors.yellow && myCube[9] == Colors.red)
            {
                execute("RU'R'", ref myCube);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.green && myCube[9] == Colors.yellow)
            {
                execute("U'RUR'", ref myCube);
            }
            else
            {
                //already correct
            }

            //solve the second layer-2 edge
            if (myCube[14] == Colors.red && myCube[21] == Colors.green)
            {
                execute("RU'R2U2RU'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.green && myCube[30] == Colors.red)
            {
                execute("R'U2R2U'R'U'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.red && myCube[30] == Colors.green)
            {
                execute("R'U'R2U'R2UR2UR'y", ref myCube);
            }
            else if (myCube[32] == Colors.red && myCube[39] == Colors.green)
            {
                execute("LU'L'URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[32] == Colors.green && myCube[39] == Colors.red)
            {
                execute("LU2L'U'R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.green)
            {
                execute("URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[7] == Colors.green && myCube[10] == Colors.red)
            {
                execute("U2R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.green && myCube[19] == Colors.red)
            {
                execute("U'R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.green)
            {
                execute("U2RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[1] == Colors.green && myCube[28] == Colors.red)
            {
                execute("R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.green)
            {
                execute("U'RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.green)
            {
                execute("RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.green && myCube[37] == Colors.red)
            {
                execute("UR'FRF'RUR'y", ref myCube);
            }
            else
            {
                //already correct
                execute("y", ref myCube);
            }

            //solve the third bottom-corner
            if (myCube[17] == Colors.blue && myCube[24] == Colors.yellow && myCube[47] == Colors.red)
            {
                execute("RUR'U'RUR'", ref myCube);
            }
            else if (myCube[17] == Colors.yellow && myCube[24] == Colors.red && myCube[47] == Colors.blue)
            {
                execute("RU'R2FRF'", ref myCube);
            }
            else if (myCube[26] == Colors.red && myCube[33] == Colors.blue && myCube[53] == Colors.yellow)
            {
                execute("R'U2RU'RUR'", ref myCube);
            }
            else if (myCube[26] == Colors.yellow && myCube[33] == Colors.red && myCube[53] == Colors.blue)
            {
                execute("R'U2R2U'R'", ref myCube);
            }
            else if (myCube[26] == Colors.blue && myCube[33] == Colors.yellow && myCube[53] == Colors.red)
            {
                execute("R'URURUR'", ref myCube);
            }
            else if (myCube[8] == Colors.red && myCube[11] == Colors.yellow && myCube[18] == Colors.blue)
            {
                execute("URU'R'", ref myCube);
            }
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.blue && myCube[18] == Colors.red)
            {
                execute("RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.red && myCube[18] == Colors.yellow)
            {
                execute("RUR'", ref myCube);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.red && myCube[27] == Colors.yellow)
            {
                execute("URUR'", ref myCube);
            }
            else if (myCube[2] == Colors.red && myCube[20] == Colors.yellow && myCube[27] == Colors.blue)
            {
                execute("U2RU'R'", ref myCube);
            }
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.blue && myCube[27] == Colors.red)
            {
                execute("R2FR2F'", ref myCube);
            }
            else if (myCube[0] == Colors.red && myCube[29] == Colors.yellow && myCube[36] == Colors.blue)
            {
                execute("RU2R'", ref myCube);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.red && myCube[36] == Colors.yellow)
            {
                execute("U2RUR'", ref myCube);
            }
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.blue && myCube[36] == Colors.red)
            {
                execute("UR2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.blue && myCube[9] == Colors.red)
            {
                execute("U2R2FR2F'", ref myCube);
            }
            else if (myCube[6] == Colors.red && myCube[38] == Colors.yellow && myCube[9] == Colors.blue)
            {
                execute("RU'R'", ref myCube);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.red && myCube[9] == Colors.yellow)
            {
                execute("U'RUR'", ref myCube);
            }
            else
            {
                //already correct
            }

            //solve the third layer-2 edge
            if (myCube[14] == Colors.blue && myCube[21] == Colors.red)
            {
                execute("RU'R2U2RU'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.red && myCube[30] == Colors.blue)
            {
                execute("R'U2R2U'R'U'yL'UL", ref myCube);
            }
            else if (myCube[23] == Colors.blue && myCube[30] == Colors.red)
            {
                execute("R'U'R2U'R2UR2UR'y", ref myCube);
            }
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.red)
            {
                execute("URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[7] == Colors.red && myCube[10] == Colors.blue)
            {
                execute("U2R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.red && myCube[19] == Colors.blue)
            {
                execute("U'R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.red)
            {
                execute("U2RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[1] == Colors.red && myCube[28] == Colors.blue)
            {
                execute("R'FRF'RUR'y", ref myCube);
            }
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.red)
            {
                execute("U'RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.red)
            {
                execute("RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.red && myCube[37] == Colors.blue)
            {
                execute("UR'FRF'RUR'y", ref myCube);
            }
            else
            {
                //already correct
                execute("y", ref myCube);
            }

            //solve the last bottom-corner
            if (myCube[17] == Colors.orange && myCube[24] == Colors.yellow && myCube[47] == Colors.blue)
            {
                execute("RUR'U'RUR'", ref myCube);
            }
            else if (myCube[17] == Colors.yellow && myCube[24] == Colors.blue && myCube[47] == Colors.orange)
            {
                execute("RU'R2FRF'", ref myCube);
            }
            else if (myCube[8] == Colors.blue && myCube[11] == Colors.yellow && myCube[18] == Colors.orange)
            {
                execute("URU'R'", ref myCube);
            }
            else if (myCube[8] == Colors.yellow && myCube[11] == Colors.orange && myCube[18] == Colors.blue)
            {
                execute("RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[8] == Colors.orange && myCube[11] == Colors.blue && myCube[18] == Colors.yellow)
            {
                execute("RUR'", ref myCube);
            }
            else if (myCube[2] == Colors.orange && myCube[20] == Colors.blue && myCube[27] == Colors.yellow)
            {
                execute("URUR'", ref myCube);
            }
            else if (myCube[2] == Colors.blue && myCube[20] == Colors.yellow && myCube[27] == Colors.orange)
            {
                execute("U2RU'R'", ref myCube);
            }
            else if (myCube[2] == Colors.yellow && myCube[20] == Colors.orange && myCube[27] == Colors.blue)
            {
                execute("URU2R'U'RUR'", ref myCube);
            }
            else if (myCube[0] == Colors.blue && myCube[29] == Colors.yellow && myCube[36] == Colors.orange)
            {
                execute("RU2R'", ref myCube);
            }
            else if (myCube[0] == Colors.orange && myCube[29] == Colors.blue && myCube[36] == Colors.yellow)
            {
                execute("U2RUR'", ref myCube);
            }
            else if (myCube[0] == Colors.yellow && myCube[29] == Colors.orange && myCube[36] == Colors.blue)
            {
                execute("U2RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[6] == Colors.yellow && myCube[38] == Colors.orange && myCube[9] == Colors.blue)
            {
                execute("U'RU2R'U'RUR'", ref myCube);
            }
            else if (myCube[6] == Colors.blue && myCube[38] == Colors.yellow && myCube[9] == Colors.orange)
            {
                execute("RU'R'", ref myCube);
            }
            else if (myCube[6] == Colors.orange && myCube[38] == Colors.blue && myCube[9] == Colors.yellow)
            {
                execute("U'RUR'", ref myCube);
            }
            else
            {
                //already correct
            }

            //solve the last layer-2 edge
            if (myCube[14] == Colors.orange && myCube[21] == Colors.blue)
            {
                execute("RU'R'y'UR'U2RU2R'UR", ref myCube);
            }
            else if (myCube[7] == Colors.orange && myCube[10] == Colors.blue)
            {
                execute("URU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[7] == Colors.blue && myCube[10] == Colors.orange)
            {
                execute("U2R'FRF'RUR'", ref myCube);
            }
            else if (myCube[5] == Colors.blue && myCube[19] == Colors.orange)
            {
                execute("U'R'FRF'RUR'", ref myCube);
            }
            else if (myCube[5] == Colors.orange && myCube[19] == Colors.blue)
            {
                execute("U2RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[1] == Colors.blue && myCube[28] == Colors.orange)
            {
                execute("R'FRF'RUR'", ref myCube);
            }
            else if (myCube[1] == Colors.orange && myCube[28] == Colors.blue)
            {
                execute("U'RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.orange && myCube[37] == Colors.blue)
            {
                execute("RU'R'U'yL'UL", ref myCube);
            }
            else if (myCube[3] == Colors.blue && myCube[37] == Colors.orange)
            {
                execute("UR'FRF'RUR'", ref myCube);
            }
            else
            {
                //already correct
            }
        }
        private void execute(string moves, ref Colors[] myCube)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                switch (moves[i])
                {
                    case 'R':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoRPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoR2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoR(ref myCube);
                            }
                        }
                        else
                        {
                            DoR(ref myCube);
                        }
                        break;
                    case 'L':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoLPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoL2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoL(ref myCube);
                            }
                        }
                        else
                        {
                            DoL(ref myCube);
                        }
                        break;
                    case 'U':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoUPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoU2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoU(ref myCube);
                            }
                        }
                        else
                        {
                            DoU(ref myCube);
                        }
                        break;
                    case 'F':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoFPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoF2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoF(ref myCube);
                            }
                        }
                        else
                        {
                            DoF(ref myCube);
                        }
                        break;
                    case 'B':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoBPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoB2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoB(ref myCube);
                            }
                        }
                        else
                        {
                            DoB(ref myCube);
                        }
                        break;
                    case 'D':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoDPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoD2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoD(ref myCube);
                            }
                        }
                        else
                        {
                            DoD(ref myCube);
                        }
                        break;
                    case 'y':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoYPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoY2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoY(ref myCube);
                            }
                        }
                        else
                        {
                            DoY(ref myCube);
                        }
                        break;
                    case 'r':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DorPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                Dor2(ref myCube);
                                i++;
                            }
                            else
                            {
                                Dor(ref myCube);
                            }
                        }
                        else
                        {
                            Dor(ref myCube);
                        }
                        break;
                    case 'f':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DofPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                Dof2(ref myCube);
                                i++;
                            }
                            else
                            {
                                Dof(ref myCube);
                            }
                        }
                        else
                        {
                            Dof(ref myCube);
                        }
                        break;
                    case 'M':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                //DofMrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoM2(ref myCube);
                                i++;
                            }
                            else
                            {
                                //DoM(ref myCube);
                            }
                        }
                        else
                        {
                            //DoM(ref myCube);
                        }
                        break;
                    case 'x':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoXPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoX2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoX(ref myCube);
                            }
                        }
                        else
                        {
                            DoX(ref myCube);
                        }
                        break;
                    case 'z':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                DoZPrime(ref myCube);
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                DoZ2(ref myCube);
                                i++;
                            }
                            else
                            {
                                DoZ(ref myCube);
                            }
                        }
                        else
                        {
                            DoZ(ref myCube);
                        }
                        break;
                }
            }
        }
        private void pictureTranslator(string moves, ref List<string> subList)
        {
            for (int i = 0; i < moves.Length; i++)
            {
                switch (moves[i])
                {
                    case 'R':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("R'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("R2");
                                i++;
                            }
                            else
                            {
                                subList.Add("R");
                            }
                        }
                        else
                        {
                            subList.Add("R");
                        }
                        break;
                    case 'L':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("L'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("L2");
                                i++;
                            }
                            else
                            {
                                subList.Add("L");
                            }
                        }
                        else
                        {
                            subList.Add("L");
                        }
                        break;
                    case 'U':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("U'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("U2");
                                i++;
                            }
                            else
                            {
                                subList.Add("U");
                            }
                        }
                        else
                        {
                            subList.Add("U");
                        }
                        break;
                    case 'F':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("F'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("F2");
                                i++;
                            }
                            else
                            {
                                subList.Add("F");
                            }
                        }
                        else
                        {
                            subList.Add("F");
                        }
                        break;
                    case 'B':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("B'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("B2");
                                i++;
                            }
                            else
                            {
                                subList.Add("B");
                            }
                        }
                        else
                        {
                            subList.Add("B");
                        }
                        break;
                    case 'D':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("D'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("D2");
                                i++;
                            }
                            else
                            {
                                subList.Add("D");
                            }
                        }
                        else
                        {
                            subList.Add("D");
                        }
                        break;
                    case 'y':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("y'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("y2");
                                i++;
                            }
                            else
                            {
                                subList.Add("y");
                            }
                        }
                        else
                        {
                            subList.Add("y");
                        }
                        break;
                    case 'r':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("r'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("r2");
                                i++;
                            }
                            else
                            {
                                subList.Add("r");
                            }
                        }
                        else
                        {
                            subList.Add("r");
                        }
                        break;
                    case 'f':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("f'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("f2");
                                i++;
                            }
                            else
                            {
                                subList.Add("f");
                            }
                        }
                        else
                        {
                            subList.Add("f");
                        }
                        break;
                    case 'M':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("M'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("M2");
                                i++;
                            }
                            else
                            {
                                subList.Add("M");
                            }
                        }
                        else
                        {
                            subList.Add("M");
                        }
                        break;
                    case 'x':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("x'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("x2");
                                i++;
                            }
                            else
                            {
                                subList.Add("x");
                            }
                        }
                        else
                        {
                            subList.Add("x");
                        }
                        break;
                    case 'z':
                        if (i != moves.Length - 1)
                        {
                            if (moves[i + 1] == '\'')
                            {
                                subList.Add("z'");
                                i++;
                            }
                            else if (moves[i + 1] == '2')
                            {
                                subList.Add("z2");
                                i++;
                            }
                            else
                            {
                                subList.Add("z");
                            }
                        }
                        else
                        {
                            subList.Add("z");
                        }
                        break;
                }
            }
        }
        private void TopCross(ref Colors[] myCube)
        {
            if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white)
            {
                //already correct
            }
            else if (myCube[1] != Colors.white && myCube[3] != Colors.white && myCube[5] != Colors.white && myCube[7] != Colors.white)
            {
                execute("FRUR'U'F'fRUR'U'f'", ref myCube);
            }
            else if (myCube[7] == Colors.white && myCube[5] == Colors.white && myCube[1] != Colors.white && myCube[3] != Colors.white)
            {
                execute("fRUR'U'f'", ref myCube);
            }
            else if (myCube[7] == Colors.white && myCube[3] == Colors.white && myCube[1] != Colors.white && myCube[5] != Colors.white)
            {
                execute("U'fRUR'U'f'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[5] != Colors.white && myCube[7] != Colors.white)
            {
                execute("U2fRUR'U'f'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[5] == Colors.white && myCube[7] != Colors.white && myCube[3] != Colors.white)
            {
                execute("UfRUR'U'f'", ref myCube);
            }
            else if (myCube[3] == Colors.white && myCube[5] == Colors.white && myCube[1] != Colors.white && myCube[7] != Colors.white)
            {
                execute("FRUR'U'F'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[7] == Colors.white && myCube[3] != Colors.white && myCube[5] != Colors.white)
            {
                execute("UFRUR'U'F'", ref myCube);
            }

        }
        private void Oll(ref Colors[] myCube)
        {
            //Oll 1
            if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[11] == Colors.white && myCube[20] == Colors.white && myCube[29] == Colors.white)
            {
                execute("RUR'URU2R'", ref myCube);
            }
            else if (myCube[0] == Colors.white && myCube[0] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[38] == Colors.white && myCube[11] == Colors.white && myCube[20] == Colors.white)
            {
                execute("U'RUR'URU2R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[11] == Colors.white && myCube[38] == Colors.white && myCube[29] == Colors.white)
            {
                execute("U2RUR'URU2R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[8] == Colors.white && myCube[20] == Colors.white && myCube[38] == Colors.white && myCube[29] == Colors.white)
            {
                execute("URUR'URU2R'", ref myCube);
            }
            //Oll 2
            else if (myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[18] == Colors.white && myCube[9] == Colors.white && myCube[36] == Colors.white)
            {
                execute("RU2R'U'RU'R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[8] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[9] == Colors.white && myCube[27] == Colors.white)
            {
                execute("U'RU2R'U'RU'R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[6] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[18] == Colors.white && myCube[27] == Colors.white)
            {
                execute("U2RU2R'U'RU'R'", ref myCube);
            }
            else if (myCube[0] == Colors.white && myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[9] == Colors.white && myCube[18] == Colors.white && myCube[27] == Colors.white)
            {
                execute("URU2R'U'RU'R'", ref myCube);
            }
            //Oll 3
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[38] == Colors.white && myCube[18] == Colors.white && myCube[20] == Colors.white)
            {
                execute("R'U'RU'R'URU'R'U2R", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[9] == Colors.white && myCube[11] == Colors.white && myCube[27] == Colors.white && myCube[29] == Colors.white)
            {
                execute("RU2R'U'RUR'U'RU'R'", ref myCube);
            }
            //Oll 4
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[38] == Colors.white && myCube[27] == Colors.white && myCube[11] == Colors.white)
            {
                execute("RU2R2U'R2U'R2U2R", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[18] == Colors.white && myCube[38] == Colors.white && myCube[27] == Colors.white && myCube[29] == Colors.white)
            {
                execute("U'RU2R2U'R2U'R2U2R", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[9] == Colors.white && myCube[29] == Colors.white && myCube[18] == Colors.white && myCube[20] == Colors.white)
            {
                execute("U2RU2R2U'R2U'R2U2R", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[20] == Colors.white && myCube[9] == Colors.white && myCube[11] == Colors.white)
            {
                execute("URU2R2U'R2U'R2U2R", ref myCube);
            }
            //Oll 5
            else if (myCube[9] == Colors.white && myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[8] == Colors.white && myCube[29] == Colors.white)
            {
                execute("rUR'U'r'FRF'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[8] == Colors.white && myCube[20] == Colors.white && myCube[36] == Colors.white)
            {
                execute("U'rUR'U'r'FRF'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[0] == Colors.white && myCube[11] == Colors.white && myCube[27] == Colors.white)
            {
                execute("U2rUR'U'r'FRF'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[0] == Colors.white && myCube[7] == Colors.white && myCube[2] == Colors.white && myCube[18] == Colors.white && myCube[38] == Colors.white)
            {
                execute("UrUR'U'r'FRF'", ref myCube);
            }
            //Oll 6
            else if (myCube[0] == Colors.white && myCube[1] == Colors.white && myCube[27] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[8] == Colors.white && myCube[38] == Colors.white)
            {
                execute("FRU'R'U'RU2R'U'F'", ref myCube);
            }
            else if (myCube[6] == Colors.white && myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[18] == Colors.white && myCube[29] == Colors.white)
            {
                execute("U'FRU'R'U'RU2R'U'F'", ref myCube);
            }
            else if (myCube[0] == Colors.white && myCube[1] == Colors.white && myCube[8] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[9] == Colors.white && myCube[20] == Colors.white)
            {
                execute("U2FRU'R'U'RU2R'U'F'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[11] == Colors.white && myCube[36] == Colors.white)
            {
                execute("UFRU'R'U'RU2R'U'F'", ref myCube);
            }
            //Oll 7
            else if (myCube[0] == Colors.white && myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[9] == Colors.white && myCube[11] == Colors.white)
            {
                execute("R2DR'U2RD'R'U2R'", ref myCube);
            }
            else if (myCube[8] == Colors.white && myCube[1] == Colors.white && myCube[2] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[7] == Colors.white && myCube[36] == Colors.white && myCube[38] == Colors.white)
            {
                execute("U'R2DR'U2RD'R'U2R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[8] == Colors.white && myCube[27] == Colors.white && myCube[29] == Colors.white)
            {
                execute("U2R2DR'U2RD'R'U2R'", ref myCube);
            }
            else if (myCube[1] == Colors.white && myCube[3] == Colors.white && myCube[4] == Colors.white && myCube[5] == Colors.white && myCube[6] == Colors.white && myCube[7] == Colors.white && myCube[0] == Colors.white && myCube[18] == Colors.white && myCube[20] == Colors.white)
            {
                execute("UR2DR'U2RD'R'U2R'", ref myCube);
            }
        }
        private void Last4Corners(ref Colors[] myCube)
        {
            if (myCube[9] == myCube[11] && myCube[36] == myCube[38] && myCube[27] == myCube[29] && myCube[18] == myCube[20])
            {
                //already correct
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
                Last4CornersAgain(ref myCube);
            }
            else if (myCube[9] == myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("URUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] == myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] == myCube[29] && myCube[18] != myCube[20])
            {
                execute("U'RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] == myCube[20])
            {
                execute("U2RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            if (myCube[13] == Colors.red)
            {
                if (myCube[9] == Colors.red)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U", ref myCube);
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U'", ref myCube);
                }
            }
            else if (myCube[13] == Colors.green)
            {
                if (myCube[9] == Colors.green)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U", ref myCube);
                }
            }
            else if (myCube[13] == Colors.orange)
            {
                if (myCube[9] == Colors.orange)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U", ref myCube);
                }
            }
            else if (myCube[13] == Colors.blue)
            {
                if (myCube[9] == Colors.blue)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U", ref myCube);
                }
            }
        }
        private void Last4CornersAgain(ref Colors[] myCube)
        {
            if (myCube[9] == myCube[11] && myCube[36] == myCube[38] && myCube[27] == myCube[29] && myCube[18] == myCube[20])
            {
                //already correct
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] == myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("URUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] == myCube[38] && myCube[27] != myCube[29] && myCube[18] != myCube[20])
            {
                execute("RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] == myCube[29] && myCube[18] != myCube[20])
            {
                execute("U'RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            else if (myCube[9] != myCube[11] && myCube[36] != myCube[38] && myCube[27] != myCube[29] && myCube[18] == myCube[20])
            {
                execute("U2RUR'U'R'FR2U'R'U'RUR'F'", ref myCube);
            }
            if (myCube[13] == Colors.red)
            {
                if (myCube[9] == Colors.red)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U", ref myCube);
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U'", ref myCube);
                }
            }
            else if (myCube[13] == Colors.green)
            {
                if (myCube[9] == Colors.green)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U", ref myCube);
                }
            }
            else if (myCube[13] == Colors.orange)
            {
                if (myCube[9] == Colors.orange)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.blue)
                {
                    execute("U", ref myCube);
                }
            }
            else if (myCube[13] == Colors.blue)
            {
                if (myCube[9] == Colors.blue)
                {
                    //already correct
                }
                else if (myCube[9] == Colors.orange)
                {
                    execute("U'", ref myCube);
                }
                else if (myCube[9] == Colors.green)
                {
                    execute("U2", ref myCube);
                }
                else if (myCube[9] == Colors.red)
                {
                    execute("U", ref myCube);
                }
            }
        }
        private void GreenAlwaysInFront(ref Colors[] myCube)
        {
            if (myCube[13] == Colors.green)
            {
                //already correct
            }
            else if (myCube[13] == Colors.red)
            {
                execute("y'", ref myCube);
            }
            else if (myCube[13] == Colors.blue)
            {
                execute("y2", ref myCube);
            }
            else if (myCube[13] == Colors.orange)
            {
                execute("y", ref myCube);
            }
        }
        private void Last4Edges(ref Colors[] myCube)
        {
            if (myCube[10] == Colors.green && myCube[37] == Colors.red && myCube[28] == Colors.orange && myCube[19] == Colors.blue)
            {
                execute("y2RU'RURURU'R'U'R2", ref myCube);
            }
            else if (myCube[10] == Colors.green && myCube[37] == Colors.blue && myCube[28] == Colors.red && myCube[19] == Colors.orange)
            {
                execute("y2R2URUR'U'R'U'R'UR'", ref myCube);
            }
            else if (myCube[10] == Colors.blue && myCube[37] == Colors.green && myCube[28] == Colors.orange && myCube[19] == Colors.red)
            {
                execute("y'RU'RURURU'R'U'R2", ref myCube);
            }
            else if (myCube[10] == Colors.orange && myCube[37] == Colors.blue && myCube[28] == Colors.green && myCube[19] == Colors.red)
            {
                execute("y'R2URUR'U'R'U'R'UR'", ref myCube);
            }
            else if (myCube[10] == Colors.red && myCube[37] == Colors.orange && myCube[28] == Colors.green && myCube[19] == Colors.blue)
            {
                execute("yRU'RURURU'R'U'R2", ref myCube);
            }
            else if (myCube[10] == Colors.blue && myCube[37] == Colors.orange && myCube[28] == Colors.red && myCube[19] == Colors.green)
            {
                execute("yR2URUR'U'R'U'R'UR'", ref myCube);
            }
            else if (myCube[10] == Colors.red && myCube[37] == Colors.green && myCube[28] == Colors.blue && myCube[19] == Colors.orange)
            {
                execute("RU'RURURU'R'U'R2", ref myCube);
            }
            else if (myCube[10] == Colors.orange && myCube[37] == Colors.red && myCube[28] == Colors.blue && myCube[19] == Colors.green)
            {
                execute("R2URUR'U'R'U'R'UR'", ref myCube);
            }
            else if (myCube[10] == Colors.blue && myCube[37] == Colors.red && myCube[28] == Colors.green && myCube[19] == Colors.orange)
            {
                execute("M2UM2U2M2UM2", ref myCube);
            }
            else if (myCube[10] == Colors.orange && myCube[37] == Colors.green && myCube[28] == Colors.red && myCube[19] == Colors.blue)
            {
                execute("R'U'RU'RURU'R'URUR2U'R'U2", ref myCube);
            }
            else if (myCube[10] == Colors.red && myCube[37] == Colors.blue && myCube[28] == Colors.orange && myCube[19] == Colors.green)
            {
                execute("UR'U'RU'RURU'R'URUR2U'R'U", ref myCube);
            }
            //done.
        }
        private void getCurrentScramble()
        {
            for (int i = 0; i < 54; i++)
            {
                Button btn = this.Controls.Find("button" + i, true).FirstOrDefault() as Button;
                if (btn.BackColor == Color.Red)
                {
                    myCube[i] = Colors.red;
                }
                else if (btn.BackColor == Color.Green)
                {
                    myCube[i] = Colors.green;
                }
                else if (btn.BackColor == Color.Blue)
                {
                    myCube[i] = Colors.blue;
                }
                else if (btn.BackColor == Color.Yellow)
                {
                    myCube[i] = Colors.yellow;
                }
                else if (btn.BackColor == Color.Orange)
                {
                    myCube[i] = Colors.orange;
                }
                else if (btn.BackColor == Color.White)
                {
                    myCube[i] = Colors.white;
                }
            }
        }
        private void ClearColorOfButton()
        {
            for (int i = 0; i < 54; i++)
            {
                Button Btn = this.Controls.Find("button" + i, true).FirstOrDefault() as Button;
                Btn.BackColor = Color.White;
            }
        }
        private void buttonReset_Click(object sender, EventArgs e)
        {
            CleanPanel();
            InitializeColor();
            ClearColorOfButton();
            label1.Text = "";
        }
        private void CleanPanel()
        {
            panel3.Controls.Clear();
        }
        private void initializeCube()
        {
            for (int i = 0; i < 54; i++)
            {
                if (i <= 8) myCube[i] = Colors.white;
                else if (i <= 17) myCube[i] = Colors.green;
                else if (i <= 26) myCube[i] = Colors.red;
                else if (i <= 35) myCube[i] = Colors.blue;
                else if (i <= 44) myCube[i] = Colors.orange;
                else myCube[i] = Colors.yellow;
            }
        }
        private void scramble(string scramble)
        {
            execute(scramble, ref myCube);
        }
        private void calculateAverageMoves()
        {
            List<string> result = new List<string>();
            string fileName = "C:/Users/pc/Desktop/1MillionScrambles.txt";
            StringBuilder report = new StringBuilder();
            report.Append("Detail: ");
            int totalMoves = 0;
            string fileOutput = "C:/Users/pc/Desktop/1MillionScramblesOutput.txt";
            try
            {
                if (File.Exists(fileOutput))
                {
                    File.Delete(fileOutput);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("error file " + Ex.ToString());
            }
            try
            {
                int countCorrect = 0;
                int countFail = 0;
                int min = 150;
                int max = 0;
                StringBuilder minScript = new StringBuilder();
                StringBuilder minSolution = new StringBuilder();
                StringBuilder maxScript = new StringBuilder();
                StringBuilder maxSolution = new StringBuilder();
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    using (StreamWriter sw = File.CreateText(fileOutput))
                    {

                        while ((s = sr.ReadLine()) != null)
                        {
                            initializeCube();
                            scramble(s);
                            CenterOrient();
                            BindingStateForKociembaCube();
                            Solve(ref myCube);
                            if (checkFinished() == true)
                            {
                                countCorrect++;
                                StringBuilder process = new StringBuilder();
                                c.corners = corners;
                                c.edges = edges;
                                result.Clear();
                                result = Search.patternSolve(c, TwoPhaseSolver.Move.None, 30, printInfo: true);
                                report.Append(Int32.Parse(result[2]) + "-");
                                totalMoves += Int32.Parse(result[2]);
                                if (Int32.Parse(result[2]) < min)
                                {
                                    min = Int32.Parse(result[2]);
                                    minScript.Clear();
                                    minScript.Append(s);
                                    minSolution.Clear();
                                    minSolution.Append(result[0] + " " + result[1]);
                                }
                                if (Int32.Parse(result[2]) > max)
                                {
                                    max = Int32.Parse(result[2]);
                                    maxScript.Clear();
                                    maxScript.Append(s);
                                    maxSolution.Clear();
                                    maxSolution.Append(result[0] + " " + result[1]);
                                }
                            }
                            else
                            {
                                countFail++;
                            }
                        }
                        int average = totalMoves / (countCorrect);
                        sw.WriteLine("Total test cases: " + (countFail + countCorrect).ToString("#,#"));
                        sw.WriteLine("Success: " + countCorrect + "/" + (countFail + countCorrect) + "\nFail: " + countFail + "/" + (countFail + countCorrect));
                        sw.WriteLine("Average number of moves: " + average);
                        sw.WriteLine("Fewest moves case: " + min + " (" + minScript + ")");
                        sw.WriteLine("Solution for fewest moves case: " + minSolution);
                        sw.WriteLine("Most moves: " + max + " (" + maxScript + ")");
                        sw.WriteLine("Solution for most moves case: " + maxSolution);
                        sw.WriteLine(report);
                    }
                }
                MessageBox.Show("Test done.");
                MessageBox.Show("Success cases: " + countCorrect + "/" + (countFail + countCorrect) + "\nFail cases: " + countFail + "/" + (countFail + countCorrect));
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }
    }
}
