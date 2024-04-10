using System;

namespace SistCamerasGuarita.Classes
{
    public class CamerasLayout
    {
        public int IdLayout { get; set; }
        public int Linhas { get; set; }
        public int Colunas { get; set; }
        public int Monitor { get; set; }
        public int UserCadastra { get; set; }
        public int UserAtualiza { get; set; }
        public DateTime? DataCadastra { get; set; }
        public DateTime? DataAtualiza { get; set; }
    }
}