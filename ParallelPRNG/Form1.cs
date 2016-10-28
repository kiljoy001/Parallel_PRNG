﻿using ParallelRandomClassLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParallelPRNG
{
    public partial class Form1 : Form
    {
        PRNG prng;
        PPRNG pprng;
        RichTextBox[] txtOutputArray = new RichTextBox[4];

        Bitmap bmap;
        Graphics g;
        
        public Form1()
        {
            InitializeComponent();

            bmap = new Bitmap(canvasTab3.Width, canvasTab3.Height);
            g = Graphics.FromImage(bmap);

            float dx = bmap.Width * 0F;
            float dy = bmap.Height * 1F;

            Matrix matrix = new Matrix(1, 0, 0, -1, dx, dy);
            g.Transform = matrix;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            prng = new PRNG("Huy's Parallel PRNG Class");
            pprng = new PPRNG();

            txtOutputArray[0] = (RichTextBox)txtOutput0;
            txtOutputArray[1] = (RichTextBox)txtOutput1;
            txtOutputArray[2] = (RichTextBox)txtOutput2;
            txtOutputArray[3] = (RichTextBox)txtOutput3;
        }

        #region TAB1 INDEX-TRIGGERS

        private void txtConsole_TextChanged(object sender, EventArgs e)
        {
            txtConsole.SelectionStart = txtConsole.Text.Length;
            txtConsole.ScrollToCaret();
        }

        private void txtConsole_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            txtConsole.Clear();
        }

        #endregion

        #region TAB1 BUTTONS

        private void btnSeed_Click(object sender, EventArgs e)
        {
            BigInteger seed = new BigInteger(prng.HashedSeedByte);
            txtConsole.Text += "Original Seed: " + seed + "\n";
        }

        private void btnCurrentEntropy_Click(object sender, EventArgs e)
        {
            BigInteger currentEntropy = new BigInteger(prng.CurrentEntropyHash);
            txtConsole.Text += "Current Seed: " + currentEntropy + "\n";
        }

        private void btnNextUInteger_Click(object sender, EventArgs e)
        {
            BigInteger randomNext = prng.NextUInteger(new BigInteger(numUpDownMaxU.Value));
            txtConsole.Text += randomNext + ", ";
        }

        private void btnNextRanged_Click(object sender, EventArgs e)
        {
            BigInteger min = new BigInteger(numUpDownMin.Value);
            BigInteger max = new BigInteger(numUpDownMax.Value);
            int iterations = Int32.Parse(numUpDownIterations.Value.ToString());
            List<BigInteger> randomValuesList = new List<BigInteger>(prng.GenerateListOfEntropyValuesBigInteger(min, max, iterations));

            string[] stringArray = randomValuesList.Select(i => i.ToString()).ToArray();

            string blockOfValues;
            blockOfValues = String.Join(", ", stringArray.Select(i => i.ToString()));

            txtConsole.Text += blockOfValues + " ";
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            numUpDownMax.Value = numUpDownMax.Maximum;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            numUpDownMin.Value = numUpDownMin.Minimum;
        }

        private void btnMaxIterations_Click(object sender, EventArgs e)
        {
            numUpDownIterations.Value = numUpDownIterations.Maximum;
        }

        private void btnUMax_Click(object sender, EventArgs e)
        {
            numUpDownMaxU.Value = numUpDownMaxU.Maximum;
        }

        #endregion

        #region TAB2 INDEX-TRIGGERS

        private void txtOutput0_TextChanged(object sender, EventArgs e)
        {
            txtOutput0.SelectionStart = txtOutput0.Text.Length;
            txtOutput0.ScrollToCaret();
        }

        private void txtOutput1_TextChanged(object sender, EventArgs e)
        {
            txtOutput1.SelectionStart = txtOutput1.Text.Length;
            txtOutput1.ScrollToCaret();
        }

        private void txtOutput2_TextChanged(object sender, EventArgs e)
        {
            txtOutput2.SelectionStart = txtOutput2.Text.Length;
            txtOutput2.ScrollToCaret();
        }

        private void txtOutput3_TextChanged(object sender, EventArgs e)
        {
            txtOutput3.SelectionStart = txtOutput3.Text.Length;
            txtOutput3.ScrollToCaret();
        }

        #endregion

        #region TAB2 BUTTONS

        private void btnBenchMax_Click(object sender, EventArgs e)
        {
            numUpDownBenchMax.Value = numUpDownBenchMax.Maximum;
        }

        private void btnBenchMin_Click(object sender, EventArgs e)
        {
            numUpDownBenchMin.Value = numUpDownBenchMin.Minimum;
        }

        private void btnBenchIterations_Click(object sender, EventArgs e)
        {
            numUpDownBenchIterations.Value = numUpDownBenchIterations.Maximum;
        }

        private void btnTimeSingleThread_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            int iterations = (int)numUpDownBenchIterations.Value;
            BigInteger min = (BigInteger)numUpDownBenchMin.Value;
            BigInteger max = (BigInteger)numUpDownBenchMax.Value;

            stopwatch.Start();
            List<BigInteger> bigIntegerList = new List<BigInteger>(prng.GenerateListOfEntropyValuesBigInteger(min, max, iterations));
            stopwatch.Stop();

            stopwatch2.Start();
            List<byte[]> byteArraylist = new List<byte[]>(prng.GenerateListOfEntropy32ByteArrays(iterations));
            stopwatch2.Stop();

            txtOutput0.Text += "Single Threaded PRNG Finished" + "\n";
            txtOutput0.Text += "Maximum Threads: " + ThreadUsage(DesiredCPUUtilization.SingleThread) + "\n";
            txtOutput0.Text += "Iterations: " + iterations + "\n";
            txtOutput0.Text += "Min Value: " + min + "\n";
            txtOutput0.Text += "Max Value: " + max + "\n";
            txtOutput0.Text += "Byte Array: " + stopwatch.Elapsed + "\n";
            txtOutput0.Text += "BigInteger: " + stopwatch2.Elapsed + "\n\n";
        }

        private void btnTimeHalfAvailThread_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            int iterations = (int)numUpDownBenchIterations.Value;
            BigInteger min = (BigInteger)numUpDownBenchMin.Value;
            BigInteger max = (BigInteger)numUpDownBenchMax.Value;

            stopwatch.Start();
            pprng.GenerateDesiredQuantityOfRandom32ByteArrays(DesiredCPUUtilization.HalfAvailThreads, iterations);
            stopwatch.Stop();

            stopwatch2.Start();
            pprng.GenerateDesiredQuantityOfRandomIntegers(DesiredCPUUtilization.HalfAvailThreads, iterations, min, max);
            stopwatch2.Stop();

            txtOutput1.Text += "Half-Available Threaded PRNG Finished" + "\n";
            txtOutput1.Text += "Maximum Threads: " + ThreadUsage(DesiredCPUUtilization.HalfAvailThreads) + "\n";
            txtOutput1.Text += "Iterations: " + iterations + "\n";
            txtOutput1.Text += "Min Value: " + min + "\n";
            txtOutput1.Text += "Max Value: " + max + "\n";
            txtOutput1.Text += "Byte Array: " + stopwatch.Elapsed + "\n";
            txtOutput1.Text += "BigInteger: " + stopwatch2.Elapsed + "\n\n";
        }

        private void btnTimeHalfPlusOneAvailThread_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            int iterations = (int)numUpDownBenchIterations.Value;
            BigInteger min = (BigInteger)numUpDownBenchMin.Value;
            BigInteger max = (BigInteger)numUpDownBenchMax.Value;

            stopwatch.Start();
            pprng.GenerateDesiredQuantityOfRandom32ByteArrays(DesiredCPUUtilization.HalfAvailPlusOneThread, iterations);
            stopwatch.Stop();

            stopwatch2.Start();
            pprng.GenerateDesiredQuantityOfRandomIntegers(DesiredCPUUtilization.HalfAvailPlusOneThread, iterations, min, max);
            stopwatch2.Stop();

            txtOutput2.Text += "Half-Available+1 Threaded PRNG Finished" + "\n";
            txtOutput2.Text += "Maximum Threads: " + ThreadUsage(DesiredCPUUtilization.HalfAvailPlusOneThread) + "\n";
            txtOutput2.Text += "Iterations: " + iterations + "\n";
            txtOutput2.Text += "Min Value: " + min + "\n";
            txtOutput2.Text += "Max Value: " + max + "\n";
            txtOutput2.Text += "Byte Array: " + stopwatch.Elapsed + "\n";
            txtOutput2.Text += "BigInteger: " + stopwatch2.Elapsed + "\n\n";
        }

        private void btnTimeFullThread_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            Stopwatch stopwatch2 = new Stopwatch();

            int iterations = (int)numUpDownBenchIterations.Value;
            BigInteger min = (BigInteger)numUpDownBenchMin.Value;
            BigInteger max = (BigInteger)numUpDownBenchMax.Value;

            stopwatch.Start();
            pprng.GenerateDesiredQuantityOfRandom32ByteArrays(DesiredCPUUtilization.AllThreads, iterations);
            stopwatch.Stop();

            stopwatch2.Start();
            pprng.GenerateDesiredQuantityOfRandomIntegers(DesiredCPUUtilization.AllThreads, iterations, min, max);
            stopwatch2.Stop();

            txtOutput3.Text += "All-Threaded PRNG Finished" + "\n";
            txtOutput3.Text += "Maximum Threads: " + ThreadUsage(DesiredCPUUtilization.AllThreads) + "\n";
            txtOutput3.Text += "Iterations: " + iterations + "\n";
            txtOutput3.Text += "Min Value: " + min + "\n";
            txtOutput3.Text += "Max Value: " + max + "\n";
            txtOutput3.Text += "Byte Array: " + stopwatch.Elapsed + "\n";
            txtOutput3.Text += "BigInteger: " + stopwatch2.Elapsed + "\n\n";
        }

        #endregion

        #region TAB3 BUTTONS

        private void btnGenerateRGBNoise_Click(object sender, EventArgs e)
        {
            int integersNeeded = canvasTab3.Height * canvasTab3.Width * 3;
            pprng.GenerateDesiredQuantityOfRandomIntegers(DesiredCPUUtilization.AllThreads, integersNeeded, 0, 256);
            ConcurrentBag<BigInteger> bagOfIntegers = new ConcurrentBag<BigInteger>(pprng.GetBagOfRandomIntegers);

            ConcurrentBag<Bitmap> bagOfBitmaps = new ConcurrentBag<Bitmap>();
            int canvasHeight = canvasTab3.Height;

            Parallel.For(0, bmap.Width, i => {
                
                Graphics x;
                Bitmap bmapx;
                bmapx = new Bitmap(1, canvasHeight);
                x = Graphics.FromImage(bmapx);

                for (int j = 0; j < canvasHeight; j++)
                {
                    BigInteger red, green, blue;

                    bool success1 = bagOfIntegers.TryTake(out red);
                    bool success2 = bagOfIntegers.TryTake(out green);
                    bool success3 = bagOfIntegers.TryTake(out blue);

                    Color randomColor = Color.FromArgb((int)red, (int)green, (int)blue);

                    SolidBrush randomSolidBrush = new SolidBrush(randomColor);
                    x.FillRectangle(randomSolidBrush, 0, j, 1, 1);
                }

                bagOfBitmaps.Add(bmapx);
                x.Dispose();
            });

            for (int i = 0; i < bmap.Width; i++)
            {
                Bitmap slice;
                bool success = bagOfBitmaps.TryTake(out slice);

                g.DrawImage(slice, i, 0);
                canvasTab3.Image = bmap;
            }
        }

        private void btnGenerateBWNoise_Click(object sender, EventArgs e)
        {
            int integersNeeded = canvasTab3.Height * canvasTab3.Width;
            pprng.GenerateDesiredQuantityOfRandomIntegers(DesiredCPUUtilization.AllThreads, integersNeeded, 0, 256);
            ConcurrentBag<BigInteger> bagOfIntegers = new ConcurrentBag<BigInteger>(pprng.GetBagOfRandomIntegers);

            ConcurrentBag<Bitmap> bagOfBitmaps = new ConcurrentBag<Bitmap>();
            int canvasHeight = canvasTab3.Height;

            Parallel.For(0, bmap.Width, i =>
            {

                Graphics x;
                Bitmap bmapx;
                bmapx = new Bitmap(1, canvasHeight);
                x = Graphics.FromImage(bmapx);

                for (int j = 0; j < canvasHeight; j++)
                {
                    BigInteger greyscale;

                    bool success = bagOfIntegers.TryTake(out greyscale);

                    Color randomColor = Color.FromArgb((int)greyscale, (int)greyscale, (int)greyscale);

                    SolidBrush randomSolidBrush = new SolidBrush(randomColor);
                    x.FillRectangle(randomSolidBrush, 0, j, 1, 1);
                }

                bagOfBitmaps.Add(bmapx);
                x.Dispose();
            });

            for (int i = 0; i < bmap.Width; i++)
            {
                Bitmap slice;
                bool success = bagOfBitmaps.TryTake(out slice);

                g.DrawImage(slice, i, 0);
                canvasTab3.Image = bmap;
            }
        }

        private void btnGenerateVerticalBars_Click(object sender, EventArgs e)
        {
            Color color = Color.FromArgb((int)prng.NextUInteger(256), (int)prng.NextUInteger(256), (int)prng.NextUInteger(256));
            SolidBrush solidBrush = new SolidBrush(color);

            for (int i = 0; i < bmap.Width; i++)
            {
                g.FillRectangle(solidBrush, i, 0, 1, (int)prng.NextUInteger(bmap.Height));
            }

            canvasTab3.Image = bmap;
        }

        private void btnRandomWalk_Click(object sender, EventArgs e)
        {
            int iterations = 1000;
            
            Color color = Color.FromArgb((int)prng.NextUInteger(256), (int)prng.NextUInteger(256), (int)prng.NextUInteger(256));
            Pen pen = new Pen(color, 3);

            Point originPoint = new Point(canvasTab3.Width / 2, canvasTab3.Height / 2);

            for (int i = 0; i < iterations; i++)
            {
                Point point1 = originPoint;
                Point point2 = new Point((point1.X + 4*(int)prng.Next(-1, 2)) % canvasTab3.Width, (point1.Y + 4*(int)prng.Next(-1, 2)) % canvasTab3.Height);
                g.DrawLine(pen, point1, point2);

                originPoint = point2;
            }
 
            canvasTab3.Image = bmap;
        }

        #endregion

        #region METHODS

        private int ThreadUsage(DesiredCPUUtilization desiredCPUUtilization)
        {
            int threadUsage = 1;

            if (desiredCPUUtilization == DesiredCPUUtilization.AllThreads)
                threadUsage = Environment.ProcessorCount;
            else if (desiredCPUUtilization == DesiredCPUUtilization.HalfAvailPlusOneThread)
                threadUsage = (Environment.ProcessorCount / 2) + 1;
            else if (desiredCPUUtilization == DesiredCPUUtilization.HalfAvailThreads)
                threadUsage = (Environment.ProcessorCount / 2);
            else if (desiredCPUUtilization == DesiredCPUUtilization.SingleThread)
                threadUsage = 1;

            return threadUsage;
        }

        #endregion
    }
}
