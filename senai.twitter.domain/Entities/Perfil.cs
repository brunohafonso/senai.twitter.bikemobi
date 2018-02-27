using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Perfil
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PerfilId { get; set; }

        [Required]
        [StringLength(50, MinimumLength=4, ErrorMessage="O campo nome deve ter no minimo 4 caracteres e no maximo 50.")]
        public string Nome { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength=3, ErrorMessage="O campo nome deve ter no minimo 3 caracteres e no maximo 50.")]
        public string Localidade { get; set; }
        
        [StringLength(200)]
        public string Bio { get; set; }

        [StringLength(200)]
        public string AvatarUrl { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DataCriacao { get; set; } 
        
        
        public Login Login { get; set; }
        
        public Perfil()
        {
            
        }

        public Perfil(string Nome, DateTime DataNascimento, string Localidade, string Bio, string AvatarUrl, DateTime DataCriacao)
        {
          this.Nome = Nome;
          this.DataNascimento = DataNascimento;
          this.Localidade = Localidade;
          this.Bio = Bio;
          this.AvatarUrl = AvatarUrl;
          this.DataCriacao = DataCriacao; 
        }
    }
}