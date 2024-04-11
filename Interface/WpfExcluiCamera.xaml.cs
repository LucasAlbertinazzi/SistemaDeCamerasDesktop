using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistCamerasGuarita.Interface
{
    public partial class WpfExcluiCamera : Window
    {
        #region 1 - VARIÁVEIS
        DAL obj = new DAL();
        #endregion

        #region 2 - CONSTRUTORES
        public WpfExcluiCamera()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaCameras();
        }
        #endregion

        #region 3 - MÉTODOS
        private void CarregaCameras()
        {
            string sql = $"SELECT c.*, m.marca, " +
                         $"CASE WHEN c.ativo = 'true' THEN 'Ativo' " +
                         $"ELSE 'Desativado' " +
                         $"END AS status " +
                         $"FROM tbl_cameras_cd AS c " +
                         $"LEFT JOIN tbl_cameras_cd_marcas AS m ON c.idmarca = m.idmarca " +
                         $"WHERE c.userexclui = 0 " +
                         $"AND casacam = '{InfoGlobal.CasaCam}'";

            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<CamerasInfos> cameras = new List<CamerasInfos>();

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
                        Marca = dt.Rows[i]["marca"].ToString(),
                        Ip = dt.Rows[i]["ip"].ToString(),
                        Porta = Convert.ToInt32(dt.Rows[i]["porta"]),
                        Status = dt.Rows[i]["status"].ToString(),
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

                dgCameras.ItemsSource = cameras.OrderBy(x => x.Status).OrderBy(x => x.Marca);
            }
        }

        private void AtivarDesativar()
        {
            CamerasInfos selecionado = (CamerasInfos)dgCameras.SelectedItem;

            if (selecionado != null)
            {
                string sql = string.Empty;

                if (selecionado.Ativo)
                {
                    sql = $"UPDATE tbl_cameras_cd SET " +
                          $"ativo = 'false', " +
                          $"useratualiza = {InfoGlobal.IdUser}, " +
                          $"dataatualiza = '{DateTime.Now}' " +
                          $"WHERE id = {selecionado.Id}";

                    selecionado.Ativo = false;
                    selecionado.Status = "Desativado";
                }
                else
                {
                    sql = $"UPDATE tbl_cameras_cd SET " +
                          $"ativo = 'true', " +
                          $"useratualiza = {InfoGlobal.IdUser}, " +
                          $"dataatualiza = '{DateTime.Now}' " +
                          $"WHERE id = {selecionado.Id}";

                    selecionado.Ativo = true;
                    selecionado.Status = "Ativo";
                }

                obj.ComandoSql(sql);
            }
        }

        private void ExcluirCamera()
        {
            CamerasInfos selecionado = (CamerasInfos)dgCameras.SelectedItem;

            if (selecionado != null)
            {
                NewMessageBox.Infos($"Tem certeza que deseja excluir a câmera: {selecionado.Camera} ?", 1);

                if (NewMessageBox.Resposta)
                {
                    string sql = $"UPDATE tbl_cameras_cd SET " +
                                 $"userexclui = {InfoGlobal.IdUser}, " +
                                 $"ativo = 'false', " +
                                 $"userexclui = {InfoGlobal.IdUser}, " +
                                 $"dataexclui = '{DateTime.Now}' " +
                                 $"WHERE id = {selecionado.Id}";

                    obj.ComandoSql(sql);

                    NewMessageBox.Infos($"Câmera - {selecionado.Camera} excluida com sucesso!", 2);

                    CarregaCameras();
                }
            }
        }

        private void Pesquisar()
        {
            string filtro = txbPesquisaCamera.Text.ToLower();

            // Verificar se o filtro está vazio
            if (string.IsNullOrEmpty(filtro))
            {
                // Restaurar os valores originais do dgCameras
                CarregaCameras();
            }
            else
            {
                // Obtenha o IEnumerable atual do ItemsSource
                var camerasEnumerable = dgCameras.ItemsSource as IEnumerable<CamerasInfos>;

                // Se o IEnumerable não for nulo, filtre os resultados
                if (camerasEnumerable != null)
                {
                    // Converta o IEnumerable em uma lista e aplique o filtro
                    var camerasFiltradas = camerasEnumerable
                                            .Where(c => c.Marca.ToLower().Contains(filtro)
                                                     || c.Camera.ToLower().Contains(filtro)
                                                     || c.Ip.ToLower().Contains(filtro))
                                            .ToList();

                    // Atualize o ItemsSource com os resultados filtrados
                    dgCameras.ItemsSource = camerasFiltradas;
                }
            }
        }


        #endregion

        #region 4 - EVENTOS DE CONTROLE
        private void txbPesquisaCamera_TextChanged(object sender, TextChangedEventArgs e)
        {
            Pesquisar();
        }

        private void dgCameras_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AtivarDesativar();
            InfoGlobal.AlteracaoCam = true;
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            ExcluirCamera();
            InfoGlobal.AlteracaoCam = true;
        }
        #endregion
    }
}
