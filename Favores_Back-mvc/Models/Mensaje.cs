using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Mensaje
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Texto { get; set; } = null!;

        public DateTime FechaHora { get; set; } = DateTime.Now;

        [ForeignKey("Chat")]
        public int ChatId { get; set; }
        public Chat? Chat { get; set; }

        [ForeignKey("Remitente")]
        public int RemitenteId { get; set; }
        public Usuario? Remitente { get; set; }
    }
}
