using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Chat
    {
        public int Id { get; set; }

        [ForeignKey("Favor")]
        public int FavorId { get; set; }
        public Favor? Favor { get; set; }

        [ForeignKey("Creador")]
        public int CreadorId { get; set; }
        public Usuario? Creador { get; set; }

        [ForeignKey("Ejecutor")]
        public int EjecutorId { get; set; }
        public Usuario? Ejecutor { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<Mensaje>? Mensajes { get; set; }
    }
}
