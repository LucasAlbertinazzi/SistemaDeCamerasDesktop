using SistCamerasGuarita.Suporte.MessageBox;
using System.Net.NetworkInformation;
using System.Timers;
using System.Windows;

namespace SistCamerasGuarita
{
    public partial class App : Application
    {
        #region 1 - VARIÁVEIS
        private Timer timer;
        private bool isConnected = true;
        private bool avisoOpen = false;
        #endregion

        #region 2 - MÉTODOS 
        private void CheckConnection()
        {
            using (Ping ping = new Ping())
            {
                try
                {
                    PingReply reply = ping.Send("192.168.10.1");

                    if (reply.Status == IPStatus.Success)
                    {
                        isConnected = true;
                        // Se a conexão for bem-sucedida e a janela de aviso estiver aberta, feche-a
                        if (isConnected)
                        {
                            // Feche a janela de aviso se estiver aberta
                            if (avisoOpen)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    NewMessageBox.CloseLayout();
                                });

                                avisoOpen = false;
                            }
                        }
                    }
                }
                catch
                {
                    if (!avisoOpen)
                    {
                        avisoOpen = true;
                        isConnected = false;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // Exibir a janela de aviso aqui
                            NewMessageBox.Infos("A conexão com a internet foi perdida. O sistema tentará reconectar.", 5);
                        });
                    }
                }
            }
        }

        public void FecharSistema()
        {
            OnExit(null);
        }
        #endregion

        #region 3 - EVENTOS DE CONTROLE

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configura e inicia o timer para verificar a conexão a cada 5 segundos
            timer = new Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Verifica a conexão imediatamente ao iniciar o aplicativo
            CheckConnection();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckConnection();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        #endregion
    }
}