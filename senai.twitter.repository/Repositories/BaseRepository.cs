using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using senai.twitter.domain.Contracts;
using senai.twitter.repository.Context;

namespace senai.twitter.repository.Repositories {
    public class BaseRepository<T> : IBaseRepository<T> where T : class {
        private readonly BikeMobiContext _context;

        public BaseRepository (BikeMobiContext context) {
            _context = context;
        }
        
        public IEnumerable<T> Listar (string[] includes = null) {
            try {
                var query = _context.Set<T> ().AsQueryable ();
                if (includes == null) return query.ToList ();

                foreach (var item in includes) {
                    query = query.Include (item);
                }

                return query.ToList ();
            } catch (System.Exception ex) {
                throw new Exception ("Erro ao listar dados" + ex.Message);
            }
        }

        public int Atualizar (T dados) {
            try {

                _context.Set<T> ().Update (dados);
                return _context.SaveChanges ();

            } catch (System.Exception ex) {
                throw new Exception (ex.Message);
            }

        }

        public T BuscarPorId (int id, string[] includes = null) {
            try {
                //Para buscar a chave primaria da classe
                var chavePrimaria = _context.Model.FindEntityType (typeof (T)).FindPrimaryKey ().Properties[0];
                return _context.Set<T> ().FirstOrDefault (e => EF.Property<int> (e, chavePrimaria.Name) == id);
            } catch (System.Exception ex) {
                throw new Exception (ex.Message);
            }
        }

        public int Deletar (T dados) {
            try {

                _context.Set<T> ().Remove (dados);
                return _context.SaveChanges ();

            } catch (System.Exception ex) {
                throw new Exception (ex.Message);
            }

        }

        public int Inserir (T dados) {
            try {

                _context.Set<T> ().Add (dados);
                return _context.SaveChanges ();

            } catch (System.Exception ex) {
                throw new Exception (ex.Message);
            }
        }
    }
}