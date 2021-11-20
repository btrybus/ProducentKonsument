using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProducentKonsument
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private const int N = 10;

        private static int[] buffer = new int[N];                                               // bufor
        private static int Pdelay = 500, Kdelay = 1000;                                         // czas produkcji/konsupmcji
        private static int Pcounter = 0, Kcounter = 0;                                          // licznik produkcji/konsumpcji
        private static int Bcounter = 0;

        private static Semaphore Wolne = new Semaphore(N, N);
        private static Semaphore Pelne = new Semaphore(0, N);

        private Thread tp;                                                               // watek producenta
        private Thread tk;                                                               // watek konsumenta

        //producent
        private static void Producent(object num)
        {
            int j = 0;

            while( true )
            {
                Thread.Sleep(Pdelay);

                // opuszczenie Wolne
                Wolne.WaitOne();

                buffer[j] = 1;
                j = (j + 1) % N;
                Pcounter++;
                Bcounter++;

                // podniesienie Pelne
                Pelne.Release();
            }
        }

        //konsument
        private static void Konsument(object num)
        {
            int k = 0;

            while (true)
            {
                Pelne.WaitOne();

                buffer[k] = 0;
                k = (k + 1) % N;
                Kcounter++;
                Bcounter--;

                Wolne.Release();

                Thread.Sleep( Kdelay );
            }
        }

        //wlaczenie producenta
        private void button1_Click(object sender, EventArgs e)
        {
            Pdelay = int.Parse(m_Pdelay.Text);

            tp = new Thread(Producent);
            tp.Start();
            tp.Name = "Producent";

            button1.Enabled = false;
        }
        
        //wlaczenie konsumenta
        private void button2_Click(object sender, EventArgs e)
        {
            Kdelay = int.Parse(m_Kdelay.Text);

            tk = new Thread(Konsument);
            tk.Start();
            tk.Name = "Konsument";
            
            button2.Enabled = false;
        }

        private void m_Pdelay_TextChanged(object sender, EventArgs e)
        {
            Pdelay = int.Parse(m_Pdelay.Text);
            Kdelay = int.Parse(m_Kdelay.Text);
        }

        //OnTimer
        private void timer1_Tick(object sender, EventArgs e)
        {
            string temp;
            string temp2 = null;

            for (int i = 0; i < N; i++)
            {
                temp = buffer[i] + "  |  ";
                temp2 += temp;
            }
            textBox1.Text = temp2;

            m_Pcounter.Text = Pcounter.ToString();
            m_Kcounter.Text = Kcounter.ToString();

            m_wBuforze.Text = Bcounter.ToString();

            button1.Enabled = tp is null || !tp.IsAlive;
            button2.Enabled = tk is null || !tk.IsAlive;
                
        }

        //przycisk Zatrzymaj
        private void button3_Click(object sender, EventArgs e)
        {
            tp?.Abort();
            tk?.Abort();
        }

    }
}
