using OpenNETCF.MQTT;
using proyecto1.Clases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using HTTPupt;
using Microcharts;
using Entry = Microcharts.ChartEntry;//esto son los entrys del nuget charts
using SkiaSharp;//libreria para dar color a los graficos
//using Entry = Microcharts.BarChart;



namespace proyecto1
{
    public partial class MainPage : ContentPage
    {
       
        MQTTClient mqtt;
        Sensores datos; //objeto de la clase Sensores
        Random rand = new Random();
        List<Entry> entryList;

        public MainPage()
        {
            InitializeComponent();
           
            Title = "Ejemplo MQTT";

            //cargar lista de entries
           
            String ClienteID = "UPT" + rand.Next(1000);
            

            mqtt = new MQTTClient("ioticos.org", 1883);
            mqtt.ConnectAsync(ClienteID, "jIB477bFx6Cnnz0", "sUK4y8eXkaWFe0L");
            mqtt.MessageReceived += Mqtt_MessageReceived;
            mqtt.Subscriptions.Add(new Subscription("0jtBoAJzggGTE3s/Danny"));

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (mqtt.IsConnected)//reconnect
                {
                    if (datos != null)
                    {
                        // pgbTemperatura.Progress = datos.Temperatura / 100; 
                        // pgbHumedad.Progress = datos.Humedad / 100;

                        entryList = new List<Entry>();
                        if (datos.EstatusLuz == true)
                            estatusLuz.Text = "Encendida";
                        else
                            estatusLuz.Text = "Apagado";


                        fecha.Text = datos.Fecha.ToString();
                        cicloTrabajo.Text = datos.CicloTrabajo;
                        idPrueba.Text = datos.IdPrueba.ToString();
                        LlenarGrafico();
                        //asiganar los datos de los entries a los graficos de la vista
                        chartGrafica.Chart = new BarChart()
                        {                          
                            Entries = entryList
                            
                        };
                        
                     

                    }
                }
                else
                {
                    mqtt = new MQTTClient("ioticos.org", 1883);
                    mqtt.ConnectAsync(ClienteID, "jIB477bFx6Cnnz0", "sUK4y8eXkaWFe0L");
                    mqtt.MessageReceived += Mqtt_MessageReceived;
                    mqtt.Subscriptions.Add(new Subscription("0jtBoAJzggGTE3s/Danny"));

                }
                return true;
            });


        }


        //cargar lista de entrys
        private void LlenarGrafico()
        {

            Entry temperatura = new Entry(datos.Temperatura)
            {
                Label = "Temperatura",
                ValueLabel = datos.Temperatura.ToString(),
                Color = SKColor.Parse("9BEFE7")
            };
            Entry humedad = new Entry(datos.Humedad)
            {
                Label = "Humedad",
                ValueLabel = datos.Humedad.ToString(),
                Color = SKColor.Parse("F8F478")
            };

            entryList.Add(temperatura);
            entryList.Add(humedad);


        }

     
        private void Mqtt_MessageReceived(String topic, QoS qos, byte[] payload)//callback
        {
            String mensaje = System.Text.Encoding.UTF8.GetString(payload); //convierte el payload(arreglo de bytes) en string
            datos = JsonConvertidor.Json_Objeto<Sensores>(mensaje); //el jason recibido  se convierte a objeto de la clase sensores
        }

      /*  private void btnEnviarMQTT_Clicked(object sender, EventArgs e)//Boton para enviar mensaje al broker
        {
            byte[] datos = Encoding.ASCII.GetBytes("Mensaje envíado desde el dispositivo móvil"); //converitr mensaje en ADCCI para que sea convetido a bytes
            mqtt.PublishAsync("0jtBoAJzggGTE3s/UPT", datos, QoS.FireAndForget, false);


        }*/


    }
}
