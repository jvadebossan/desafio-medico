using System;

namespace desafioMedicos.Models;

public class Medico
{
    public string NomeMedico { get; protected set; }
    public List<string> Especialidades { get; protected set; }

    public Medico(string nomeMedico, List<string> especialidades)
    {
        NomeMedico = nomeMedico;
        Especialidades = especialidades;
    }

    public void SetEspecialidades(List<string> especialidades)
    {
        Especialidades = especialidades;
    }

    public void SetNomeMedico(string nomeMedico)
    {
        NomeMedico = nomeMedico;
    }
}
