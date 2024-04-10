using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Services;
using SistCamerasGuarita.Suporte.MessageBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for WpfAutenticacao.xaml
    /// </summary>
    public partial class WpfAutenticacao : Window
    {
        ServiceUser serviceUser = new ServiceUser();

        public WpfAutenticacao()
        {
            InitializeComponent();
            txbUsuario.Focus();
        }

        private void btnEntrar_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(txbUsuario.Text) && !string.IsNullOrEmpty(txbSenha.Text))
            {
                DefineAmbiente();

                if (!serviceUser.AutenticaUsuario(txbUsuario.Text, txbSenha.Text))
                {
                    NewMessageBox.Infos("Senha ou usuário incorretos!", 2);
                    return;
                }

                this.Close();
            }
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
    }
}
