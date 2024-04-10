using AxAXISMEDIACONTROLLib;
using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Services;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace SistCamerasGuarita.Interface
{
    public partial class WpfPosicionaCameras : Window
    {
        #region 1 - VARIÁVEIS
        DAL obj = new DAL();
        ServiceHikvision svHikvision = new ServiceHikvision();
        List<CamerasInfos> listaCameras = new List<CamerasInfos>();
        #endregion

        #region 2 - CONSTRUTORES
        public WpfPosicionaCameras()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GeraEventosGridPosicao();
            CarregaCameras();
            CarregaLayout();
            UpdateGrid();
        }
        #endregion

        #region 3 - MÉTODOS

        #region 3.1 - GRID POSIÇÃO

        private void GeraEventosGridPosicao()
        {
            // Preencher o combo de monitor com os monitores disponíveis
            foreach (Screen screen in Screen.AllScreens)
            {
                cmbMonitor.Items.Add($"Monitor {Array.IndexOf(Screen.AllScreens, screen) + 1}");
            }

            // Preencher as comboboxes de linha e coluna com números de 1 a 10
            for (int i = 1; i <= 10; i++)
            {
                cmbLinha.Items.Add(i);
                cmbColuna.Items.Add(i);
            }

            // Evento para atualizar o grid quando o usuário alterar as comboboxes
            cmbLinha.SelectionChanged += CmbLinha_SelectionChanged;
            cmbColuna.SelectionChanged += CmbColuna_SelectionChanged;

            // Evento para pintar o quadrado correspondente de cinza claro quando o usuário selecionar uma posição
            cmbPosicao.SelectionChanged += cmbPosicao_SelectionChanged;

            cmbLinha.SelectedIndex = 0;
            cmbColuna.SelectedIndex = 0;
            cmbPosicao.SelectedIndex = 0;
            cmbMonitor.SelectedIndex = 0;
        }

        private void UpdatePositionComboBox()
        {
            cmbPosicao.Items.Clear();

            int numRows = cmbLinha.SelectedIndex + 1;
            int numCols = cmbColuna.SelectedIndex + 1;

            // Preencher o combo de posição com as multiplicações dos valores selecionados nas comboboxes de linha e coluna
            for (int i = 1; i <= numRows * numCols; i++)
            {
                cmbPosicao.Items.Add(i);
            }
        }

        private void UpdateGrid()
        {
            gridNumbers.Children.Clear();
            gridNumbers.RowDefinitions.Clear();
            gridNumbers.ColumnDefinitions.Clear();

            int numRows = cmbLinha.SelectedIndex + 1;
            int numCols = cmbColuna.SelectedIndex + 1;

            // Adicionar RowDefinitions ao grid
            for (int i = 0; i < numRows; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                gridNumbers.RowDefinitions.Add(rowDef);
            }

            // Adicionar ColumnDefinitions ao grid
            for (int i = 0; i < numCols; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                gridNumbers.ColumnDefinitions.Add(colDef);
            }

            int position = 1;

            // Adicionar "quadrados" ao grid com números dentro
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);

                    foreach (var item in listaCameras)
                    {
                        if (item.Monitor == cmbMonitor.SelectedIndex + 1 && item.Posicao == position)
                        {
                            border.Background = Brushes.Red;
                        }
                    }

                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = position.ToString();
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;

                    border.Child = textBlock;

                    gridNumbers.Children.Add(border);

                    position++;
                }
            }


        }
        #endregion

        #region 3.2 - GERAL

        private void CarregaCameras()
        {
            string sql = $"SELECT * FROM tbl_cameras_cd WHERE ativo = 'true' AND userexclui = 0";

            if (InfoGlobal.CasaCam)
            {
                sql = $"SELECT * FROM tbl_cameras_cd WHERE ativo = 'true' AND userexclui = 0 AND casacam = 'true'";
            }

            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<CamerasInfos> cameras = new List<CamerasInfos>();
                listaCameras = new List<CamerasInfos>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int componenteValue = Convert.ToInt32(dt.Rows[i]["componente"]);
                    IntPtr componenteIntPtr = new IntPtr(componenteValue);

                    // Verifica se dataexclui é DBNull ou null
                    DateTime? dataExclui = dt.Rows[i]["dataexclui"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["dataexclui"]);

                    // Verifica se dataatualiza é DBNull ou null
                    DateTime? dataAtualiza = dt.Rows[i]["dataatualiza"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["dataatualiza"]);

                    cameras.Add(new CamerasInfos
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["id"]),
                        IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                        IdMarca = Convert.ToInt32(dt.Rows[i]["idmarca"]),
                        Posicao = Convert.ToInt32(dt.Rows[i]["posicao"]),
                        Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
                        Componente = componenteIntPtr,
                        Camera = dt.Rows[i]["camera"].ToString(),
                        CameraCombo = dt.Rows[i]["camera"].ToString() + " - " + dt.Rows[i]["posicao"].ToString(),
                        Ip = dt.Rows[i]["ip"].ToString(),
                        Porta = Convert.ToInt32(dt.Rows[i]["porta"]),
                        UserCamera = dt.Rows[i]["usercamera"].ToString(),
                        SenhaCamera = dt.Rows[i]["senhacamera"].ToString(),
                        UserCadastra = Convert.ToInt32(dt.Rows[i]["usercadastra"]),
                        UserExclui = Convert.ToInt32(dt.Rows[i]["userexclui"]),
                        UserAtualiza = Convert.ToInt32(dt.Rows[i]["useratualiza"]),
                        Ativo = Convert.ToBoolean(dt.Rows[i]["ativo"]),
                        DataCadastra = Convert.ToDateTime(dt.Rows[i]["datacadastra"]),
                        DataExclui = dataExclui,
                        DataAtualiza = dataAtualiza
                    });
                }

                cmbCameras.ItemsSource = cameras.OrderBy(x => x.Posicao).ThenBy(x => x.Monitor);
                listaCameras = cameras.OrderBy(x => x.Posicao).ThenBy(x => x.Monitor).ToList();
            }
        }

        private void CarregaLayout()
        {
            if (cmbCameras.ItemsSource != null)
            {
                CamerasInfos camSelecionada = (CamerasInfos)cmbCameras.SelectedItem;

                string sql = $"SELECT * FROM tbl_cameras_cd_layout WHERE idlayout = {camSelecionada.IdLayout}";

                if (InfoGlobal.CasaCam)
                {
                    sql = $"SELECT * FROM tbl_cameras_cd_layout WHERE idlayout = {camSelecionada.IdLayout} AND casacam = 'true'";
                }

                DataTable dt = obj.RetornaTabela(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<CamerasLayout> camLayout = new List<CamerasLayout>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DateTime? dataCad = dt.Rows[i]["datacadastra"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["datacadastra"]);
                        DateTime? dataAtt = dt.Rows[i]["dataatualiza"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["dataatualiza"]);

                        camLayout.Add(new CamerasLayout
                        {
                            IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                            Linhas = Convert.ToInt32(dt.Rows[i]["linhas"]),
                            Colunas = Convert.ToInt32(dt.Rows[i]["colunas"]),
                            Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
                            UserCadastra = Convert.ToInt32(dt.Rows[i]["usercadastra"]),
                            UserAtualiza = Convert.ToInt32(dt.Rows[i]["useratualiza"]),
                            DataCadastra = dataCad,
                            DataAtualiza = dataAtt,
                        });

                        if (Convert.ToInt32(dt.Rows[i]["monitor"]) != 0)
                        {
                            cmbLinha.IsEnabled = false;
                            cmbColuna.IsEnabled = false;
                        }
                    }

                    cmbLinha.Items.Clear();
                    cmbColuna.Items.Clear();

                    for (int i = 1; i <= camLayout[0].Linhas; i++)
                    {
                        cmbLinha.Items.Add(i);
                    }

                    for (int i = 1; i <= camLayout[0].Colunas; i++)
                    {
                        cmbColuna.Items.Add(i);
                    }

                    cmbMonitor.SelectedIndex = camLayout[0].Monitor - 1;
                    cmbLinha.SelectedValue = camLayout[0].Linhas;
                    cmbColuna.SelectedValue = camLayout[0].Colunas;
                    cmbPosicao.SelectedValue = camSelecionada.Posicao;
                }
            }
        }

        private void CarregaLayoutMonitor()
        {
            string sql = $"SELECT * FROM tbl_cameras_cd_layout WHERE idlayout = {cmbMonitor.SelectedIndex + 1}";

            if (InfoGlobal.CasaCam)
            {
                sql = $"SELECT * FROM tbl_cameras_cd_layout WHERE idlayout = {cmbMonitor.SelectedIndex + 1} AND casacam = 'true'";
            }

            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<CamerasLayout> camLayout = new List<CamerasLayout>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime? dataCad = dt.Rows[i]["datacadastra"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["datacadastra"]);
                    DateTime? dataAtt = dt.Rows[i]["dataatualiza"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["dataatualiza"]);

                    camLayout.Add(new CamerasLayout
                    {
                        IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                        Linhas = Convert.ToInt32(dt.Rows[i]["linhas"]),
                        Colunas = Convert.ToInt32(dt.Rows[i]["colunas"]),
                        Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
                        UserCadastra = Convert.ToInt32(dt.Rows[i]["usercadastra"]),
                        UserAtualiza = Convert.ToInt32(dt.Rows[i]["useratualiza"]),
                        DataCadastra = dataCad,
                        DataAtualiza = dataAtt,
                    });
                }

                cmbLinha.Items.Clear();
                cmbColuna.Items.Clear();

                for (int i = 1; i <= camLayout[0].Linhas; i++)
                {
                    cmbLinha.Items.Add(i);
                }

                for (int i = 1; i <= camLayout[0].Colunas; i++)
                {
                    cmbColuna.Items.Add(i);
                }

                cmbLinha.SelectedValue = camLayout[0].Linhas;
                cmbColuna.SelectedValue = camLayout[0].Colunas;
                cmbPosicao.SelectedValue = 0;
            }
            else
            {
                cmbLinha.Items.Clear();
                cmbColuna.Items.Clear();
                cmbPosicao.Items.Clear();

                for (int i = 1; i <= 10; i++)
                {
                    cmbLinha.Items.Add(i);
                    cmbColuna.Items.Add(i);
                }

                cmbLinha.SelectedIndex = 0;
                cmbColuna.SelectedIndex = 0;
                cmbPosicao.SelectedIndex = 0;
            }
        }

        private bool ValidaCampos()
        {
            if (cmbMonitor.SelectedItem == null)
            {
                NewMessageBox.Infos("Por favor, selecione um monitor.", 2);
                return false;
            }

            if (cmbPosicao.SelectedItem == null)
            {
                NewMessageBox.Infos("Por favor, selecione uma posição.", 2);
                return false;
            }

            if (cmbCameras.SelectedItem == null)
            {
                NewMessageBox.Infos("Por favor, selecione uma câmera.", 2);
                return false;
            }

            if (cmbLinha.SelectedItem == null)
            {
                NewMessageBox.Infos("Por favor, selecione uma linha.", 2);
                return false;
            }

            if (cmbColuna.SelectedItem == null)
            {
                NewMessageBox.Infos("Por favor, selecione uma coluna.", 2);
                return false;
            }

            return true;
        }

        private int VerificaLayout(int monitor)
        {
            string sql = $"SELECT idlayout FROM tbl_cameras_cd_layout WHERE monitor = {monitor}";

            if (InfoGlobal.CasaCam)
            {
                sql = $"SELECT idlayout FROM tbl_cameras_cd_layout WHERE monitor = {monitor} AND casacam = 'true'";
            }

            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["idlayout"]);
            }

            return 0;
        }

        private void PreviewCameras()
        {
            CamerasInfos selecionada = (CamerasInfos)cmbCameras.SelectedItem;

            if (selecionada != null)
            {
                if (selecionada.IdMarca == 1)
                {
                    PictureBox pictureBox = new PictureBox();
                    previewBox.Child = pictureBox;
                    selecionada.Componente = pictureBox.Handle;
                    svHikvision.PreviewCameras(selecionada);
                }
                else
                {
                    // Criar uma instância do controle AxAxisMediaControl
                    AxAxisMediaControl axs = new AxAxisMediaControl();

                    // Adicionar o controle ao WindowsFormsHost
                    previewBox.Child = axs;

                    // Configurar as propriedades do controle
                    axs.MediaURL = $"http://{selecionada.Ip}/channel2";
                    axs.MediaUsername = selecionada.UserCamera;
                    axs.MediaPassword = selecionada.SenhaCamera;
                    axs.EnableReconnect = true;
                    axs.StretchToFit = true;

                    // Iniciar a reprodução do feed de vídeo
                    axs.Play();
                }
            }
        }

        private void Restart()
        {
            Window.GetWindow(this).Close();
            new WpfPosicionaCameras().ShowDialog();
        }
        #endregion

        #endregion

        #region 4 - EVENTOS DE CONTROLE
        private void btnEditLinhasColunas_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Infos($"Tem certeza que deseja alterar a formação de linhas e colunas do {cmbMonitor.Text} ?", 1);

            if (NewMessageBox.Resposta)
            {
                cmbLinha.Items.Clear();
                cmbColuna.Items.Clear();

                cmbLinha.IsEnabled = true;
                cmbColuna.IsEnabled = true;

                // Preencher as comboboxes de linha e coluna com números de 1 a 10
                for (int i = 1; i <= 10; i++)
                {
                    cmbLinha.Items.Add(i);
                    cmbColuna.Items.Add(i);
                }
            }
        }

        private void CmbLinha_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePositionComboBox();
            UpdateGrid();
        }

        private void CmbColuna_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePositionComboBox();
            UpdateGrid();
        }

        private void cmbPosicao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verifica se um item foi selecionado no cmbPosicao
            if (cmbPosicao.SelectedItem != null)
            {
                CamerasInfos camerasSelecionada = (CamerasInfos)cmbCameras.SelectedItem;

                int monitorSelecionado = cmbMonitor.SelectedIndex + 1;
                int posicaoSelecionada = cmbPosicao.SelectedIndex + 1;

                // Verificar se a posição selecionada já está ocupada por outra câmera
                foreach (var item in listaCameras)
                {
                    if (item.Monitor == monitorSelecionado && item.Posicao == posicaoSelecionada && item.Id != camerasSelecionada.Id)
                    {
                        NewMessageBox.Infos("A posição escolhida já está ocupada por outra câmera. Desative a câmera na posição atual antes de adicionar outra câmera nesta posição", 2);
                        return; // Sair do evento se a posição estiver ocupada
                    }
                }

                // Pinta o quadrado correspondente com a cor desejada
                foreach (var item in gridNumbers.Children)
                {
                    if (item is Border border)
                    {
                        int gridPosition = Grid.GetRow(border) * (cmbColuna.SelectedIndex + 1) + Grid.GetColumn(border) + 1;

                        if (gridPosition == posicaoSelecionada)
                        {
                            border.Background = Brushes.LightGray;
                        }
                    }
                }
            }
        }

        private void btnSalvaPosicao_Click(object sender, RoutedEventArgs e)
        {
            if (ValidaCampos())
            {
                int monitorSelecionado = cmbMonitor.SelectedIndex + 1;
                int posicaoSelecionada = cmbPosicao.SelectedIndex + 1;
                int linhaSelecionada = cmbLinha.SelectedIndex + 1;
                int colunaSelecionada = cmbColuna.SelectedIndex + 1;

                CamerasInfos camerasSelecionada = (CamerasInfos)cmbCameras.SelectedItem;

                // Verificar se a posição selecionada já está ocupada por outra câmera
                foreach (var item in listaCameras)
                {
                    if (item.Monitor == monitorSelecionado && item.Posicao == posicaoSelecionada && item.Id != camerasSelecionada.Id)
                    {
                        NewMessageBox.Infos("A posição escolhida já está ocupada por outra câmera. Desative a câmera na posição atual antes de adicionar outra câmera nesta posição", 2);
                        return; // Sair do evento se a posição estiver ocupada
                    }
                }

                string sql = string.Empty;

                if (VerificaLayout(monitorSelecionado) == 0)
                {
                    sql = $"INSERT INTO tbl_cameras_cd_layout (linhas,colunas,monitor,usercadastra,datacadastra) " +
                          $"VALUES ({linhaSelecionada},{colunaSelecionada},{monitorSelecionado},{InfoGlobal.IdUser},'{DateTime.Now}') " +
                          $"RETURNING idlayout";

                    if (InfoGlobal.CasaCam)
                    {
                        sql = $"INSERT INTO tbl_cameras_cd_layout (linhas,colunas,monitor,usercadastra,datacadastra, casacam) " +
                          $"VALUES ({linhaSelecionada},{colunaSelecionada},{monitorSelecionado},{InfoGlobal.IdUser},'{DateTime.Now}','true') " +
                          $"RETURNING idlayout";
                    }
                }
                else
                {
                    sql = $"UPDATE tbl_cameras_cd_layout " +
                          $"SET " +
                          $"linhas = {linhaSelecionada}, " +
                          $"colunas = {colunaSelecionada}, " +
                          $"useratualiza = {InfoGlobal.IdUser}, " +
                          $"dataatualiza = '{DateTime.Now}' " +
                          $"WHERE monitor = {monitorSelecionado} " +
                          $"RETURNING idlayout";

                    if (InfoGlobal.CasaCam)
                    {
                        sql = $"UPDATE tbl_cameras_cd_layout " +
                         $"SET " +
                         $"linhas = {linhaSelecionada}, " +
                         $"colunas = {colunaSelecionada}, " +
                         $"useratualiza = {InfoGlobal.IdUser}, " +
                         $"dataatualiza = '{DateTime.Now}' " +
                         $"WHERE monitor = {monitorSelecionado} " +
                         $"AND casacam = 'true' " +
                         $"RETURNING idlayout";
                    }
                }

                string result = obj.ComandoRetornaValor(sql);

                string sqlCameras = $"UPDATE tbl_cameras_cd " +
                          $"SET " +
                          $"idlayout = {result}, " +
                          $"posicao = {posicaoSelecionada}, " +
                          $"monitor = {monitorSelecionado}, " +
                          $"useratualiza = {InfoGlobal.IdUser}, " +
                          $"dataatualiza = '{DateTime.Now}' " +
                          $"WHERE id = {camerasSelecionada.Id}";

                obj.ComandoSql(sqlCameras);

                NewMessageBox.Infos("Câmera salva com sucesso!", 2);
                InfoGlobal.AlteracaoCam = true;
                Restart();
            }
        }

        private void cmbCameras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaLayout();
            PreviewCameras();
            CarregaLayoutMonitor();

            CamerasInfos cam = (CamerasInfos)cmbCameras.SelectedItem;

            if (cam != null)
            {
                cmbPosicao.SelectedIndex = cam.Posicao - 1;
            }

            UpdateGrid();
        }

        private void cmbMonitor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarregaLayoutMonitor();
        }
        #endregion
    }
}
