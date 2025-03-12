using System.IO.Pipelines;
using System.Text.RegularExpressions;
using desafioMedicos.Models;
using NPOI.SS.UserModel;

namespace DesafioMedios;

class Program
{
    private static string filePath = Path.Combine(Environment.CurrentDirectory, "DesafioMedicos.xlsx");

    private static List<Consulta> consultas = [];

    private static void DataImport()
    {
        try
        {
            //* Excel workbook abstraction
            IWorkbook wd = WorkbookFactory.Create(filePath);

            //ISheet sheet = wd.GetSheetAt(0);        //? Get sheet by index 
            ISheet sheet = wd.GetSheet("medicos");    //? Get sheet by name

            for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
            {
                IRow row = sheet.GetRow(i);

                DateTime dataConsulta = DateTime.Parse(row.GetCell(0).StringCellValue);
                string horaConsulta = row.GetCell(1).StringCellValue;
                string nomePaciente = row.GetCell(2).StringCellValue;
                string numeroTelefone = row.GetCell(3)?.StringCellValue; //! as vezes vem vazio, ai tem q botar ?
                long cpf = Convert.ToInt64(Regex.Replace(row.GetCell(4).StringCellValue, @"\D", ""));
                string rua = row.GetCell(5).StringCellValue;
                string cidade = row.GetCell(6).StringCellValue;
                string estado = row.GetCell(7).StringCellValue;
                string especialidade = row.GetCell(8).StringCellValue;
                string nomeMedico = row.GetCell(9).StringCellValue;
                bool particular = row.GetCell(10).StringCellValue == "Sim" ? true : false;
                long numeroCarteirinha = Convert.ToInt64(row.GetCell(11).NumericCellValue);
                double valorConsulta = row.GetCell(12).NumericCellValue;

                Consulta consulta = new Consulta(
                    dataConsulta,
                    horaConsulta,
                    nomePaciente,
                    numeroTelefone,
                    cpf,
                    rua,
                    cidade,
                    estado,
                    especialidade,
                    nomeMedico,
                    particular,
                    numeroCarteirinha,
                    valorConsulta);

                consultas.Add(consulta);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Erro ao importar Excel");
            Console.WriteLine(e.Message);
        }
    }


    public static void Main(string[] args)
    {
        DataImport();
        //Ex1();
        //Ex2();
        //Ex3();
        //Ex4();
        //Ex5();
        Desafio1();
        //Desafio2();
        //Desafio3();

    }

    //TODO 1 – Liste ao total quantos pacientes temos para atender do dia 27/03 até dia 31/03. Sem repetições.
    static void Ex1()
    {
        DateTime inicio = new DateTime(2023, 03, 27);
        DateTime fim = new DateTime(2023, 03, 31);
        var pacientes = consultas.Where(c => c.DataConsulta >= inicio && c.DataConsulta <= fim);
        var total = pacientes.DistinctBy(c => c.NomePaciente).Count();

        Console.WriteLine($"Total: {total}");
        foreach (var paciente in pacientes)
        {
            Console.WriteLine($"{paciente.NomePaciente}");
        }

        // Total: 47
        // João da Silva
        // Ana Souza
        // Maria Santos
        // João da Silva
    }

    //TODO 2 – Liste ao total quantos médicos temos trabalhando em nosso consultório. Conte a quantidade de médicos sem repetições.
    static void Ex2()
    {
        var total = consultas.DistinctBy(c => c.NomeMedico);

        Console.WriteLine($"Total: {total.Count()}");

        foreach (var res in consultas)
        {
            Console.WriteLine($"Médico: {res.NomeMedico}");
        }

        // Total: 37
        // Médico: Ana Luiza Pereira
        // Médico: Rafaela Silva    
        // Médico: Lucas Oliveira 
    }

    //TODO 3 – Liste o nome dos médicos e suas especialidades.
    static void Ex3()
    {
        var result = consultas.GroupBy(c => c.NomeMedico)
        .Select(c => new { nome = c.Key, especialidade = c.Select(c => c.Especialidade).Distinct() });

        foreach (var res in result)
        {
            Console.WriteLine($"{res.nome} - {string.Join(", ", res.especialidade)}");
        }

        // Ana Luiza Pereira - Ortopedia
        // Rafaela Silva - Cardiologia
        // Lucas Oliveira - Neurologia
        // Marcos Costa - Oftalmologia
        // Carla Oliveira - Pediatria, Dermatologia
    }
    //TODO 4 – Liste o total em valor de consulta que receberemos. Some o valor de todas as consultas. Depois liste o valor por especialidade.
    static void Ex4()
    {
        var result = consultas.GroupBy(c => c.Especialidade);
        var total = consultas.Sum(c => c.ValorConsulta);

        Console.WriteLine($"Total: {total:c}");
        foreach (var res in result)
        {
            Console.WriteLine($"{res.Key} - {res.Sum(c => c.ValorConsulta):c}");
        }

        // Total: R$ 24.450,00
        // Ortopedia - R$ 4.600,00   
        // Cardiologia - R$ 4.600,00 
        // Neurologia - R$ 3.900,00 
    }

    //TODO 5 – Para o dia 30/03. Quantas consultas vão ser realizadas? Quantas são Particular? 
    //TODO Liste para esse dia os horários de consulta de cada médico e suas especialidades.
    static void Ex5()
    {
        DateTime data = new DateTime(2023, 03, 30).Date;

        var diaEsp = consultas.Where(c => c.DataConsulta == data);

        var part = diaEsp.Where(c => c.Particular == true);

        Console.WriteLine($"Total de consultas para o dia 30/03: {diaEsp.Count()}");
        Console.WriteLine($"Dessas, {part.Count()} são particulares");

        var result = consultas.GroupBy(c => c.NomeMedico)
        .Select(c => new
        {
            nome = c.Key,
            especialidades = c.Select(c => c.Especialidade).Distinct(),
            horaConsulta = c.Select(c => c.HoraConsulta).Distinct()
        });

        foreach (var med in result)
        {
            Console.WriteLine($"{med.nome} - {string.Join(", ", med.especialidades)}: terá uma consulta as {string.Join(", ", med.horaConsulta)}");
        }

        // Total de consultas para o dia 30/03: 13
        // Dessas, 4 são particulares
        // Ana Luiza Pereira - Ortopedia: terá uma consulta as 08:00, 09:30, 11:00
        // Rafaela Silva - Cardiologia: terá uma consulta as 08:30
        // Lucas Oliveira - Neurologia: terá uma consulta as 09:00
        // Marcos Costa - Oftalmologia: terá uma consulta as 10:00, 12:30
        // Carla Oliveira - Pediatria, Dermatologia: terá uma consulta as 10:30, 12:00, 16:30, 14:00, 16:00
    }

    //TODO 1 – Verifique se algum paciente tem alguma consulta marcada no mesmo horário. Tem? Aponte quais, pois precisaremos ligar para o paciente. Não tem telefone? Procure se há alguém que more na mesma Rua, Cidade e Estado que o paciente para tentarmos entrar em contato.
    static void Desafio1()
    {
        //! TESTE (voltar caso de merda)
        // var repetidos = consultas.GroupBy(c => c.DataConsulta);

        // foreach (var consulta in repetidos)
        // {
        //     Console.WriteLine(consulta.Key.ToString("dd/MM/yyyy"));
        //     var horasRept = consulta.GroupBy(c => c.HoraConsulta);
        //     foreach (var con in horasRept)
        //     {
        //         if (con.Count() > 1)
        //         {
        //             var horaRept = con.Key;
        //             var pacientesRept = consulta.Where(c => c.HoraConsulta == horaRept);

        //             var nomePaciente = pacientesRept.Select(c => c.NomePaciente);
        //             Console.WriteLine($"{horaRept}: {string.Join(", ", nomePaciente)} (numero aq))  /");
        //         }
        //     }
        //     Console.WriteLine("\n");
        // }

        var agendamentos = consultas
            .GroupBy(c => new { c.DataConsulta, c.HoraConsulta })
            .OrderBy(c => c.Key.DataConsulta);

        var consultasDuplicadas = agendamentos
            .Where(g => g.Count() > 1)
            .SelectMany(g => g)
            .ToList();

        //? SELECT MANY 
        //? transforma tudo em uma lista só. 
        //? [ {[consulta], [consulta]} ] -> [consulta, consulta]

        foreach (var consulta in consultasDuplicadas)
        {
            Console.WriteLine($"\n{consulta.NomePaciente} tem consulta em {consulta.DataConsulta:dd/MM/yyyy} às {consulta.HoraConsulta}. [{consulta.NumeroTelefone ?? "sem número"}]");

            if (string.IsNullOrEmpty(consulta.NumeroTelefone))
            {
                var contatoAlternativo = consultas.FirstOrDefault(c =>
                    c.Rua == consulta.Rua &&
                    c.Cidade == consulta.Cidade &&
                    c.Estado == consulta.Estado &&
                    !string.IsNullOrEmpty(c.NumeroTelefone)); // ve se o vizinho tbm nn tem o número

                if (contatoAlternativo != null)
                {
                    Console.WriteLine($"Paciente sem telefone. Tenta entrar em contato com {contatoAlternativo.NomePaciente} [{contatoAlternativo.NumeroTelefone}]");
                }
                else
                {
                    Console.WriteLine("Paciente sem telefone e nenhum contato alternativo encontrado.");
                }
            }
        }
    }
}