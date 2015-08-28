using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Circles
{
    public partial class Form1 : Form
    {                      
        Pack pack;
        Graphics g;
        int nbFrames = 0;
        int nbHarry = 0;
        int nbMaxframes = 360;
        bool bGenerateFile = false;


        public List<string> LstFrame = new List<string>();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitDurationAndRecord();

            g = this.CreateGraphics();
            this.Text = "Circles v " + Application.ProductVersion;
        }

        //AM_2012_08_06
        private void InitDurationAndRecord()
        {
            nbMaxframes = 25 * (int)numericUpDown_Duration.Value;
            label_Chrono.Text = "--";
            bGenerateFile = true;

        }        

        private void button4_Click(object sender, EventArgs e)
        {
            nbHarry = 6;
            nbFrames = 0;
            InitDurationAndRecord();
            LstFrame.Clear();
            pack = Pack.BuildPack(6, Convert.ToInt32(nudBig.Value), 305, Convert.ToInt32(numericUpDown2.Value));

            g.Clear(SystemColors.Control);
            pack.Draw(g);
            timer1.Enabled = true;
        }

		int cpt = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {                   
            if (pack != null)
            {
               
                g.Clear(SystemColors.Control);
                pack.Draw(g);
                pack.Animate();                
                //on transmet les infos aux autres applications

                string sfrmXml = pack.GetFormattedPack();
                if (nbFrames > 0) { LstFrame.Add(sfrmXml); }
                   
                //AM pack.ProcessXY(100, 100);
                //AM pack.CheckCollison();
                //AM if (checkBox1.Checked) pack.PreventCollision();
               
                label_Chrono.Text = ((nbMaxframes-nbFrames)/25).ToString();
                nbFrames++;
                if (nbFrames > nbMaxframes)
                {
                    timer1.Enabled = false;
                    if (bGenerateFile == false)
                    {
                        return;
                    }

                    string fileName = "\\fichiers\\TabXml\\Tab_Harry" + nbHarry + "v" + Convert.ToString(nudManche.Value) + "Big" + Convert.ToString(nudBig.Value) + ".xml";
                    if (MessageBox.Show("save " + fileName + " ?", "record ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        using (StreamWriter writer = new StreamWriter(Application.StartupPath + fileName))
                        {
                            writer.WriteLine("<root>");
                            writer.WriteLine("<Keyframes>");

                            int i = 0;
                            foreach (string s in LstFrame)
                            {
                                char c = Convert.ToChar(0x22); //la double cote
                                writer.WriteLine("<Keyframe pos=" + c + i + c + ">");
                                writer.WriteLine(s);
                                writer.WriteLine("</Keyframe>");
                                i++;
                            }
                            writer.WriteLine("</Keyframes>");
                            writer.WriteLine("</root>");
                        }

                    }
                }
            }
        }    

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }    

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    pack = Pack.BuildPack(5, 2, 305, Convert.ToInt32(numericUpDown1.Value));
        //    pack.ProcessXY(400, 400);
        //    pack.InitialiseBigOneCoord();

        //    Bitmap bmp = new Bitmap(1000, 1000);
        //    Graphics gBmp = Graphics.FromImage(bmp);

        //    gBmp.Clear(Color.Transparent);
        //    pack.Draw(gBmp);
        //    bmp.Save("last__begin.png");

        //    for (int i = 0; i < 10; i++)
        //    {
        //        gBmp.Clear(Color.Transparent);
        //        pack.ProcessInAnim(i, 10, 400, 400);
        //        pack.Draw(gBmp);
        //        bmp.Save("last_" + (i+1).ToString("00") + ".png");
        //    }

        //    gBmp.Clear(Color.Transparent);
        //    pack.Draw(gBmp);
        //    bmp.Save("last_end.png");
        //    gBmp.Dispose();
        //    bmp.Dispose();
			
        //}

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            label_Chrono.Text = "--";
            bGenerateFile = false;
            timer1.Enabled = false;
        }

       
    }
}
