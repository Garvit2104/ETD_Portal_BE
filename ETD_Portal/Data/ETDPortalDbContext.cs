using System;
using System.Collections.Generic;
using ETD_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace ETD_Portal.Data;

public partial class ETDPortalDbContext : DbContext
{
    public ETDPortalDbContext(DbContextOptions<ETDPortalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GradeHistory> GradeHistories { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<ReimbursementRequest> ReimbursementRequests { get; set; }

    public virtual DbSet<ReimbursementType> ReimbursementTypes { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationDoc> ReservationDocs { get; set; }

    public virtual DbSet<ReservationType> ReservationTypes { get; set; }

    public virtual DbSet<TravelBudgetAllocation> TravelBudgetAllocations { get; set; }

    public virtual DbSet<TravelRequest> TravelRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grade__3214EC078D459AEF");
        });

        modelBuilder.Entity<GradeHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GradeHis__3213E83F06E32754");

            entity.HasOne(d => d.Employee).WithMany(p => p.GradeHistories).HasConstraintName("FK_users_grade_histories");

            entity.HasOne(d => d.Grade).WithMany(p => p.GradeHistories).HasConstraintName("FK_grades_grade_histories");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Location__3213E83F3406CF87");
        });

        modelBuilder.Entity<ReimbursementRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reimbursement_requests");

            entity.Property(e => e.RequestDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ReimbursementType).WithMany(p => p.ReimbursementRequests).HasConstraintName("FK_reimbursement_types_reimbursement_requests");

            entity.HasOne(d => d.RequestProcessedByEmployee).WithMany(p => p.ReimbursementRequestRequestProcessedByEmployees).HasConstraintName("FK_users_reimbursement_requests_processed");

            entity.HasOne(d => d.RequestRaisedByEmployee).WithMany(p => p.ReimbursementRequestRequestRaisedByEmployees).HasConstraintName("FK_users_reimbursement_requests_raised");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.ReimbursementRequests).HasConstraintName("FK_travel_requests_reimbursement_requests");
        });

        modelBuilder.Entity<ReimbursementType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reimbursement_types");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reservations");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ReservationDoneByEmployee).WithMany(p => p.Reservations).HasConstraintName("FK_users_reservations");

            entity.HasOne(d => d.ReservationType).WithMany(p => p.Reservations).HasConstraintName("FK_reservation_types_reservations");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.Reservations).HasConstraintName("FK_travel_requests_reservations");
        });

        modelBuilder.Entity<ReservationDoc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_reservation_docs");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ReservationDocs).HasConstraintName("FK_reservations_reservation_docs");
        });

        modelBuilder.Entity<ReservationType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__Reservat__2C0005985820C150");
        });

        modelBuilder.Entity<TravelBudgetAllocation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_travel_budget_allocations");

            entity.HasOne(d => d.TravelRequest).WithMany(p => p.TravelBudgetAllocations).HasConstraintName("FK_travel_requests_travel_budget_allocations");
        });

        modelBuilder.Entity<TravelRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__TravelRe__18D3B90F16721975");

            entity.Property(e => e.RequestRaisedOn).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Location).WithMany(p => p.TravelRequests).HasConstraintName("FK_locations_travel_requests");

            entity.HasOne(d => d.RaisedByEmployee).WithMany(p => p.TravelRequestRaisedByEmployees).HasConstraintName("FK_users_travel_requests_employee");

            entity.HasOne(d => d.ToBeApprovedByHr).WithMany(p => p.TravelRequestToBeApprovedByHrs).HasConstraintName("FK_users_travel_requests_hr");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__User__C52E0BA83680E2F3");

            entity.HasOne(d => d.CurrentGrade).WithMany(p => p.Users).HasConstraintName("FK_grades_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
