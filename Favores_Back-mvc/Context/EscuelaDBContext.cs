

using Favores_Back_mvc.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Favores_Back_mvc.Context

{
    public class EscuelaDBContext : DbContext
    {
        public EscuelaDBContext(DbContextOptions<EscuelaDBContext>
        options) : base(options)
        {
        }
        public DbSet<Estudiante> Estudiantes { get; set; }
    }
}
