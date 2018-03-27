using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class RotaRealizada : Base
    {
        [Required]
        public double LatInicio { get; set; }
        
        [Required]
        public double LngInicio { get; set; }

        [Required]
        public double LatFim { get; set; }

        [Required]
        public double LngFim { get; set; }

        [Required]
        public string DuracaoString { get; set; }

        [Required]
        public int DuracaoInt { get; set; }

        [Required]
        public int Kilometros { get; set; }

        [ForeignKey("IdLogin")]
        public Login Login { get; set; }

        public int IdLogin { get; set; }

        
        [ForeignKey("IdRotaPesquisada")]
        public RotaPesquisada RotaPesquisada { get; set; }
        public int IdRotaPesquisada { get; set; }
        public Avaliacao Avaliacao { get; set; }
        public RotaRealizada()
        {
            
        }

        public RotaRealizada(int IdLogin, int IdRotaPesquisada, double LatInicio, double LngInicio, double LatFim, double LngFim, string DuracaoString, int DuracaoInt, int Kilometros)
        {
            this.IdLogin = IdLogin;
            this.IdRotaPesquisada = IdRotaPesquisada;
            this.LatInicio = LatInicio;
            this.LngInicio = LngInicio;
            this.LatInicio = LatInicio;
            this.LatFim = LatFim;
            this.LngFim = LngFim;
            this.DuracaoString = DuracaoString;
            this.DuracaoInt = DuracaoInt;
            this.Kilometros = Kilometros;
            this.CriadoEm = DateTime.Now; 
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}