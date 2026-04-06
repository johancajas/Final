using System;
using System.Collections.Generic;
using Back_EndAPI.Entities.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Back_EndAPI.Data;

public partial class WarehouseDbContext : DbContext
{
    public WarehouseDbContext()
    {
    }

    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aisle> Aisles { get; set; }

    public virtual DbSet<AisleBay> AisleBays { get; set; }

    public virtual DbSet<AisleShelf> AisleShelves { get; set; }

    public virtual DbSet<Bay> Bays { get; set; }

    public virtual DbSet<Bin> Bins { get; set; }

    public virtual DbSet<Discrepancy> Discrepancies { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<OrderedItem> OrderedItems { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<ReceivedHistory> ReceivedHistories { get; set; }

    public virtual DbSet<ReceivedItem> ReceivedItems { get; set; }

    public virtual DbSet<ReceivedShipment> ReceivedShipments { get; set; }

    public virtual DbSet<Shelf> Shelves { get; set; }

    public virtual DbSet<TransferRecord> TransferRecords { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=database-1.cisqkskacvfb.us-west-2.rds.amazonaws.com;Port=5432;Database=db26_teamtwo;Username=teamtwo;Password=no123one456knows789this!;SSL Mode=Require;Trust Server Certificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aisle>(entity =>
        {
            entity.HasKey(e => e.AisleNumber).HasName("aisle_pkey");

            entity.ToTable("aisle", "Team2Part2");

            entity.Property(e => e.AisleNumber)
                .ValueGeneratedNever()
                .HasColumnName("aisle_number");
        });

        modelBuilder.Entity<AisleBay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aisle_bay_pkey");

            entity.ToTable("aisle_bay", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AisleNumber).HasColumnName("aisle_number");
            entity.Property(e => e.BayNumber).HasColumnName("bay_number");

            entity.HasOne(d => d.AisleNumberNavigation).WithMany(p => p.AisleBays)
                .HasForeignKey(d => d.AisleNumber)
                .HasConstraintName("aisle_bay_aisle_number_fkey");

            entity.HasOne(d => d.BayNumberNavigation).WithMany(p => p.AisleBays)
                .HasForeignKey(d => d.BayNumber)
                .HasConstraintName("aisle_bay_bay_number_fkey");
        });

        modelBuilder.Entity<AisleShelf>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("aisle_shelf_pkey");

            entity.ToTable("aisle_shelf", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AisleNumber).HasColumnName("aisle_number");
            entity.Property(e => e.ShelfHeight).HasColumnName("shelf_height");
            entity.Property(e => e.ShelfLetter)
                .HasMaxLength(1)
                .HasColumnName("shelf_letter");

            entity.HasOne(d => d.AisleNumberNavigation).WithMany(p => p.AisleShelves)
                .HasForeignKey(d => d.AisleNumber)
                .HasConstraintName("aisle_shelf_aisle_number_fkey");

            entity.HasOne(d => d.ShelfLetterNavigation).WithMany(p => p.AisleShelves)
                .HasForeignKey(d => d.ShelfLetter)
                .HasConstraintName("aisle_shelf_shelf_letter_fkey");
        });

        modelBuilder.Entity<Bay>(entity =>
        {
            entity.HasKey(e => e.BayNumber).HasName("bay_pkey");

            entity.ToTable("bay", "Team2Part2");

            entity.Property(e => e.BayNumber)
                .ValueGeneratedNever()
                .HasColumnName("bay_number");
        });

        modelBuilder.Entity<Bin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("storagelocation_pkey");

            entity.ToTable("bin", "Team2Part2");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"Team2Part2\".storagelocation_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.AisleBayId).HasColumnName("aisle_bay_id");
            entity.Property(e => e.AisleShelfId).HasColumnName("aisle_shelf_id");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.Qtystored).HasColumnName("qtystored");
            entity.Property(e => e.SkuNumber).HasColumnName("sku_number");
            entity.Property(e => e.Volume).HasColumnName("volume");

            entity.HasOne(d => d.AisleBay).WithMany(p => p.Bins)
                .HasForeignKey(d => d.AisleBayId)
                .HasConstraintName("storagelocation_bay_numberid_fkey");

            entity.HasOne(d => d.AisleShelf).WithMany(p => p.Bins)
                .HasForeignKey(d => d.AisleShelfId)
                .HasConstraintName("storagelocation_shelf_numberid_fkey");

            entity.HasOne(d => d.SkuNumberNavigation).WithMany(p => p.Bins)
                .HasForeignKey(d => d.SkuNumber)
                .HasConstraintName("fk_sku_number");
        });

        modelBuilder.Entity<Discrepancy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("discrepancy_pkey");

            entity.ToTable("discrepancy", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.OrderedItemId).HasColumnName("ordered_item_id");
            entity.Property(e => e.ReceivedItemId).HasColumnName("received_item_id");

            entity.HasOne(d => d.OrderedItem).WithMany(p => p.Discrepancies)
                .HasForeignKey(d => d.OrderedItemId)
                .HasConstraintName("discrepancy_ordered_item_id_fkey");

            entity.HasOne(d => d.ReceivedItem).WithMany(p => p.Discrepancies)
                .HasForeignKey(d => d.ReceivedItemId)
                .HasConstraintName("discrepancy_received_item_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.SkuNumber).HasName("item_pkey");

            entity.ToTable("item", "Team2Part2");

            entity.Property(e => e.SkuNumber).HasColumnName("sku_number");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SuggestedSellingPrice).HasColumnName("suggested_selling_price");
            entity.Property(e => e.VolumePerUnit).HasColumnName("volume_per_unit");
        });

        modelBuilder.Entity<OrderedItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ordered_item_pkey");

            entity.ToTable("ordered_item", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CostPerUnit)
                .HasPrecision(12, 2)
                .HasColumnName("cost_per_unit");
            entity.Property(e => e.PurchaseId).HasColumnName("purchase_id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.SkuNumber).HasColumnName("sku_number");

            entity.HasOne(d => d.Purchase).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.PurchaseId)
                .HasConstraintName("ordered_item_purchase_id_fkey");

            entity.HasOne(d => d.SkuNumberNavigation).WithMany(p => p.OrderedItems)
                .HasForeignKey(d => d.SkuNumber)
                .HasConstraintName("ordered_item_sku_number_fkey");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("purchase_order_pkey");

            entity.ToTable("purchase_order", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateOrdered).HasColumnName("date_ordered");
            entity.Property(e => e.ExpectedTotalCost).HasColumnName("expected_total_cost");
            entity.Property(e => e.Vendorid).HasColumnName("vendorid");

            entity.HasOne(d => d.Vendor).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.Vendorid)
                .HasConstraintName("purchase_order_vendorid_fkey");
        });

        modelBuilder.Entity<ReceivedHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("received_history_pkey");

            entity.ToTable("received_history", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .HasColumnName("comment");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.ReceivedItemId).HasColumnName("received_item_id");

            entity.HasOne(d => d.ReceivedItem).WithMany(p => p.ReceivedHistories)
                .HasForeignKey(d => d.ReceivedItemId)
                .HasConstraintName("received_history_received_item_id_fkey");
        });

        modelBuilder.Entity<ReceivedItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("received_item_pkey");

            entity.ToTable("received_item", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.SkuNumber).HasColumnName("sku_number");

            entity.HasOne(d => d.Shipment).WithMany(p => p.ReceivedItems)
                .HasForeignKey(d => d.ShipmentId)
                .HasConstraintName("received_item_shipment_id_fkey");

            entity.HasOne(d => d.SkuNumberNavigation).WithMany(p => p.ReceivedItems)
                .HasForeignKey(d => d.SkuNumber)
                .HasConstraintName("received_item_sku_number_fkey");
        });

        modelBuilder.Entity<ReceivedShipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("received_shipment_pkey");

            entity.ToTable("received_shipment", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.ReceivedShipments)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("received_shipment_purchase_order_id_fkey");
        });

        modelBuilder.Entity<Shelf>(entity =>
        {
            entity.HasKey(e => e.ShelfLetter).HasName("shelf_pkey");

            entity.ToTable("shelf", "Team2Part2");

            entity.Property(e => e.ShelfLetter)
                .HasMaxLength(1)
                .ValueGeneratedNever()
                .HasColumnName("shelf_letter");
        });

        modelBuilder.Entity<TransferRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transferrecord_pkey");

            entity.ToTable("transfer_record", "Team2Part2");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"Team2Part2\".transferrecord_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Datetime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("datetime");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.Qty).HasColumnName("qty");
            entity.Property(e => e.Receiveditemid).HasColumnName("receiveditemid");
            entity.Property(e => e.Storagelocationid).HasColumnName("storagelocationid");
            entity.Property(e => e.Withdrawl).HasColumnName("withdrawl");

            entity.HasOne(d => d.Receiveditem).WithMany(p => p.TransferRecords)
                .HasForeignKey(d => d.Receiveditemid)
                .HasConstraintName("transferrecord_receiveditemid_fkey");

            entity.HasOne(d => d.Storagelocation).WithMany(p => p.TransferRecords)
                .HasForeignKey(d => d.Storagelocationid)
                .HasConstraintName("transferrecord_storagelocationid_fkey");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vendor_pkey");

            entity.ToTable("vendor", "Team2Part2");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
