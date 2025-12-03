using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace MozizzAPI.Models;

public partial class MozizzContext : DbContext
{
    public MozizzContext()
    {
    }


    public MozizzContext(DbContextOptions<MozizzContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Emaillog> Emaillogs { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Reservedseat> Reservedseats { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseMySQL(configuration.GetConnectionString("MozizzConnection"));
        }
    }

      



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        

        modelBuilder.Entity<Emaillog>(entity =>
        {
            entity.HasKey(e => e.EmailLogId).HasName("PRIMARY");

            entity.ToTable("emaillogs");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.EmailLogId)
                .HasColumnType("int(11)")
                .HasColumnName("email_log_id");
            entity.Property(e => e.Body)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("body");
            entity.Property(e => e.EmailType)
                .HasMaxLength(50)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("email_type");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("sent_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("subject");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Emaillogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("emaillogs_ibfk_1");
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.HallId).HasName("PRIMARY");

            entity.ToTable("halls");

            entity.Property(e => e.HallId)
                .HasColumnType("int(11)")
                .HasColumnName("hall_id");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.SeatingCapacity)
                .HasColumnType("int(11)")
                .HasColumnName("seating_capacity");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PRIMARY");

            entity.ToTable("movies");

            entity.Property(e => e.MovieId)
                .HasColumnType("int(11)")
                .HasColumnName("movie_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Duration)
                .HasColumnType("int(11)")
                .HasColumnName("duration");
            entity.Property(e => e.Genre)
                .HasMaxLength(100)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("genre");
            entity.Property(e => e.Rating)
                .HasMaxLength(10)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("rating");
            entity.Property(e => e.ReleaseDate)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("date")
                .HasColumnName("release_date");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.ToTable("payments");

            entity.HasIndex(e => e.ReservationId, "reservation_id");

            entity.Property(e => e.PaymentId)
                .HasColumnType("int(11)")
                .HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10)
                .HasColumnName("amount");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("payment_status");
            entity.Property(e => e.ReservationId)
                .HasColumnType("int(11)")
                .HasColumnName("reservation_id");
            entity.Property(e => e.TransactionReference)
                .HasMaxLength(255)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("transaction_reference");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PRIMARY");

            entity.ToTable("reservations");

            entity.HasIndex(e => e.ShowtimeId, "showtime_id");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.ReservationId)
                .HasColumnType("int(11)")
                .HasColumnName("reservation_id");
            entity.Property(e => e.ReservationDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("reservation_date");
            entity.Property(e => e.ShowtimeId)
                .HasColumnType("int(11)")
                .HasColumnName("showtime_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'''pending'''")
                .HasColumnName("status");
            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reservations_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reservations_ibfk_1");
        });

        modelBuilder.Entity<Reservedseat>(entity =>
        {
            entity.HasKey(e => e.ReservedSeatId).HasName("PRIMARY");

            entity.ToTable("reservedseats");

            entity.HasIndex(e => new { e.ReservationId, e.SeatId }, "reservation_id").IsUnique();

            entity.HasIndex(e => e.SeatId, "seat_id");

            entity.Property(e => e.ReservedSeatId)
                .HasColumnType("int(11)")
                .HasColumnName("reserved_seat_id");
            entity.Property(e => e.ReservationId)
                .HasColumnType("int(11)")
                .HasColumnName("reservation_id");
            entity.Property(e => e.SeatId)
                .HasColumnType("int(11)")
                .HasColumnName("seat_id");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Reservedseats)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("reservedseats_ibfk_1");

            entity.HasOne(d => d.Seat).WithMany(p => p.Reservedseats)
                .HasForeignKey(d => d.SeatId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("reservedseats_ibfk_2");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.SeatId).HasName("PRIMARY");

            entity.ToTable("seats");

            entity.HasIndex(e => new { e.HallId, e.SeatNumber }, "hall_id").IsUnique();

            entity.Property(e => e.SeatId)
                .HasColumnType("int(11)")
                .HasColumnName("seat_id");
            entity.Property(e => e.HallId)
                .HasColumnType("int(11)")
                .HasColumnName("hall_id");
            entity.Property(e => e.IsVip)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_vip");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(10)
                .HasColumnName("seat_number");

            entity.HasOne(d => d.Hall).WithMany(p => p.Seats)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("seats_ibfk_1");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.ShowtimeId).HasName("PRIMARY");

            entity.ToTable("showtimes");

            entity.HasIndex(e => e.HallId, "hall_id");

            entity.HasIndex(e => new { e.MovieId, e.HallId, e.ShowDate, e.ShowTime1 }, "movie_id").IsUnique();

            entity.Property(e => e.ShowtimeId)
                .HasColumnType("int(11)")
                .HasColumnName("showtime_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.HallId)
                .HasColumnType("int(11)")
                .HasColumnName("hall_id");
            entity.Property(e => e.MovieId)
                .HasColumnType("int(11)")
                .HasColumnName("movie_id");
            entity.Property(e => e.ShowDate)
                .HasColumnType("date")
                .HasColumnName("show_date");
            entity.Property(e => e.ShowTime1)
                .HasColumnType("time")
                .HasColumnName("show_time");

            entity.HasOne(d => d.Hall).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.HallId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtimes_ibfk_2");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("showtimes_ibfk_1");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("tickets");

            entity.HasIndex(e => e.ReservationId, "reservation_id");

            entity.HasIndex(e => e.TicketCode, "ticket_code").IsUnique();

            entity.Property(e => e.TicketId)
                .HasColumnType("int(11)")
                .HasColumnName("ticket_id");
            entity.Property(e => e.IssuedDate)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("issued_date");
            entity.Property(e => e.ReservationId)
                .HasColumnType("int(11)")
                .HasColumnName("reservation_id");
            entity.Property(e => e.TicketCode)
                .HasMaxLength(100)
                .HasColumnName("ticket_code");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("tickets_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.RoleId, "role_id");

            entity.Property(e => e.UserId)
                .HasColumnType("int(11)")
                .HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasDefaultValueSql("'NULL'")
                .HasColumnName("phone");
            entity.Property(e => e.RoleId)
                .HasColumnType("int(11)")
                .HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("users_ibfk_1");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("userroles");

            entity.HasIndex(e => e.RoleName, "role_name").IsUnique();

            entity.Property(e => e.RoleId)
                .HasColumnType("int(11)")
                .HasColumnName("role_id");
            entity.Property(e => e.Description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
