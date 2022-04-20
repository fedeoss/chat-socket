using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net.Sockets;
using System.Net;
using System.Windows.Threading;


namespace ChatScoket
{
    /// <summary>
    /// Applicativo chat con l'utilizzo della classe Socket
    /// Livello base: consegna del software realizzato in classe funzionante e con commenti
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null; //creo il socket
        DispatcherTimer dTimer = null;

        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //creo il socket; interNetwork= comunicazione tra socket ipv4; Dgram usa UDP

            //creo il socket sorgente e socket destinatario
            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endPoint = new IPEndPoint(local_address.MapToIPv4(), 65000); //specifico la porta: 65000 

            socket.Bind(local_endPoint); //unisco socket a endpoint

            dTimer = new DispatcherTimer();

            dTimer.Tick += new EventHandler(aggiornamento_dTimer); //per leggere il messaggio; gli do il metodo che lo fa leggere dopo il tick; interrotta la normale azione viene fatto questo
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250); //ogni quanto far scattare il tick, noi abbiamo scelto 250 ms
            dTimer.Start(); //inizia

        }

        private void btnInvia_Click(object sender, RoutedEventArgs e) //per inviare
        {
            IPAddress remote_address = IPAddress.Parse(txtIP.Text); //prendo l'ip dal wpf

            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, int.Parse(txtPorta.Text)); //prendo la porta dal wpf

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);//prendo il messaggio dal wpf

            socket.SendTo(messaggio, remote_endpoint); //invio e gli passo il messggio e l'endpoint

        }
        private void aggiornamento_dTimer(object sender, EventArgs e) //per ricevere
        {
            int nBytes = 0; //conta bytes ricevuti

            if ((nBytes = socket.Available) > 0) //controlla se ci sono bytes
            {
                byte[] buffer = new byte[nBytes]; //creo l'array che conterrà i bytes 

                EndPoint remoteendPoint = new IPEndPoint(IPAddress.Any, 0); //do valori di dafaul al end point perche non li so

                nBytes = socket.ReceiveFrom(buffer, ref remoteendPoint);//riceve il messaggio e  memorizza l'end point 

                string from = ((IPEndPoint)remoteendPoint).Address.ToString(); //creo una stringa che mi dice chi mi ha inviato il messaggio

                string messaggio = Encoding.UTF8.GetString(buffer, 0, nBytes);//converto il messaggio ricevuto e lo metto in una stringa

                lslboxMessaggi.Items.Add(from + ":" + messaggio); //mostro il messaggio al destinatario
            }

        }
    }
}