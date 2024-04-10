using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace SistCamerasGuarita.Interface
{
    /// <summary>
    /// Interaction logic for WpfAddModelos.xaml
    /// </summary>
    public partial class WpfAddMarcas : Window
    {
        #region 1 - VARIÁVEIS
        DAL obj = new DAL();
        #endregion

        #region 2 - CONSTRUTORES
        public WpfAddMarcas()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregaMarcas();
        }
        #endregion

        #region 3 - MÉTODOS
        private void CarregaMarcas()
        {
            List<CamerasMarcas> marcas = new List<CamerasMarcas>();

            string sql = $"SELECT * FROM tbl_cameras_cd_marcas";
            DataTable dt = obj.RetornaTabela(sql);

            if(dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    marcas.Add(new CamerasMarcas
                    {
                        IdMarca = Convert.ToInt32(dt.Rows[i]["idmarca"]),
                        Marca = dt.Rows[i]["marca"].ToString()
                    });
                }

                dgMarcas.ItemsSource = marcas;
            }
        }

        private void SalvaMarcas()
        {
            if (!string.IsNullOrEmpty(txbMarcaCamera.Text))
            {
                string sql = $"INSERT INTO tbl_cameras_cd_marcas (marca) VALUES ('{txbMarcaCamera.Text.ToUpper()}')";
                obj.ComandoSql(sql);

                NewMessageBox.Infos("Marca cadastrada com sucesso!",2);

                txbMarcaCamera.Text = string.Empty;
                txbMarcaCamera.Focus();
                CarregaMarcas();
            }
        }
        #endregion

        #region 4 - EVENTOS DE CONTROLE
        private void btnAddMarca_Click(object sender, RoutedEventArgs e)
        {
            SalvaMarcas();
        }
        #endregion
    }
}
