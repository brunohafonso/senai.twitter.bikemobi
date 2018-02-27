using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Login
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength=3, ErrorMessage="O campo nome de usuario deve ter no minimo 3 caracteres e no maximo 50.")]
        public string NomeUsuario { get; set; }
        
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength=8, ErrorMessage="A senha deve ter no minimo 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public Perfil Perfil { get; set; }
        
        public Login()
        {
            
        }

        public Login(string NomeUsuario, string Email, string Senha)
        {
            this.NomeUsuario = NomeUsuario;
            this.Email = Email;
            this.Senha = Senha;
        }
    }
}