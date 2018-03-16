using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace senai.twitter.domain.Entities
{
    public class Login : Base
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O campo nome de usuario deve ter no minimo 3 caracteres e no maximo 50.")]
        public string NomeUsuario { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Favor insira um email valido.")]
        public string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "A senha deve ter no minimo 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        public Perfil Perfil { get; set; }

        public ICollection<RotaPesquisada> RotasPesquisadas { get; set; }

        public ICollection<RotaRealizada> RotasRealizadas { get; set; }
        
        public Login()
        {

        }

        public Login(string NomeUsuario, string Email, string Senha)
        {
            this.NomeUsuario = NomeUsuario;
            this.Email = Email;
            this.Senha = EncriptarSenha(Senha);
            this.CriadoEm = DateTime.Now;
            this.QtdAtualizacoes = 0;
            this.AtualizadoPor = null;
        }

        public static string EncriptarSenha(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}