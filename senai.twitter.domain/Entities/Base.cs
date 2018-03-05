using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace senai.twitter.domain.Entities
{
    public class Base
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CriadoEm { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AtualizadoEm { get; set; }

        public int QtdAtualizacoes { get; set; }

        public string AtualizadoPor { get; set; }    
    }
}