﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using senai.twitter.repository.Context;
using System;

namespace senai.twitter.repository.Migrations
{
    [DbContext(typeof(BikeMobiContext))]
    [Migration("20180227035330_BancoInic")]
    partial class BancoInic
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("senai.twitter.domain.Entities.Login", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("NomeUsuario")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PerfilId");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("PerfilId")
                        .IsUnique();

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("senai.twitter.domain.Entities.Perfil", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarUrl")
                        .HasMaxLength(200);

                    b.Property<string>("Bio")
                        .HasMaxLength(200);

                    b.Property<DateTime>("DataCriacao");

                    b.Property<DateTime>("DataNascimento");

                    b.Property<string>("Localidade")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("LoginId");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Perfis");
                });

            modelBuilder.Entity("senai.twitter.domain.Entities.Login", b =>
                {
                    b.HasOne("senai.twitter.domain.Entities.Perfil", "Perfil")
                        .WithOne("Login")
                        .HasForeignKey("senai.twitter.domain.Entities.Login", "PerfilId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
