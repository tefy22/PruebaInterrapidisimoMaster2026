using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Subjects
{
    public static class SubjectErrors
    {
        public static Error Empty = new Error("Subject.Empty", "El nombre de la materia no puede ser vacio");
        public static Error NotFound = new Error("Subject.NotFound", "La materia con el Id especificado no fue encontrado");
        public static Error CreditsTeacherComplete = new Error("Subject.CreditsTeacherComplete", "El profesor superó el limite de materias asignadas");
        public static Error TeacherNotExist = new Error("Subject.TeacherNotExist", "El profesor no existe");
        public static Error RepeatName = new Error("Subject.RepeatName", "La materia con la descripcion adjunta ya existe");
        public static Error Overlap = new Error("Subject.Overlap", "La materia esta siendo registrado por otro agente en este momento");
        public static Error CreateError = new Error("Subject.CreateError", "Error al crear la materia");
        public static Error UpdateError = new Error("Subject.UpdateError", "Error al actualizar la materia");
        public static Error DeleteError = new Error("Subject.DeleteError", "Error al eliminar la materia");
        public static Error SearchError = new Error("Subject.SearchError", "Ocurrio un error al buscar la materia");
        public static Error CannotDeleteSubjectWithRegistrations = new Error("Subject.CannotDeleteSubjectWithRegistrations", "No se puede eliminar/modificar una materia que tiene registros de estudiantes");
    }
}
