using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AyiHockWebAPI.Models
{
    public partial class d5qp1l4f2lmt76Context : DbContext
    {
        public d5qp1l4f2lmt76Context()
        {
        }

        public d5qp1l4f2lmt76Context(DbContextOptions<d5qp1l4f2lmt76Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Customertype> Customertypes { get; set; }
        public virtual DbSet<Jtiblacklist> Jtiblacklists { get; set; }
        public virtual DbSet<Manager> Managers { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<Mealtype> Mealtypes { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Newscategory> Newscategories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Ordercontent> Ordercontents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.UTF-8");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.CustomerId)
                    .ValueGeneratedNever()
                    .HasColumnName("customer_id");

                entity.Property(e => e.CreateTime).HasColumnName("create_time");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.Enable).HasColumnName("enable");

                entity.Property(e => e.Isblack).HasColumnName("isblack");

                entity.Property(e => e.Modifier).HasColumnName("modifier");

                entity.Property(e => e.ModifyTime).HasColumnName("modify_time");

                entity.Property(e => e.Money).HasColumnName("money");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone");

                entity.Property(e => e.Role).HasColumnName("role");
            });

            modelBuilder.Entity<Customertype>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("customertype_pkey");

                entity.ToTable("customertype");

                entity.Property(e => e.TypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("type_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.UpgradeLimit).HasColumnName("upgrade_limit");
            });

            modelBuilder.Entity<Jtiblacklist>(entity =>
            {
                entity.HasKey(e => e.Jti)
                    .HasName("jtiblacklist_pkey");

                entity.ToTable("jtiblacklist");

                entity.Property(e => e.Jti).HasColumnName("jti");

                entity.Property(e => e.Expire).HasColumnName("expire");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.ToTable("manager");

                entity.Property(e => e.ManagerId)
                    .ValueGeneratedNever()
                    .HasColumnName("manager_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email");

                entity.Property(e => e.Enable).HasColumnName("enable");

                entity.Property(e => e.IsAdmin).HasColumnName("is_admin");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone");
            });

            modelBuilder.Entity<Meal>(entity =>
            {
                entity.ToTable("meal");

                entity.HasIndex(e => e.Type, "fki_meals_type_fkey");

                entity.Property(e => e.MealId).HasColumnName("meal_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Picture)
                    .IsRequired()
                    .HasColumnName("picture");

                entity.Property(e => e.Picturename)
                    .IsRequired()
                    .HasColumnName("picturename");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.Meals)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("meal_type_fkey");
            });

            modelBuilder.Entity<Mealtype>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("mealtype_pkey");

                entity.ToTable("mealtype");

                entity.Property(e => e.TypeId)
                    .HasColumnName("type_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("news");

                entity.HasIndex(e => e.Manager, "fki_News_Manager_fkey");

                entity.HasIndex(e => e.Category, "fki_news_category_fkey");

                entity.Property(e => e.NewsId)
                    .HasColumnName("news_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.IsHot).HasColumnName("is_hot");

                entity.Property(e => e.Manager).HasColumnName("manager");

                entity.Property(e => e.ModifyTime).HasColumnName("modify_time");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("news_category_fkey");

                entity.HasOne(d => d.ManagerNavigation)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.Manager)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("news_Manager_fkey");
            });

            modelBuilder.Entity<Newscategory>(entity =>
            {
                entity.ToTable("newscategory");

                entity.Property(e => e.NewsCategoryId)
                    .HasColumnName("news_category_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.IsAdminChoose).HasColumnName("is_admin_choose");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("create_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ModifyTime)
                    .HasColumnName("modify_time")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Orderer).HasColumnName("orderer");

                entity.Property(e => e.OrdererPhone)
                    .IsRequired()
                    .HasColumnName("orderer_phone");

                entity.Property(e => e.Payrule)
                    .HasColumnName("payrule")
                    .HasComment("1: CreditCard\n2: LINE PAY\n3. GOOGLE PAY\n4. APPLE PAY");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            });

            modelBuilder.Entity<Ordercontent>(entity =>
            {
                entity.ToTable("ordercontent");

                entity.HasIndex(e => e.OrderId, "fki_ordercontent_orderid_fkey");

                entity.Property(e => e.OrdercontentId)
                    .HasColumnName("ordercontent_id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.MealId).HasColumnName("meal_id");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("order_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Ordercontents)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
