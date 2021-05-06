using JrTunes.Data;
using JrTunes.MetodoExtensao;
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

            SelectToEntitiesMain();

        }

        private static void SelectToEntitiesMain()
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
                //SelectToEntitiesConsultaSimples(contexto);

                //SelectToEntitiesConsultaTop10(contexto);

                //SelectToEntitiesContains(contexto);

                //SelectToEntitiesConsultaComJoin_E_DepoisMesmaConsultaSemJoin(contexto);


                //Traz todos os album do Led Zeppelin
                //Select_NomeArista_E_NomeAlbum(contexto, "Led Zeppelin", "");

                //Console.WriteLine("\n=========================================\n");

                //Traz todos os album do Led Zeppelin que contém 'Graffiti' no nome album'
                //Select_NomeArista_E_NomeAlbum(contexto, "Led Zeppelin", "Graffiti");


                //SelectCountLinq(contexto);


                //LinqSumarizando(contexto);

                //LinqGoupBy(contexto);


                //Linq_Max_Min_Avg(contexto);


                /*
                 * Podemos nos deparar com situações onde não existe uma função para o que desejamos
                 * fazer. Um exemplo disso é a Medina, não existe essa função no LINQ. Mas podemos
                 * Criar utilizando métódo de exensão veja o exemplo abaixo:
                 * 
                 */


                /*
                 * Criando método comum e consumindo
                 */

                var query = from nf in contexto.NotaFiscais
                            select nf.Total;

                Console.WriteLine("A Mediana (Método comum) é {0}", Mediana(query));



                /*
                 * Criando método de extensão. Note que conseguimo aplicar lambda no mesmo.
                 */


                //Veja a definição do mesmo na pasta 'MetodoExtensao'
                Console.WriteLine("A Mediana (Método de extensão) é {0}", 
                                        contexto.NotaFiscais.Mediana(nf => nf.Total));

                
            }




        }

        private static Decimal Mediana(IQueryable<decimal> query)
        {

           
            int qtndeElementos = query.Count();

            var aux_elementoCentral1 = query.OrderBy(total => total).Skip(qtndeElementos / 2).First();
            var aux_elementoCentral2 = query.OrderBy(total => total).Skip((qtndeElementos - 1) / 2).First();

            if (qtndeElementos % 2 == 0)
            {
                return (aux_elementoCentral1 + aux_elementoCentral2) / 2;
            }

            
            return aux_elementoCentral1;

        }

        private static void Linq_Max_Min_Avg(JrTunesEntities contexto)
        {
            //MAX
            //MIN
            //AVG



            contexto.Database.Log = Console.WriteLine; // Exibe no console query geradas

            var maiorVenda = contexto.NotaFiscais.Max(nf => nf.Total);
            var menorVenda = contexto.NotaFiscais.Min(nf => nf.Total);
            var vendaMedia = contexto.NotaFiscais.Average(nf => nf.Total);

            Console.WriteLine("A maior venda é {0},", maiorVenda);
            Console.WriteLine("A Menor venda é {0},", menorVenda);
            Console.WriteLine("A venda média é {0},", vendaMedia);


            /*
             * No console podemos observar que o procedimento acima funciona porém ele faz três 
             * requisões ao servidor SQL. Para evitar essas viagens podemos consolidar em uma única
             * querys as funções: Max, Min, e AVG (Média). Veja o exemplo abaixo.
             * 
             */


            var vendas = (from nf in contexto.NotaFiscais
                          group nf by 1 into agrupado
                          select new
                          {
                              maiorVenda = agrupado.Max(nf => nf.Total),
                              menorVenda = agrupado.Min(nf => nf.Total),
                              vendaMedia = agrupado.Average(nf => nf.Total),
                          }).Single();


            Console.WriteLine("A maior venda é {0},", vendas.maiorVenda);
            Console.WriteLine("A Menor venda é {0},", vendas.menorVenda);
            Console.WriteLine("A venda média é {0},", vendas.vendaMedia);


            /*
             * Note que o resultado é o mesmo e só fazemos uma única requisão.
             */
        }

        private static void LinqGoupBy(JrTunesEntities contexto)
        {
            var query = from inf in contexto.ItemNotaFiscal
                        where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                        group inf by inf.Faixa.Album into agrupado
                        orderby agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                            descending
                        select new
                        {
                            TitulodoAlbum = agrupado.Key.Titulo,
                            TotalPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                        };

            //foreach (var agrupado in query)
            //{
            //    Console.WriteLine("{0} \t {1}", agrupado.TitulodoAlbum.PadRight(40),
            //        agrupado.TotalPorAlbum);
            //}


            /* Note na consulta acima que o linq nos permite agrupar 
             * por objeto (Trecho de código: group inf by inf.Faixa.Album) "Album"
             */

            /*Note que consulta acima possui repetição de código, será que não podemos
             * evitar essa repetição??? Sim podemos usar o 'let' veja o exemplo abaixo:
             */

            query = from inf in contexto.ItemNotaFiscal
                    where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                    group inf by inf.Faixa.Album into agrupado
                    /*===>*/
                    let vendasporAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                    orderby vendasporAlbum /*<===*/
                    descending
                    select new
                    {
                        TitulodoAlbum = agrupado.Key.Titulo,
                        TotalPorAlbum = agrupado.Sum(a => a.Quantidade * a.PrecoUnitario)
                    };



            foreach (var agrupado in query)
            {
                Console.WriteLine("{0} \t {1}", agrupado.TitulodoAlbum.PadRight(40),
                    agrupado.TotalPorAlbum);
            }
        }

        private static void LinqSumarizando(JrTunesEntities contexto)
        {
            var query = from inf in contexto.ItemNotaFiscal
                        where inf.Faixa.Album.Artista.Nome == "Led Zeppelin"
                        select new { total = inf.PrecoUnitario * inf.Quantidade };


            Console.WriteLine("Sintaxe query: Total: {0}", query.Sum(q => q.total));

            var Soma = contexto.ItemNotaFiscal.Where(q => q.Faixa.Album.Artista.Nome == "Led Zeppelin")
                             .Sum(q => (q.Quantidade * q.PrecoUnitario));

            Console.WriteLine("Sintaxe método: Total: {0}", query.Sum(q => q.total));
        }

        private static void SelectCountLinq(JrTunesEntities contexto)
        {
            var quantidade = contexto.Faixas.Count(f => f.Album.Artista.Nome == "Led Zeppelin");

            Console.WriteLine("Quantidade: {0}", quantidade);
        }

        private static void Select_NomeArista_E_NomeAlbum(JrTunesEntities contexto, string nomeArtista, string nomeAlbum)
        {

            /*Função:
             * Traz faixa a partir do nome do artista, se o nome do album estiver preenchido filtra o Album
             */

            /*

            var query7 = from f in contexto.Faixas
                     where f.Album.Artista.Nome.Contains(nomeArtista)
                     select f;


            if (!string.IsNullOrEmpty(nomeAlbum))
            {
                query7 = query7.Where(q => q.Album.Titulo.Contains(nomeAlbum));

            }

            query7 = query7.OrderBy(q => q.Nome).ThenBy(q => q.Album.Titulo);


            foreach (var faixa in query7)
            {
                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }

            */


            /*
             *  Podemos tbm criar um consulta  única que já verifique se 'nomeAlbum' foi
             *  informado, e caso tenha traga o artista e album informado.
             *  Veja abaixo uma consulta que contém IF
             */


            var query8 = from f in contexto.Faixas
                         where f.Album.Artista.Nome.Contains(nomeArtista)
                         && (!String.IsNullOrEmpty(nomeAlbum) ? f.Album.Titulo.Contains(nomeAlbum) : true)
                         orderby f.Album.Titulo descending, f.Nome
                         select f;


            foreach (var faixa in query8)
            {
                Console.WriteLine("{0}\t{1}", faixa.Album.Titulo.PadRight(40), faixa.Nome);
            }


        }

        private static void SelectToEntitiesConsultaComJoin_E_DepoisMesmaConsultaSemJoin(JrTunesEntities contexto)
        {
            //Trazendo dados da entidades: 'Artista' e 'Album' (Com join)

            var query5 = from a in contexto.Artistas
                         join alb in contexto.Albums
                         on a.ArtistaId equals alb.ArtistaId
                         where a.Nome.Contains("led")
                         select new
                         {
                             NomeArtista = a.Nome,
                             NomeAlbum = alb.Titulo
                         };

            foreach (var artista in query5)
            {
                Console.WriteLine("{0}\t{1}", artista.NomeArtista, artista.NomeAlbum);
            }

            Console.WriteLine("\n----------------------------\n");


            /* Trazendo dados da entidades: 'Artista' e 'Album' (Sem join) 
             * Também é possível ter o mesmo resultado acima sem utilizar o join. Isso é possível
             * graças a propriedade 'Artista' dentro de Albuns
             */
            var query6 = from alb in contexto.Albums
                         where alb.Artista.Nome.Contains("led")
                         select new
                         {
                             NomeArtista = alb.Artista.Nome,
                             NomeAlbum = alb.Titulo
                         };


            foreach (var artista in query6)
            {
                Console.WriteLine("{0}\t{1}", artista.NomeArtista, artista.NomeAlbum);
            }
        }

        private static void SelectToEntitiesContains(JrTunesEntities contexto)
        {

            //Select com contains (sintaxe query)
            Console.WriteLine("\n----------------------------\n");
            var query3 = from a in contexto.Artistas
                         where a.Nome.Contains("Led")
                         select a;

            foreach (var artista in query3)
            {
                Console.WriteLine("{1} - {0}", artista.ArtistaId, artista.Nome);
            }


            //Realizando a consulta acima de outra forma (sintaxe método)
            Console.WriteLine("\n----------------------------\n");
            var query4 = contexto.Artistas.Where(a => a.Nome.Contains("Led"));

            foreach (var artista in query4)
            {
                Console.WriteLine("{1} - {0}", artista.ArtistaId, artista.Nome);
            }
        }

        private static void SelectToEntitiesConsultaTop10(JrTunesEntities contexto)
        {

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
        }

        private static void SelectToEntitiesConsultaSimples(JrTunesEntities contexto)
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
