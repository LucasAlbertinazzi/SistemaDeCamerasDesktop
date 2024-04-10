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
using System.Windows.Shapes;

namespace SistCamerasGuarita.Suporte.MessageBox
{
    /// <summary>
    /// Interaction logic for WpfDialogResult.xaml
    /// </summary>
    public partial class WpfDialogResult : Window
    {
        private string _texto = string.Empty;
        private string[] _opcoes;
        private int _index;

        public WpfDialogResult()
        {
            InitializeComponent();
        }

        public WpfDialogResult(string pergunta, int index)
        {
            InitializeComponent();
            _texto = pergunta;
            _index = index;
        }

        public WpfDialogResult(string pergunta, string[] opcao, int index)
        {
            InitializeComponent();
            _texto = pergunta;
            _index = index;
            _opcoes = opcao;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_index == 1)
            {
                grid1ao3.Visibility = Visibility.Visible;
                txbPergunta.Visibility = Visibility.Visible;
                gridPergunta.Visibility = Visibility.Visible;
                txbPergunta.Text = _texto;
            }

            if (_index == 2)
            {
                grid1ao3.Visibility = Visibility.Visible;
                txbPergunta.Visibility = Visibility.Visible;
                gridAviso.Visibility = Visibility.Visible;
                txbPergunta.Text = _texto;
            }

            if (_index == 3)
            {
                grid1ao3.Visibility = Visibility.Visible;
                txbPergunta.Visibility = Visibility.Visible;
                gridOpcao.Visibility = Visibility.Visible;
                txbPergunta.Text = _texto;
                btnUm.Content = _opcoes[0];
                btnDois.Content = _opcoes[1];
            }

            if (_index == 4)
            {
                grid4.Visibility = Visibility.Visible;
                txbTextoObs.Text = _texto;
            }

            if( _index == 5)
            {
                grid1ao3.Visibility = Visibility.Visible;
                txbPergunta.Visibility = Visibility.Visible;
                gridAviso.Visibility = Visibility.Visible;
                txbPergunta.Text = _texto;
                btnOk.IsEnabled = false;
            }
        }

        private void btnSim_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Resposta = true;
            CloseLayout();
        }

        private void btnNao_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Resposta = false;
            CloseLayout();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            CloseLayout();
        }

        private void btnUm_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Opcao = 1;
            CloseLayout();
        }

        private void btnDois_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Opcao = 2;
            CloseLayout();
        }

        public void CloseLayout()
        {
            grid1ao3.Visibility = Visibility.Collapsed;
            grid4.Visibility = Visibility.Collapsed;
            txbPergunta.Visibility = Visibility.Collapsed;
            gridPergunta.Visibility = Visibility.Collapsed;
            gridAviso.Visibility = Visibility.Collapsed;
            gridOpcao.Visibility = Visibility.Collapsed;
            Close();
        }

        private void txbObs_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (txbObs.Text.Length > 0)
            {
                lblMarcaObs.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblMarcaObs.Visibility = Visibility.Visible;
            }
        }

        private void btnObs_Click(object sender, RoutedEventArgs e)
        {
            NewMessageBox.Observacao = txbObs.Text;
            CloseLayout();
        }
    }
}
