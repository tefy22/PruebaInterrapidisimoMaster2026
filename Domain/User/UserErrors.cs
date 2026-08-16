using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.User
{
    public static class UserErrors
    {
        public static Error UserNotFound = new Error("User.UserNotFound", "No existe ningun registro con dicho email");
        public static Error PasswordInvalid = new Error("User.PasswordInvalid", "Credenciales invalidas");
        public static Error ExistsEmail = new Error("User.ExistsEmail", "El email ya existe");
        public static Error StudentNotFound = new Error("User.StudentNotFound", "El estudiante con el id mencionado no existe");
        public static Error CannotDeleteTeacherWithSubjects = new Error("User.CannotDeleteTeacherWithSubjects", "El profesor tiene materias asignadas o matriculas asociadas; no se puede eliminar");


        public static Error SearchError = new Error("User.SearchError", "Error al buscar el usuario");
        public static Error CreateError = new Error("User.CreateError", "Error al crear el usuario");
        public static Error UpdateError = new Error("User.UpdateError", "Error al actualizar el usuario");
        public static Error DeleteError = new Error("User.DeleteError", "Error al eliminar el usuario");
    }
}
