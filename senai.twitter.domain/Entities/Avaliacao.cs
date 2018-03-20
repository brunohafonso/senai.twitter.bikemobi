using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Avaliacao : Base
    {
        [Required]
        [Range(1, 5, ErrorMessage="O valor deve estar entre 1 e 5.")]
        public int AvTrajeto { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage="O valor deve estar entre 1 e 5.")]
        public int AvSeguranca { get; set; }

        [ForeignKey("IdRotaRealizada")]
        public RotaRealizada RotaRealizada { get; set; }

        public int IdRotaRealizada { get; set; }  

        [ForeignKey("IdLogin")]
        public Login Login { get; set; }

        public int IdLogin { get; set; }  

        public Avaliacao()
        {
            
        }    

        public Avaliacao(int IdRotaRealizada, int IdLogin, int AvTrajeto, int AvSeguranca)
        {
            this.IdRotaRealizada = IdRotaRealizada;
            this.IdLogin = IdLogin;
            this.AvTrajeto = AvTrajeto;
            this.AvSeguranca = AvSeguranca;
            this.CriadoEm = DateTime.Now; 
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}