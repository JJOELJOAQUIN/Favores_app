using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        [ForeignKey("Favor")]
        public int FavorId { get; set; }
        public Favor? Favor { get; set; }

        [ForeignKey("Evaluador")]
        public int EvaluadorId { get; set; }
        public Usuario? Evaluador { get; set; }

        [ForeignKey("Evaluado")]
        public int EvaluadoId { get; set; }
        public Usuario? Evaluado { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [MaxLength(300)]
        public string? Comentario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
