using Mousemove;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Board
{
    public partial class Form1 : Form
    {

        private bool isdrawing = false;
        private float witdh;
        private Color color = Color.Black;
        private Point prev = new Point();
        private Curve cv;
        private Drawing draw = new Drawing();
        private DateTime starttime;

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();
            witdh = 10;
        }


        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            

        }

        private void RePaint()
        {
            using (Graphics gr = this.CreateGraphics())
            {
                for (int i = 0; i < draw.Curves.Count; i++)
                {
                    Curve tempcv = draw.Curves[i];
                    Pen temp = new Pen(tempcv.Color, tempcv.Width);

                    for (int j = 0; j < tempcv.Coordinates.Count; j++)
                    {
                        gr.DrawLine(temp, tempcv.Coordinates[j], tempcv.Coordinates[j + 1]);
                    }
                    temp.Dispose();
                }
            }
        }

        private bool serialize(string name)
        {
            FileStream st = new FileStream(name, FileMode.OpenOrCreate, FileAccess.Write);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(st, Convert(draw));
            }
            catch (SerializationException)
            {
                return false;
            }
            finally
            {
                st.Close();
            }
            return true;
        }

        private Drawing Convert(Drawing input)
        {
            Drawing output = new Drawing();
            for (int i = 0; i < input.Curves.Count; i++)
            {
                Curve temp = input.Curves[i];
                output.Curves.Add(new Curve(temp.Duration, temp.Pause, temp.Color, temp.Width));
                for (int j = 0; j < temp.Coordinates.Count; j++)
                {
                    output.Curves[i].Coordinates.Add(new Point());
                    output.Curves[i].Coordinates[j] = PointToScreen(temp.Coordinates[j]);
                }
            }

            if (output.Curves.Count > 0)
            {
                output.Curves[0].Pause = TimeSpan.Zero;
            }
            return output;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            prev = e.Location;
            isdrawing = true;
            cv = new Curve();
            cv.Color = color;
            cv.Width = witdh;
            cv.Pause = DateTime.Now - starttime;
            starttime = DateTime.Now;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Pen p = new Pen(color, witdh);
            if (isdrawing)
            {
                using (Graphics gr = this.CreateGraphics())
                {
                    gr.DrawLine(p, prev, e.Location);
                    cv.Coordinates.Add(prev);
                    prev = e.Location;
                }
            }
            p.Dispose();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isdrawing = false;
            cv.Coordinates.Add(prev);
            cv.Duration = DateTime.Now - starttime;
            starttime = DateTime.Now;
            draw.Curves.Add(cv);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            RePaint();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
        }

        private void cleanButton_Click(object sender, EventArgs e)
        {
            draw = new Drawing();
            prev = new Point();
            this.Invalidate();
        }

        private void PenButton_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Black;
        }

        private void EraserButton_Click(object sender, EventArgs e)
        {
            witdh = 50;
            color = Color.White;
        }

        private void BlackColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Black;
        }

        private void RedColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Red;
        }

        private void YellowColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Yellow;
        }

        private void OrangeColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Orange;
        }

        private void LightGreenColor_Click(object sender, EventArgs e)
        {
            color = Color.LightGreen;
        }

        private void SkyBlueColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.SkyBlue;
        }

        private void BlueColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Blue;
        }

        private void PinkColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Pink;
        }

        private void BrownColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Brown;
        }

        private void VioletColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Violet;
        }

        private void GreenColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Green;
        }

        private void SilverColor_Click(object sender, EventArgs e)
        {
            witdh = 10;
            color = Color.Silver;
        }
    }
}
