using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Favor
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Titulo { get; set; } = null!;

        [Required]
        public string Descripcion { get; set; } = null!;

        [Required]
        public string Ubicacion { get; set; } = null!;

        [Required]
        public string Categoria { get; set; } = null!;

        [Range(0.01, double.MaxValue)]
        [Precision(10, 2)]
        public decimal Recompensa { get; set; }


        [Required]
        public string TipoRecompensa { get; set; } = null!;

        public string Estado { get; set; } = "Publicado";

        [ForeignKey("Creador")]
        public int CreadorId { get; set; }
        public Usuario? Creador { get; set; }

        public ICollection<Postulacion>? Postulaciones { get; set; }
    }
}