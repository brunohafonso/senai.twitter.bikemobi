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
            

            var perfil = new Perfil(1, "Bruno Afonso", DateTime.Parse("25/04/1995"), "SP", "São Paulo", "sou ciclista com orgulho", "www.google.com.br");

            var rotaPesquisada = new RotaPesquisada(1, 3, "10 mins", "R. Bom Pastor - Ipiranga, São Paulo - SP, Brazil", -23.5901388, -46.607101, "R. do Capitarizinho - Vila Liviero, São Paulo - SP, Brazil", -23.6479125, -46.596678, "lvioCf|k{GVw@");
            login.Perfil = perfil;
            
            _context.Logins.Add(login);
            _context.Perfis.Add(perfil);
            _context.RotasPesquisadas.Add(rotaPesquisada);
            _context.SaveChanges();
        }
    }
}