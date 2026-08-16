using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Roles
{
    public static class RolErrors
    {
        public static Error Empty = new Error("Rol.Empty", "El rol con el Id especificado no puede ser vacio");
        public static Error NotFound = new Error("Rol.NotFound", "El rol con el Id especificado no fue encontrado");
        public static Error Exists = new Error("Rol.Exists", "El rol con la descripcion especificada ya existe");
        public static Error ExistsDescription = new Error("Rol.ExistsDescription", "El rol con la descripcion especificada no ya existe");
        public static Error InvalidDescription = new Error("Rol.InvalidDescription", "Descripción de rol inválida");
        public static Error CreateError = new Error("Rol.CreateError", "Error al crear el rol");
        public static Error UpdateError = new Error("Rol.UpdateError", "Error al actualizar el rol");
        public static Error DeleteError = new Error("Rol.DeleteError", "Error al eliminar el rol");
        public static Error SearchError = new Error("Rol.SearchError", "Ocurrio un error al buscar el rol");
    }
}
