using System.Linq;
using Microsoft.EntityFrameworkCore;
using senai.twitter.domain.Entities;

namespace senai.twitter.repository.Context {
    public class BikeMobiContext : DbContext {
        public BikeMobiContext (DbContextOptions<BikeMobiContext> options) : base (options) { }

        public DbSet<Login> Logins { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<RotaPesquisada> RotasPesquisadas { get; set; }
        public DbSet<RotaRealizada> RotasRealizadas { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<RequisicaoAlterarSenha> RequisicoesAlteracaoSenha { get; set; }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.Entity<Login> ().ToTable ("Logins");
            modelBuilder.Entity<Perfil> ().ToTable ("Perfis");
            modelBuilder.Entity<RotaPesquisada> ().ToTable ("RotasPesquisadas");
            modelBuilder.Entity<RotaRealizada>().ToTable("RotasRealizadas");
            modelBuilder.Entity<Avaliacao>().ToTable("Avaliacoes");
            modelBuilder.Entity<RequisicaoAlterarSenha>().ToTable("RequisicoesAlteracaoSenha");

            modelBuilder
                .Entity<Login> ()
                .HasIndex (u => u.Email)
                .IsUnique ();

            modelBuilder
                .Entity<Login> ()
                .HasIndex (u => u.NomeUsuario)
                .IsUnique ();

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating (modelBuilder);
        }
    }
}