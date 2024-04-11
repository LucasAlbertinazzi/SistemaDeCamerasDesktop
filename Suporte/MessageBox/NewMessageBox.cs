using System.Windows;

namespace SistCamerasGuarita.Suporte.MessageBox
{
    public static class NewMessageBox
    {
        private static Window messageWindow;
        // Resposta: Indica a ação:
        // Botão SIM: true
        // Botão NÃO: false
        public static bool Resposta { get; set; }

        // Retorna o cmapo observação
        public static string Observacao { get; set; }

        // Opcao: Indica a ação:
        // Escolha de opcao: 1,2,3 ....
        public static int Opcao { get; set; }

        // Tipo:
        // Tipo 1: - Pergunta
        // Tipo 2: - Aviso
        // Tipo 3: - Opção
        // Tipo 4: - Observação
        // Tipo 5: - Aviso Enable

        // Texto: Recebe o texto a ser exibido.
        public static void Infos(string Texto, int Tipo)
        {
            messageWindow = new WpfDialogResult(Texto, Tipo);
            messageWindow.ShowDialog();
        }

        // Texto: Recebe o texto a ser exibido.
        // Opção: Recebe as opções.
        public static void Opcoes(string Texto, string[] Opcao, int Tipo)
        {
            messageWindow = new WpfDialogResult(Texto, Opcao, Tipo);
            messageWindow.ShowDialog();
        }

        public static void CloseLayout()
        {
            if (messageWindow != null)
            {
                messageWindow.Close();
            }
        }
    }
}
