﻿// <auto-generated />
using System;
using AuditLogDemo.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuditLogDemo.Migrations
{
    [DbContext(typeof(AuditLogDBContent))]
    partial class AuditLogDBContentModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("AuditLogDemo.Models.AuditInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BrowserInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientIpAddress")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientName")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomData")
                        .HasColumnType("TEXT");

                    b.Property<string>("Exception")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExecutionDuration")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("MethodName")
                        .HasColumnType("TEXT");

                    b.Property<string>("Parameters")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReturnValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServiceName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserInfo")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuditInfos");
                });
#pragma warning restore 612, 618
        }
    }
}