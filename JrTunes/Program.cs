using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JrTunes
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //SelectSimplesLinq();

            SelectComJoinLinq();

        }

        private static void SelectComJoinLinq()
        {
            var musicas = new List<Musica>
            {
                new Musica(){Id=1, Nome="American idiot", GeneroId = 1},
                new Musica(){Id=2, Nome="Welcome to jungle", GeneroId = 1},
                new Musica(){Id=3, Nome="American jesus", GeneroId = 4}
            };

            var generos = new List<Genero>
            {
                new Genero{Id=1, Nome="Rock"},
                new Genero{Id=2, Nome="Reggae"},
                new Genero{Id=3, Nome="Rock Progressivo"},
                new Genero{Id=4, Nome="Punk Rock"},
                new Genero{Id=5, Nome="Clássica"}
            };

            var query = from m in musicas
                        join g in generos
                        on m.GeneroId equals g.Id
                        select new { m, g }; // Tipo anônimo -> new{}

            foreach (var musica in query)
            {
                Console.WriteLine($"{musica.m.Id} - {musica.m.Nome} - {musica.g.Nome}");
            }
        }

        private static void SelectSimplesLinq()
        {
            var generos = new List<Genero>
            {
                new Genero{Id=1, Nome="Rock"},
                new Genero{Id=2, Nome="Reggae"},
                new Genero{Id=3, Nome="Rock Progressivo"},
                new Genero{Id=4, Nome="Punk Rock"},
                new Genero{Id=5, Nome="Clássica"}
            };

            //Utilizando o LINQ
            var query = from g in generos
                        where g.Nome.Contains("Rock")
                        select g;

            //Impriminso retorno
            foreach (var genero in query)
            {
                Console.WriteLine($"{genero.Id} - {genero.Nome}");
            }
        }
    }
}
