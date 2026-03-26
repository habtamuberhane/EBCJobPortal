using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EBCJobPortal.Models;
public partial class EbcJobPortalContext : DbContext
{
    public EbcJobPortalContext()
    {
    }

    public EbcJobPortalContext(DbContextOptions<EbcJobPortalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblApplicant> TblApplicants { get; set; }

    public virtual DbSet<TblJobList> TblJobLists { get; set; }

    public virtual DbSet<TblJobPortalUser> TblJobPortalUsers { get; set; }

    public virtual DbSet<TblReference> TblReferences { get; set; }

    public virtual DbSet<TblRegion> TblRegions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var configSection = configBuilder.GetSection("ConnectionStrings");
        var connectionString = configSection["EBCJOBDB"] ?? null;
        optionsBuilder.UseSqlServer(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<TblApplicant>(entity =>
        {
            entity.HasKey(e => e.ApplyId);

            entity.ToTable("tbl_Applicants");

            entity.Property(e => e.Cgpa).HasColumnName("CGPA");
            entity.Property(e => e.CompanyPhoneNumber).HasMaxLength(13);
            entity.Property(e => e.CompanyPostNumber).HasMaxLength(10);
            entity.Property(e => e.CvShorttermTranings).HasColumnName("CV_shortterm_tranings");
            entity.Property(e => e.Cvfile).HasColumnName("CVFile");
            entity.Property(e => e.EducationField).HasMaxLength(250);
            entity.Property(e => e.EducationLevel).HasMaxLength(250);
            entity.Property(e => e.Gender).HasMaxLength(250);
            entity.Property(e => e.GraduationYear).HasMaxLength(250);
            entity.Property(e => e.HouseNumber).HasMaxLength(350);
            entity.Property(e => e.MaritalStatus).HasMaxLength(350);
            entity.Property(e => e.MonthlySalary).HasColumnType("money");
            entity.Property(e => e.Nation).HasMaxLength(350);
            entity.Property(e => e.NumberofExprianceYears).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PhoneNumber).HasMaxLength(350);
            entity.Property(e => e.Regid).HasColumnName("REGID");
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.Worede).HasMaxLength(350);
            entity.Property(e => e.ZoneSubcity)
                .HasMaxLength(350)
                .HasColumnName("Zone_Subcity");

            entity.HasOne(d => d.Job).WithMany(p => p.TblApplicants)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_tbl_Applicants_tbl_JobLists");

            entity.HasOne(d => d.Reg).WithMany(p => p.TblApplicants)
                .HasForeignKey(d => d.Regid)
                .HasConstraintName("FK_tbl_Applicants_tbl_Regions");
        });

        modelBuilder.Entity<TblJobList>(entity =>
        {
            entity.HasKey(e => e.JobId);

            entity.ToTable("tbl_JobLists");

            entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
            entity.Property(e => e.PostedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblJobPortalUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("tbl_JobPortalUsers");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.EmailAdress).HasMaxLength(350);
            entity.Property(e => e.PassWord).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(250);
        });

        modelBuilder.Entity<TblReference>(entity =>
        {
            entity.HasKey(e => e.ReferencesId);

            entity.ToTable("tbl_References");

            entity.Property(e => e.ReferencesId).HasColumnName("ReferencesID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(3);
        });

        modelBuilder.Entity<TblRegion>(entity =>
        {
            entity.HasKey(e => e.Regid);

            entity.ToTable("tbl_Regions");

            entity.Property(e => e.Regid).HasColumnName("REGID");
            entity.Property(e => e.RegionName).HasMaxLength(300);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
