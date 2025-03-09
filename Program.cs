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


                DateTime dataConsulta = Convert.ToDateTime(row.GetCell(0).StringCellValue);
                string horaConsulta = row.GetCell(1).StringCellValue;
                string nomePaciente = row.GetCell(2).StringCellValue;
                string numeroTelefone = row.GetCell(3)?.StringCellValue; // !as vezes vem vazio, ai tem q botar ?
                long cpf = Convert.ToInt64(Regex.Replace(row.GetCell(4).StringCellValue, @"\D", ""));
                string rua = row.GetCell(5).StringCellValue;
                string cidade = row.GetCell(6).StringCellValue;
                string estado = row.GetCell(7).StringCellValue;
                string especialidade = row.GetCell(8).StringCellValue;
                string nomeMedico = row.GetCell(9).StringCellValue;
                bool particular = row.GetCell(10).StringCellValue == "Sim" ? true : false;
                long numeroCarteirinha = Convert.ToInt64(row.GetCell(11).NumericCellValue);

                Consulta consulta = new Consulta(dataConsulta, horaConsulta, nomePaciente, numeroTelefone, cpf, rua, cidade, estado, especialidade, nomeMedico, particular, numeroCarteirinha);

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
    }

    
}