# Admin & User Accounts Setup

## Overview

The `scripts/supabase_seed.sql` script creates three default user accounts with pre-hashed passwords. Simply run this script in Supabase SQL Editor and all accounts are ready to use.

## Default Accounts

| Role         | Username                     | Email                        | Password    |
| ------------ | ---------------------------- | ---------------------------- | ----------- |
| **Admin**    | admin@cafemanagement.local   | admin@cafemanagement.local   | Admin@123   |
| **Kế toán**  | keytoan@cafemanagement.local | keytoan@cafemanagement.local | Keytoan@123 |
| **Thu ngân** | thunga@cafemanagement.local  | thunga@cafemanagement.local  | Thunga@123  |

## How to Deploy

### Step 1: Copy SQL Script

The file `scripts/supabase_seed.sql` contains the complete bootstrap script.

### Step 2: Run in Supabase

1. Go to [Supabase Dashboard](https://supabase.com/dashboard)
2. Select your project
3. Navigate to **SQL Editor**
4. Click **New Query**
5. Copy entire contents of `scripts/supabase_seed.sql`
6. Paste into the editor
7. Click **Run**

### Step 3: Wait for Completion

The script is idempotent and will:

- Create all Identity tables (AspNetRoles, AspNetUsers, AspNetUserRoles, etc.)
- Create all business tables (NhomSP, Sanpham, Khachhang, Nhanvien, Hoadon, HoadonCT, Dinhluong)
- Seed three roles (Admin, Kế toán, Thu ngân)
- Create three user accounts with hashed passwords
- Assign each user to their respective role
- Populate sample data (products, customers, employees)

### Step 4: Verify Login

1. Go to app URL
2. Navigate to **Login** page
3. Try logging in with any account (e.g., `admin@cafemanagement.local` / `Admin@123`)
4. You should be redirected to dashboard

## Security Notes

⚠️ **Important**: These are default development/initial credentials.

### After First Login:

1. Change password immediately
2. Create strong personal admin account
3. Delete or disable default accounts if not needed
4. Set up SMTP for email-based password reset

### For Production:

- Regenerate new passwords BEFORE going live
- Use the hash generation utility (see below) to create new hashes
- Store passwords in secure location (e.g., Azure Key Vault)
- Consider implementing MFA

## Generating New Password Hashes

If you want to create new accounts or change passwords, use the CLI utility:

```bash
# Generate hash for any password
dotnet run --project CafeManagement.csproj -- --generate-admin-hash "YourPassword123"

# Output: AQAAAAIAAYagAAAAEP83Iigfh4QEAByrtrARZ8urSbftiPb/qWvt2dp4s+QDfLUjYQiXidSTAunOgUeSRw==
```

Then update SQL with the new hash:

```sql
INSERT INTO "AspNetUsers"
("Id", "DisplayName", "IsActive", "CreatedAt", "UpdatedAt", "UserName", "NormalizedUserName",
 "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash")
VALUES
(gen_random_uuid(), 'Your Display Name', true, now(), now(),
 'username@cafemanagement.local', 'USERNAME@CAFEMANAGEMENT.LOCAL',
 'username@cafemanagement.local', 'USERNAME@CAFEMANAGEMENT.LOCAL', true,
 'PASTE_HASH_HERE');
```

## Troubleshooting

### Script Already Run - Shows Errors

✅ **This is fine!** The script is idempotent. Running it twice won't duplicate data.

### Can't Log In After SQL Run

1. Check the app logs in Render
2. Verify `AspNetUsers` table has data: `SELECT * FROM "AspNetUsers" LIMIT 5;`
3. Verify role assignments: `SELECT * FROM "AspNetUserRoles" LIMIT 5;`
4. Check connection string is correct

### Password Not Working

The hashes are generated using ASP.NET Identity's PasswordHasher.

- Admin@123 → `AQAAAAIAAYagAAAAEP83Iigfh4QEAByrtrARZ8urSbftiPb/qWvt2dp4s+QDfLUjYQiXidSTAunOgUeSRw==`
- Keytoan@123 → `AQAAAAIAAYagAAAAEO23LKoJRydBt8QxO2/ZOKv93ngM0OnqOAkx4qdvNJ1jsByp70a57RZv7+VSwrwoJQ==`
- Thunga@123 → `AQAAAAIAAYagAAAAEGxjTGEMyd1Tnhs/VPs6JNlKCVGkzvGw/L7HhP76tB8sgdk7tEitoDyE6ekc6+lq7g==`

If these hashes don't work, regenerate using the CLI utility above.

## File Locations

- **SQL Script**: `scripts/supabase_seed.sql`
- **Hash Generator**: `Program.cs` (CLI utility `--generate-admin-hash`)
- **Models**: `Models/ApplicationUser.cs`
- **DbContext**: `Data/ApplicationDbContext.cs`

## What Gets Seeded

### Identity (Users & Roles)

- ✅ AspNetRoles (3 roles: Admin, Kế toán, Thu ngân)
- ✅ AspNetUsers (3 users with pre-hashed passwords)
- ✅ AspNetUserRoles (role assignments)
- ✅ Supporting tables (Claims, Logins, Tokens)

### Business Data

- ✅ NhomSP (Product Categories: Ca phe, Tra sua, Banh ngot, Nguyen lieu)
- ✅ Sanpham (Products with prices)
- ✅ Khachhang (Sample customers)
- ✅ Nhanvien (Sample employees)
- ✅ Hoadon (Sample invoices)
- ✅ HoadonCT (Invoice details)
- ✅ Dinhluong (Product recipes/formulas)

## Reference

- [ASP.NET Core Identity Documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Supabase SQL Editor](https://supabase.com/docs/guides/database/overview)
- [PostgreSQL UUID Type](https://www.postgresql.org/docs/current/uuid-ossp.html)
