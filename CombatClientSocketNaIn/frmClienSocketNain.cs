using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CombatClientSocketNaIn.Classes;


namespace CombatClientSocketNaIn
{
    public partial class frmClienSocketNain : Form
    {
        Random m_r;
        Elfe m_elfe;
        Nain m_nain;
        public frmClienSocketNain()
        {
            InitializeComponent();
            m_r = new Random();

            btnReset.Enabled = false;
            Control.CheckForIllegalCrossThreadCalls = false;
            picElfe.Image = Image.FromFile("elfe.jpg");
            picNain.Image = Image.FromFile("nain.jpg");
            //créer elfe et nain

            m_nain = new Nain(1,0,0);
            lblVieNain.Text = "Vie: " + m_nain.Vie.ToString();
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblArmeNain.Text = "Arme: " + m_nain.Arme;

            m_elfe = new Elfe(m_r.Next(10, 20), m_r.Next(2, 6), m_r.Next(0, 3));
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();
        }
        void Reset()
        {
            m_nain = new Nain(m_r.Next(10, 20), m_r.Next(2, 6), m_r.Next(0, 3));
            picNain.Image = m_nain.Avatar;
            lblVieNain.Text = "Vie: " + m_nain.Vie.ToString(); 
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblArmeNain.Text = "Arme: " + m_nain.Arme;

            m_elfe = new Elfe(1, 0, 0);
            picElfe.Image = m_elfe.Avatar;
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            lblForceElfe.Text = "Force: " + m_elfe.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnFrappe.Enabled = true;
            Reset();
        }

        private void btnFrappe_Click(object sender, EventArgs e)
        {
            /* déclaration de variables */
            Socket client;
            string envoie = "rien";
            int nbOctetReception;
            string reponse = "aucune";

            byte[] tByteReception = new byte[100];
            ASCIIEncoding textByte = new ASCIIEncoding();
            byte[] tByteEnvoie = textByte.GetBytes("Tirage");
            try
            {
                // initialisation et connection du socket au serveur TCP
                client = new Socket(SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
                MessageBox.Show("assurez-vous que le serveur est en attente d'un client");

                if (client.Connected)
                {
                    envoie = "vieNain;forceNain;armeNain;";

                    MessageBox.Show("Client: \r\nTransmet..." + envoie);
                    tByteEnvoie = textByte.GetBytes(envoie);
                    //transmission
                    client.Send(tByteEnvoie);
                    Thread.Sleep(500);

                    //réception
                    MessageBox.Show("Client: réception de données serveur");
                    nbOctetReception = client.Receive(tByteReception);

                    reponse = Encoding.ASCII.GetString(tByteReception);
                    MessageBox.Show("\r\nReception..." + reponse);
                    //un split
                    string[] substring = reponse.Split(';');
                    substring[0] = m_nain.Vie.ToString();
                    substring[1] = m_nain.Force.ToString();
                    substring[2] = m_nain.Arme;
                    substring[3] = m_elfe.Vie.ToString();
                    substring[4] = m_elfe.Force.ToString();
                    substring[5] = m_elfe.Sort.ToString();
                    //assigne au lbl
                    substring[0] = lblVieNain.Text;
                    substring[1] = lblForceNain.Text;
                    substring[2] = lblArmeNain.Text;
                    substring[3] = lblVieElfe.Text;
                    substring[4] = lblForceElfe.Text;
                    substring[5] = lblSortElfe.Text;
                } 

                //vérifier le gagnant
                if (m_nain.Vie > m_elfe.Vie)
                {
                    MessageBox.Show("Le nain gagne!!");
                }
                else
                {
                    MessageBox.Show("L'elfe gagne!!");
                }

                // fermeture du socket
                client.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            m_nain.Frapper(m_elfe);
            lblVieElfe.Text = "Vie: " + m_elfe.Vie.ToString();
            if(m_elfe.Vie <= 0)
            {
                btnFrappe.Enabled = false; btnReset.Enabled = false;
                picNain.Image = m_nain.Avatar;
            }

            m_elfe.LancerSort(m_nain);
            lblVieNain.Text = "Vie: " + m_nain.Force.ToString();
            lblForceNain.Text = "Force: " + m_nain.Force.ToString();
            lblSortElfe.Text = "Sort: " + m_elfe.Sort.ToString();

            if (m_nain.Vie <= 0)
            {
                btnFrappe.Enabled = false; btnReset.Enabled = false;
                picNain.Image = m_elfe.Avatar;
            }
        }
    }
}
