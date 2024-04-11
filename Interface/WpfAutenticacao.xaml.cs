using SistCamerasGuarita.Services;
using SistCamerasGuarita.Suporte.MessageBox;
using System.Windows;

namespace SistCamerasGuarita.Interface
{
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
            if (!string.IsNullOrEmpty(txbUsuario.Text) && !string.IsNullOrEmpty(txbSenha.Text))
            {
                if (!serviceUser.AutenticaUsuario(txbUsuario.Text, txbSenha.Text))
                {
                    NewMessageBox.Infos("Senha ou usuário incorretos!", 2);
                    return;
                }

                this.Close();
            }
        }
    }
}
