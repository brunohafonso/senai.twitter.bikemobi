using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Login : Base
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo nome de usuario deve ter no minimo 3 caracteres e no maximo 50.")]
        //[Index("NomeUsuario", IsUnique = true)]
        public string NomeUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Favor insira um email valido.")]
        //[Index("Email", IsUnique = true)]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve ter no minimo 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public Perfil Perfil { get; set; }

        public ICollection<RotaPesquisa> RotasPesquisas { get; set; }
        public Login()
        {

        }

        public Login(string NomeUsuario, string Email, string Senha)
        {
            this.NomeUsuario = NomeUsuario;
            this.Email = Email;
            this.Senha = Senha;
            this.CriadoEm = DateTime.Now;
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }
    }
}