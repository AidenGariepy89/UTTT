using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UTTT.Models;

public partial class UTTTContext : DbContext
{
    public UTTTContext()
    {
    }

    public UTTTContext(DbContextOptions<UTTTContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conn> Conns { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameMove> GameMoves { get; set; }

    public virtual DbSet<Lobby> Lobbies { get; set; }

    public virtual DbSet<Move> Moves { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;database=UTTT;User Id=sa;Password=Hello123@#;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conn>(entity =>
        {
            entity.HasKey(e => e.ConnId).HasName("PK__conn__9FC140B6526CEBCC");

            entity.ToTable("conn");

            entity.Property(e => e.ConnId)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("conn_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");

            entity.HasOne(d => d.Player).WithMany(p => p.Conns)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__conn__player_id__14270015");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__game__3213E83F51667A8D");

            entity.ToTable("game");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.OPlayer).HasColumnName("o_player");
            entity.Property(e => e.Winner).HasColumnName("winner");
            entity.Property(e => e.XPlayer).HasColumnName("x_player");

            entity.HasOne(d => d.OPlayerNavigation).WithMany(p => p.GameOPlayerNavigations)
                .HasForeignKey(d => d.OPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__game__o_player__1AD3FDA4");

            entity.HasOne(d => d.WinnerNavigation).WithMany(p => p.GameWinnerNavigations)
                .HasForeignKey(d => d.Winner)
                .HasConstraintName("FK__game__winner__1BC821DD");

            entity.HasOne(d => d.XPlayerNavigation).WithMany(p => p.GameXPlayerNavigations)
                .HasForeignKey(d => d.XPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__game__x_player__19DFD96B");
        });

        modelBuilder.Entity<GameMove>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__game_mov__3213E83FDA22407D");

            entity.ToTable("game_moves");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.MoveId).HasColumnName("move_id");
            entity.Property(e => e.Turn).HasColumnName("turn");

            entity.HasOne(d => d.Game).WithMany(p => p.GameMoves)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__game_move__game___208CD6FA");

            entity.HasOne(d => d.Move).WithMany(p => p.GameMoves)
                .HasForeignKey(d => d.MoveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__game_move__move___2180FB33");
        });

        modelBuilder.Entity<Lobby>(entity =>
        {
            entity.HasKey(e => e.ConnId).HasName("PK__lobby__9FC140B61CEDC977");

            entity.ToTable("lobby");

            entity.Property(e => e.ConnId)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("conn_id");

            entity.HasOne(d => d.Conn).WithOne(p => p.Lobby)
                .HasForeignKey<Lobby>(d => d.ConnId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lobby__conn_id__17036CC0");
        });

        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__moves__3213E83F2F7D1594");

            entity.ToTable("moves");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Board).HasColumnName("board");
            entity.Property(e => e.Cell).HasColumnName("cell");
            entity.Property(e => e.Piece)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("piece");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__player__3213E83F84CC6C3D");

            entity.ToTable("player");

            entity.HasIndex(e => e.Username, "UQ__player__F3DBC572C7AB2C2C").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
