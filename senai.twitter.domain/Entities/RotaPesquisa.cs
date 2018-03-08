using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class RotaPesquisa : Base
    {
        [Required]
        public int Distancia { get; set; }

        [Required]
        public string Duracao { get; set; }

        [Required]
        public string DestinoEnd { get; set; }

        
        [Required]
        public double DestinoLat { get; set; }

        [Required]
        public double DestinoLng { get; set; }
        
        [Required]
        public string OrigemEnd { get; set; }


        [Required]
        public double OrigemLat { get; set; }

        [Required]
        public double OrigemLng { get; set; }

        [Required]
        public string PolylinePoints { get; set; }

        [ForeignKey("IdLogin")]
        
        public Login Login { get; set; }
        
        public int IdLogin { get; set; }

        public RotaPesquisa()
        {
            
        }

        public RotaPesquisa(int IdLogin, int Distancia, string Duracao, string DestinoEnd, double DestinoLat, double DestinoLng, string OrigemEnd, double OrigemLat, double OrigemLng, string PolylinePoints)
        {
            this.IdLogin = IdLogin;
            this.Distancia = Distancia;
            this.Duracao = Duracao;
            this.DestinoEnd = DestinoEnd;
            this.DestinoLat = DestinoLat;
            this.DestinoLng = DestinoLat;
            this.OrigemEnd = OrigemEnd;
            this.OrigemLat = OrigemLat;
            this.OrigemLng = OrigemLng;
            this.PolylinePoints = PolylinePoints;
            this.CriadoEm = DateTime.Now; 
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}