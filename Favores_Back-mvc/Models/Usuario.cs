using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(8)]
        public string PasswordHash { get; set; } = null!;

        public string? FotoPerfil { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;


        // ============================
        //   ROLES
        // ============================
        // ADMIN  → puede crear favores
        // USER   → solo se postula y ejecuta favores
        [Required]
        public string Rol { get; set; } = "USER";

        // ============================
        //   REPUTACIÓN
        // ============================
        public double Reputacion { get; set; } = 0;

        // Cantidad de calificaciones recibidas
        public int CantidadCalificaciones { get; set; } = 0;

        // ============================
        //   ESTADO
        // ============================
        public bool Activo { get; set; } = true;

        // ============================
        //   RELACIONES
        // ============================
        public ICollection<Favor>? FavoresCreados { get; set; }
    }
}
