using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Postulacion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        public string Mensaje { get; set; } = null!;

        [ForeignKey("Favor")]
        public int FavorId { get; set; }
        public Favor? Favor { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime FechaPostulacion { get; set; } = DateTime.Now;
    }
}
