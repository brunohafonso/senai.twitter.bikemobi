using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class RequisicaoAlterarSenha : Base
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int IdLogin { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Expiracao { get; set; }

        public RequisicaoAlterarSenha()
        {
            
        }
        public RequisicaoAlterarSenha(int IdLogin, string Email)
        {
            this.IdLogin = IdLogin;
            this.Email = Email;
            this.Expiracao = DateTime.Now.Add(new TimeSpan(0,0,30,0));
            this.Status = "Ativo";
            this.CriadoEm = DateTime.Now; 
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}