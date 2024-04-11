using AxAXISMEDIACONTROLLib;
using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Interface;
using SistCamerasGuarita.Services;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;

namespace SistCamerasGuarita
{
    public partial class WpfPrincipal : Window
    {
        #region 1 - VARIÁVEIS
        ServiceHikvision hikvision = new ServiceHikvision();
        ServiceBrickcom brickcom = new ServiceBrickcom();
        ServiceLayout svLayout = new ServiceLayout();
        List<CamerasInfos> camerasPainel = new List<CamerasInfos>();
        List<CamerasLayout> camerasLayout = new List<CamerasLayout>();
        #endregion

        #region 2 - CONSTRUTORES
        public WpfPrincipal()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            DefineAmbiente();
            DefineAutenticacao();
            LocalCam();
            CarregaCameras();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Obter a instância atual da aplicação
            App app = (App)System.Windows.Application.Current;

            // Chamar o método FecharSistema()
            app.FecharSistema();
        }
        #endregion

        #region 3 - MÉTODOS
        private void ReiniciaVideo()
        {
            InfoGlobal.AlteracaoCam = false;

            // Limpa todos os controles do grid
            gridCameras.Children.Clear();

            // Limpa a lista de câmeras do painel e do layout
            camerasPainel.Clear();
            camerasLayout.Clear();

            Thread.Sleep(2000);

            CarregaCameras();

            Thread.Sleep(2000);

            // Verifica se todos os controles de vídeo foram adicionados ao grid
            int expectedControlsCount = camerasPainel.Count;
            int actualControlsCount = gridCameras.Children.OfType<Border>().Count();

            if (expectedControlsCount != actualControlsCount)
            {
                NewMessageBox.Infos("Ocorreu um problema ao carregar os vídeos. Aguarde o sistema ser reiniciado!", 2);

                Process.Start(System.Windows.Application.ResourceAssembly.Location);
                System.Windows.Application.Current.Shutdown();
            }

            // Força a coleta de lixo para liberar recursos não utilizados
            GC.Collect();

            // Reinicia a execução do coletor de lixo para garantir a liberação imediata de recursos
            GC.WaitForPendingFinalizers();
        }

        private void DefineAmbiente()
        {
            if (Debugger.IsAttached)
            {
                string[] op = { "Produção", "Desenvolvimento" };

                NewMessageBox.Opcoes("Deseja usar qual ambiente?", op, 3);

                if (NewMessageBox.Opcao == 1)
                {
                    InfoGlobal.Ambiente = "Produção";
                    ConexaoConfig.AbrirConexao("DbProd");
                }
                else
                {
                    InfoGlobal.Ambiente = "Desenvolvimento";
                    ConexaoConfig.AbrirConexao("DbDev");
                }
            }
            else
            {
                InfoGlobal.Ambiente = "Produção";
                ConexaoConfig.AbrirConexao("DbProd");
            }
        }

        private void LocalCam()
        {
            // Obtém o nome do host da máquina local
            string hostName = Dns.GetHostName();

            // Resolve o nome do host em um endereço IP
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);

            // Verifica se algum dos endereços IP corresponde ao IP desejado
            foreach (IPAddress address in addresses)
            {
                if (IPAddress.Parse("192.168.78.6").Equals(address))
                {
                    InfoGlobal.CasaCam = true;
                }
            }
        }

        private void ResetaInfosUser()
        {
            InfoGlobal.Autenticacao = false;
            InfoGlobal.Usuario = string.Empty;
            InfoGlobal.IdUser = 0;
            InfoGlobal.Ambiente = string.Empty;
        }

        private void CarregaCameras()
        {
            camerasPainel = new List<CamerasInfos>();
            camerasPainel.AddRange(hikvision.HikvisionCameras());
            camerasPainel.AddRange(brickcom.BrickcomCameras());
            camerasLayout = svLayout.CarregaLayout();
            CriaPainelCameras();
        }

        private void CriaPainelCameras()
        {
            // Itera sobre a lista de layouts de câmera
            foreach (var layout in camerasLayout)
            {
                // Verifica o monitor associado ao layout
                int monitor = layout.Monitor;

                if (monitor == 1)
                {
                    // Cria o painel de câmeras no grid da janela principal
                    CriarPainelPrincipal(layout);
                }
                else
                {
                    // Cria uma nova janela para o monitor secundário
                    CriarNovaJanela(layout);
                }
            }
        }

        private void CriarPainelPrincipal(CamerasLayout layout)
        {
            // Limpa o grid antes de adicionar os novos controles
            gridCameras.Children.Clear();
            gridCameras.RowDefinitions.Clear();
            gridCameras.ColumnDefinitions.Clear();

            // Adicionar definições de linha com base no número total de linhas
            for (int i = 0; i < layout.Linhas; i++)
            {
                gridCameras.RowDefinitions.Add(new RowDefinition());
            }

            // Adicionar definições de coluna com base no número total de colunas
            for (int i = 0; i < layout.Colunas; i++)
            {
                gridCameras.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Adiciona as câmeras aos controles PictureBox e os posiciona no grid
            foreach (var camera in camerasPainel.Where(c => c.Monitor == layout.Monitor))
            {
                //Câmeras Hickvison
                if (camera.IdMarca == 1)
                {
                    // Configurações do PictureBox
                    PictureBox pictureBox = new PictureBox();
                    camera.Componente = pictureBox.Handle;
                    hikvision.PreviewCameras(camera);

                    // Crie um WindowsFormsHost e adicione o PictureBox a ele
                    WindowsFormsHost host = new WindowsFormsHost();
                    host.Child = pictureBox;

                    // Crie um Border e defina o WindowsFormsHost como seu conteúdo
                    Border border = new Border();
                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(0);
                    border.Child = host; // Defina o WindowsFormsHost como conteúdo do Border

                    // Calcula a posição corrigida para o grid
                    int correctedRow = (camera.Posicao - 1) / layout.Colunas;
                    int correctedColumn = (camera.Posicao - 1) % layout.Colunas;

                    // Defina a posição do Border no grid
                    Grid.SetRow(border, correctedRow);
                    Grid.SetColumn(border, correctedColumn);

                    // Adicione o Border (com o WindowsFormsHost e o PictureBox dentro) ao grid
                    gridCameras.Children.Add(border);
                }
                //Câmeras Brickcom
                else if (camera.IdMarca == 2)
                {
                    // Criar uma instância do controle AxAxisMediaControl
                    AxAxisMediaControl axs = new AxAxisMediaControl();

                    // Crie um WindowsFormsHost e adicione o AxAxisMediaControl a ele
                    WindowsFormsHost host = new WindowsFormsHost();
                    host.Child = axs;

                    // Calcula a posição corrigida para o grid
                    int correctedRow = (camera.Posicao - 1) / layout.Colunas;
                    int correctedColumn = (camera.Posicao - 1) % layout.Colunas;

                    // Defina a posição do WindowsFormsHost no grid
                    Grid.SetRow(host, correctedRow);
                    Grid.SetColumn(host, correctedColumn);

                    // Adicione o WindowsFormsHost ao grid
                    gridCameras.Children.Add(host);

                    // Configurar as propriedades do controle
                    axs.MediaURL = $"http://{camera.Ip}/channel2";
                    axs.MediaUsername = camera.UserCamera;
                    axs.MediaPassword = camera.SenhaCamera;
                    axs.EnableReconnect = true;
                    axs.StretchToFit = true;

                    // Iniciar a reprodução do feed de vídeo
                    axs.Play();
                }
            }

            // Verifica as posições sem câmeras e adiciona a representação correspondente ao grid
            for (int posicao = 1; posicao <= layout.Linhas * layout.Colunas; posicao++)
            {
                if (camerasPainel.Where(c => c.Monitor == layout.Monitor).All(camera => camera.Posicao != posicao))
                {
                    // Calcule a posição atual corrigida
                    int correctedRow = (posicao - 1) / layout.Colunas;
                    int correctedColumn = (posicao - 1) % layout.Colunas;

                    // Se não houver uma câmera na posição atual, adiciona o fundo preto com o texto "SEM CÂMERA"
                    Border emptyBorder = new Border();
                    emptyBorder.Background = Brushes.Black;

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = "SEM CÂMERA";
                    textBlock.Foreground = Brushes.White;
                    textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;

                    emptyBorder.Child = textBlock;

                    // Defina a posição do Border no grid
                    Grid.SetRow(emptyBorder, correctedRow);
                    Grid.SetColumn(emptyBorder, correctedColumn);

                    // Adiciona ao grid
                    gridCameras.Children.Add(emptyBorder);
                }
            }
        }

        private void CriarNovaJanela(CamerasLayout layout)
        {
            if (Screen.AllScreens.Length <= 1)
            {
                // Se houver apenas um monitor, não faz nada
                return;
            }

            // Obtém o monitor correspondente ao layout
            Screen monitor = Screen.AllScreens[layout.Monitor - 1];

            // Cria uma nova instância de janela
            Window novaJanela = new Window();
            novaJanela.Title = $"Painel de Câmeras - Monitor {layout.Monitor}";
            novaJanela.WindowStartupLocation = WindowStartupLocation.Manual;
            novaJanela.WindowState = WindowState.Maximized;

            // Define a posição da janela com base nas coordenadas do monitor desejado
            novaJanela.Left = monitor.Bounds.Left;
            novaJanela.Top = monitor.Bounds.Top;

            // Cria um novo grid para a nova janela
            Grid gridCamerasSecundario = new Grid();

            // Define as colunas e linhas do grid com base no layout do monitor secundário
            for (int i = 0; i < layout.Linhas; i++)
            {
                gridCamerasSecundario.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < layout.Colunas; i++)
            {
                gridCamerasSecundario.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Adiciona as câmeras ao grid
            foreach (var camera in camerasPainel.Where(c => c.Monitor == layout.Monitor))
            {
                //Câmeras Hickvison
                if (camera.IdMarca == 1)
                {
                    // Configurações do PictureBox
                    PictureBox pictureBox = new PictureBox();
                    camera.Componente = pictureBox.Handle;
                    hikvision.PreviewCameras(camera);

                    // Crie um WindowsFormsHost e adicione o PictureBox a ele
                    WindowsFormsHost host = new WindowsFormsHost();
                    host.Child = pictureBox;

                    // Crie um Border e defina o WindowsFormsHost como seu conteúdo
                    Border border = new Border();
                    border.BorderBrush = Brushes.Transparent;
                    border.BorderThickness = new Thickness(0);
                    border.Child = host; // Defina o WindowsFormsHost como conteúdo do Border

                    // Calcula a posição corrigida para o grid
                    int correctedRow = (camera.Posicao - 1) / layout.Colunas;
                    int correctedColumn = (camera.Posicao - 1) % layout.Colunas;

                    // Defina a posição do Border no grid
                    Grid.SetRow(border, correctedRow);
                    Grid.SetColumn(border, correctedColumn);

                    // Adicione o Border (com o WindowsFormsHost e o PictureBox dentro) ao grid
                    gridCamerasSecundario.Children.Add(border);
                }
                //Câmeras Brickcom
                else if (camera.IdMarca == 2)
                {
                    // Criar uma instância do controle AxAxisMediaControl
                    AxAxisMediaControl axs = new AxAxisMediaControl();

                    // Crie um WindowsFormsHost e adicione o AxAxisMediaControl a ele
                    WindowsFormsHost host = new WindowsFormsHost();
                    host.Child = axs;

                    // Calcula a posição corrigida para o grid
                    int correctedRow = (camera.Posicao - 1) / layout.Colunas;
                    int correctedColumn = (camera.Posicao - 1) % layout.Colunas;

                    // Defina a posição do WindowsFormsHost no grid
                    Grid.SetRow(host, correctedRow);
                    Grid.SetColumn(host, correctedColumn);

                    // Adicione o WindowsFormsHost ao grid
                    gridCamerasSecundario.Children.Add(host);

                    // Manipular o evento HandleCreated para garantir que o controle esteja completamente inicializado
                    axs.HandleCreated += (sender, e) =>
                    {
                        // Configurar as propriedades do controle
                        axs.MediaURL = $"http://{camera.Ip}/channel2";
                        axs.MediaUsername = camera.UserCamera;
                        axs.MediaPassword = camera.SenhaCamera;
                        axs.EnableReconnect = true;
                        axs.StretchToFit = true;

                        // Iniciar a reprodução do feed de vídeo
                        axs.Play();
                    };
                }
            }

            // Verifica as posições sem câmeras e adiciona a representação correspondente ao grid
            for (int posicao = 1; posicao <= layout.Linhas * layout.Colunas; posicao++)
            {
                if (camerasPainel.Where(c => c.Monitor == layout.Monitor).All(camera => camera.Posicao != posicao))
                {
                    // Calcule a posição atual corrigida
                    int correctedRow = (posicao - 1) / layout.Colunas;
                    int correctedColumn = (posicao - 1) % layout.Colunas;

                    // Se não houver uma câmera na posição atual, adiciona o fundo preto com o texto "SEM CÂMERA"
                    Border emptyBorder = new Border();
                    emptyBorder.Background = Brushes.Black;

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = "SEM CÂMERA";
                    textBlock.Foreground = Brushes.White;
                    textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;

                    emptyBorder.Child = textBlock;

                    // Defina a posição do Border no grid
                    Grid.SetRow(emptyBorder, correctedRow);
                    Grid.SetColumn(emptyBorder, correctedColumn);

                    // Adiciona ao grid
                    gridCamerasSecundario.Children.Add(emptyBorder);
                }
            }

            // Define o grid como conteúdo da nova janela
            novaJanela.Content = gridCamerasSecundario;

            // Exibe a nova janela
            novaJanela.Show();

            // Move a janela para o monitor desejado
            IntPtr handle = new WindowInteropHelper(novaJanela).Handle;
            WinApiHelper.MoveWindowToMonitor(handle, monitor);
        }

        private void DefineAutenticacao()
        {
            btnAdmCasa.Visibility = Visibility.Collapsed;

            if (InfoGlobal.Autenticacao)
            {
                lblUserAmbiente.Header = InfoGlobal.Usuario + " - " + InfoGlobal.Ambiente;
                lblMenuLogin.Header = "Sair";
                btnConfig.Visibility = Visibility.Visible;

                if (InfoGlobal.CodDep == 1)
                {
                    btnAdmCasa.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lblUserAmbiente.Header = string.Empty;
                lblMenuLogin.Header = "Entrar";
                btnConfig.Visibility = Visibility.Collapsed;
            }
        }

        private class WinApiHelper
        {
            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            private const uint SWP_SHOWWINDOW = 0x0040;

            public static void MoveWindowToMonitor(IntPtr windowHandle, Screen monitor)
            {
                SetWindowPos(windowHandle, IntPtr.Zero, monitor.Bounds.Left, monitor.Bounds.Top, monitor.Bounds.Width, monitor.Bounds.Height, SWP_SHOWWINDOW);
            }
        }
        #endregion

        #region 4 - EVENTOS DE CONTROLE

        private void AdicionarCamera_Click(object sender, RoutedEventArgs e)
        {
            new WpfAddCameras().ShowDialog();
        }

        private void PosicionarCamera_Click(object sender, RoutedEventArgs e)
        {
            new WpfPosicionaCameras().ShowDialog();

            if (InfoGlobal.AlteracaoCam)
            {
                ReiniciaVideo();
            }
        }

        private void AdicionarMarca_Click(object sender, RoutedEventArgs e)
        {
            new WpfAddMarcas().ShowDialog();
        }

        private void ExcluirCamera_Click(object sender, RoutedEventArgs e)
        {
            new WpfExcluiCamera().ShowDialog();

            if (InfoGlobal.AlteracaoCam)
            {
                ReiniciaVideo();
            }
        }

        private void lblMenuLogin_Click(object sender, RoutedEventArgs e)
        {
            if (InfoGlobal.Autenticacao)
            {
                ResetaInfosUser();
            }
            else
            {
                new WpfAutenticacao().ShowDialog();
            }

            DefineAutenticacao();
        }

        private void AdmCasa_Click(object sender, RoutedEventArgs e)
        {
            if (InfoGlobal.Admin)
            {
                NewMessageBox.Infos("A partir de agora, todas as configurações estão relacionadas ao Sistema de Câmeras do CD.", 2);
                InfoGlobal.CasaCam = false;
                InfoGlobal.Admin = false;
                CarregaCameras();
            }
            else
            {
                NewMessageBox.Infos("A partir de agora, todas as configurações estão relacionadas ao Sistema de Câmeras da casa do Marcio", 2);
                InfoGlobal.CasaCam = true;
                InfoGlobal.Admin = true;
                CarregaCameras();
            }
        }
        #endregion
    }
}
