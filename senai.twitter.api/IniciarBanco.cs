using System;
using System.Linq;
using senai.twitter.domain.Entities;
using senai.twitter.repository.Context;

namespace senai.twitter.api
{
    public class IniciarBanco
    {
        public static void Inicializar(BikeMobiContext _context)
        {
            _context.Database.EnsureCreated();
            if(_context.Logins.Any()) {
                return;
            }

            var login = new Login("brunohafonso", "brunohafonso@gmail.com", "bbc259521");
            _context.Logins.Add(login);

            var perfil = new Perfil("Bruno Afonso", DateTime.Parse("25/04/1995"), "São Paulo", "sou ciclista com orgulho", "www.google.com.br", DateTime.Now);
            _context.Perfis.Add(perfil);

            _context.SaveChanges();
        }
    }
}