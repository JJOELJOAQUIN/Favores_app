using System.ComponentModel.DataAnnotations;

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
        public double Reputacion { get; set; } = 0;
        public bool Activo { get; set; } = true;

        // Relación uno a muchos con Favor
        public ICollection<Favor>? FavoresCreados { get; set; }
    }
}
