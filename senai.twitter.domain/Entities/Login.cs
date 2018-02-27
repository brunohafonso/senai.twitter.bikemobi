using System;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Localidade { get; set; }
        public string Bio { get; set; }
        public string AvatarUrl { get; set; }
        
        public Perfil Perfil { get; set; }
        
        public Login()
        {
            
        }

        public Login(string Nome, DateTime DataNascimento, string Localidade, string Bio, string AvatarUrl)
        {
          this.Nome = Nome;
          this.DataNascimento = DataNascimento;
          this.Localidade = Localidade;
          this.Bio = Bio;
          this.AvatarUrl = AvatarUrl;  
        }
    }
}