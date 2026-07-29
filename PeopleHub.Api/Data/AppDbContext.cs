using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeopleHub.Api.People;

namespace PeopleHub.Api.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Contribuicao> Contribuicoes => Set<Contribuicao>();
    public DbSet<TipoPessoa> TiposPessoa => Set<TipoPessoa>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TipoPessoa>(e =>
        {
            e.ToTable("TIPO_PESSOA");
            e.HasKey(x => x.Id);

            e.Property(x => x.Nome).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Nome).IsUnique();
        });

        builder.Entity<Person>(e =>
        {
            e.ToTable("People");
            e.HasKey(x => x.Id);

            e.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            e.Property(x => x.Cpf).HasMaxLength(11).IsRequired();
            e.HasIndex(x => x.Cpf).IsUnique(); // CPF único

            e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.Telefone).HasMaxLength(30);
            e.Property(x => x.Ativo).HasDefaultValue(true);

            e.HasOne(p => p.TipoPessoa)
             .WithMany(tp => tp.Pessoas)
             .HasForeignKey(p => p.TipoPessoaId)
             .OnDelete(DeleteBehavior.Restrict);

        });

     
        builder.Entity<Socio>(e =>
        {
            e.ToTable("SOCIO");
            e.HasKey(x => x.PersonId);

            e.HasOne(x => x.Person)
             .WithOne()
             .HasForeignKey<Socio>(x => x.PersonId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(x => x.Ativo).HasDefaultValue(true);
        });

        builder.Entity<Contribuicao>(e =>
        {
            e.ToTable("CONTRIBUICAO");
            e.HasKey(x => x.Id);

            e.Property(x => x.Valor)
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.Property(x => x.CompetenciaAno).IsRequired();
            e.Property(x => x.CompetenciaMes).IsRequired();

            e.HasOne(x => x.Person)
             .WithMany()
             .HasForeignKey(x => x.PersonId)
             .OnDelete(DeleteBehavior.Restrict);

            // 1 por competência (recomendado)
            e.HasIndex(x => new { x.PersonId, x.CompetenciaAno, x.CompetenciaMes })
             .IsUnique();
        });

    }
}