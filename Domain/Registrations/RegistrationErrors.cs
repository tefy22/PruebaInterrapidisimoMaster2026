using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Registrations
{
    public static class RegistrationErrors
    {
        public static Error Empty = new Error("Registration.Empty", "El registro con el Id especificado no puede ser vacío");
        public static Error NotFound = new Error("Registration.NotFound", "El registro con el Id especificado no fue encontrado");
        public static Error Exists = new Error("Registration.Exists", "El registro ya existe");
        public static Error EmptySubjects = new Error("Registration.EmptySubjects", "Se requiere al menos una materia.");
        public static Error MaxSubjects = new Error("Registration.MaxSubjects", "Ha excedido el limite maximo de registro de materias");
        public static Error SameTeacherInSelection = new Error("Registration.SameTeacherInSelection", "El estudiante no puede ver 2 materias con el mismo profesor");
        public static Error AlreadyExists = new Error("Registration.AlreadyExists", "Ya hay un registro en curso, contactarse con el administrador");
        public static Error NotExists = new Error("Registration.NotExists", "No hay un registro en curso, favor realice la creacion");
        public static Error InconsistencyStudent = new Error("Registration.InconsistencyStudent", "El Id del estudiante no corresponde con el que se encuentra registrado, favor validar");

        public static Error CreateError = new Error("Registration.CreateError", "Error al crear el registro");
        public static Error UpdateError = new Error("Registration.UpdateError", "Error al actualizar el registro");
        public static Error DeleteError = new Error("Registration.DeleteError", "Error al eliminar el registro");
        public static Error SearchError = new Error("Registration.SearchError", "Ocurrió un error al buscar el registro");
    }
}
