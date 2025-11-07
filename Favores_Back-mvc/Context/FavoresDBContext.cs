using Microsoft.EntityFrameworkCore;
using Favores_Back_mvc.Models;

namespace Favores_Back_mvc.Context
{
    public class FavoresDBContext : DbContext
    {
        public FavoresDBContext(DbContextOptions<FavoresDBContext> options)
            : base(options) { }

        // Tablas principales
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Favor> Favores { get; set; }
        public DbSet<Postulacion> Postulaciones { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

        public DbSet<Recompensa> Recompensas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación: Usuario (1) → (N) Favor
            modelBuilder.Entity<Favor>()
                .HasOne(f => f.Creador)
                .WithMany(u => u.FavoresCreados)
                .HasForeignKey(f => f.CreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Favor (1) → (N) Postulaciones
            modelBuilder.Entity<Postulacion>()
                .HasOne(p => p.Favor)
                .WithMany(f => f.Postulaciones)
                .HasForeignKey(p => p.FavorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Usuario (1) → (N) Postulaciones
            modelBuilder.Entity<Postulacion>()
                .HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación: Favor (1) → (1) Chat
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Favor)
                .WithOne()
                .HasForeignKey<Chat>(c => c.FavorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relaciones del Chat con Usuarios (creador/ejecutor)
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Creador)
                .WithMany()
                .HasForeignKey(c => c.CreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Ejecutor)
                .WithMany()
                .HasForeignKey(c => c.EjecutorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación: Chat (1) → (N) Mensajes
            modelBuilder.Entity<Mensaje>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Mensajes)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            // ⚙️ Arreglo: Calificación (evitar múltiples cascadas)
            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Favor)
                .WithOne()
                .HasForeignKey<Calificacion>(c => c.FavorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Evaluador)
                .WithMany()
                .HasForeignKey(c => c.EvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Evaluado)
                .WithMany()
                .HasForeignKey(c => c.EvaluadoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
