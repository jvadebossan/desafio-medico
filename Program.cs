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
        Ex5();
        //Desafio1();
        //Desafio2();
        //Desafio3();

    }

    //TODO 1 – Liste ao total quantos pacientes temos para atender do dia 27/03 até dia 31/03. Sem repetições.
    static void Ex1()
    {
        DateTime inicio = new DateTime(2023, 03, 27, 0, 0, 0);
        DateTime fim = new DateTime(2023, 03, 31, 0, 0, 0);

        var result = consultas.DistinctBy(c => c.NomePaciente)
        .Where(c => c.DataConsulta >= inicio && c.DataConsulta <= fim)
        .Count();

        Console.WriteLine($"Total: {result}");

        //Total: 47
    }

    //TODO 2 – Liste ao total quantos médicos temos trabalhando em nosso consultório. Conte a quantidade de médicos sem repetições.
    static void Ex2()
    {
        var result = consultas.DistinctBy(c => c.NomeMedico);

        Console.WriteLine($"Total: {result.Count()}");

        foreach (var res in result)
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
        var result = consultas.DistinctBy(c => c.NomeMedico);

        foreach (var res in result)
        {
            Console.WriteLine($"Médico: {res.NomeMedico} - Especialidade {res.Especialidade}");
        }

        // Médico: Ana Luiza Pereira - Especialidade Ortopedia
        // Médico: Rafaela Silva - Especialidade Cardiologia    
        // Médico: Lucas Oliveira - Especialidade Neurologia 
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

        var agenda = diaEsp.DistinctBy(c => c.NomeMedico);

        foreach (var med in agenda)
        {
            Console.WriteLine($"O médico {med.NomeMedico} (especialidade: {med.Especialidade}) terá uma consulta as {med.HoraConsulta}");
        }

        // Total de consultas para o dia 30/03: 13
        // Dessas, 4 são particulares
        // O médico Rafaela Silva (especialidade: Cardiologia) terá uma consulta as 08:30
        // O médico Carla Oliveira (especialidade: Pediatria) terá uma consulta as 10:30
        // O médico João Santos (especialidade: Ortopedia) terá uma consulta as 10:30
    }
}