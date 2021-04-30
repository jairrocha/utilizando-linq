using JrTunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace JrTunes
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //SelectSimplesLinq();

            //SelectComJoinLinq();

            //Select simples no xml

            //SelectNoXMLLinq();

            SelectToEntities();

        }

        private static void SelectToEntities()
        {

            /*Preparando o ambiente
             * 
             * 1) Na pasta Data: ADD > New item > Data > Service-based Database
             * 2) No BD criado: Em tables executar o script salvo em data: AluraTunes.sql
             * 3) Na pasta Data: ADD : New item > Data > ADO.NET Entity Data Model >
             * Selecionar 'EF Designer form database' > Next > Selecionar o BD >
             * Next > Next > Selecionar tables, marque como o Checkbox plurarize or....>
             * Finish.
             *
             */


            using (var contexto = new JrTunesEntities())
            {

                /*
                 * Consulta simples
                 */

                var query = from g in contexto.Generos
                            select g;

                foreach (var genero in query)
                {
                    Console.WriteLine("{0} - {1}", genero.GeneroId, genero.Nome);
                }

                Console.WriteLine("\n----------------------------\n");


                /*
                 * A consulta abaixo embora pareça está trazendo para
                 * a memória toda informação e depois filtrando os dez primeiros 
                 * registros, na verdade está apenas trazendo 10 registros para 
                 * aplicação, como podemos visualizar isso???? 
                 * Basta visualizamos o log no console atráves do 
                 * comando: 'contexto.Database.Log = Console.WriteLine;'
                 *                 
                 */

                /*exibe o comando que executado no BD*/
                contexto.Database.Log = Console.WriteLine;


                /* Consulta com Join e limitação de exibição (TOP 10)*/
                var query2 = from g in contexto.Generos
                             join f in contexto.Faixas
                             on g.GeneroId equals f.GeneroId
                             select new { f, g };

                query2 = query2.Take(10);


                foreach (var item in query2)
                {
                    Console.WriteLine("{0} \t {1}", item.f.Nome, item.g.Nome);
                }

                Console.ReadKey();

            }
        }

        private static void SelectNoXMLLinq()
        {
            XElement root = XElement.Load(@"C:\Users\jairm\Downloads\"
                                        + @"utilizando-linq\JrTunes\Data\AluraTunes.xml");

            var query = from g in root
                        .Element("Generos")
                        .Elements("Genero")
                        select g;

            foreach (var genero in query)
            {
                Console.WriteLine($"{genero.Element("GeneroId").Value} - " +
                                  $"{genero.Element("Nome").Value}");
            }


            Console.WriteLine("\n===============================\n");


            //Select com join no xml

            var query2 = from g in root.Element("Generos").Elements("Genero")
                         join m in root.Element("Musicas").Elements("Musica")
                            on g.Element("GeneroId").Value
                            equals m.Element("GeneroId").Value
                         select new
                         {
                             Genero = g.Element("Nome").Value,
                             Musica = m.Element("Nome").Value
                         };

            foreach (var item in query2)
            {
                Console.WriteLine("{0} - {1}", item.Musica, item.Genero);
            }
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
