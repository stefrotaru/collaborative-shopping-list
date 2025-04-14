using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

public class ShoppingListContext : DbContext
{
    public ShoppingListContext(DbContextOptions<ShoppingListContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingItem> ShoppingItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Token);
            entity.Property(u => u.CreatedAt).IsRequired();
            //entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired();
            entity.Property(g => g.CreatedAt).IsRequired();

            entity.HasOne(g => g.CreatedBy)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(gm => gm.Id);
            entity.Property(gm => gm.Role).IsRequired();

            entity.HasOne(gm => gm.User)
                .WithMany(u => u.GroupMembers)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(gm => gm.Group)
                .WithMany(g => g.GroupMembers)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShoppingList>(entity =>
        {
            entity.HasKey(sl => sl.Id);
            entity.Property(sl => sl.Name).IsRequired();
            entity.Property(sl => sl.CreatedAt).IsRequired();

            entity.HasOne(sl => sl.CreatedBy)
                .WithMany()
                .HasForeignKey(sl => sl.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(sl => sl.Group)
                .WithMany(g => g.ShoppingLists)
                .HasForeignKey(sl => sl.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ShoppingItem>(entity =>
        {
            entity.HasKey(si => si.Id);
            entity.Property(si => si.Name).IsRequired();
            entity.Property(si => si.Quantity).IsRequired();
            entity.Property(si => si.CreatedAt).IsRequired();

            entity.HasOne(si => si.CreatedBy)
                .WithMany()
                .HasForeignKey(si => si.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(si => si.ShoppingList)
                .WithMany(sl => sl.ShoppingItems)
                .HasForeignKey(si => si.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}