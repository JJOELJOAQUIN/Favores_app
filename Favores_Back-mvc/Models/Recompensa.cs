using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Favores_Back_mvc.Models
{
    public class Recompensa
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("Favor")]
        public int FavorId { get; set; }
        public Favor? Favor { get; set; }

        [Required]
        [Precision(10, 2)]
        public decimal Monto { get; set; }

        [Required, StringLength(50)]
        public string TipoPago { get; set; } = null!; // "Transferencia", "GiftCard", "Trueque"

        [Required]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, Cancelado

        public DateTime? FechaPago { get; set; }
    }
}
