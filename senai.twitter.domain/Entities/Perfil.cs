using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Perfil : Base
    {
        [Required]
        [StringLength(50, MinimumLength=4, ErrorMessage="O campo nome deve ter no minimo 4 caracteres e no maximo 50.")]
        public string Nome { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DataNascimento { get; set; }

        [Required]
        [StringLength(50, MinimumLength=2, ErrorMessage="O campo nome deve ter no minimo 2 caracteres e no maximo 50.")]
        public string Estado { get; set; }

        [Required]
        [StringLength(50, MinimumLength=2, ErrorMessage="O campo nome deve ter no minimo 2 caracteres e no maximo 50.")]
        public string Cidade { get; set; }
        
        [StringLength(200)]
        public string Bio { get; set; }

        [StringLength(200)]
        public string AvatarUrl { get; set; }
        
        [ForeignKey("IdLogin")]
        public Login Login { get; set; }

        public int IdLogin { get; set; }
        
        public Perfil()
        {
            
        }

        public Perfil(string Nome, DateTime DataNascimento, string Estado, string Cidade, string Bio, string AvatarUrl)
        {
            this.Nome = Nome;
            this.DataNascimento = DataNascimento;
            this.Estado = Estado;
            this.Cidade = Cidade;
            this.Bio = Bio;
            this.AvatarUrl = AvatarUrl;
            this.CriadoEm = DateTime.Now; 
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}