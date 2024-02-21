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

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameMove> GameMoves { get; set; }

    public virtual DbSet<Move> Moves { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_pk");

            entity.ToTable("game");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OPlayer).HasColumnName("o_player");
            entity.Property(e => e.Winner).HasColumnName("winner");
            entity.Property(e => e.XPlayer).HasColumnName("x_player");

            entity.HasOne(d => d.OPlayerNavigation).WithMany(p => p.GameOPlayerNavigations)
                .HasForeignKey(d => d.OPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_o_player");

            entity.HasOne(d => d.WinnerNavigation).WithMany(p => p.GameWinnerNavigations)
                .HasForeignKey(d => d.Winner)
                .HasConstraintName("fk_winner");

            entity.HasOne(d => d.XPlayerNavigation).WithMany(p => p.GameXPlayerNavigations)
                .HasForeignKey(d => d.XPlayer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_x_player");
        });

        modelBuilder.Entity<GameMove>(entity =>
        {
            entity.HasKey(e => new { e.MoveId, e.GameId }).HasName("game_moves_pk");

            entity.ToTable("game_moves");

            entity.Property(e => e.MoveId).HasColumnName("move_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.Turn).HasColumnName("turn");

            entity.HasOne(d => d.Game).WithMany(p => p.GameMoves)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_moves_game");

            entity.HasOne(d => d.Move).WithMany(p => p.GameMoves)
                .HasForeignKey(d => d.MoveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_moves_moves");
        });

        modelBuilder.Entity<Move>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("moves_pk");

            entity.ToTable("moves");

            entity.Property(e => e.Id).HasColumnName("id");
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
            entity.HasKey(e => e.Id).HasName("player_pk");

            entity.ToTable("player");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PwdHash)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("pwd_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
