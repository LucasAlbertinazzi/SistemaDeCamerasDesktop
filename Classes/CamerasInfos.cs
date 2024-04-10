using System;
using System.ComponentModel;

namespace SistCamerasGuarita.Classes
{
    public class CamerasInfos : INotifyPropertyChanged
    {
        private int id;
        private int idLayout;
        private int idMarca;
        private int posicao;
        private int monitor;
        private IntPtr componente;
        private string camera;
        private string cameraCombo;
        private string marca;
        private string ip;
        private int porta;
        private string status;
        private string userCamera;
        private string senhaCamera;
        private int userCadastra;
        private int userExclui;
        private int userAtualiza;
        private bool casaCam;
        private bool ativo;
        private DateTime dataCadastra;
        private DateTime? dataExclui;
        private DateTime? dataAtualiza;
        private double widthCam;
        private double heightCam;

        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(nameof(Id)); }
        }

        public int IdLayout
        {
            get { return idLayout; }
            set { idLayout = value; OnPropertyChanged(nameof(IdLayout)); }
        }

        public int IdMarca
        {
            get { return idMarca; }
            set { idMarca = value; OnPropertyChanged(nameof(IdMarca)); }
        }

        public int Posicao
        {
            get { return posicao; }
            set { posicao = value; OnPropertyChanged(nameof(Posicao)); }
        }

        public int Monitor
        {
            get { return monitor; }
            set { monitor = value; OnPropertyChanged(nameof(Monitor)); }
        }

        public IntPtr Componente
        {
            get { return componente; }
            set { componente = value; OnPropertyChanged(nameof(Componente)); }
        }

        public string CameraCombo
        {
            get { return cameraCombo; }
            set { cameraCombo = value; OnPropertyChanged(nameof(CameraCombo)); }
        }

        public string Camera
        {
            get { return camera; }
            set { camera = value; OnPropertyChanged(nameof(Camera)); }
        }

        public string Marca
        {
            get { return marca; }
            set { marca = value; OnPropertyChanged(nameof(Marca)); }
        }

        public string Ip
        {
            get { return ip; }
            set { ip = value; OnPropertyChanged(nameof(Ip)); }
        }

        public int Porta
        {
            get { return porta; }
            set { porta = value; OnPropertyChanged(nameof(Porta)); }
        }

        public string UserCamera
        {
            get { return userCamera; }
            set { userCamera = value; OnPropertyChanged(nameof(UserCamera)); }
        }

        public string SenhaCamera
        {
            get { return senhaCamera; }
            set { senhaCamera = value; OnPropertyChanged(nameof(SenhaCamera)); }
        }

        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public int UserCadastra
        {
            get { return userCadastra; }
            set { userCadastra = value; OnPropertyChanged(nameof(UserCadastra)); }
        }

        public int UserExclui
        {
            get { return userExclui; }
            set { userExclui = value; OnPropertyChanged(nameof(UserExclui)); }
        }

        public int UserAtualiza
        {
            get { return userAtualiza; }
            set { userAtualiza = value; OnPropertyChanged(nameof(UserAtualiza)); }
        }

        public bool Ativo
        {
            get { return ativo; }
            set { ativo = value; OnPropertyChanged(nameof(Ativo)); }
        }

        public bool CasaCam
        {
            get { return casaCam; }
            set { casaCam = value; OnPropertyChanged(nameof(CasaCam)); }
        }

        public DateTime DataCadastra
        {
            get { return dataCadastra; }
            set { dataCadastra = value; OnPropertyChanged(nameof(DataCadastra)); }
        }

        public DateTime? DataExclui
        {
            get { return dataExclui; }
            set { dataExclui = value; OnPropertyChanged(nameof(DataExclui)); }
        }

        public DateTime? DataAtualiza
        {
            get { return dataAtualiza; }
            set { dataAtualiza = value; OnPropertyChanged(nameof(DataAtualiza)); }
        }

        public double WidthCam
        {
            get { return widthCam; }
            set { widthCam = value; OnPropertyChanged(nameof(WidthCam)); }
        }

        public double HeightCam
        {
            get { return heightCam; }
            set { heightCam = value; OnPropertyChanged(nameof(HeightCam)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}