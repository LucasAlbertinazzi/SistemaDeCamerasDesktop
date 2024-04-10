using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Services;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace SistCamerasGuarita.Interface
{
    public partial class WpfAddCameras : Window
    {
        #region 1 - VARIÁVEIS

        private DAL obj = new DAL();
        private ServiceHikvision serviceHikvision = new ServiceHikvision();
        private CamerasInfos camerasInfos = new CamerasInfos();

        #endregion 1 - VARIÁVEIS

        #region 2 - CONSTRUTORES

        public WpfAddCameras()
        {
            InitializeComponent();
            txbNomeCamera.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaMarcas();
        }

        #endregion 2 - CONSTRUTORES

        #region 3 - MÉTODOS

        private void LimpaCampos()
        {
            camerasInfos = new CamerasInfos();

            txbNomeCamera.Text = string.Empty;
            txbIp.Text = string.Empty;
            txbPorta.Text = string.Empty;
            txbUser.Text = string.Empty;
            txbSenha.Text = string.Empty;
        }

        private bool VerificaCampos()
        {
            CamerasMarcas cameraMarca = (CamerasMarcas)cmbMarcaCameras.SelectedItem;
            string marca = cameraMarca?.Marca;
            string camera = txbNomeCamera.Text;
            string ip = txbIp.Text;
            string porta = txbPorta.Text;
            string usuario = txbUser.Text;
            string senha = txbSenha.Text;

            if (string.IsNullOrEmpty(marca))
            {
                NewMessageBox.Infos("Campo Marca não preenchido.", 2);
                return false;
            }
            else if (string.IsNullOrEmpty(camera))
            {
                NewMessageBox.Infos("Campo Câmera não preenchido.", 2);
                return false;
            }
            else if (string.IsNullOrEmpty(ip))
            {
                NewMessageBox.Infos("Campo IP não preenchido.", 2);
                return false;
            }
            else if (string.IsNullOrEmpty(porta))
            {
                NewMessageBox.Infos("Campo Porta não preenchido.", 2);
                return false;
            }
            else if (string.IsNullOrEmpty(usuario))
            {
                NewMessageBox.Infos("Campo Usuário não preenchido.", 2);
                return false;
            }
            else if (string.IsNullOrEmpty(senha))
            {
                NewMessageBox.Infos("Campo Senha não preenchido.", 2);
                return false;
            }

            camerasInfos = new CamerasInfos();
            camerasInfos.IdMarca = cameraMarca.IdMarca;
            camerasInfos.Camera = camera;
            camerasInfos.Ip = ip;
            camerasInfos.Porta = Convert.ToInt32(porta);
            camerasInfos.UserCamera = usuario;
            camerasInfos.SenhaCamera = senha;
            camerasInfos.UserCadastra = InfoGlobal.IdUser;
            camerasInfos.DataCadastra = DateTime.Now;
            camerasInfos.Ativo = true;

            return true;
        }

        private void SalvaCameras()
        {
            camerasInfos.CasaCam = InfoGlobal.CasaCam;

            string sql = $"INSERT INTO tbl_cameras_cd " +
                            $"(idmarca, componente, camera, ip, porta, usercamera, senhacamera, usercadastra, ativo, casacam, datacadastra) " +
                            $"VALUES (" +
                            $"{camerasInfos.IdMarca}," +
                            $"'{camerasInfos.Componente}'," +
                            $"'{camerasInfos.Camera}'," +
                            $"'{camerasInfos.Ip}'," +
                            $"'{camerasInfos.Porta}'," +
                            $"'{camerasInfos.UserCamera}'," +
                            $"'{camerasInfos.SenhaCamera}'," +
                            $"{camerasInfos.UserCadastra}," +
                            $"'{camerasInfos.Ativo}'," +
                            $"'{camerasInfos.CasaCam}'," +
                            $"'{camerasInfos.DataCadastra}')";

            if (obj.ComandoSql(sql) == 1)
            {
                NewMessageBox.Infos("Câmera cadastrada com sucesso!", 2);
                LimpaCampos();
            }
            else
            {
                NewMessageBox.Infos("Erro ao cadastrar câmera!", 2);
            }
        }

        private void CarregaMarcas()
        {
            string sql = $"SELECT * FROM tbl_cameras_cd_marcas";
            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<CamerasMarcas> marcas = new List<CamerasMarcas>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    marcas.Add(new CamerasMarcas
                    {
                        IdMarca = Convert.ToInt32(dt.Rows[i]["idmarca"]),
                        Marca = dt.Rows[i]["marca"].ToString()
                    });
                }

                cmbMarcaCameras.ItemsSource = marcas;
            }
        }

        #endregion 3 - MÉTODOS

        #region 4 - EVENTOS DE CONTROLE

        private void btnTesteConexao_Click(object sender, RoutedEventArgs e)
        {
            if (VerificaCampos())
            {
                if (camerasInfos.Id == 1)
                {
                    if (serviceHikvision.HikvisionAutenticacao(camerasInfos.Ip, camerasInfos.Porta, camerasInfos.UserCamera, camerasInfos.SenhaCamera))
                    {
                        NewMessageBox.Infos("Conexão bem-sucedida!", 2);
                    }
                    else
                    {
                        NewMessageBox.Infos("Sem conexão com a câmera!", 2);
                    }
                }
            }
        }

        private void btnAddCamera_Click(object sender, RoutedEventArgs e)
        {
            if (VerificaCampos())
            {
                if (camerasInfos.Id == 1)
                {
                    camerasInfos.IdMarca = 1;

                    if (serviceHikvision.HikvisionAutenticacao(camerasInfos.Ip, camerasInfos.Porta, camerasInfos.UserCamera, camerasInfos.SenhaCamera))
                    {
                        SalvaCameras();
                    }
                    else
                    {
                        NewMessageBox.Infos("Sem conexão com a câmera. Deseja adicioná-la mesmo assim?", 1);

                        if (NewMessageBox.Resposta)
                        {
                            SalvaCameras();
                        }
                    }
                }
                else
                {
                    camerasInfos.IdMarca = 2;

                    SalvaCameras();
                }
            }
        }

        private void cmbMarcaCameras_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CamerasMarcas selecionado = (CamerasMarcas)cmbMarcaCameras.SelectedItem;

            if (selecionado != null)
            {
                if (selecionado.IdMarca == 1)
                {
                    btnTesteConexao.Visibility = Visibility.Visible;
                }
                else
                {
                    btnTesteConexao.Visibility = Visibility.Collapsed;
                }
            }
        }


        #endregion 4 - EVENTOS DE CONTROLE
    }
}