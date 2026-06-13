using System;
using System.Collections.Generic;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Data;

public partial class ETDPortalDbContext : DbContext
{
    public ETDPortalDbContext()
    {
    }

    public ETDPortalDbContext(DbContextOptions<ETDPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeHistory> GradeHistories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ReimbursementRequest> ReimbursementRequests { get; set; }

    public virtual DbSet<ReimbursementType> ReimbursementTypes { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationDoc> ReservationDocs { get; set; }

    public virtual DbSet<ReservationType> ReservationTypes { get; set; }

    public virtual DbSet<TravelBudgetAllocation> TravelBudgetAllocations { get; set; }

    public virtual DbSet<TravelRequest> TravelRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ETD_Portal;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grade__3214EC078D459AEF");

            entity.ToTable("Grade");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("name");
        });



        modelBuilder.Entity<GradeHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GradeHis__3213E83F06E32754");

            entity.ToTable("GradeHistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedOn).HasColumnName("assigned_on");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GradeId).HasColumnName("grade_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.GradeHistories)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_users_grade_histories");

            entity.HasOne(d => d.Grade).WithMany(p => p.GradeHistories)
                .HasForeignKey(d => d.GradeId)
                .HasConstraintName("FK_grades_grade_histories");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3213E83F3406CF87");

            entity.ToTable("Location");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3213E83F8FBA7542");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.Token)
                .HasMaxLength(500)
                .HasColumnName("token");

            entity.HasOne(d => d.Employee).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<ReimbursementRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reimbursement_requests");

            entity.ToTable("ReimbursementRequest");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocumentUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("document_url");
            entity.Property(e => e.InvoiceAmount).HasColumnName("invoice_amount");
            entity.Property(e => e.InvoiceDate).HasColumnName("invoice_date");
            entity.Property(e => e.InvoiceNo)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("invoice_no");
            entity.Property(e => e.ReimbursementTypeId).HasColumnName("reimbursement_type_id");
            entity.Property(e => e.Remarks)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("remarks");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("request_date");
            entity.Property(e => e.RequestProcessedByEmployeeId).HasColumnName("request_processed_by_employee_id");
            entity.Property(e => e.RequestProcessedOn).HasColumnName("request_processed_on");
            entity.Property(e => e.RequestRaisedByEmployeeId).HasColumnName("request_raised_by_employee_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TravelRequestId).HasColumnName("travel_request_id");

            entity.HasOne(d => d.ReimbursementType).WithMany(p => p.ReimbursementRequests)
                .HasForeignKey(d => d.ReimbursementTypeId)
                .HasConstraintName("FK_reimbursement_types_reimbursement_requests");

            entity.HasOne(d => d.RequestProcessedByEmployee).WithMany(p => p.ReimbursementRequestRequestProcessedByEmployees)
                .HasForeignKey(d => d.RequestProcessedByEmployeeId)
                .HasConstraintName("FK_users_reimbursement_requests_processed");

            entity.HasOne(d => d.RequestRaisedByEmployee).WithMany(p => p.ReimbursementRequestRequestRaisedByEmployees)
                .HasForeignKey(d => d.RequestRaisedByEmployeeId)
                .HasConstraintName("FK_users_reimbursement_requests_raised");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.ReimbursementRequests)
                .HasForeignKey(d => d.TravelRequestId)
                .HasConstraintName("FK_travel_requests_reimbursement_requests");
        });

        modelBuilder.Entity<ReimbursementType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reimbursement_types");

            entity.ToTable("ReimbursementType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reservations");

            entity.ToTable("Reservation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ConfirmationId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("confirmation_id");
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_on");
            entity.Property(e => e.Remarks)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("remarks");
            entity.Property(e => e.ReservationDate).HasColumnName("reservation_date");
            entity.Property(e => e.ReservationDoneByEmployeeId).HasColumnName("reservation_done_by_employee_id");
            entity.Property(e => e.ReservationDoneWithEntity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reservation_done_with_entity");
            entity.Property(e => e.ReservationTypeId).HasColumnName("reservation_type_id");
            entity.Property(e => e.TravelRequestId).HasColumnName("travel_request_id");

            entity.HasOne(d => d.ReservationDoneByEmployee).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReservationDoneByEmployeeId)
                .HasConstraintName("FK_users_reservations");

            entity.HasOne(d => d.ReservationType).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ReservationTypeId)
                .HasConstraintName("FK_reservation_types_reservations");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.TravelRequestId)
                .HasConstraintName("FK_travel_requests_reservations");
        });

        modelBuilder.Entity<ReservationDoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reservation_docs");

            entity.ToTable("ReservationDoc");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DocumentUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("document_url");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ReservationDocs)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_reservations_reservation_docs");
        });

        modelBuilder.Entity<ReservationType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Reservat__2C0005985820C150");

            entity.ToTable("ReservationType");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.TypeName)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("type_name");
        });

        modelBuilder.Entity<TravelBudgetAllocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_travel_budget_allocations");

            entity.ToTable("TravelBudgetAllocation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApprovedBudget).HasColumnName("approved_budget");
            entity.Property(e => e.ApprovedHotelStarRating)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("approved_hotel_star_rating");
            entity.Property(e => e.ApprovedModeOfTravel)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("approved_mode_of_travel");
            entity.Property(e => e.TravelRequestId).HasColumnName("travel_request_id");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.TravelBudgetAllocations)
                .HasForeignKey(d => d.TravelRequestId)
                .HasConstraintName("FK_travel_requests_travel_budget_allocations");
        });

        modelBuilder.Entity<TravelRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__TravelRe__18D3B90F16721975");

            entity.ToTable("TravelRequest");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Priority)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("priority");
            entity.Property(e => e.PurposeOfTravel)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("purpose_of_travel");
            entity.Property(e => e.RaisedByEmployeeId).HasColumnName("raised_by_employee_id");
            entity.Property(e => e.RequestApprovedOn).HasColumnName("request_approved_on");
            entity.Property(e => e.RequestRaisedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("request_raised_on");
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("request_status");
            entity.Property(e => e.ToBeApprovedByHrId).HasColumnName("to_be_approved_by_hr_id");
            entity.Property(e => e.ToDate).HasColumnName("to_date");

            entity.HasOne(d => d.Location).WithMany(p => p.TravelRequests)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_locations_travel_requests");

            entity.HasOne(d => d.RaisedByEmployee).WithMany(p => p.TravelRequestRaisedByEmployees)
                .HasForeignKey(d => d.RaisedByEmployeeId)
                .HasConstraintName("FK_users_travel_requests_employee");

            entity.HasOne(d => d.ToBeApprovedByHr).WithMany(p => p.TravelRequestToBeApprovedByHrs)
                .HasForeignKey(d => d.ToBeApprovedByHrId)
                .HasConstraintName("FK_users_travel_requests_hr");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__User__C52E0BA83680E2F3");

            entity.ToTable("User");

            entity.HasIndex(e => e.EmailAddress, "UQ_users_email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_users_phone").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.CurrentGradeId).HasColumnName("current_grade_id");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email_address");
            entity.Property(e => e.FirstName)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone_number");
            entity.Property(e => e.Role)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("role");

            entity.HasOne(d => d.CurrentGrade).WithMany(p => p.Users)
                .HasForeignKey(d => d.CurrentGradeId)
                .HasConstraintName("FK_grades_users");

        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
